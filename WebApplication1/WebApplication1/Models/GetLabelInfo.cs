using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class GetLabelInfo
    {
        public string custInvName { get; set; } //物料名称
        public string custInvSpecies { get; set;} //物料规格
        public string lengthInM { get; set; } //长度
        public string devCode { get; set; } 
        public string widthInMM { get; set; } //宽幅
        public string barcode { get; set;} //条码
        public string custBarcode { get; set;} //客户条码
        public string stacking { get; set;} //叠托方式
        public string boxWidthInMM { get; set;} //纸箱宽幅
        public string tubeCoreDiameter { get; set;} //管芯卷径
        public string trayLength { get; set;} 
        public string trayWidth { get; set;} 


    }
}