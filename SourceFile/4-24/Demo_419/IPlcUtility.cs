using S7.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo
{
    public interface IPlcUtility
    {
        bool open();
        bool close();
        object ReadStruct(Type structType, int db);
        bool WriteStruct(object structValue, int db);
        object ReadSingle(string variable);
        bool WriteSingle(string variable, object value);
        
        string ReadSingleString(DataType dataType, int db, int startByteAdr, VarType varType, int VarCount);
        int ReadSingleStringLength(DataType dataType, int db, int startByteAdr, VarType varType, int VarCount);
        bool ReadClasss(object sourceClass, int db);
        bool WriteClass(object structValue, int db);
        bool IsRunning { get; set; }
    }
}
