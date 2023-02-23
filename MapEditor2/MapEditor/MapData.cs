using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace MapEditor
{
    [DataContract]
    class MapData : jsonData
    {
        [DataMember]public int width;
        [DataMember]public int height;
        [DataMember]public List<int> noMap;

        public MapData()
            :base("MapData.json")
        {

        }

        protected override void ReadCallBack(object json)
        {
            throw new NotImplementedException();
        }
    }
}
