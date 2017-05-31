using AddingLocalization.Correcter;
using AddingLocalization.Deserializer;
using AddingLocalization.Serializer;
using System.Linq;
using LinqToExcel;
using Logger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AddingLocalization.Localizer
{
    public abstract class AbstractLocalizer : ILocalizer
    {
        protected static readonly MyLogger _mainLog = new MyLogger("log " + DateTime.Now.ToString("hh-mm-ss") + ".log");

        public MyLogger MainLog
        {
            get { return _mainLog; }
        }

        protected DeserializerFactory DeserializerFactory { get; set; }

        public ICorrecter Correcter { get; }
        public ISerializer Serializer { get; }

        public AbstractLocalizer(ISerializer serializer, ICorrecter correcter)
        {
            if (serializer == null) throw new ArgumentNullException(nameof(serializer));
            if (correcter == null) throw new ArgumentNullException(nameof(correcter));

            Correcter = correcter;
            Serializer = serializer;
        }

        public void Localize(IEnumerable<UnitOfWork> unitsOfWork, IEnumerable<string> languageKeyCodes)
        {
            Correcter.Correct(unitsOfWork);

            unitsOfWork = Filter(unitsOfWork);

            foreach (var u in unitsOfWork)
            {
                u.SerializeLanguages(Serializer, languageKeyCodes);
            }
        }

        protected abstract List<UnitOfWork> Filter(IEnumerable<UnitOfWork> unitsOfWork);
    }
}
