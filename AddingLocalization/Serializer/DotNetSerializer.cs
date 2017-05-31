using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace AddingLocalization.Serializer
{
    public class DotNetSerializer : AbstractSerializer
    {
        public override void Serialize(string path, IEnumerable<KeyValuePair<string, string>> dic)
        {            
            var resDic = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (SerializeMode.HasFlag(SerializeModeEnum.Append) && File.Exists(path))
            {
                var previousItems = new ResXResourceReader(path).ToDictionary();

                foreach (var kvp in previousItems)
	            {
                    resDic.Add(kvp.Key, kvp.Value);
                }
            }

            foreach (var kvp in dic)
            {
                if (resDic.ContainsKey(kvp.Key))
                {
                    if (SerializeMode.HasFlag(SerializeModeEnum.OverwriteOldWithNew))
                    {
                        resDic[kvp.Key] = kvp.Value;
                    }
                }
                else
                {
                    if (SerializeMode.HasFlag(SerializeModeEnum.AddNonExisting))
                    {
                        resDic.Add(kvp.Key, kvp.Value);
                    }
                }
            }

            using (var rWriter = new ResXResourceWriter(path))
            {
                foreach (var kvp in resDic)
                {
                    rWriter.AddResource(kvp.Key, kvp.Value);
                }
            }
        }

        public override void Serialize(string path, string languageCode, IEnumerable<KeyValuePair<string, string>> dic)
        {
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
