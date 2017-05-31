using System;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Resources;
using AddingLocalization.Serializer;

namespace AddingLocalization
{
    public class UnitOfWork 
    {
        public string ResourceFile { get; set; }

        public List<LocalizedEntry> Localizations { get; set; }

        public UnitOfWork()
        {
            Localizations = new List<LocalizedEntry>();
        }

        public void SerializeLocalized(ISerializer Serializer)
        {
            var langs = Localizations.Select(x => x.Localizations.Keys).First();

            SerializeLanguages(Serializer, langs);
        }

        public void SerializeLanguages(ISerializer Serializer, IEnumerable<string> languagesCode)
        {
            foreach (var lang in languagesCode)
            {
                SerializeLanguage(Serializer, lang);
            }
        }

        public void SerializeLanguage(ISerializer Serializer, string languageCode)
        {
            var locs = Localizations
                .Select(e => new KeyValuePair<string, string>(e.Key, e.Localizations[languageCode]))
                .Where(kvp => kvp.Value != null)
                .ToDictionary(x => x.Key, x => x.Value);

            Serializer.Serialize(ResourceFile, languageCode, locs);
        }
    }

    [DebuggerDisplay("[{Key} {EngText}]")]
    public class LocalizedEntry
    {
        public string Key { get; set; }
        public string EngText { get; set; }

        public Dictionary<string, string> Localizations { get; set; }

        public LocalizedEntry(string key, string engText)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException(nameof(key));

            if (string.IsNullOrEmpty(engText))
                throw new ArgumentException(nameof(engText));

            Key = key;
            EngText = engText;

            Localizations = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var other = (LocalizedEntry)obj;

            if (StringComparer.OrdinalIgnoreCase.Compare(Key, other.Key) != 0)
                return false;

            if (StringComparer.OrdinalIgnoreCase.Compare(EngText, other.EngText) != 0)
                return false;

            if (other.Localizations == null)
                return false;

            if (Localizations.Count != other.Localizations.Count)
                return false;

            foreach(var kvp in Localizations)
            {
                var key = kvp.Key;
                var val = kvp.Value;

                if (!other.Localizations.ContainsKey(key))
                    return false;

                if (StringComparer.OrdinalIgnoreCase.Compare(Localizations[key], other.Localizations[key]) != 0)
                    return false;
            }

            return true;
        }
        
        public override int GetHashCode()
        {
            return Key.GetHashCode() ^ (EngText.GetHashCode() >> 16);
        }

        public static bool operator ==(LocalizedEntry l, LocalizedEntry r)
        {
            return l.Equals(r);
        }

        public static bool operator !=(LocalizedEntry l, LocalizedEntry r)
        {
            return !l.Equals(r);
        }

        public override string ToString()
        {
            return $"{Key}\t{EngText}\t{String.Join("\t", Localizations.Values)}";
        }
    }
}
