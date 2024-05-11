using HslCommunication.LogNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo
{
    class GlobalLog
    {
        private static ILogNet logNet = new LogNetDateTime(Application.StartupPath + "\\Logs\\", GenerateMode.ByEveryDay);

        static public void WriteInfoLog(string strMsg)
        {
            logNet.WriteInfo(strMsg);
        }
        static public void WriteWarningLog(string strMsg)
        {
            logNet.WriteWarn(strMsg);
        }
        static public void WriteErrorLog(string strMsg)
        {
            logNet.WriteError(strMsg);
        }
        static public void WriteErrorLog(string strMsg, string strExMsg)
        {
            logNet.WriteError(strMsg + strExMsg);
        }
    }
}
