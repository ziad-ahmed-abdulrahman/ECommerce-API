using System.Security.Cryptography;
using System.Text;

namespace ECommerce.Api.Helper
{
    public static class GenerateCode
    {
        private const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public static string GenerateActivationCode(int length = 6)
        {
            if (length <= 0) 
                length = 6;

            var codeChars = new char[length];
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] randomBytes = new byte[length];

                rng.GetBytes(randomBytes); 
               

                for (int i = 0; i < length; i++)
                {
                    int index = randomBytes[i] % chars.Length; 
                    codeChars[i] = chars[index];
                }
            }

            return new string(codeChars);
        }
    }
}
