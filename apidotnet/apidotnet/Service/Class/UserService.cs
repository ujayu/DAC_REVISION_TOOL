using apidotnet.DTO;
using apidotnet.Helper;
using AutoMapper;
using dotnetapi.Service.Interface;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using RevisionTool.Data;
using RevisionTool.Entity;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;


namespace apidotnet.Service.Class
{
    public class UserService : IUserService
    {
        private readonly RevisionToolContext context;
        private readonly IMapper mapper;
        private readonly JwtSettings jwtSettings;
        private const int maxFailedAttempts = 3;           // Maximum allowed consecutive failed login attempts
        private const int lockoutDurationMinutes = 15;     // Lockout duration in minutes


        public UserService(RevisionToolContext context, IMapper mapper, IOptions<JwtSettings> options)
        {
            this.context = context;
            this.mapper = mapper;
            jwtSettings = options.Value;
        }

        private async Task<string> HashPassword(string password)
        {
            // Generate a random salt
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // Hash the password using PBKDF2
            string hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            return $"{Convert.ToBase64String(salt)}.{hashedPassword}";
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            // Extract the salt and hash from the stored hashed password
            string[] parts = hashedPassword.Split('.');
            byte[] salt = Convert.FromBase64String(parts[0]);
            string storedHash = parts[1];

            // Hash the provided password using the extracted salt
            string hashedPasswordProvided = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            // Compare the generated hash with the stored hash
            return storedHash == hashedPasswordProvided;
        }


        public async Task<ApiResponse> Login(LoginCredential loginCredential, ApiResponse response)
        { 
            var user = await this.context.Users.FirstOrDefaultAsync(item => item.Email == loginCredential.Email);

            if (user != null)
            {
                if (user.IsActive == 0 && user.LastWrongAttempt.HasValue &&
                        (DateTime.UtcNow - user.LastWrongAttempt.Value).TotalMinutes <= lockoutDurationMinutes)
                {
                    // User's account is locked due to too many failed attempts within the lockout duration.
                    response.StatusCode = 401;
                    response.IsSuccess = false;
                    response.Message = "Account is for locked due to too many failed login attempts.";
                    double accountLockTime = lockoutDurationMinutes - (DateTime.UtcNow - user.LastWrongAttempt.Value).TotalMinutes;
                    response.Data = new
                    {
                        isAccountLock = true,
                        accountLockTime = accountLockTime,
                    };
                    return response;
                }

                if (VerifyPassword(loginCredential.Password, user.Password))
                {
                    // Successful login
                    // Reset failed login attempt count and timestamp.
                    user.WrongAttempts = 0;
                    user.LastWrongAttempt = null;

                    var tokenhandler = new JwtSecurityTokenHandler();
                    var tokenkey = Encoding.UTF8.GetBytes(this.jwtSettings.securitykey);
                    var tokendesc = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new Claim[]
                        {
                        new Claim(ClaimTypes.Email,user.Email),
                        new Claim(ClaimTypes.Role,user.Role)
                        }),
                        Expires = DateTime.UtcNow.AddHours(1), ///session time
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenkey), SecurityAlgorithms.HmacSha256)
                    };
                    var token = tokenhandler.CreateToken(tokendesc);
                    var finaltoken = tokenhandler.WriteToken(token);

