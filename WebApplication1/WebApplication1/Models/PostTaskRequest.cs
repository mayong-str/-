using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class PostTaskRequest
    {
        public string RequestCode { get; set; } //请求编号
        public string Task_No { get; set; } //任务编号
        public string Task_Type { get; set; } //任务类型
        public string PLINState { get; set; } //入料平台编号
        public string JiTaiNo { get; set; } //库位编号
    }
}