using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace WebApplication1
{
    public class XmlUtility
    {
        /// <summary>
        /// 查询节点下所有子节点的值写入集合
        /// </summary>
        /// <param name="xmlPath"></param>
        /// <param name="nodeName"></param>
        /// <param name="storeNo"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static List<string> SelectNode(string xmlPath, string nodeName, int storeNo)
        {
            try
            {
                List<string> list = new List<string>();
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(xmlPath);
                string node = "state" + storeNo.ToString();
                XmlNode xmlNode = xmlDocument.SelectSingleNode("//" + nodeName + "//" + node);
                foreach (XmlNode nodeValue in xmlNode.ChildNodes)
                {
                    list.Add(nodeValue.InnerText);
                }
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("获取节点异常", ex);
            }
        }
        /// <summary>
        /// 查询单个节点的值
        /// </summary>
        /// <param name="xmlPath"></param>
        /// <param name="storeNo"></param>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static string SelectSingleNode(string xmlPath, string nodeName, int storeNo)
        {
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(xmlPath);
                string node = "state" + storeNo.ToString();
                XmlNode xmlNode = xmlDocument.SelectSingleNode("//" + nodeName +"//" + node);
                return xmlNode.InnerText;
            }
            catch (Exception ex)
            {
                throw new Exception("获取GroupPalletInfo节点异常", ex);
            }
        }
        /// <summary>
        /// 查询节点并返回节点名称
        /// </summary>
        /// <param name="xmlPath"></param>
        /// <param name="parNodeName"></param>
        /// <param name="nodeValue"></param>
        /// <returns></returns>
        public static string SelectNodeName(string xmlPath, string parNodeName, string nodeValue) 
        {
            string NodeName = "";
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(xmlPath);
            XmlNode xmlNode = xmlDocument.SelectSingleNode("//" + parNodeName);
            for (int i = 0; i < xmlNode.ChildNodes.Count; i++)
            {
                if (xmlNode.ChildNodes[i].InnerText == nodeValue.ToString())
                {
                    NodeName = xmlNode.ChildNodes[i].Name;
                }
            }
            return NodeName;
        }

        /// <summary>
        /// 集合写入xml文件指定节; 集合最大值6
        /// </summary>
        /// <param name="xmlPath"></param>
        /// <param name="myList"></param>
        /// <param name="nodeParName"></param>
        /// <param name="nodeName"></param>
        public static void ListToXml(string xmlPath, List<string> myList, string nodeParName, string nodeName)
        {
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(xmlPath);
                XmlNode xmlNode = xmlDocument.SelectSingleNode("//" + nodeParName);
                for (int i = 0; i < xmlNode.ChildNodes.Count; i++)
                {
                    if (xmlNode.ChildNodes[i].Name == nodeName && i< myList.Count)
                    {
                        xmlNode.ChildNodes[i].InnerText = myList[i].ToString();
                    }
                }
                xmlDocument.Save(xmlPath);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// 集合写入xml文件指定节点
        /// </summary>
        /// <param name="xmlPath"></param>
        /// <param name="myList"></param>
        /// <param name="nodeName"></param>
        public static void ListToXml(string xmlPath, List<string> myList, int nodeName)
        {
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(xmlPath);
                string node = "state" + nodeName.ToString();
                XmlNode xmlNode = xmlDocument.SelectSingleNode("//" + node);
                for (int i = 0; i < xmlNode.ChildNodes.Count; i++)
                {
                    xmlNode.ChildNodes[i].InnerText = myList[i].ToString();
                }
                xmlDocument.Save(xmlPath);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        ///值写入xml文件指定节点
        /// </summary>
        /// <param name="xmlpath"></param>
        /// <param name="nodeName"></param>
        /// <param name="nodeValue"></param>
        public static void StringToXml(string xmlpath, string nodeName, string nodeValue)
        {
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(xmlpath);
                XmlNode xmlNode = xmlDocument.SelectSingleNode("//" + nodeName);
                xmlNode.InnerText = nodeValue;
                xmlDocument.Save(xmlpath);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static void StringToXml2(string xmlpath, string nodeName, int storeNo, string nodeValue)
        {
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(xmlpath);
                string node = "state"+ storeNo.ToString();
                XmlNode xmlNode = xmlDocument.SelectSingleNode("//" + nodeName + "//" + node);
                xmlNode.InnerText = nodeValue;
                xmlDocument.Save(xmlpath);
            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// xml转json
        /// </summary>
        /// <param name="xmlpath"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static string XmlToJson(string xmlPath)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlPath);
                // 将XML转换为JSON字符串
                string jsonString = JsonConvert.SerializeXmlNode(xmlDoc);
                // 输出JSON字符串
                return ConvertJsonString(jsonString.Substring(15, jsonString.Length - 16)); //去掉根节点
                //return ConvertJsonString(jsonString.Substring(21, jsonString.Length - 22));
            }
            catch (Exception ex)
            {
                throw new Exception("xml转josn异常", ex);
            }
        }
        /// <summary>
        /// 整理json格式
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ConvertJsonString(string str)
        {
            JsonSerializer serializer = new JsonSerializer();
            TextReader reader = new StringReader(str);
            JsonTextReader jsonreader = new JsonTextReader(reader);
            object ob = serializer.Deserialize(jsonreader);

            StringWriter writer = new StringWriter();
            JsonTextWriter jsonWriter = new JsonTextWriter(writer)
            {
                Formatting = Newtonsoft.Json.Formatting.Indented,
                Indentation = 4,  //缩进
                IndentChar = ' '  //空格
            };
            serializer.Serialize(jsonWriter, ob);
            return writer.ToString();
        }

        /// <summary>
        /// 按照节点名更新值
        /// </summary>
        /// <param name="xmlPath"></param>
        /// <param name="storeNo"></param>
        /// <param name="storeValue"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static bool UpdateXmlNode(string xmlPath, string nodeName, int storeNo, int storeValue)
        {
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(xmlPath);
                string node = "state" + storeNo.ToString();
                XmlNode xmlNode = xmlDocument.SelectSingleNode("//" + nodeName + "//" +node);
                xmlNode.InnerText = storeValue.ToString();
                xmlDocument.Save(xmlPath);
                GC.Collect();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("获取节点异常", ex);
            }
        }

        //写入西门子plc-字符串
        public static byte[] GetPLCStringByteArray(string str)
        {
            byte[] value = Encoding.Default.GetBytes(str);
            byte[] head = new byte[2];
            head[0] = Convert.ToByte(254);
            head[1] = Convert.ToByte(str.Length);
            value = head.Concat(value).ToArray();
            return value;
        }
    }
}