using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class SecureRandom
    {
        private RandomNumberGenerator _Random;

        public SecureRandom()
        {
            _Random = new RNGCryptoServiceProvider();
        }

        /// <summary>
        /// Get the next random integer
        /// </summary>
        /// <returns>Random [Int32]</returns>
        public int Next(int min, int max)
        {
            max = max - 1;

            var bytes = new byte[sizeof(int)];
            _Random.GetNonZeroBytes(bytes);
            var val = BitConverter.ToInt32(bytes, 0);
 
            var result = ((val - min) % (max - min + 1) + (max - min + 1)) % (max - min + 1) + min;
 
            return result;
        }
    }
}
