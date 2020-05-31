using System.Collections.Generic;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using ImageProject.ViewModel;
using System.Linq;

namespace ImageProject
{
    public class Storage : IDictionary<string, object>, INotifyCollectionChanged
    {
        Dictionary<string, object> dict = new Dictionary<string, object>();

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected void OnCollectionChanged(NotifyCollectionChangedAction action, string key)
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action, key));
        }

        protected void OnCollectionChanged(NotifyCollectionChangedAction action)
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action));
        }


        public object this[string key] 
        {
            get => ((IDictionary<string, object>)dict)[key];
            set
            {
                if (ContainsKey(key))
                {
                    ((IDictionary<string, object>)dict)[key] = value;
                    OnCollectionChanged(NotifyCollectionChangedAction.Replace, key);
                }
                else
                {
                    ((IDictionary<string, object>)dict)[key] = value;
                    OnCollectionChanged(NotifyCollectionChangedAction.Add, key);
                }
            }
        }

        public ICollection<string> Keys => ((IDictionary<string, object>)dict).Keys;

        public ICollection<object> Values => ((IDictionary<string, object>)dict).Values;

        public int Count => ((IDictionary<string, object>)dict).Count;

        public bool IsReadOnly => ((IDictionary<string, object>)dict).IsReadOnly;
        public void Add(string key, object value)
        {
            ((IDictionary<string, object>)dict).Add(key, value);
            OnCollectionChanged(NotifyCollectionChangedAction.Add, key);
        }

        public void Add(KeyValuePair<string, object> item)
        {
            ((IDictionary<string, object>)dict).Add(item);
            OnCollectionChanged(NotifyCollectionChangedAction.Add, item.Key);
        }

        public void Clear()
        {
            ((IDictionary<string, object>)dict).Clear();
            OnCollectionChanged(NotifyCollectionChangedAction.Reset);
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            return ((IDictionary<string, object>)dict).Contains(item);
        }

        public bool ContainsKey(string key)
        {
            return ((IDictionary<string, object>)dict).ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            ((IDictionary<string, object>)dict).CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return ((IDictionary<string, object>)dict).GetEnumerator();
        }

        public bool Remove(string key)
        {
            if(((IDictionary<string, object>)dict).Remove(key))
            {
                OnCollectionChanged(NotifyCollectionChangedAction.Remove, key);
                try
                {
                    Delete(key);
                }
                catch { }
                return true;
            }
            return false;
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            if (((IDictionary<string, object>)dict).Remove(item))
            {
                OnCollectionChanged(NotifyCollectionChangedAction.Remove, item.Key);
                try
                {
                    Delete(item.Key);
                }
                catch { }
                return true;
            }
            return false;
        }

        public bool TryGetValue(string key, out object value)
        {
            return ((IDictionary<string, object>)dict).TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IDictionary<string, object>)dict).GetEnumerator();
        }


        public string StoragePath { get; set; }
        
        static BinaryFormatter binaryFormatter = new BinaryFormatter();

        public void Save(string key)
        {
            string path = StoragePath;
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            if (dict.TryGetValue(key, out object value))
            {
                Stream fileStream = File.Create($"{path}\\{key}.bin");
                try
                {

                    if (value is IGetModel model)
                    {
                        value = new ModelTypeValue { Model = model.GetModel(), ModelViewType = model.GetType() };
                    }
                    binaryFormatter.Serialize(fileStream, value);
                }
                finally
                {
                    fileStream.Close();
                }
            }
        }

        public void Load(string key)
        {
            string path = $"{StoragePath}\\{key}.bin";
            Stream fileStream = File.OpenRead(path);
            object obj = null;
            try
            {
                obj = binaryFormatter.Deserialize(fileStream);
            }
            finally
            {
                fileStream.Close();
            }
            if (obj is ModelTypeValue mtv)
            {
                obj = Activator.CreateInstance(mtv.ModelViewType);
                ((IGetModel)obj).SetModel(mtv.Model);
            }
            this[key] = obj;
        }

        public void LoadAll()
        {
            string path = StoragePath;
            if (Directory.Exists(path))
                foreach (var item in Directory.EnumerateFiles(path, "*.bin"))
                {
                    try
                    {
                        var item2 = item.Split('\\').Last();
                        Load(item2.Substring(0, item2.Length - 4));
                    }
                    catch { }
                }
            else Directory.CreateDirectory(path);
        }

        public void Delete(string key)
        {
            string path = $"{StoragePath}\\{key}.bin";
            if (File.Exists(path)) File.Delete(path);
        }

    }

    [Serializable]
    public class ModelTypeValue
    {
        public Type ModelViewType { get; set; }
        public object Model { get; set; }
    }
}
