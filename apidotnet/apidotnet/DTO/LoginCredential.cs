namespace apidotnet.DTO
{
    public class LoginCredential
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
