using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using S7.Net;
using SeimensPLC;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Web.Http;
using WebApplication1.Models;
using static System.Net.WebRequestMethods;

namespace WebApplication1.Controllers
{
    public class ValuesController : ApiController
    {
        static string storeXml = @"D:\xml\store.xml"; //库位   //C:\Program Files\IIS Express\store.xml
        static string materialQRXml = @"D:\xml\materialQR.xml"; //物料二维码
        static string ResTaskXml = @"D:\xml\WebApp\ResTask.xml";  //申请任务-出库
        static string ResTask1Xml = @"D:\xml\WebApp\ResTask1.xml";  //申请任务-上料
        static string ResProductStateXml = @"D:\xml\WebApp\ResProductState.xml";  //获取生产线状态
        static string zutuo1Xml = @"D:\xml\WebApp\zutuo1.xml";  //返回组托
        static string postTaskOverXml = @"D:\xml\WebApp\PostTaskOver.xml";  //任务执行通知-出库
        static string postTaskOver1Xml = @"D:\xml\WebApp\PostTaskOver1.xml";  //任务执行通知-上料
        static string taskNoXml = @"D:\xml\taskno.xml";  //保存任务号
        static string materialSizeXml = @"D:\xml\materialSize.xml";  //保存物料尺寸信息
        readonly string printIni = @"D:\xml\print.ini";  //printer信息
        private static readonly object OResProductState = new object();
        string DB100DBW = "DB100.DBW";
        string DB207DBW = "DB207.DBW";

        object objProductState = new object();
        [HttpGet]
        [Route("ProductionLineState")]
        // 获取生产线状态（库位）接口
        public object Get()
        {
            lock (OResProductState)
            {
                return JsonConvert.DeserializeObject(XmlUtility.XmlToJson(ResProductStateXml)); 
            }
        }


        [HttpGet]
        [Route("api/AutoPack/GetProInfo")]
        public object Get(string code)
        {
            string labInfoXml = @"D:\xml\labinfo.xml";
            return JsonConvert.DeserializeObject(XmlUtility.XmlToJson(labInfoXml));
        }


