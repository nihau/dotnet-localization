using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddingLocalization.Serializer
{
    public abstract class AbstractSerializer : ISerializer
    {
        public SerializeModeEnum SerializeMode { get; set; }

        public AbstractSerializer()
        {
            SerializeMode = SerializeModeEnum.OverwriteOldWithNew | SerializeModeEnum.Append;
        }

        public abstract void Serialize(string path, IEnumerable<KeyValuePair<string, string>> dic);

        public abstract void Serialize(string path, string languageCode, IEnumerable<KeyValuePair<string, string>> dic);
    }
}
