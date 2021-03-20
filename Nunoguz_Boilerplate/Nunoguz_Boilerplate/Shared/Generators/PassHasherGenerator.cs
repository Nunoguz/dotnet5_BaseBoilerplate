using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Security.Cryptography;

namespace Nunoguz_Boilerplate.Shared.Generators
{
    public static class PassHasherGenerator
    {
        /// <summary>
        /// Generates hashed output by a given password, then returns a tuple contains salt and hash respectively.
        /// </summary>
        /// <param name="password">Password to hash</param>
        /// <param name="saltStr"></param>
        /// <returns>Returns a tuple contains salt and hash respectively</returns>
        public static Tuple<string, string> HashPassword(string password, string saltStr)
        {
            // generate a 128-bit salt using a secure PRNG
            byte[] salt = new byte[128 / 8];

            if (saltStr == null)
            {
                var rng = RandomNumberGenerator.Create();
                rng.GetBytes(salt);
            }
            else
            {
                salt = Convert.FromBase64String(saltStr);
            }

            // derive a 256-bit subkey (use HMACSHA1 with 10,000 iterations)
            var hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            return new Tuple<string, string>(Convert.ToBase64String(salt), hashed);
        }
    }
}