        [HttpPost]
        [Route("PostTaskRequest")]
        // 任务申请接口
        public object Post([FromBody] Models.PostTaskRequest postTaskRequest)
        {
            int Task_Type = Convert.ToInt16(postTaskRequest.Task_Type); //任务类型：1：上料 2：出库
            if (Task_Type == 1)
            {
                lock (objProductState)
                {
                    string json = JsonConvert.SerializeObject(postTaskRequest);
                    GlobalLog.WriteInfoLog("上料任务：" + XmlUtility.ConvertJsonString(json));
                    //int PLINState = Convert.ToInt16(postTaskRequest.PLINState); //上料号
                    string requestCode = postTaskRequest.RequestCode;
                    XmlUtility.StringToXml(ResTask1Xml, "RequestCode", requestCode);
                    string task_No = postTaskRequest.Task_No;
                    XmlUtility.StringToXml(ResTask1Xml, "Task_No", task_No);
                    string task_Type = postTaskRequest.Task_Type;
                    XmlUtility.StringToXml(ResTask1Xml, "Task_Type", task_Type);
                    XmlUtility.StringToXml(ResTask1Xml, "data", "");
                    //任务编号和机台号绑定，写入xml
                    int plinState = Convert.ToInt32(postTaskRequest.PLINState);
                    //获取生产线状态 上料口置1
                     

                    XmlUtility.StringToXml2(taskNoXml, "up", plinState, task_No);
                    IniFileUtility iniFileUtility = new IniFileUtility(printIni);
                    string plcIp = iniFileUtility.IniReadValue("PLC1","Ip");
                    Plc Plc = new Plc(S7.Net.CpuType.S71500, plcIp, 0, 0);
                    Plc.Open();
                    ////上料工位状态占用
                    //int[] v = new int[7] { 716, 1434, 2152, 2870, 3588, 4306, 5024 };
                    //string Wvariable = DB100DBW + v[plinState - 1].ToString();
                    //Plc.Write(Wvariable, Convert.ToInt16(1));
                    //申请光栅
                    //int[] g = new int[7] { 5026, 5028, 5030, 5032, 5034, 5036, 5038 };
                    int[] g = new int[7] { 5026, 5026, 5026, 5026, 5026, 5026, 5026 };
                    string Gvariable = DB100DBW + g[plinState - 1].ToString();
                    Plc.Write(Gvariable, Convert.ToInt16(1));
                    Plc.Close();
                    GlobalLog.WriteInfoLog("上料任务反馈：" + JsonConvert.DeserializeObject(XmlUtility.XmlToJson(ResTask1Xml)));
                    return JsonConvert.DeserializeObject(XmlUtility.XmlToJson(ResTask1Xml));
                } 
            }
            else
            {
                lock (objProductState)
                {
                    string json = JsonConvert.SerializeObject(postTaskRequest);
                    GlobalLog.WriteInfoLog("出库任务：" + XmlUtility.ConvertJsonString(json));
                    string requestCode = postTaskRequest.RequestCode;
                    XmlUtility.StringToXml(ResTaskXml, "RequestCode", requestCode);
                    string task_No = postTaskRequest.Task_No;
                    XmlUtility.StringToXml(ResTaskXml, "Task_No", task_No);
                    string task_Type = postTaskRequest.Task_Type;
                    XmlUtility.StringToXml(ResTaskXml, "Task_Type", task_Type);
                    int jiTaiNo = Convert.ToInt16(postTaskRequest.JiTaiNo); //库位号 
                                                                            //任务编号和库位号绑定，写入ini
                    XmlUtility.StringToXml2(taskNoXml, "down", jiTaiNo, task_No);
                    //int JiTaiNo = 1; //库位号 
                    List<string> list = XmlUtility.SelectNode(materialQRXml, "MaterialQRInfo", jiTaiNo);  //获取物料二维码
                    XmlUtility.ListToXml(ResTaskXml, list, "data", "GroupPalletCode");
                    string GroupPallet = XmlUtility.SelectSingleNode(materialQRXml, "GroupPalletInfo", jiTaiNo); //获取组托码
                    XmlUtility.StringToXml(ResTaskXml, "QsCodeArray", GroupPallet);

                    IniFileUtility iniFileUtility = new IniFileUtility(printIni);
                    string plcIp = iniFileUtility.IniReadValue("PLC2", "Ip");
                    Plc Plc = new Plc(S7.Net.CpuType.S71500, plcIp, 0, 0);
                    Plc.Open();
                    int[] g = new int[13] { 2, 4, 6, 8, 10, 12, 14, 16, 18, 20, 22, 24, 26 };
                    string Gvariable = "DB207.DBW" + g[jiTaiNo - 1].ToString();
                    Plc.Write(Gvariable, Convert.ToInt16(1));
                    Plc.Close();
                    GlobalLog.WriteInfoLog("出库任务反馈：" + JsonConvert.DeserializeObject(XmlUtility.XmlToJson(ResTaskXml)));
                    return JsonConvert.DeserializeObject(XmlUtility.XmlToJson(ResTaskXml));
                }   
            }
        }

        //组托
        [HttpPost]
        [Route("api/AutoPack/ToPackage")]
        public object Post([FromBody] Models.Parameter Parameter) 
        {
            return JsonConvert.DeserializeObject(XmlUtility.XmlToJson(zutuo1Xml));
        }

