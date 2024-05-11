using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo
{
    public class SeimensFactory : IPlcFactory
    {
        public IPlcUtility CreatePlc(string ip)
        {
            return new SeimensImpl(ip);
        }
    }
}
