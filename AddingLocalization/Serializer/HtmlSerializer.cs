using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.IO.Path;


namespace AddingLocalization.Serializer
{
    class ReplaceKeySerializer : AbstractSerializer
    {
        public override void Serialize(string path, IEnumerable<KeyValuePair<string, string>> dic)
        {
            //var doc = new HtmlDocument();

            //doc.Load(path);

            //var rootNode = doc.DocumentNode;

            //foreach (var kvp in dic)
            //{
            //    var nodes = rootNode.SelectNodes($"//*[text()='{kvp.Key}']");
            //}

            if (!path.Contains("ExMsg."))
                return;

            if (!File.Exists(path))
                File.Create(path).Close();

            var text = File.ReadAllLines(path);

            var sb = new StringBuilder();

            sb.AppendLine("var ExMsgLocalization = {");

            foreach (var kvp in dic)
            {
                sb.AppendLine($"\t\"{kvp.Key}\": \"{kvp.Value}\",");
            }
            sb.AppendLine("}");

            File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
        }

        public override void Serialize(string path, string languageCode, IEnumerable<KeyValuePair<string, string>> dic)
        {
            path = path.Replace(".en.", ".");
            var index = path.IndexOf(Path.GetExtension(path));
            var newPath = path;

            if (StringComparer.OrdinalIgnoreCase.Compare(languageCode, "en") != 0)
            {
                newPath = newPath.Insert(index, "." + languageCode);
            }

            Serialize(newPath, dic);
        }
    }
}