                    response.Data = new TokenResponse() { Token = finaltoken, RefreshToken = await GenerateToken(user.UserId) };
                    response.StatusCode = 201;
                    response.IsSuccess = true;
                    response.Message = "Login successful.";
                }
                else
                {
                    // Failed login attempt
                    user.WrongAttempts++;
                    user.LastWrongAttempt = DateTime.UtcNow;

                    if (user.WrongAttempts >= maxFailedAttempts)
                    {
                        // Lock the account due to too many failed attempts.
                        user.IsActive = 0;
                        user.LastWrongAttempt = DateTime.UtcNow;
                    }

                    await this.context.SaveChangesAsync();

                    response.StatusCode = 401;
                    response.IsSuccess = false;
                    response.Message = "Invalid email or password.";
                    response.Data = new
                    {
                    };
                }
            }
            else
            {
                response.StatusCode = 401;
                response.IsSuccess = false;
                response.Message = "Invalid email or password. 1";
                response.Data = new object();
            }

            return response;
        }


        public async Task<ApiResponse> RefreshLogin(TokenResponse token, ApiResponse response)
        {
            var _refreshtoken = await this.context.Tokens.FirstOrDefaultAsync(item => item.RefreshToken == token.RefreshToken);
            if (_refreshtoken != null)
            {
                //generate token
                var tokenhandler = new JwtSecurityTokenHandler();
                var tokenkey = Encoding.UTF8.GetBytes(this.jwtSettings.securitykey);
                SecurityToken securityToken;
                var principal = tokenhandler.ValidateToken(token.Token, new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(tokenkey),
                    ValidateIssuer = false,
                    ValidateAudience = false,

                }, out securityToken);

                var _token = securityToken as JwtSecurityToken;
                if (_token != null && _token.Header.Alg.Equals(SecurityAlgorithms.HmacSha256))
                {
                    //int.TryParse(principal.Identity?.Name, out int userID);
                    var userEmailClaim = principal.FindFirst(ClaimTypes.Email)?.Value;
                    var user = await this.context.Users.FirstOrDefaultAsync(item => item.Email == userEmailClaim);
                    int userID = user.UserId;
                    var _existdata = await this.context.Tokens.FirstOrDefaultAsync(item => item.UserId == userID
                    && item.RefreshToken == token.RefreshToken);
                    if (_existdata != null)
                    {
                        var _newtoken = new JwtSecurityToken(
                            claims: principal.Claims.ToArray(),
                            expires: DateTime.Now.AddDays(7),
                            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.jwtSettings.securitykey)),
                            SecurityAlgorithms.HmacSha256)
                            );

                        var _finaltoken = tokenhandler.WriteToken(_newtoken);

                        response.Data = new TokenResponse() { Token = _finaltoken, RefreshToken = await GenerateToken(userID) };

                        response.StatusCode = 201;
                        response.IsSuccess = true;
                        response.Message = "Token refreshed successfully.";
                    }
                    else
                    {
                        response.StatusCode = 401;
                        response.IsSuccess = false;
                        response.Message = "Invalid refreshd token.";
                    }
                }
                else
                {
                    response.StatusCode = 401;
                    response.IsSuccess = false;
                    response.Message = "Invalid token.";
                }

            }
            else
            {
                response.StatusCode = 401;
                response.IsSuccess = false;
                response.Message = "Invalid refresh token.";
            }

            return response;
        }
        public async Task<string> GenerateToken(int UserId)
        {
            var randomnumber = new byte[32];
            using (var randomnumbergenerator = RandomNumberGenerator.Create())
            {
                randomnumbergenerator.GetBytes(randomnumber);
                string refreshtoken = Convert.ToBase64String(randomnumber);
                var Existtoken = this.context.Tokens.FirstOrDefaultAsync(item => item.UserId == UserId).Result;
                if (Existtoken != null)
                {
                    Existtoken.RefreshToken = refreshtoken;
                }
                else
                {
                    await this.context.Tokens.AddAsync(new Token
                    {
                        UserId = UserId,
                        RefreshToken = refreshtoken
                    });
                }
                await this.context.SaveChangesAsync();

                return refreshtoken;

            }
        }

        public async Task<ApiResponse> Register(Register registerResponse, ApiResponse response)
        {
            

            if (await EmailExists(registerResponse.Email))
            {
                response.StatusCode = 400;
                response.IsSuccess = false;
                response.Message = "Email already exists.";
                return response;
            }

            try
            {
                string hashedPassword = await HashPassword(registerResponse.Password);

                User user = mapper.Map<User>(registerResponse);
                user.Password = hashedPassword;
                user.IsActive = 1;

                var _data = await this.context.Users.AddAsync(user);
                await this.context.SaveChangesAsync();
                response.StatusCode = 201;
                response.IsSuccess = true;
                response.Data = user.Email;
            }
            catch (Exception ex)
            {
                response.StatusCode = 400;
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;

        }
        public Task<bool> EmailExists(string email)
        {
            return context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<ApiResponse> Get(string email, ApiResponse response)
        {
            try
            {
                var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);

                if (user != null)
                {
                    var userInfo = mapper.Map<UserInfo>(user); // Assuming you have a UserDto class
                    response.Data = userInfo;
                    response.StatusCode = 200;
                    response.IsSuccess = true;
                }
                else
                {
                    response.StatusCode = 404;
                    response.IsSuccess = false;
                    response.Message = "User not found.";
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ApiResponse> GetAllUsers(ApiResponse response)
        {
            try
            {
                var users = await context.Users.ToListAsync();
                var userInfos = mapper.Map<List<UserInfo>>(users); // Assuming you have a List<UserInfo> type
                response.Data = userInfos;
                response.StatusCode = 200;
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ApiResponse> DeleteUser(int userId, ApiResponse response)
        {
            var user = await context.Users.FindAsync(userId);

            if (user != null)
            {
                context.Users.Remove(user);
                await context.SaveChangesAsync();

                response.StatusCode = 200;
                response.IsSuccess = true;
                response.Message = "User deleted successfully.";
            }
            else
            {
                response.StatusCode = 404;
                response.IsSuccess = false;
                response.Message = "User not found.";
            }

            return response;
        }

        public async Task<ApiResponse> UpdateUser(int userId, User user, ApiResponse response)
        {
            if (user != null)
            {
                context.Users.Update(user);
                await context.SaveChangesAsync();

                response.StatusCode = 200;
                response.IsSuccess = true;
                response.Message = "User update successfully.";
            }
            else
            {
                response.StatusCode = 404;
                response.IsSuccess = false;
                response.Message = "User not found.";
            }
            return response;
        }

       public async Task<ApiResponse> SendEmailOTP(string email, ApiResponse response)
        {
            Console.WriteLine(email);
            try
            {
                var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);

                if (user != null)
                {
                    string otp = GenerateRandomOTP();

                    // Store the OTP in the VerificationCode entity
                    Verificationcode verificationCode = new Verificationcode
                    {
                        UserId = user.UserId,
                        Code = otp,
                        ExpirationTime = DateTime.UtcNow.AddMinutes(10), // Set expiration time, e.g., 10 minutes
                        CreatedAt = DateTime.UtcNow
                    };

                    context.Verificationcodes.Add(verificationCode);
                    await context.SaveChangesAsync();

                    // Send OTP via email
                    bool emailSent = SendEmail(user.Email, otp);

                    if (emailSent)
                    {
                        response.StatusCode = 200;
                        response.IsSuccess = true;
                        response.Message = "OTP email sent successfully.";
                    }
                    else
                    {
                        response.StatusCode = 500;
                        response.IsSuccess = false;
                        response.Message = "Error sending OTP email.";
                    }
                }
                else
                {
                    response.StatusCode = 404;
                    response.IsSuccess = false;
                    response.Message = "User not found.";
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }



        private string GenerateRandomOTP()
        {
            // Generate a random OTP (e.g., 6-digit number)
            Random random = new Random();
            int otpValue = random.Next(1000, 9999);
            return otpValue.ToString();
        }

        private bool SendEmail(string email, string otp)
        {
            try
            {
                string fromMail = "jayantuttarwar2000@gmail.com";
                string fromPassword = "ndpzipsbgbswxcqj";

                MailMessage message = new MailMessage();
                message.From = new MailAddress(fromMail);
                message.Subject = "Your OTP Code";
                message.To.Add(new MailAddress(email));
                message.Body = $"Your OTP code is: {otp}";
                message.IsBodyHtml = true;

                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(fromMail, fromPassword),
                    EnableSsl = true,

            };
                smtpClient.Send(message);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<ApiResponse> VerifyOTPEmail(string email, string otp, ApiResponse response)
        {
            try
            {
                var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);

                if (user != null)
                {
                    var verificationCode = await context.Verificationcodes
                        .Where(vc => vc.UserId == user.UserId && vc.Code == otp && vc.ExpirationTime > DateTime.UtcNow)
                        .OrderByDescending(vc => vc.CreatedAt)
                        .FirstOrDefaultAsync();

                    if (verificationCode != null)
                    {
                        // Perform actions after OTP verification (e.g., update user's email verified status)
                        user.IsEmailVerify = 1;
                        context.Users.Update(user);
                        await context.SaveChangesAsync();

                        response.StatusCode = 200;
                        response.IsSuccess = true;
                        response.Message = "Email OTP verified successfully.";
                    }
                    else
                    {
                        response.StatusCode = 400;
                        response.IsSuccess = false;
                        response.Message = "Invalid or expired OTP.";
                    }
                }
                else
                {
                    response.StatusCode = 404;
                    response.IsSuccess = false;
                    response.Message = "User not found.";
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }


    }

}
