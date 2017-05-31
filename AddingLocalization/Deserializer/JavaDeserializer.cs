using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CheatDictionary;

namespace AddingLocalization.Deserializer
{
    public class JavaDeserializer : AbstractFileDeserializer
    {
        public JavaDeserializer(string filePath)
            : base(filePath)
        {
        }

        public override IDictionary<string, string> Deserialize()
        {
            var lines = File.ReadAllText(_filePath);

            var matches = Regex.Matches(lines, @"\{\s*""(.+)"",\s*""(.+)""\s*\}");

            var dic = new CheatDictionary<string,string>(Comparer);

            foreach (Match match in matches)
            {
                var key = match.Groups[1].Value;
                var value = match.Groups[2].Value;

                dic.Add(key, value);
            }

            return dic;
        }
    }
}
