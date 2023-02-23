using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// ソリューションエクスプローラー > 参照 > 参照の追加 > 
// System.Runtime.Serialization;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace MapEditor
{

    /// <summary>
    /// モデルで扱うﾃﾞｰﾀ
    /// </summary>
    [DataContract] // ｸﾗｽや構造体をjsonとして保存するためのAttribute
    class ToolSetting : jsonData
    {
        //--- シングルトン用インスタンス
        public static ToolSetting m_instance = new ToolSetting();

        //--- メンバ変数
        [DataMember]
        public string m_name = "test";
        [DataMember]
        public bool m_bEnable = true;
        [DataMember]
        public int m_nKind = 1;
        [DataMember]
        public int m_nPanelW = 3;
        [DataMember]
        public int m_nPanelH = 3;

        [DataMember]
        public string folderPath = string.Empty; // 実行ファイルが格納されている場所
        [DataMember]
        public int m_nGridSize = 30;

        [DataMember]
        public string csvFile = string.Empty; // 実行ファイルが格納されている場所

        public ToolSetting()
            :base("ToolSetting.json")
        {
            Read();
        }

        ~ToolSetting()
        {
            Write();
        }

        protected override void ReadCallBack(object json)
        {
            // 変換
            ToolSetting data = json as ToolSetting;

            if (data == null)
                return;

            m_bEnable = data.m_bEnable;
            m_nKind = data.m_nKind;
            m_name = data.m_name;
            m_nPanelH = data.m_nPanelH;
            m_nPanelW = data.m_nPanelW;
            m_nGridSize = data.m_nGridSize;

            // ??(Null合体演算子)
            folderPath = data.folderPath ?? "";

           // csvFile = data.csvFile ?? "";
        }
    }
}
