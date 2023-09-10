using apidotnet.DTO;
using apidotnet.Helper;
using dotnetapi.Helper;
using dotnetapi.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RevisionTool.Data;
using RevisionTool.Entity;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;

namespace apidotnet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService service;

        public UserController(IUserService service)
        {
            this.service = service;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(Register registerResponse)
        {
            ApiResponse response = new ApiResponse();

            if (!Validations.IsPasswordValid(registerResponse.Password))
            {
                response.StatusCode = 400;
                response.IsSuccess = false;
                response.Message = "Password is not valid.";
                return BadRequest(response);
            }

            if (!Validations.IsNameValid(registerResponse.FirstName)) {
                response.StatusCode = 400;
                response.IsSuccess = false;
                response.Message = "First name is not valid.";
                return BadRequest(response);
            }
            if (!Validations.IsNameValid(registerResponse.LastName))
            {
                response.StatusCode = 400;
                response.IsSuccess = false;
                response.Message = "Last name is not valid.";
                return BadRequest(response);
            }

            if (!Validations.IsValidEmail(registerResponse.Email)){
                response.StatusCode = 400;
                response.IsSuccess = false;
                response.Message = "Email is not valid.";
                return BadRequest(response);
            }

            var data = await this.service.Register(registerResponse, response);

            if (!data.IsSuccess)
            {
                return BadRequest(data);
            }

            await SendOTPEmail(registerResponse.Email);
            return Ok(data);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> GenerateToken([FromBody] LoginCredential loginCredential)
        {
            ApiResponse response = new ApiResponse();
            var data = await this.service.Login(loginCredential, response);
            if (!data.IsSuccess)
            {
                return Unauthorized(data);
            }
            return Ok(data);
        }

        [HttpPost("LoginRefresh")]
        public async Task<IActionResult> GenerateToken([FromBody] TokenResponse token)
        {
            ApiResponse response = new ApiResponse();
            var data = await this.service.RefreshLogin(token, response);
            if (!data.IsSuccess)
            {
                return Unauthorized(data);
            }
            return Ok(data);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            ApiResponse response = new ApiResponse();

            // Get the claims associated with the authenticated user
            var claimsIdentity = HttpContext.User.Identity as ClaimsIdentity;
            if (claimsIdentity != null)
            {
                var emailClaim = claimsIdentity.FindFirst(ClaimTypes.Email);
                if (emailClaim != null)
                {
                    string email = emailClaim.Value;
                    var data = await this.service.Get(email, response);
                    return Ok(data);
                }
            }

            // If the email claim is not found, return an error response
            response.StatusCode = 401;
            response.IsSuccess = false;
            response.Message = "Email cannot find in token.";
            return Unauthorized(response);
        }

        [Authorize(Roles = "admin")] // This attribute restricts access to users with the "admin" role
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllUsers()
        {
            ApiResponse response = new ApiResponse();

            try
            {
                var users = await service.GetAllUsers(response);

                if (users.IsSuccess)
                {
                    return Ok(users);
                }
                else
                {
                    return BadRequest(users);
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.IsSuccess = false;
                response.Message = ex.Message;
                return StatusCode(500, response);
            }
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            ApiResponse response = new ApiResponse();

            try
            {
                var data = await service.DeleteUser(userId, response);

                if (data.IsSuccess)
                {
                    return Ok(data);
                }
                else
                {
                    return NotFound(data);
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.IsSuccess = false;
                response.Message = ex.Message;
                return StatusCode(500, response);
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser(int userId, [FromBody] User user)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                var data = await service.UpdateUser(userId, user, response);

                if (data.IsSuccess)
                {
                    return Ok(data);
                }
                else
                {
                    return NotFound(data);
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.IsSuccess = false;
                response.Message = ex.Message;
                return StatusCode(500, response);
            }
        }

        [Authorize]
        [HttpPut("SendOTPEmail/{email}")]
        public async Task<IActionResult> SendOTPEmail(string email)
        {
            ApiResponse response = new ApiResponse();

            try
            {
                var data = await service.SendEmailOTP(email, response);

                if (data.IsSuccess)
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

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.IsSuccess = false;
                response.Message = ex.Message;
                return StatusCode(500, response);
            }
        }

        [Authorize]
        [HttpPut("VerifyOTPEmail/{otp}")]
        public async Task<IActionResult> VerifyOTPEmail(string otp)
        {
            ApiResponse response = new ApiResponse();

            try
            {
                // Get the email of the authenticated user
                var claimsIdentity = HttpContext.User.Identity as ClaimsIdentity;
                if (claimsIdentity != null)
                {
                    var emailClaim = claimsIdentity.FindFirst(ClaimTypes.Email);
                    if (emailClaim != null)
                    {
                        string email = emailClaim.Value;

                        var data = await this.service.VerifyOTPEmail(email, otp, response);

                        if (data.IsSuccess)
                        {
                            return Ok(data);
                        }
                        else
                        {
                            return BadRequest(data);
                        }
                    }
                }

                // If the email claim is not found, return an error response
                response.StatusCode = 401;
                response.IsSuccess = false;
                response.Message = "Email cannot find in token.";
                return Unauthorized(response);
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.IsSuccess = false;
                response.Message = ex.Message;
                return StatusCode(500, response);
            }
        }

    }
}
