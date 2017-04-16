using System.IO;
using System.Xml.Serialization;

namespace DragonWar.Utils.Config
{
    public  class Configuration<T>
    {
        public void CreateDefaultFolder()
        {
            string folder = "Configuration";
            if(!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
        }

        public void WriteXml()
        {
            CreateDefaultFolder();
            string path = Path.Combine(
                "Configuration",
                string.Format("{0}.xml", GetType().Name));
            var writer = new XmlSerializer(GetType());
            var file = new StreamWriter(path);
            writer.Serialize(file, this);
            file.Close();
        }
        public static T ReadXml()
        {
   
            string path = Path.Combine(
                "Configuration",
                string.Format("{0}.xml", typeof(T).Name));

            if (File.Exists(path))
            {
                var reader = new XmlSerializer(typeof(T));
                var file = new StreamReader(path);
               
               T xml = (T)reader.Deserialize(file);

                file.Close();
                return xml;

            }
            else return default(T);
        }
    }
}
