using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class PostTaskOver
    {
        public string RequestCode { get; set; }
        public string Task_No { get; set; }
        public string Task_Type { get; set; }
        public Data Data {  get; set; }
    }

    public class Data
    {
        public List<string> QsCodeArray { get; set; }
    }
}