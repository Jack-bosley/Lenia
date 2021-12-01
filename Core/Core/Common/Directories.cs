using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Lenia.Core.Common
{
    public static class Directories
    {
        static Directories()
        {
            DirectoryPrefix = Directory.GetCurrentDirectory();
        }


        public static string DirectoryPrefix { get; private set; } 

        public static string Assets => Path.Combine(Path.GetFullPath(DirectoryPrefix), "assets");
        public static string Shaders => Path.Combine(Assets, "shaders");
        public static string Materials => Path.Combine(Assets, "materials");


        public static void SetDirectoryPrefix(string newPrefix) => DirectoryPrefix = newPrefix;
    }
}
