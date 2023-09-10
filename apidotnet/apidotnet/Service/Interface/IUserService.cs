using apidotnet.DTO;
using apidotnet.Helper;
using RevisionTool.Entity;

namespace dotnetapi.Service.Interface
{
    public interface IUserService
    {
        Task<ApiResponse> Register(Register registerResponse, ApiResponse response);
        Task<ApiResponse> Login(LoginCredential loginCredential, ApiResponse response);
        Task<ApiResponse> RefreshLogin(TokenResponse token, ApiResponse response);
        Task<ApiResponse> Get(string email, ApiResponse response);
        Task<ApiResponse> GetAllUsers(ApiResponse response);
        Task<ApiResponse> DeleteUser(int userId, ApiResponse response);
        Task<ApiResponse> UpdateUser(int UserId, User user, ApiResponse response);
        Task<ApiResponse> SendEmailOTP(string email, ApiResponse response);
        Task<ApiResponse> VerifyOTPEmail(string email, string otp, ApiResponse response);

    }

}
