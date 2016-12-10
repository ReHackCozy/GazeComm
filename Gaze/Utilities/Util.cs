using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gaze.Utilities
{
    class Util
    {
        public static string HashSHA1(string input)
        {
            var hash = (new System.Security.Cryptography.SHA1Managed()).ComputeHash(Encoding.UTF8.GetBytes(input));
            return string.Join("", hash.Select(b => b.ToString("x2")).ToArray());
        }
    }
}
