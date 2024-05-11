using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Demo
{
    public partial class StoreInfo : Form
    {
        readonly string storeXml = @"D:\xml\store.xml"; //库位
        int Store = 1;
        public StoreInfo()
        {
            InitializeComponent();
        }

        public void ShowInfo(List<string> list, int storeNo)
        {
            Store = storeNo;
            txt_Code1.Text = list[0];
            txt_Code2.Text = list[1];
            txt_Code3.Text = list[2];
            txt_Code4.Text = list[3];
            txt_Code5.Text = list[4];
            txt_Code6.Text = list[5];
        }

        private void btn_StoreNumber_Click(object sender, EventArgs e)
        {
            bool updateStoreNumber = XmlUtility.UpdateXmlNode(storeXml, "StoreInfo", Store, Convert.ToInt32(com_StoreNumber.Text));
            if (updateStoreNumber)
            {
                MessageBox.Show("更改完成！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("更改失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btn_Out_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
