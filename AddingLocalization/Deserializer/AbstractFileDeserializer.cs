using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddingLocalization.Deserializer
{
    public abstract class AbstractFileDeserializer : IDeserializer
    {
        protected readonly string _filePath;

        public string FilePath
        {
            get { return _filePath; }
        }

        public IEqualityComparer<string> Comparer { get; set; }

        public AbstractFileDeserializer(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException();

            _filePath = filePath;
            Comparer = StringComparer.OrdinalIgnoreCase;
        }

        public abstract IDictionary<string, string> Deserialize();
    }
}
