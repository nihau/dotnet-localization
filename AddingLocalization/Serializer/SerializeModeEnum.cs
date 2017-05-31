using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddingLocalization.Serializer
{
    public enum SerializeModeEnum
    {
        None = 0,
        Append = 1,
        OverwriteOldWithNew = 2,
        AddNonExisting = 4,
    }


}
