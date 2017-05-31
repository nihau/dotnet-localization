using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddingLocalization.Correcter
{
    public interface ICorrecter
    {
        void Correct(IEnumerable<UnitOfWork> unitsOfWork);
        void Correct(UnitOfWork unitOfWork);
    }
}
