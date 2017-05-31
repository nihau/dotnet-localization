using Logger;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace AddingLocalization
{
    public enum ResourceType
    {
        Content = 0,
        EmbeddedResource
    }

    public struct CsprojModificationUnit
    {
        public string CsProjPath { get; set; }

        public List<string> ChangedResx { get; set; }
        public List<string> NewLanguages { get; set; }
    }

    /// <summary>
    /// recommended to get well-know to xPath before changing anything
    /// e.g. http://www.rpbourret.com/xml/XPathIn5.htm
    /// 
    /// terrible class
    /// </summary>
    static class VisualStudioCsProjModifier
    {        
        private static readonly MyLogger MainLog = new MyLogger("csproj migration log " + DateTime.Now.ToString("hh-mm-ss") + ".log");

        public static void Modify(CsprojModificationUnit csprojModificationUnit)
        {
            var csProjDocument = new XmlDocument();
            csProjDocument.Load(csprojModificationUnit.CsProjPath);

            var mgr = new XmlNamespaceManager(csProjDocument.NameTable);
            mgr.AddNamespace("x", "http://schemas.microsoft.com/developer/msbuild/2003");

            foreach (var resourceName in csprojModificationUnit.ChangedResx)
            {
                UpdateContentResx(csProjDocument, mgr, resourceName, csprojModificationUnit.NewLanguages);                
            }

            //if (!ContainsMasterInclude(csProjDocument, mgr))
            //    AddMasterInclude(csProjDocument, mgr);

            csProjDocument.Save(csprojModificationUnit.CsProjPath);
        }

        private static void UpdateContentResx(XmlDocument document, XmlNamespaceManager mgr, string resourceName, IEnumerable<string> newLangs)
        {
            try
            {
                var type = GetResourceType(document, mgr, resourceName);

                switch(type)
                {
                    case ResourceType.Content:
                        AddLocalizedContent(document, mgr, resourceName, newLangs);
                        break;
                    case ResourceType.EmbeddedResource:
                        AddEmbeddedLocalizedResources(document, mgr, resourceName, newLangs);
                        break;
                    default:
                        break;
                }
            }
            catch(Exception ex)
            {
                MainLog.WriteLine("Couldn't update {0}", resourceName);
            }
        }

        private static ResourceType GetResourceType(XmlDocument document, XmlNamespaceManager mgr, string resourceName)
        {
            if (IsEmbeddedResource(document, mgr, resourceName))
                return ResourceType.EmbeddedResource;

            return ResourceType.Content;
        }

        private static bool IsEmbeddedResource(XmlDocument document, XmlNamespaceManager mgr, string resourceName)
        {
            //thats the only way here to put it to lower case
            //and its the only way of case-insensitive comparison
            const string translate = "translate(@Include,'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz')";

            var xpathResxNode = $"//x:*[contains(concat(' ', {translate}, ' '), '{resourceName.ToLowerInvariant()}') and x:Generator]";
                

            return document.DocumentElement.SelectSingleNode(xpathResxNode, mgr) != null;
        }

        private static void AddLocalizedContent(XmlDocument document, XmlNamespaceManager mgr, string resourceName, IEnumerable<string> newLangs)
        {
            var contentXpath = String.Format("//x:Content[contains(concat(' ', translate(@Include,'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), ' '), '{0}')]", resourceName.ToLowerInvariant());

            var contentNode = document.DocumentElement.SelectSingleNode(contentXpath, mgr);

            foreach (var lang in newLangs)
            {
                var locContentNode = contentNode.CloneNode(true);

                var contentInclude = InsertLangBeforeExtension(locContentNode.Attributes["Include"].Value, lang);
                locContentNode.Attributes["Include"].Value = contentInclude;

                contentNode.ParentNode.AppendChild(locContentNode);
            }
        }

        private static void AddEmbeddedLocalizedResources(XmlDocument document, XmlNamespaceManager mgr, string resourceName, IEnumerable<string> newLangs)
        {
            var designerXpath = String.Format("//x:Compile[translate(x:DependentUpon/text(),'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz')='{0}']", Path.GetFileName(resourceName).ToLowerInvariant());
            var resourceXpath = String.Format("//x:*[contains(concat(' ', translate(@Include,'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), ' '), '{0}')]", resourceName.ToLowerInvariant());

            var designerNode = document.DocumentElement.SelectSingleNode(designerXpath, mgr);
            var resourceNode = document.DocumentElement.SelectSingleNode(resourceXpath, mgr);

            var csprojPath = Path.GetDirectoryName(new Uri(document.BaseURI).LocalPath);

            foreach(var lang in newLangs)
            {
                var locDesignerNode = designerNode.CloneNode(true);
                var locResourcesNode = resourceNode.CloneNode(true);

                var designerInclude = InsertLangBeforeExtension(locDesignerNode.Attributes["Include"].Value, lang);
                locDesignerNode.Attributes["Include"].Value = designerInclude;

                var resourceInclude =  InsertLangBeforeExtension(locResourcesNode.Attributes["Include"].Value, lang);
                locResourcesNode.Attributes["Include"].Value = resourceInclude;

                //var designerDependent = locDesignerNode.SelectSingleNode("/x:DependentUpon", mgr).InnerText;
                locDesignerNode.SelectSingleNode("/x:DependentUpon", mgr).InnerText = Path.GetFileName(resourceInclude);
                locResourcesNode.SelectSingleNode("/x:LastGenOutput", mgr).InnerText = Path.GetFileName(designerInclude);

                designerNode.ParentNode.AppendChild(locDesignerNode);
                resourceNode.ParentNode.AppendChild(locResourcesNode);

                MainLog.WriteLine("added resource {0}", resourceInclude);
                MainLog.WriteLine("added designer {0}", designerInclude);
                
                var file = Path.Combine(csprojPath, designerInclude);
                if (!File.Exists(file))
                    File.Create(file);
            }

        }

        private static string InsertLangBeforeExtension(string fileNameWithExtension, string lang)
        {
            var n = fileNameWithExtension;

            Debug.Assert(n.Length > 1);
            Debug.Assert(n.IndexOf('.') != n.Length - 1);

            if (n.IndexOf(".Designer.cs", StringComparison.OrdinalIgnoreCase) > -1)
            {
                n = Regex.Replace(n, ".Designer.cs", $".{lang}.Designer.cs", RegexOptions.IgnoreCase);
            }
            else
            {
                var index = n.IndexOf(Path.GetExtension(fileNameWithExtension), StringComparison.OrdinalIgnoreCase);

                n = n.Insert(index, "." + lang);
            }

            return n;
        }

        public static string FindCsproj(string targetResourceFile)
        {
            var csproj = String.Empty;

            for (; targetResourceFile.Length > 3 && String.IsNullOrWhiteSpace(csproj); targetResourceFile = Directory.GetParent(targetResourceFile).FullName)
                csproj = Directory.GetFiles(Path.GetDirectoryName(targetResourceFile), "*.csproj").FirstOrDefault();

            if (String.IsNullOrWhiteSpace(csproj)) throw new FileNotFoundException("Couldn't manage to find .csproj");

            return csproj;
        }
    }
}
