using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MapEditor
{

    class ViewOperation
    {
        public static ViewOperation m_instance = new ViewOperation();
        public bool m_bViewMode = false;
        public int m_nSelect = -1;
        public double m_fStartMapHeight = 0.0f;
        public double m_fStartMapWidth = 0.0f;

    }

}
