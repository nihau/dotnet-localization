using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddingLocalization.Deserializer
{
    public class DeserializerFactory
    {
        public IDeserializer Get(string resourcePath)
        {
            var ext = Path.GetExtension(resourcePath);
            IDeserializer des;

            switch(ext)
            {
                case ".java":
                    des = new JavaDeserializer(resourcePath);
                    break;
                case ".resx":
                    des = new DotNetDeserializer(resourcePath);
                    break;
                default:
                    throw new NotSupportedException("I can't recognize this file");
            }

            return des;
        }
    }
}
