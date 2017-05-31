using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AddingLocalization.Serializer
{
    public class JavaSerializer : AbstractSerializer
    {
        public override void Serialize(string path, IEnumerable<KeyValuePair<string, string>> dic)
        {
            var lines = File.ReadAllLines(path, Encoding.UTF8);
            var l = dic.ToList();

            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];

                var match = Regex.Match(line, @"\{\s*""(.+)"",\s*""(.+)""\s*\}");

                if (!match.Success)
                    continue;

                var tabulation = line.Substring(0, line.IndexOf('{'));
                var key = match.Groups[1].Value;

                var kvp = l.Find(m => StringComparer.OrdinalIgnoreCase.Compare(key, m.Key) == 0);

                //no key in xlsx file
                if (kvp.Key == null && kvp.Value == null)
                    continue;

                l.Remove(kvp);

                lines[i] = tabulation + String.Format("{{ \"{0}\", \"{1}\" }},", kvp.Key, kvp.Value);
            }

            File.WriteAllLines(path, lines);
        }

        public override void Serialize(string path, string languageCode, IEnumerable<KeyValuePair<string, string>> dic)
        {
            var ext = Path.GetExtension(path);

            var index = path.LastIndexOf("_");
            var newPath = path.Remove(index) + "_" + languageCode.Replace("-", "_") + ext;

            Serialize(newPath, dic);
        }
    }
}
