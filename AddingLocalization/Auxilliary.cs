using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddingLocalization
{
    static class Auxilliary
    {
        /// <summary>
        /// Deep
        /// </summary>
        public static List<string> GetFiles(string folder, string pattern)
        {
            var dirs = Directory.GetDirectories(folder);

            var l = new List<string>();

            for (int i = 0; i < dirs.Length; i++)
            {
                l.AddRange(GetFiles(dirs[i], pattern));
            }

            l.AddRange(Directory.GetFiles(folder, pattern));

            return l;
        }

        public static string GetRelativePath(string filespec, string folder)
        {
            if (filespec.IndexOf(folder, StringComparison.OrdinalIgnoreCase) < 0)
                throw new Exception("ti cho ");

            if (!folder.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                folder += Path.DirectorySeparatorChar;
            }

            return filespec.Substring(folder.Length);
        }
    }
}
