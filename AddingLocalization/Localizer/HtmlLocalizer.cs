using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AddingLocalization.Correcter;
using AddingLocalization.Serializer;

namespace AddingLocalization.Localizer
{
    class HtmlLocalizer : AbstractLocalizer
    {
        public HtmlLocalizer() : base(null, null)
        {
            throw new NotImplementedException();
        }

        protected override List<UnitOfWork> Filter(IEnumerable<UnitOfWork> unitsOfWork)
        {
            return unitsOfWork.ToList();
        }
    }
}
