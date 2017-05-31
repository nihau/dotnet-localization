using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using Logger;
using AddingLocalization.Deserializer;
using AddingLocalization;
using CheatDictionary;

namespace AddingLocalization.Correcter
{
    public class JavaCorrecter : AbstractCorrecter
    {
        public override void Correct(UnitOfWork unitOfWork)
        {
            Loggers.WriteLine("Starting to correct unitOfWork \"{0}\"", unitOfWork.ResourceFile);
            var unmatchedXlsxLog = new MyLogger("UnmatchedXlsx keys correct " + DateTime.Now.ToString("hh-mm-ss") + ".log");
            var unmatchedJavaLog = new MyLogger("UnmatchedJava keys correct " + DateTime.Now.ToString("hh-mm-ss") + ".log");

            var unmatchedXlsxKeys = new List<string>();
            var unmatchedJavaKeys = new List<string>();

            var javaKvps = new JavaDeserializer(unitOfWork.ResourceFile).Deserialize().ToList();

            var xlsxKeysToCount = unitOfWork.Localizations.Select(l => l.Key).ToList().ToCountDictionary(StringComparer.OrdinalIgnoreCase);
            var javaKeysToCount = javaKvps.Select(kvp => kvp.Key).ToList().ToCountDictionary(StringComparer.OrdinalIgnoreCase);

            var union = xlsxKeysToCount.Select(x => x.Key).Union(javaKeysToCount.Select(x => x.Key));

            foreach(var key in union)
            {
                var timesInXlsx = xlsxKeysToCount.ContainsKey(key) ? xlsxKeysToCount[key] : 0;
                var timesInJava = javaKeysToCount.ContainsKey(key) ? javaKeysToCount[key] : 0;

                if (timesInXlsx > timesInJava)
                {
                    unmatchedXlsxKeys.Add(key + " was found " + (timesInXlsx - timesInJava) + " more than in java");
                }
                else if (timesInJava > timesInXlsx)
                {
                    unmatchedJavaKeys.Add(key + " was found " + (timesInJava - timesInXlsx) + " more than in xlsx");
                }
            }

            //xlsx verification

            if (unmatchedXlsxKeys.Count > 0)
            {
                var record = String.Format("Left unmatched keys of xlsx: {0}", String.Join("; ", unmatchedXlsxKeys.Select(k => "\"" + k + "\"")));

                Loggers.WriteLine(record);

                unmatchedXlsxLog.WriteLine(unitOfWork.ResourceFile);
                unmatchedXlsxLog.WriteLine(record);
            }

            if (unmatchedJavaKeys.Count > 0)
            {
                var record = String.Format("Left unmatched keys of java: {0}", String.Join("; ", unmatchedJavaKeys.Select(k => "\"" + k + "\"")));

                Loggers.WriteLine(record);

                unmatchedJavaLog.WriteLine(unitOfWork.ResourceFile);
                unmatchedJavaLog.WriteLine(record);
            }

            Loggers.WriteLine("Ended correcting unitOfWork \"{0}\"", unitOfWork.ResourceFile);
        }
    }
}
