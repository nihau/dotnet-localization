using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddingLocalization.Serializer
{
    public interface ISerializer
    {
        void Serialize(string path, IEnumerable<KeyValuePair<string, string>> dic);
        void Serialize(string path, string languageCode, IEnumerable<KeyValuePair<string, string>> dic);

        SerializeModeEnum SerializeMode { get; set; }
    }
}
