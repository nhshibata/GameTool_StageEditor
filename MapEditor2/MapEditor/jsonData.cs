using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;

namespace MapEditor
{
    [DataContract]
    abstract class jsonData
    {
        public string m_filePath;

        public jsonData(string path)
        {
            m_filePath = path;
        }

        public void Write()
        {
            if (m_filePath == null)
                return;

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(GetType());

            using (MemoryStream stream = new MemoryStream())
            {
                serializer.WriteObject(stream, this);
                File.WriteAllBytes(m_filePath, stream.ToArray());
                stream.Close();
            }
        }

        public void Read()
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(GetType());
            byte[] bytes = Encoding.UTF8.GetBytes(File.ReadAllText(m_filePath));

            using (MemoryStream stream = new MemoryStream(bytes))
            {
                object json = null;
                json = serializer.ReadObject(stream);
                ReadCallBack(json);
                stream.Close();
            }
        }


        protected abstract void ReadCallBack(object json);
    }
}
