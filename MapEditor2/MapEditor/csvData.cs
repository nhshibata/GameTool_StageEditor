using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace MapEditor
{
   
    public class csvData
    {
        public string m_filePath;
        public string m_saveFilePath;
        public List<string> m_aData = new List<string>();
        public List<int> m_aMap = new List<int>();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="path"></param>
        public csvData(string path)
        {
            m_saveFilePath = path;
        }


        public void Load()
        {
            StreamReader sr = new StreamReader(m_filePath);
            {
                while (!sr.EndOfStream)
                {
                    // 一行を読みこむ
                    string line = sr.ReadLine();

                    // 一行をカンマ毎に分けて配列に格納する
                    string[] aStr = line.Split(',');

                    // 配列からリストに格納する
                    m_aData = new List<string>();
                    m_aData.AddRange(aStr);

                }
                sr.Close();
            }
        }

        public void Write()
        {
            // 保存先ファイルが存在するか
            if(!File.Exists(m_saveFilePath))
            {
                return;
            }

            StreamWriter sw = new StreamWriter(m_saveFilePath);
            {
                // 一行ずつ書き込み
                for (int column = 0; column < m_aData.Count; column++)
                {                    
                    sw.WriteLine(m_aData[column]);
                }
            }
            sw.Close();

        }


    }
}
