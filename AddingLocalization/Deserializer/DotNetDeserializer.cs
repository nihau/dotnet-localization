using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace AddingLocalization.Deserializer
{
    public class DotNetDeserializer : AbstractFileDeserializer
    {
        public DotNetDeserializer(string filePath)
            :base(filePath)
        {
        }

        public override IDictionary<string, string> Deserialize()
        {
            return new ResXResourceReader(FilePath).ToDictionary(true, Comparer);
        }
    }
}
