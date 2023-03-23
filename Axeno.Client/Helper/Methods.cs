using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axeno.Client.Helper
{
    internal class Methods
    {
        private const string Alphabet = "abcdefghijklmnopqrstuvwxyz";

        public static Random Random = new Random();
        public static string GetRandomString(int length)
        {
            StringBuilder randomName = new StringBuilder(length);
            for (int i = 0; i < length; i++)
                randomName.Append(Alphabet[Random.Next(Alphabet.Length)]);

            return randomName.ToString();
        }
    }
}
