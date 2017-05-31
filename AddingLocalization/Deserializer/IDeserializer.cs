using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddingLocalization.Deserializer
{
    public interface IDeserializer
    {
        string FilePath { get; }
        IDictionary<string, string> Deserialize();
    }
}
