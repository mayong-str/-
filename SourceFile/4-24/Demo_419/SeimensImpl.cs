using S7.Net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo
{
    public class SeimensImpl : IPlcUtility
    {
        Plc Plc { get; set; }

        public bool IsRunning { get ; set ; }

        public SeimensImpl(string ip)
        {
            Plc = new Plc(S7.Net.CpuType.S71500, ip, 0, 0);
        }
        public bool close()
        {
            try { Plc.Close(); IsRunning = Plc.IsConnected; return true; } catch (Exception ex) { GlobalLog.WriteInfoLog("PLC_Close：" + ex.Message +":"+Plc.IsConnected); IsRunning = Plc.IsConnected; return false; }
        }

        public bool open()
        {
            try { Plc.Open(); IsRunning = Plc.IsConnected; return true; } catch (Exception ex) { GlobalLog.WriteInfoLog("PLC_Open：" + ex.Message + ":" + Plc.IsConnected); IsRunning = Plc.IsConnected; return false; }
        }

        public object ReadStruct(Type structType, int db)
        {
            try { object obj = Plc.ReadStruct(structType, db); return obj; } catch { return null; } 
        }

        public bool WriteStruct(object structValue, int db)
        {
            try { Plc.WriteStruct(structValue, db); return true; } catch { return false; }
        }

        public object ReadSingle(string variable)
        {
            try { object obj = Plc.Read(variable); return obj; } catch (Exception ex) { GlobalLog.WriteInfoLog("PLC_Read：" + ex.Message + ":" + Plc.IsConnected); IsRunning = Plc.IsConnected; return null; }
        }

        public bool WriteSingle(string variable, object value)
        {
            try { Plc.Write(variable, value); return true; } catch (Exception ex){ GlobalLog.WriteInfoLog("PLC_Write：" + ex.Message + ":" + Plc.IsConnected); IsRunning = Plc.IsConnected; return false; }
        }

        public bool ReadClasss(object sourceClass, int db)
        {
            try { object obj = Plc.ReadClass(sourceClass, db); if (obj != null) { return true; } else { return false; } } catch { return false; }
        }

        public bool WriteClass(object structValue, int db)
        {
            try { Plc.WriteClass(structValue, db); return true; } catch { return false; }
        }
        //读字符串
        public string ReadSingleString(DataType dataType, int db, int startByteAdr, VarType varType, int VarCount)
        {
            try { string code = Convert.ToString(Plc.Read(DataType.DataBlock, db, startByteAdr, varType, VarCount)); return code; } catch(Exception ex) { GlobalLog.WriteInfoLog("PLC_ReadString：" + ex.Message + ":" + Plc.IsConnected); IsRunning = Plc.IsConnected;  return ""; }
        }
        //读字符串长度
        public int ReadSingleStringLength(DataType dataType, int db, int startByteAdr, VarType varType, int VarCount)
        {
            try { int count = Convert.ToInt32(Plc.Read(DataType.DataBlock, db, startByteAdr, varType, VarCount)); return count; } catch(Exception ex) { GlobalLog.WriteInfoLog("PLC_ReadStringLength：" + ex.Message + ":" + Plc.IsConnected); IsRunning = Plc.IsConnected; return -1; }
        }
    }
}
