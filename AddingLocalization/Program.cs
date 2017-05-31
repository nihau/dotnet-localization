using Logger;
using LinqToExcel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;

namespace AddingLocalization
{
    static class Program
    {
        private static readonly MyLogger MainLog = new MyLogger("log " + DateTime.Now.ToString("hh-mm-ss") + ".log");

        private static readonly Dictionary<string, int> _languageToIndexMap = new Dictionary<string, int>
            {
                {"en", 2},
                {"de", 3},
                {"es", 4},
                {"it", 5},
                {"nl", 6},
                {"zh-CHS", 7},
                {"zh-CHT", 8},
                {"fr", 10}
            };

        private static readonly List<string> _langs = _languageToIndexMap.Select(x => x.Key).ToList();

        static void Main(string[] args)
        {
        }

        public static void LocalizeResource(string path, IEnumerable<string> langs)
        {
            var resx = path;
            var designer = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path)) + ".Designer.cs";

            LocalizeFile(path, _langs, true, ".resx");
            LocalizeFile(designer, _langs, false, ".Designer.cs");
        }

        public static List<string> LocalizeFile(string originPath, IEnumerable<string> langs, bool copy = false, string ext = null)
        {
            ext = ext ?? Path.GetExtension(originPath);

            var filePath = originPath.Substring(0, originPath.IndexOf(ext));

            var l = new List<string> { filePath };

            foreach (var lang in langs)
            {
                var newPath = filePath + "." + lang + ext;

                if (copy)
                    File.Copy(originPath, newPath, true);
                else
                    File.Create(newPath);

                l.Add(newPath);
            }

            return l;
        }

        public static string GetLocalizedFilePath(string originPath, string lang, string ext = null)
        {
            ext = ext ?? Path.GetExtension(originPath);

            var filePath = originPath.Substring(0, originPath.IndexOf(ext));

            return filePath + "." + lang + ext;
        }

        public static List<UnitOfWork> FetchUnitsFromFile(Dictionary<string, int> languageToIndexMap, string targetDirectory, string localizationMap)
        {
            var excel = new ExcelQueryFactory(localizationMap).Worksheet("Sheet1").Skip(1).ToArray();

            var unitsOfWork = new List<UnitOfWork>();
            Row currentRow = null;
            UnitOfWork current = null;

            for (int i = 0; i < excel.Length; i++)
            {
                currentRow = excel[i];

                var resxPath = currentRow[0];

                if (!String.IsNullOrWhiteSpace(resxPath))
                {
                    var absoluteResxPath = Path.Combine(targetDirectory, resxPath);

                    var existing = unitsOfWork.Find(x => x.ResourceFile == absoluteResxPath);

                    if (existing == null)
                    {
                        current = new UnitOfWork { ResourceFile = absoluteResxPath };

                        unitsOfWork.Add(current);
                    }
                    else
                    {
                        current = existing;
                    }
                }

                var key = currentRow[1].ToString();
                var en = currentRow[2];

                if (!String.IsNullOrWhiteSpace(key))
                {
                    var localizedEntry = new LocalizedEntry(key, en);

                    current.Localizations.Add(localizedEntry);

                    foreach (var langKvp in languageToIndexMap)
                    {
                        var localizedString = currentRow[langKvp.Value];

                        localizedEntry.Localizations.Add(langKvp.Key, localizedString);
                    }
                }
            }

            return unitsOfWork;
        }

        public static List<UnitOfWork> FetchUnitsFromSpreadsheet(string targetDirectory, string localizationMap, string sheet, Dictionary<string, int> languageToIndexMap = null)
        {
            var dt = GetExcelData(localizationMap, sheet);
            var unitsOfWork = new List<UnitOfWork>();
            string[] currentRow = null;
            UnitOfWork current = null;

            languageToIndexMap = languageToIndexMap ?? _languageToIndexMap;

            foreach (var row in dt.AsEnumerable().Skip(1))
            {
                currentRow = row.ItemArray.Select(x => x.ToString()).ToArray();

                currentRow[1] = currentRow[1];

                if (!String.IsNullOrEmpty(currentRow[0]))
                {
                    var absoluteResxPath = Path.Combine(targetDirectory, currentRow[0]);

                    var existing = unitsOfWork.Find(x => x.ResourceFile == absoluteResxPath);

                    if (existing == null)
                    {
                        current = new UnitOfWork { ResourceFile = absoluteResxPath };

                        unitsOfWork.Add(current);
                    }
                    else
                    {
                        current = existing;
                    }
                }

                var key = currentRow[1].ToString();
                var en = currentRow[2];

                if (!String.IsNullOrWhiteSpace(key))
                {
                    if (String.IsNullOrEmpty(en))
                        en = key;

                    var localizedEntry = new LocalizedEntry(key, en);

                    current.Localizations.Add(localizedEntry);

                    foreach (var langKvp in languageToIndexMap)
                    {
                        var localizedString = String.IsNullOrEmpty(currentRow[langKvp.Value])
                            ? null
                            : currentRow[langKvp.Value];

                        localizedEntry.Localizations.Add(langKvp.Key, localizedString);
                    }
                }
            }

            return unitsOfWork;
        }

        public static DataTable GetExcelData(string FileName, string strSheetName)
        {
            DataTable dt = new DataTable();
            XSSFWorkbook hssfworkbook;
            using (FileStream file = new FileStream(FileName, FileMode.Open, FileAccess.Read))
            {
                hssfworkbook = new XSSFWorkbook(file);
            }

            ISheet sheet = hssfworkbook.GetSheet(strSheetName);
            IEnumerator rows = sheet.GetRowEnumerator();

            int ii = 0;

            foreach (IRow row in sheet)
            {
                ii++;

                if (dt.Columns.Count == 0)
                {
                    for (int j = 0; j < row.LastCellNum; j++)
                    {
                        dt.Columns.Add(row.GetCell(j).ToString());
                    }

                    continue;
                }

                DataRow dr = dt.NewRow();

                for (int i = 0; i < row.LastCellNum; i++)
                {
                    ICell cell = row.GetCell(i);

                    try
                    {
                        if (cell == null)
                        {
                            dr[i] = null;
                        }
                        else
                        {
                            dr[i] = cell.ToString();
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                        break;
                    }
                }

                dt.Rows.Add(dr);
            }

            return dt;
        }

        public static IEnumerable<Dictionary<string, string>> GetResourcesDics(string targetDirectory)
        {
            var files = Auxilliary.GetFiles(targetDirectory, "*.resx");
            var resxes = files.Select(x => new ResXResourceReader(x));
            var dics = resxes.Select(x => x.ToDictionary(false, StringComparer.OrdinalIgnoreCase));

            return dics;
        }

        public static void Diff(IEnumerable<UnitOfWork> L, IEnumerable<UnitOfWork> R, string nameOfL, string nameOfR)
        {
            var loggers = new[] { MainLog, new MyLogger("diff log " + DateTime.Now.ToString("hh-mm-ss") + ".log") };

            Diff(L, R, nameOfL, loggers);
            Diff(R, L, nameOfR, loggers);
        }

        public static void Diff(IEnumerable<UnitOfWork> L, IEnumerable<UnitOfWork> R, string nameOfL, IEnumerable<MyLogger> loggers)
        {
            loggers.WriteLineSplit("Starting resx diff");

            var resxesL = L.Select(u => u.ResourceFile);
            var resxesR = R.Select(u => u.ResourceFile);

            var langs = L.First().Localizations.First().Localizations.Select(x => x.Key);

            foreach (var resxDiffL in resxesL.Except(resxesR))
                loggers.WriteLine("{0} xlsx contains {1}, second not", nameOfL, resxDiffL);

            loggers.WriteLineSplit();

            //intersect
            var intersectResxes = L.Select(u => u.ResourceFile).Intersect(R.Select(u => u.ResourceFile));

            loggers.WriteLineSplit("Starting checking key diff at intersection of xlsx's");

            foreach (var resx in intersectResxes)
            {
                loggers.WriteLineSplit("Checking at \"{0}\" resx", resx);

                var lU = L.Where(u => u.ResourceFile == resx).Single();
                var rU = R.Where(u => u.ResourceFile == resx).Single();

                var lKeys = lU.Localizations.Select(x => x.Key);
                var rKeys = rU.Localizations.Select(x => x.Key);

                foreach (var kDiff in lKeys.Except(rKeys))
                {
                    loggers.WriteLine("\"{0}\" xlsx contains \"{1}\" key", nameOfL, kDiff);
                }

                var keys = lKeys.Intersect(rKeys);

                foreach (var key in keys)
                {
                    var entryL = lU.Localizations.Where(u => u.Key == key).Single();
                    var entryR = rU.Localizations.Where(u => u.Key == key).Single();

                    if (entryL.EngText != entryR.EngText)
                        loggers.WriteLine("Diff at english text \"{0}\" key: \"{1}\" != \"{2}\"", key, entryL.EngText, entryR.EngText);

                    foreach (var lang in langs)
                    {
                        var textL = entryL.Localizations[lang];
                        var textR = entryL.Localizations[lang];

                        if (textL != textR)
                            loggers.WriteLine("Diff at \"{3}\" text \"{0}\" key: \"{1}\" != \"{2}\"", key, entryL.EngText, entryR.EngText, lang);
                    }
                }

                loggers.WriteLineSplit("going to next resx");
            }

            loggers.WriteLineSplit();
        }

        private static string FindLocalizationMap(string localizationMapName, string targetDirectory, string[] args)
        {
            var localizationMap = localizationMapName;

            foreach (var arg in args)
            {
                if (arg.EndsWith(".xlsx"))
                {
                    localizationMap = arg;
                }

                if (Directory.Exists(arg))
                {
                    targetDirectory = arg;
                }
            }

            if (String.IsNullOrWhiteSpace(localizationMap))
            {
                var xlsxInFolder = Directory.GetFiles(Environment.CurrentDirectory, "*.xlsx");

                if (xlsxInFolder.Any())
                {
                    localizationMap = xlsxInFolder.FirstOrDefault();
                }
                else
                {
                    const string plsFile = "LocalizationMap.xlsx";

                    var assembly = Assembly.GetExecutingAssembly();
                    const string resourceName = "AddingLocalization." + plsFile;

                    using (var stream = assembly.GetManifestResourceStream(resourceName))
                    using (var writer = new FileStream(plsFile, FileMode.Create, FileAccess.Write))
                    {
                        stream.CopyTo(writer);
                    }

                    localizationMap = Path.Combine(Environment.CurrentDirectory, plsFile);
                }
            }

            return localizationMap;
        }

    }
}
