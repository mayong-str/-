using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Demo
{
    public partial class DeviceMap : Form
    {
        readonly string printIni = @"D:\xml\print.ini"; 
        public DeviceMap()
        {
            InitializeComponent();
        }

        private void btn_Update_Click(object sender, EventArgs e)
        {
            try
            {
                IniFileUtility iniFileUtility = new IniFileUtility(printIni);
                iniFileUtility.IniWriteValue("device", txt_line1.Text, txt_p_line1.Text);
                iniFileUtility.IniWriteValue("device", txt_line2.Text, txt_p_line2.Text);
                iniFileUtility.IniWriteValue("device", txt_line3.Text, txt_p_line3.Text);
                iniFileUtility.IniWriteValue("device", txt_line4.Text, txt_p_line4.Text);
                iniFileUtility.IniWriteValue("device", txt_line5.Text, txt_p_line5.Text);
                iniFileUtility.IniWriteValue("device", txt_line6.Text, txt_p_line6.Text);
                iniFileUtility.IniWriteValue("device", txt_line7.Text, txt_p_line7.Text);
                iniFileUtility.IniWriteValue("device", txt_line8.Text, txt_p_line8.Text);
                iniFileUtility.IniWriteValue("device", txt_line9.Text, txt_p_line9.Text);
                iniFileUtility.IniWriteValue("device", txt_line10.Text, txt_p_line10.Text);
                iniFileUtility.IniWriteValue("device", txt_line11.Text, txt_p_line11.Text);
                iniFileUtility.IniWriteValue("device", txt_line12.Text, txt_p_line12.Text);
                iniFileUtility.IniWriteValue("device", txt_line13.Text, txt_p_line13.Text);
                MessageBox.Show("更新完成","提示",MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            
        }
    }
}
