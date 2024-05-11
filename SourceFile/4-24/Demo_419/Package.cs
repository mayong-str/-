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
    public partial class Package : Form
    {
        readonly string zutuoXml = @"D:\xml\zutuo.xml"; //组托码
        public Package()
        {
            InitializeComponent();
        }

        private void btn_Query_Click(object sender, EventArgs e)
        {
            List<string> list = new List<string>();
            list.Add(txt_Code1.Text);
            list.Add(txt_Code2.Text);
            list.Add(txt_Code3.Text);
            list.Add(txt_Code4.Text);
            list.Add(txt_Code5.Text);
            list.Add(txt_Code6.Text);
            XmlUtility.ListToXml(zutuoXml, list, "codes");
            this.Close();
        }
    }
}
