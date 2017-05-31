using AddingLocalization.Correcter;
using AddingLocalization.Serializer;
using LinqToExcel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddingLocalization.Localizer
{
    public class DotNetLocalizer : AbstractLocalizer
    {
        public DotNetLocalizer() : base(new DotNetSerializer(), new DotNetCorrecter()) { }

        protected override List<UnitOfWork> Filter(IEnumerable<UnitOfWork> unitsOfWork)
        {
            var resList = new List<UnitOfWork>();

            MainLog.WriteLine("╔════════════════════════════════╗");
            MainLog.WriteLine("║   Filtering input resx files   ║");
            MainLog.WriteLine("╚════════════════════════════════╝");

            foreach (var unitOfWork in unitsOfWork)
            {
                if (!unitOfWork.ResourceFile.EndsWith(".resx"))
                {
                    MainLog.WriteLine("Not an resx: {0}", unitOfWork.ResourceFile);
                }
                else if (!File.Exists(unitOfWork.ResourceFile))
                {
                    MainLog.WriteLine("File coudln't be found: {0}", unitOfWork.ResourceFile);
                }
                else
                {
                    resList.Add(unitOfWork);
                }
            }

            MainLog.WriteLine("╔════════════════════════════════╗");
            MainLog.WriteLine("║Ended filtering input resx files║");
            MainLog.WriteLine("╚════════════════════════════════╝");

            return resList;
        }
    }
}
