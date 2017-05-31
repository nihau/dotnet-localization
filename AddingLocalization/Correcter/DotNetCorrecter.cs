using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using Logger;
using AddingLocalization.Deserializer;

namespace AddingLocalization.Correcter
{
    public class DotNetCorrecter : AbstractCorrecter
    {
        public override void Correct(UnitOfWork unitOfWork)
        {
            Loggers.WriteLine("Starting to correct unitOfWork \"{0}\"", unitOfWork.ResourceFile);
            var unmatchedXlsx = new MyLogger("UnmatchedXlsx keys correct " + DateTime.Now.ToString("hh-mm-ss") + ".log");
            var unmatchedResx = new MyLogger("UnmatchedResx keys correct " + DateTime.Now.ToString("hh-mm-ss") + ".log");

            var unmatchedXlsxKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var unmatchedResxKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            var resxKvps =  new ResXResourceReader(unitOfWork.ResourceFile).ToDictionary<string, string>(comparer: StringComparer.OrdinalIgnoreCase);

            unmatchedXlsxKeys.AddRange(unitOfWork.Localizations.Select(x => x.Key));
            unmatchedResxKeys.AddRange(resxKvps.Select(x => x.Key));

            Loggers.WriteLine("╔═════════════════════════════════╗");
            Loggers.WriteLine("║Started swapping values from resx║");
            Loggers.WriteLine("╚═════════════════════════════════╝");

            //demanding that resx contains up-to-date values if keys match
            foreach (var localizationEntry in unitOfWork.Localizations)
            {
                var key = localizationEntry.Key;

                if (resxKvps.ContainsKey(key))
                {
                    if (localizationEntry.EngText != resxKvps[key])
                    {
                        Loggers.WriteLine("Swapping xlsx value \"{0}\" to resx value \"{1}\" at key \"{2}\"", localizationEntry.EngText, resxKvps[key], key);

                        localizationEntry.EngText = resxKvps[key];
                    }

                    unmatchedXlsxKeys.Remove(key);
                    unmatchedResxKeys.Remove(key);
                }
            }

            Loggers.WriteLine("╔═════════════════════════════════╗");
            Loggers.WriteLine("║ Ended swapping values from resx ║");
            Loggers.WriteLine("╚═════════════════════════════════╝");

            Loggers.WriteLine("╔═════════════════════════════════╗");
            Loggers.WriteLine("║ Started swapping keys from resx ║");
            Loggers.WriteLine("╚═════════════════════════════════╝");
            //finding values for unmatchedones and swapping to corresponding key of resx
            //dirtyhack with copy, but who cares
            foreach (var unmatchedKey in unmatchedXlsxKeys.ToList())
            {
                var localizationEntry = unitOfWork.Localizations.Find(x => x.Key == unmatchedKey);

                foreach (var resxKvp in resxKvps)
                {
                    if (StringComparer.OrdinalIgnoreCase.Compare(localizationEntry.EngText, resxKvp.Value) == 0)
                    {
                        Loggers.WriteLine("Swapping xlsx key \"{0}\" to resx key \"{1}\" at value \"{2}\"", localizationEntry.Key, resxKvp.Key, resxKvp.Value);

                        var oldXlsxkey = localizationEntry.Key;
                        var newResxKey = resxKvp.Key;

                        unmatchedXlsxKeys.Remove(oldXlsxkey);
                        unmatchedResxKeys.Remove(newResxKey);

                        localizationEntry.Key = resxKvp.Key;
                    }
                }
            }
            Loggers.WriteLine("╔═════════════════════════════════╗");
            Loggers.WriteLine("║  Ended swapping keys from resx  ║");
            Loggers.WriteLine("╚═════════════════════════════════╝");

            if (unmatchedXlsxKeys.Count > 0)
            {
                var record = String.Format("Left unmatched keys of xlsx: {0}", String.Join("; ", unmatchedXlsxKeys.Select(k => "\"" + k + "\"")));

                Loggers.WriteLine(record);

                unmatchedXlsx.WriteLine(unitOfWork.ResourceFile);
                unmatchedXlsx.WriteLine(record);
            }

            if (unmatchedResxKeys.Count > 0)
            {
                var record = String.Format("Left unmatched keys of resx: {0}", String.Join("; ", unmatchedResxKeys.Select(k => "\"" + k + "\"")));

                Loggers.WriteLine(record);

                unmatchedResx.WriteLine(unitOfWork.ResourceFile);
                unmatchedResx.WriteLine(record);
            }

            Loggers.WriteLine("Ended correcting unitOfWork \"{0}\"", unitOfWork.ResourceFile);
        }
    }
}
