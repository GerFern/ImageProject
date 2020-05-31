using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ImageLib
{
    public class LocaleStringContainer
    {
        private string lib;
        internal void Initialize()
        {

        }

        public string GetString(string key)
        {
            if (cashe.TryGetValue(key, out string value)) return value;
            else
            {
                throw new NotImplementedException(); //TODO: Возможность локализации
            }
        }

        private Dictionary<string, string> cashe = new Dictionary<string, string>();
        private Dictionary<string, LocaleString> locales = new Dictionary<string, LocaleString>();
        public LocaleStringContainer Register(string key, string @default)
            => Register(new LocaleString(key, @default));
        

        public LocaleStringContainer Register(LocaleString locale)
        {
            locales.Add(locale.Key, locale);
            return this;
        }

        public LocaleStringContainer Register(ICollection<LocaleString> locales)
        {
            foreach (var item in locales)
            {
                Register(item);
            }
            return this;
        }
    }
    [Serializable]
    public struct LocaleString : IEnumerable<char>
    {
        LocaleStringContainer container;
        public string Key { get; }
        public string Default { get; }
        public CharEnumerator GetEnumerator() => ToString().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        IEnumerator<char> IEnumerable<char>.GetEnumerator() => GetEnumerator();

        public LocaleString(string key, string @default)
        {
            container = null;
            Key = key;
            Default = @default;
        }

        public override string ToString()
            => container is null
            ? Default
            : container.GetString(Key);

        public override bool Equals(object obj) => obj is LocaleString ls && this.Key == ls.Key;

        public override int GetHashCode() => Key.GetHashCode();

        public static bool operator ==(LocaleString left, LocaleString right) 
            => left.Equals(right);

        public static bool operator !=(LocaleString left, LocaleString right) 
            => !left.Equals(right);
    }

}
