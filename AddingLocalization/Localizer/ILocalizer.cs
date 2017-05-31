using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddingLocalization.Localizer
{
    public interface ILocalizer
    {
        void Localize(IEnumerable<UnitOfWork> unitsOfWork, IEnumerable<string> languageKeyCodes);
    }
}