        //任务执行通知
        [HttpPost]
        [Route("PostTaskOver")]
        public object Post([FromBody] Models.PostTaskOver postTaskOver)
        {
            string task_Type = postTaskOver.Task_Type;
            if (task_Type == "1")
            {
                //lock (objProductState)
                lock (OResProductState)
                {
                    string json = JsonConvert.SerializeObject(postTaskOver);
                    GlobalLog.WriteInfoLog("上料任务执行完成通知：" + XmlUtility.ConvertJsonString(json));
                    string requestCode = postTaskOver.RequestCode;
                    XmlUtility.StringToXml(postTaskOver1Xml, "RequestCode", requestCode);
                    string task_No = postTaskOver.Task_No;
                    //获取上料编号
                    int PLINState = Convert.ToInt32(XmlUtility.SelectNodeName(taskNoXml, "up", task_No).Substring(5));
                    XmlUtility.UpdateXmlNode(ResProductStateXml, "Feeding", PLINState, 1); //AGV上料后直接置为1 后面可注释
                    XmlUtility.StringToXml(postTaskOver1Xml, "Task_No", task_No);
                    XmlUtility.StringToXml(postTaskOver1Xml, "Task_Type", task_Type);
                    XmlUtility.ListToXml(postTaskOver1Xml, postTaskOver.Data.QsCodeArray, "data", "QsCodeArray");
                    #region //执行获取标签信息接口,将尺寸信息写入materialQRXml，以便写入PLC
                    int[] address = null;
                    if (PLINState == 1)
                    {
                        address = new int[16] { 0, 2, 4, 6, 8, 10, 12, 102, 358, 360, 362, 364, 366, 368, 370, 460 }; //8
                    }
                    else if (PLINState == 2)
                    {
                        address = new int[16] { 718, 720, 722, 724, 726, 728, 730, 820, 1076, 1078, 1080, 1082, 1084, 1086, 1088, 1178 };
                    }
                    else if (PLINState == 3)
                    {
                        address = new int[16] { 1436, 1438, 1440, 1442, 1444, 1446, 1448, 1538, 1794, 1796, 1798, 1800, 1802, 1804, 1806, 1896 };
                    }
                    else if (PLINState == 4)
                    {
                        address = new int[16] { 2154, 2156, 2158, 2160, 2162, 2164, 2166, 2256, 2512, 2514, 2516, 2518, 2520, 2522, 2524, 2614 };
                    }
                    else if (PLINState == 5)
                    {
                        address = new int[16] { 2872, 2874, 2876, 2878, 2880, 2882, 2884, 2974, 3230, 3232, 3234, 3236, 3238, 3240, 3242, 3332 };
                    }
                    else if (PLINState == 6)
                    {
                        address = new int[16] { 3590, 3592, 3594, 3596, 3598, 3600, 3602, 3692, 3948, 3950, 3952, 3954, 3956, 3958, 3960, 4050 };
                    }
                    else if (PLINState == 7)
                    {
                        address = new int[16] { 4308, 4310, 4312, 4314, 4316, 4318, 4320, 4410, 4666, 4668, 4670, 4672, 4674, 4676, 4678, 4768 };
                    }
                    IniFileUtility iniFileUtility = new IniFileUtility(printIni);
                    string plcIp = iniFileUtility.IniReadValue("PLC1", "Ip");
                    string plc2Ip = iniFileUtility.IniReadValue("PLC2", "Ip");
                    string GetProInfo = iniFileUtility.IniReadValue("Http", "GetProInfo");                 
                    Plc Plc = new Plc(S7.Net.CpuType.S71500, plcIp, 0, 0);
                    Plc.Open();
                    //上料工位状态占用
                    int[] gongwei = new int[7] { 716, 1434, 2152, 2870, 3588, 4306, 5024 };
                    string Gvariable = DB100DBW + gongwei[PLINState - 1].ToString();
                    Plc.Write(Gvariable, Convert.ToInt16(1));

                    for (int i = 0; i < postTaskOver.Data.QsCodeArray.Count; i++)
                    {
                        //string url = "http://122.224.159.38:8990/api/AutoPack/GetProInfo" + "?code=" + postTaskOver.Data.QsCodeArray[i]; //获取条码信息
                        string url = GetProInfo + "?code=" + postTaskOver.Data.QsCodeArray[i]; //获取条码信息
                        string resParameter = HttpServer.GetUrl(url);
                        if (resParameter.Substring(0,2) == "-1")
                        {
                            GlobalLog.WriteInfoLog("访问ERP系统时获取条码信息：" + resParameter.Substring(2));
                            Plc Plc2 = new Plc(S7.Net.CpuType.S71500, plc2Ip, 0, 0);
                            Plc2.Open();
                            Plc2.Write("DB200.DBW8", Convert.ToInt16(1));
                            Plc2.Close();
                        }
                        else
                        {
                            GlobalLog.WriteInfoLog("获取条码信息：" + resParameter);
                        }
                        object pairs = JsonConvert.DeserializeObject(resParameter);
                        JObject jo = JObject.Parse(pairs.ToString());
                        GetLabelInfo getLabelInfo = new GetLabelInfo();
                        string status = jo["status"].ToString();
                        GlobalLog.WriteInfoLog("上料任务执行完成通知返回参数状态：" + status);
                        if (status == "0")
                        {
                            getLabelInfo.custInvName = jo["data"]["custInvName"].ToString();
                            getLabelInfo.custInvSpecies = jo["data"]["custInvSpecies"].ToString();
                            getLabelInfo.lengthInM = jo["data"]["lengthInM"].ToString();
                            getLabelInfo.devCode = jo["data"]["devCode"].ToString();
                            getLabelInfo.widthInMM = jo["data"]["widthInMM"].ToString();
                            getLabelInfo.barcode = jo["data"]["barcode"].ToString();
                            getLabelInfo.custBarcode = jo["data"]["custBarcode"].ToString();
                            getLabelInfo.stacking = jo["data"]["stacking"].ToString();
                            getLabelInfo.boxWidthInMM = jo["data"]["boxWidthInMM"].ToString();
                            getLabelInfo.tubeCoreDiameter = jo["data"]["tubeCoreDiameter"].ToString();
                            getLabelInfo.trayLength = jo["data"]["trayLength"].ToString();
                            getLabelInfo.trayWidth = jo["data"]["trayWidth"].ToString();

                            List<string> labInfo = new List<string>();
                            labInfo.Add(jo["data"]["custInvName"].ToString());
                            labInfo.Add(jo["data"]["custInvSpecies"].ToString());
                            labInfo.Add(jo["data"]["lengthInM"].ToString()); //长度
                            labInfo.Add(jo["data"]["devCode"].ToString());  //机台号
                            labInfo.Add(jo["data"]["widthInMM"].ToString()); //宽度
                            labInfo.Add(jo["data"]["barcode"].ToString());
                            labInfo.Add(jo["data"]["custBarcode"].ToString());
                            labInfo.Add(jo["data"]["stacking"].ToString());
                            labInfo.Add(jo["data"]["boxWidthInMM"].ToString()); //纸箱宽度
                            labInfo.Add(jo["data"]["tubeCoreDiameter"].ToString()); //管芯直径
                            labInfo.Add(jo["data"]["trayLength"].ToString());  //托盘长度
                            labInfo.Add(jo["data"]["trayWidth"].ToString());

                            //物料尺寸信息保存到xml，以便后面写入PLC
                            XmlUtility.ListToXml(materialSizeXml, labInfo, PLINState);

                            #region //标签信息写入PLC，并计数（入库物料+1）修改库位值（+1）
                            List<string> labInfo2 = new List<string>();
                            //labInfo2.Add(getLabelInfo.lengthInM);
                            labInfo2.Add(getLabelInfo.tubeCoreDiameter); //getLabelInfo.widthInMM长度 -->getLabelInfo.tubeCoreDiameter纸芯长度
                            labInfo2.Add("530"); //宽度
                            labInfo2.Add("0"); //重量
                            string devCode = iniFileUtility.IniReadValue("device", getLabelInfo.devCode);
                            labInfo2.Add(devCode); //机台号
                            labInfo2.Add(getLabelInfo.trayLength); //托盘长度
                            labInfo2.Add(getLabelInfo.tubeCoreDiameter); //纸芯长度
                            labInfo2.Add(getLabelInfo.boxWidthInMM); //纸箱长度
                            //labInfo2.Add(getLabelInfo.barcode);
                            labInfo2.Add(postTaskOver.Data.QsCodeArray[i]);
                            //Plc Plc = new Plc(S7.Net.CpuType.S71500, "192.168.1.1", 0, 0);
                            //Plc.Open();
                            int k = 0;
                            int start;
                            int index = (((i + 1) * 8) - 1);
                            if (i == 0)
                            {
                                start = 0;
                            }
                            else
                            {
                                start = 8;
                            }

                            for (int j = start; j < index; j++)
                            {
                                string variable = DB100DBW + address[j].ToString();
                                if (j == 3 || j == 11)
                                {
                                    Plc.Write(variable, Convert.ToInt16(labInfo2[k].Substring(2)));
                                }
                                else
                                {
                                    Plc.Write(variable, Convert.ToInt16(labInfo2[k]));
                                }
                                k++;
                            }
                            //写入物料二维码
                            string boarCode = DB100DBW + address[index].ToString();
                            Plc.Write(boarCode, XmlUtility.GetPLCStringByteArray(labInfo2[7]));  
                            ////物料尺寸写入完成-2
                            //int[] v = new int[7] { 716, 1434, 2152, 2870, 3588, 4306, 5024 };
                            //string Wvariable = DB100DBW + v[PLINState - 1].ToString();
                            //Plc.Write(Wvariable, Convert.ToInt16(2));
                            ////光栅复位-0
                            //int[] g = new int[7] { 5026, 5028, 5030, 5032, 5034, 5036, 5038 };
                            //string Gvariable = DB100DBW + g[PLINState - 1].ToString();
                            //Plc.Write(Gvariable, Convert.ToInt16(0));
                            //Plc.Close();
                            #endregion
                            XmlUtility.StringToXml(postTaskOver1Xml, "res_code", "0");
                            XmlUtility.StringToXml(postTaskOver1Xml, "res_msg", "成功");
                            XmlUtility.StringToXml(postTaskOver1Xml, "RunResult", "OK");
                        }
                        else
                        {
                            //IniFileUtility iniFileUtility = new IniFileUtility(printIni);
                            iniFileUtility.IniWriteValue("window", "window", "1");
                            iniFileUtility.IniWriteValue("window", "value", jo["msg"].ToString());
                            XmlUtility.StringToXml(postTaskOver1Xml, "res_code", "-1");
                            XmlUtility.StringToXml(postTaskOver1Xml, "res_msg", "失败");
                            XmlUtility.StringToXml(postTaskOver1Xml, "RunResult", "NG");
                        }
                    }
                    //物料尺寸写入完成-2
                    int[] v = new int[7] { 716, 1434, 2152, 2870, 3588, 4306, 5024 };
                    string Wvariable = DB100DBW + v[PLINState - 1].ToString();
                    Plc.Write(Wvariable, Convert.ToInt16(2));
                    //光栅复位-0
                    int[] g = new int[7] { 5026, 5026, 5026, 5026, 5026, 5026, 5026 };
                    string Gvariable1 = DB100DBW + g[PLINState - 1].ToString();
                    Plc.Write(Gvariable1, Convert.ToInt16(0));

                    Plc.Close();
                    GlobalLog.WriteInfoLog("上料任务执行完成通知反馈：" + JsonConvert.DeserializeObject(XmlUtility.XmlToJson(postTaskOver1Xml)));
                    return JsonConvert.DeserializeObject(XmlUtility.XmlToJson(postTaskOver1Xml));
                }
                #endregion                               
            }
            else
            {
                lock (OResProductState)
                {
                    string json = JsonConvert.SerializeObject(postTaskOver);
                    GlobalLog.WriteInfoLog("出库任务执行完成通知：" + XmlUtility.ConvertJsonString(json));
                    string requestCode = postTaskOver.RequestCode;
                    XmlUtility.StringToXml(postTaskOverXml, "RequestCode", requestCode);
                    string task_No = postTaskOver.Task_No;
                    int JiTaiNo = Convert.ToInt32(XmlUtility.SelectNodeName(taskNoXml, "down", task_No).Substring(5)); //库位号
                    XmlUtility.StringToXml(postTaskOverXml, "Task_No", task_No);
                    XmlUtility.StringToXml(postTaskOverXml, "Task_Type", task_Type);

                    XmlUtility.UpdateXmlNode(storeXml, "StoreInfo", JiTaiNo, 0);
                    XmlUtility.UpdateXmlNode(ResProductStateXml, "ColStorehouseState", JiTaiNo, 0); ////设置生产线状态中库位为：0-不允许出库

                    IniFileUtility iniFileUtility = new IniFileUtility(printIni);
                    string plcIp = iniFileUtility.IniReadValue("PLC2", "Ip");
                    Plc Plc = new Plc(S7.Net.CpuType.S71500, plcIp, 0, 0);
                    Plc.Open();
                    //光栅复位 -出库
                    int[] g = new int[13] { 2, 4, 6, 8, 10, 12, 14, 16, 18, 20, 22, 24, 26 };
                    string Gvariable = DB207DBW + g[JiTaiNo - 1].ToString();
                    Plc.Write(Gvariable, Convert.ToInt16(0));
                    XmlUtility.UpdateXmlNode(ResProductStateXml, "Grating", JiTaiNo, 0);
                    GlobalLog.WriteInfoLog("出库任务执行完成通知反馈：" + JsonConvert.DeserializeObject(XmlUtility.XmlToJson(postTaskOverXml)));
                    //AGV出库完成
                    int[] address = new int[13] { 2, 4, 6, 8, 10, 12, 14, 16, 18, 20, 22, 24, 26 };
                    string AGVku = "DB209.DBW" + address[JiTaiNo - 1].ToString();
                    Plc.Write(AGVku, Convert.ToInt16(1));
                    Plc.Close();
                    return JsonConvert.DeserializeObject(XmlUtility.XmlToJson(postTaskOverXml));
                }
            }
        }



        // PUT api/values/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
