using AddingLocalization.Correcter;
using AddingLocalization.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddingLocalization.Localizer
{
    public class JavaLocalizer : AbstractLocalizer
    {
        public JavaLocalizer() : base(new JavaSerializer(), new JavaCorrecter())
        {
        }

        protected override List<UnitOfWork> Filter(IEnumerable<UnitOfWork> unitsOfWork)
        {
            return unitsOfWork.ToList();
        }
    }
}
