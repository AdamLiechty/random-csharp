using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace Random
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length < 1)
            {
                PrintUsage();
                return -1;
            }

            string option = args[0];
            switch (option.ToLower())
            {
                case "word":
                    return DoRandomWord(args.Skip(1));
                case "character":
                    return DoRandomCharacter(args.Skip(1));
                case "integer":
                    return DoRandomInteger(args.Skip(1));
                default:
                    Console.WriteLine("Invalid option '{0}'", option.ToLower());
                    PrintUsage();
                    return -1;
            }
        }

        private static int DoRandomWord(IEnumerable<string> args)
        {
            string[] lines = GetWords();

            uint count = 1;
            if (args.Any())
            {
                if (!uint.TryParse(args.First(), out count))
                {
                    Console.WriteLine("Invalid count: '{0}'", args.First());
                    PrintUsage();
                    return -1;
                }
            }

            RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
            byte[] bytes = new byte[8];
            for (uint i = 0; i < count; ++i)
            {
                crypto.GetBytes(bytes);
                long randomLong = BitConverter.ToInt64(bytes, 0);
                long randomIndex = Math.Abs(randomLong) % lines.LongLength;
                string randomLine = lines[randomIndex];

                Console.WriteLine(randomLine);
            }

            return 0;
        }

        private static readonly string allowedCharacters = @"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789~`!@#$%^&*()_-+={}[]\|;:""',<.>/?";
        private static int DoRandomCharacter(IEnumerable<string> args)
        {
            uint count = 1;
            if (args.Any())
            {
                if (!uint.TryParse(args.First(), out count))
                {
                    Console.WriteLine("Invalid count: '{0}'", args.First());
                    PrintUsage();
                    return -1;
                }
            }

            RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
            byte[] bytes = new byte[8];
            for (uint i = 0; i < count; ++i)
            {
                crypto.GetBytes(bytes);
                long randomLong = BitConverter.ToInt64(bytes, 0);
                int randomIndex = (int)(Math.Abs(randomLong) % allowedCharacters.Length);
                char randomChar = allowedCharacters[randomIndex];

                Console.Write(randomChar);
            }

            Console.WriteLine();

            return 0;
        }

        private static int DoRandomInteger(IEnumerable<string> args)
        {
            string[] argArray = args.Take(3).ToArray();
            if (argArray.Length < 2)
            {
                PrintUsage();
                return -1;
            }

            long min, max;
            if (!long.TryParse(argArray[0], out min) || min <= long.MinValue >> 1)
            {
                Console.WriteLine("Invalid min: '{0}'", argArray[0]);
                PrintUsage();
                return -1;
            }

            if (!long.TryParse(argArray[1], out max) || max >= long.MaxValue >> 1)
            {
                Console.WriteLine("Invalid max: '{0}'", argArray[1]);
                PrintUsage();
                return -1;
            }

            if (min > max)
            {
                Console.WriteLine("Min must be <= Max");
                return -1;
            }

            uint count = 1;
            if (argArray.Length > 2)
            {
                if (!uint.TryParse(argArray[2], out count))
                {
                    Console.WriteLine("Invalid count: '{0}'", argArray[2]);
                    PrintUsage();
                    return -1;
                }
            }

            RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
            byte[] bytes = new byte[8];
            long rangeSize = max - min + 1;
            for (uint i = 0; i < count; ++i)
            {
                crypto.GetBytes(bytes);
                long randomLong = BitConverter.ToInt64(bytes, 0);
                long randomInRangeSize = Math.Abs(randomLong) % rangeSize;
                long randomInRange = randomInRangeSize + min;

                Console.WriteLine(randomInRange);
            }

            return 0;
        }

        private static string[] GetWords()
        {
            return Resources.words.Split(new string[] {"\r\n"}, StringSplitOptions.None);
        }

        private static void PrintUsage()
        {
            Console.WriteLine();
            Console.WriteLine("Random [word|character|integer min max] [count]");
            Console.WriteLine();
            Console.WriteLine("Gets a random word or number");
            Console.WriteLine();
        }
    }
}
