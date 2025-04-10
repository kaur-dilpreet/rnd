using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace Reports.Core.Caching
{
    public static class FileBasedCahce
    {
        static Dictionary<string, string> _FileMap;
        const string MAPFILENAME = "FileBasedCahceMAP.dat";
        //public static string DirectoryLocation = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Cache");
        public static string DirectoryLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DataCache");
        static FileBasedCahce()
        {
            if (!Directory.Exists(DirectoryLocation))
                Directory.CreateDirectory(DirectoryLocation); // throw new ArgumentException("directoryLocation msu exist");

            if (File.Exists(MyMapFileName))
            {
                try
                {
                    _FileMap = DeSerializeFromBin<Dictionary<string, string>>(MyMapFileName);
                }
                catch(Exception ex)
                {
                    _FileMap = new Dictionary<string, string>();
                }
            }
            else
                _FileMap = new Dictionary<string, string>();
        }

        public static void Remove(string key)
        {
            if (_FileMap != null && _FileMap.Keys != null)
            { 
                for (Int32 i = _FileMap.Keys.Count - 1; i >= 0; i--)
                {
                    String tempKey = _FileMap.Keys.ToList()[i];

                    if (!String.IsNullOrEmpty(tempKey))
                    {
                        if (tempKey.StartsWith(key))
                        {
                            System.IO.File.Delete(_FileMap[tempKey]);
                            _FileMap.Remove(tempKey);
                        }
                    }
                    else
                    {
                        _FileMap.Remove(tempKey);
                    }
                }

                SerializeToBin(_FileMap, MyMapFileName);
            }
        }

        public static void RemoveAll()
        {
            System.IO.DirectoryInfo di = new DirectoryInfo(DirectoryLocation);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }

            _FileMap = new Dictionary<string, string>();

            SerializeToBin(_FileMap, MyMapFileName);
        }

        public static T Get<T>(string key) where T : new()
        {
            if (_FileMap.ContainsKey(key))
                return (T)DeSerializeFromBin<T>(_FileMap[key]);
            else
                throw new ArgumentException("Key not found");
        }

        public static bool Contains(string key)
        {
            if (_FileMap.ContainsKey(key))
            {
                return System.IO.File.Exists(_FileMap[key]);
            }
            else
            {
                return false;
            }
        }

        public static void Insert<T>(string key, T value)
        {
            if (_FileMap.ContainsKey(key))
            {
                SerializeToBin(value, _FileMap[key]);
            }
            else
            {
                _FileMap.Add(key, GetNewFileName);
                SerializeToBin(value, _FileMap[key]);
            }
            SerializeToBin(_FileMap, MyMapFileName);
        }
        private static string GetNewFileName
        {
            get
            {
                return Path.Combine(DirectoryLocation, Guid.NewGuid().ToString());
            }
        }
        private static string MyMapFileName
        {
            get
            {
                return Path.Combine(DirectoryLocation, MAPFILENAME);
            }
        }
        private static void SerializeToBin(object obj, string filename)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filename));
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                using (FileStream fs = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    bf.Serialize(fs, obj);
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("The process cannot access the file"))
                {
                    System.Threading.Thread.Sleep(100);

                    SerializeToBin(obj, filename);
                }
                else
                {
                    throw ex;
                }
            }
        }

        private static T DeSerializeFromBin<T>(string filename) where T : new()
        {
            try
            {
                if (File.Exists(filename))
                {
                    T ret = new T();
                    System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
                    {
                        ret = (T)bf.Deserialize(fs);
                    }
                    return ret;
                }
                else
                    throw new FileNotFoundException(string.Format("file {0} does not exist", filename));
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("The process cannot access the file"))
                {
                    System.Threading.Thread.Sleep(100);

                    return DeSerializeFromBin<T>(filename);
                }
                else
                {
                    throw ex;
                }
            }
        }

    }
}
