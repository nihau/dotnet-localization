using Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddingLocalization.Correcter
{
    public abstract class AbstractCorrecter : ICorrecter
    {
        public List<MyLogger> Loggers {get;set;}

        public AbstractCorrecter (IEnumerable<MyLogger> loggers) : this()
	    {
            Loggers.AddRange(loggers);
	    }

        public AbstractCorrecter ()
	    {
            Loggers = new List<MyLogger>();

            Loggers.Add(new MyLogger("correct log " + DateTime.Now.ToString("hh-mm-ss") + ".log"));
	    }
            

        public void Correct(IEnumerable<UnitOfWork> unitsOfWork)
        {
            foreach(var unitOfWork in unitsOfWork)
            {
                Correct(unitOfWork);
            }
        }

        public abstract void Correct(UnitOfWork unitOfWork);
    }
}
