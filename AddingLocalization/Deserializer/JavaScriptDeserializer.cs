using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AddingLocalization.Deserializer
{
    public class JavaScriptDeserializer : AbstractFileDeserializer
    {
        public JavaScriptDeserializer(string filePath) 
            : base(filePath)
        {
        }

        public override IDictionary<string, string> Deserialize()
        {
            var appendix = "";

            var regex = new Regex(@"\s*['""](.+)+,?")

            var lines = File.ReadAllLines(_filePath);
        }
    }
}
