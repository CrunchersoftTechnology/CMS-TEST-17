using Microsoft.AspNet.Identity;
using System;

namespace CMS.Web.Helpers
{
    public static class PasswordHelper
    {
        public static string GeneratePassword()
        {
            var validator = GetPasswordHelper();

            bool requireNonLetterOrDigit = validator.RequireNonLetterOrDigit;
            bool requireDigit = validator.RequireDigit;
            bool requireLowercase = validator.RequireLowercase;
            bool requireUppercase = validator.RequireUppercase;

            string randomPassword = string.Empty;

            int passwordLength = validator.RequiredLength;

            Random random = new Random();
            while (randomPassword.Length != passwordLength)
            {
                int randomNumber = random.Next(48, 122);  // >= 48 && < 122 
                if (randomNumber == 95 || randomNumber == 96) continue;  // != 95, 96 _'

                char c = Convert.ToChar(randomNumber);

                if (requireDigit)
                    if (char.IsDigit(c))
                        requireDigit = false;

                if (requireLowercase)
                    if (char.IsLower(c))
                        requireLowercase = false;

                if (requireUppercase)
                    if (char.IsUpper(c))
                        requireUppercase = false;

                if (requireNonLetterOrDigit)
                    if (!char.IsLetterOrDigit(c))
                        requireNonLetterOrDigit = false;

                randomPassword += c;
            }

            if (requireDigit)
                randomPassword += Convert.ToChar(random.Next(48, 58));  // 0-9

            if (requireLowercase)
                randomPassword += Convert.ToChar(random.Next(97, 123));  // a-z

            if (requireLowercase)
                randomPassword += Convert.ToChar(random.Next(65, 91));  // A-Z

            if (requireNonLetterOrDigit)
                randomPassword += Convert.ToChar(random.Next(33, 48));  // symbols !"#$%&'()*+,-./

            return randomPassword;
        }

        public static PasswordValidator GetPasswordHelper()
        {
            return new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };
        }
    }
}