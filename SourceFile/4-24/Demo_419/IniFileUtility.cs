using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Demo
{
    public class IniFileUtility
    {
        public string Path;
        public IniFileUtility(string path)
        {
            this.Path = path;
        }
        #region 声明读写INI文件的API函数
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string defVal, StringBuilder retVal, int size, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string defVal, Byte[] retVal, int size, string filePath);
        #endregion
        /// <summary>
        /// 写ini文件
        /// </summary>
        /// <param name="section">段</param>
        /// <param name="key">键</param>
        /// <param name="iValue">值</param>
        public  void IniWriteValue(string section, string key, string iValue)
        {
            WritePrivateProfileString(section, key, iValue, this.Path);
        }
        /// <summary>
        /// 读ini文件
        /// </summary>
        /// <param name="section">段</param>
        /// <param name="key">键</param>
        /// <returns>返回值</returns>
        public string IniReadValue(string section, string key)
        {
            StringBuilder temp = new StringBuilder(255);

            int i = GetPrivateProfileString(section, key, "", temp, 255, this.Path);
            return temp.ToString();
        }
    }
}
