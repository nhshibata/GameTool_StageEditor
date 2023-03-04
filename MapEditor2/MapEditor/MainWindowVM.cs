using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MapEditor
{
    class MainWindowVM
    {
        System.Windows.HorizontalAlignment horizontalAlignment = System.Windows.HorizontalAlignment.Right;
        ViewOperation viewOperation = ViewOperation.m_instance;
        ToolSetting tool = ToolSetting.m_instance;

        //--- ビューモデル（ﾃﾞｰﾀの橋渡し

        public string Name
        {
            get { return tool.m_name; }
            set { tool.m_name = value; }
        }

        public bool BEnable { get { return tool.m_bEnable; } set { tool.m_bEnable = value; } }

        public int Kind { get { return tool.m_nKind; } set { tool.m_nKind = value; } }

        public int PanelWidth { get { return tool.m_nPanelW; } set { tool.m_nPanelW = value; } }
        public int PanelHeight { get { return tool.m_nPanelH; } set { tool.m_nPanelH = value; } }

        public int MapGridSize { get { return tool.m_nGridSize; } set { tool.m_nGridSize = value; } }


        public HorizontalAlignment HorizontalAlignment1 { get => horizontalAlignment; set => horizontalAlignment = value; }

        public string ExeFolderPath { get { return tool.folderPath; } set { tool.folderPath = value; } }
        public string CSVFolderPath { get { return tool.csvFile; } set { tool.csvFile = value; } }

        public bool ViewMode { get => ViewOperation.m_instance.m_bViewMode; set => ViewOperation.m_instance.m_bViewMode = value; }
    }
}
