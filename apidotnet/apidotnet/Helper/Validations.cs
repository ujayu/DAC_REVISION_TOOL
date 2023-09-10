using System.Text.RegularExpressions;

namespace dotnetapi.Helper
{
    public class Validations
    {
        private static readonly Regex EmailRegex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
        public static bool IsPasswordValid(string password)
        {
            string passwordPattern = @"^(?=.*[!@#$%^&*()_+{}\[\]:;<>,.?~\/\-\d])(?=.*[A-Za-z])\S{8,}$";
            return Regex.IsMatch(password, passwordPattern);
        }

        public static bool IsNameValid(string name)
        {
            return !string.IsNullOrEmpty(name) && name.Length >= 3 && Regex.IsMatch(name, @"^[A-Za-z]+$");
        }

        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return false;
            }

            return EmailRegex.IsMatch(email);
        }
    }
}
