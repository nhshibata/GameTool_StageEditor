using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MapEditor
{

    /// <summary>
    /// TabItemに子要素を追加するためのadapterパターン
    /// </summary>
    public class DynamicTabItem : TabItem
    {
        object m_child = null;

        public void Add(object child)
        {
            this.AddChild(child);
            m_child = child;
        }

        public object GetChild()
        {
            return this.m_child;
        }
    }
}
