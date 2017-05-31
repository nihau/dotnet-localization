using AddingLocalization.Correcter;
using AddingLocalization.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddingLocalization.Localizer
{
    public class CustomLocalizer : AbstractLocalizer
    {
        public CustomLocalizer(ISerializer serializer, ICorrecter correcter) : base(serializer, correcter) { }

        protected override List<UnitOfWork> Filter(IEnumerable<UnitOfWork> unitsOfWork)
        {
            return unitsOfWork.ToList();
        }
    }
}
