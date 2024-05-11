using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Demo
{
    public class XmlUtility
    {
        /// <summary>
        /// 加载xml文件为字符串
        /// </summary>
        /// <param name="xmlPath"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static string XmlToString(string xmlPath)
        {
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(xmlPath);
                string xmlString = xmlDocument.InnerXml;
                xmlDocument = null;
                GC.Collect();
                return xmlString;
            }
            catch (Exception ex)
            {
                throw new Exception("加载xml文件异常", ex);
            }
        }

        /// <summary>
        /// xml序列化 对象转xml字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static string XmlSerialize<T>(T obj)
        {
            try
            {
                using (StringWriter sw = new StringWriter())
                {
                    XmlSerializer serializer = new XmlSerializer(obj.GetType());
                    serializer.Serialize(sw, obj);
                    sw.Close();
                    return sw.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("将实体对象转换成XML异常", ex);
            }
        }

        /// <summary>
        /// xml反序列化 xml字符串转对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strXML"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static T DESerializer<T>(string strXML) where T : class
        {
            try
            {
                using (StringReader sr = new StringReader(strXML))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    return serializer.Deserialize(sr) as T;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("将XML转换成实体对象异常", ex);
            }
        }

        /// <summary>
        /// xml文件转DataTable
        /// </summary>
        /// <param name="xmlPath"></param>
        /// <returns></returns>
        public static DataTable XmlToDataTable(string xmlPath)
        {
            try
            {
                DataSet ds = new DataSet();
                using (XmlReader xmlFile = XmlReader.Create(xmlPath, new XmlReaderSettings()))
                {
                    ds.ReadXml(xmlFile);
                    return ds.Tables[0];
                }
            }
            catch (Exception ex)
            {
                throw new Exception("将XML转换成DataTable异常", ex);
            }
        }
        /// <summary>
        /// 按照库位编号查找对应的物料二维码
        /// </summary>
        /// <param name="xmlpath"></param>
        /// <param name="storeNo"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static List<string> SelectNode(string xmlpath, int storeNo)
        {
            try
            {
                List<string> list = new List<string>();
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(xmlpath);
                string node = "state" + storeNo.ToString();
                XmlNode xmlNode = xmlDocument.SelectSingleNode("//MaterialQRInfo//" + node);
                for (int i = 0; i < xmlNode.ChildNodes.Count - 1; i++)
                {
                    list.Add(xmlNode.ChildNodes[i].InnerXml);
                }
                if (list.Count < 6)
                {
                    int count = 6 - list.Count;
                    for (int i = 0; i < count; i++)
                    {
                        list.Add("0");
                    }
                }
                xmlDocument = null;
                GC.Collect();
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("获取节点异常", ex);
            }
        }
        /// <summary>
        /// 根据节点名查找
        /// </summary>
        /// <param name="xmlPath"></param>
        /// <param name="storeNo"></param>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static string SelectNode(string xmlPath, string nodeName, int storeNo)
        {
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(xmlPath);
                string node = "state" + storeNo.ToString();
                XmlNode xmlNode = xmlDocument.SelectSingleNode("//" + nodeName + "//" + node);
                string str = xmlNode.InnerText;
                xmlDocument = null;
                GC.Collect();
                return str;
            }
            catch (Exception ex)
            {
                throw new Exception("获取GroupPalletInfo节点异常", ex);
            }
        }

        public static string SelectNode(string xmlPath, string nodeName, int storeNo, string childNodeName)
        {
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(xmlPath);
                string node = "state" + storeNo.ToString();
                XmlNode xmlNode = xmlDocument.SelectSingleNode("//" + nodeName + "//" + node + "//" + childNodeName);
                string str = xmlNode.InnerText;
                xmlDocument = null;
                GC.Collect();
                return str;
            }
            catch (Exception ex)
            {
                throw new Exception("获取GroupPalletInfo节点异常", ex);
            }
        }

        /// <summary>
        /// xml转json
        /// </summary>
        /// <param name="xmlpath"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static string XmlToJson(string xmlpath)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlpath);
                // 将XML转换为JSON字符串
                string jsonString = JsonConvert.SerializeXmlNode(xmlDoc);
                string str = ConvertJsonString(jsonString.Substring(15, jsonString.Length - 16));
                xmlDoc = null;
                GC.Collect();
                // 输出JSON字符串
                return str;
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
        public static bool UpdateXmlNode(string xmlPath, string nodeParName, int storeNo, int storeValue)
        {
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(xmlPath);
                string node = "state" + storeNo.ToString();
                XmlNode xmlNode = xmlDocument.SelectSingleNode("//" + nodeParName + "//" + node);
                xmlNode.InnerText = storeValue.ToString();
                xmlDocument.Save(xmlPath);
                xmlDocument = null;
                GC.Collect();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("获取节点异常", ex);
            }
        }
        public static bool UpdateXmlNode2(string xmlPath, string nodeParName, int storeNo, int count, string storeValue)
        {
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(xmlPath);
                string node = "state" + storeNo.ToString();
                XmlNode xmlNode = xmlDocument.SelectSingleNode("//" + nodeParName + "//" + node);
                for (int i = 0; i < xmlNode.ChildNodes.Count; i++)
                {
                    if (i == count)
                    {
                        xmlNode.ChildNodes[i].InnerText = storeValue.ToString();
                        //添加日期2024-4-30  物料条码按顺序写入 
                        xmlNode.ChildNodes[xmlNode.ChildNodes.Count-1].InnerText = (count + 1).ToString();
                        xmlDocument.Save(xmlPath);
                        break;
                    }
                }
                xmlDocument = null;
                GC.Collect();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("获取节点异常", ex);
            }
        }
        /// <summary>
        /// 解析json字符串 获取组托码
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static string Parse(string json)
        {
            object obj = JsonConvert.DeserializeObject(json);
            JObject pairs = JObject.Parse(obj.ToString());
            return pairs["data"].ToString(); //组托码
        }
        /// <summary>
        /// 写入xml子节点值
        /// </summary>
        /// <param name="xmlpath"></param>
        /// <param name="nodeName"></param>
        /// <param name="value1"></param>
        /// <param name="parentNodeName"></param>
        /// <param name="value2"></param>
        public static void WriteXmlNodeValue(string xmlpath, string nodeName, string value1, string parentNodeName, string value2)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(xmlpath);
            string node = nodeName + value1;
            XmlNode xmlNode = xmlDocument.SelectSingleNode("//" + parentNodeName + "//" + node);
            xmlNode.FirstChild.InnerText = value2;
            xmlDocument.Save(xmlpath);
            xmlDocument = null;
            GC.Collect();
        }
        /// <summary>
        /// 写入Image
        /// </summary>
        /// <param name="xmlpath"></param>
        /// <param name="nodeName"></param>
        /// <param name="value1"></param>
        /// <param name="parentNodeName"></param>
        /// <param name="value2"></param>
        public static void WriteXmlNodeValue1(string xmlpath, string nodeName, string value1, string parentNodeName, string value2)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(xmlpath);
            string node = nodeName + value1;
            XmlNode xmlNode = xmlDocument.SelectSingleNode("//" + parentNodeName + "//" + node);
            xmlNode.InnerText = value2;
            xmlDocument.Save(xmlpath);
            xmlDocument = null;
            GC.Collect();
        }
        /// <summary>
        /// List写入xml文件 
        /// </summary>
        /// <param name="xmlPath"></param>
        /// <param name="myList"></param>
        /// <param name="nodeName"></param>
        public static void ListToXml(string xmlPath, List<string> myList, string nodeName)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(xmlPath);
            XmlElement root = xmlDocument.DocumentElement;
            for (int i = 0; i < root.ChildNodes.Count; i++)
            {
                if (root.ChildNodes[i].Name == nodeName)
                {
                    root.ChildNodes[i].InnerText = myList[i].ToString();
                }
            }
            xmlDocument.Save(xmlPath);
            xmlDocument = null;
            GC.Collect();
        }
        /// <summary>
        /// 将list写入xml文件
        /// </summary>
        /// <param name="xmlPath">文件路径</param>
        /// <param name="myList">list</param>
        /// <param name="NodeName">节点名</param>
        public static void WriteListToXml(string xmlPath, List<string> myList, string NodeName)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(xmlPath);
            XmlNode xmlNode = xmlDocument.SelectSingleNode("//" + NodeName);
            for (int i = 0; i < xmlNode.ChildNodes.Count; i++)
            {
                xmlNode.ChildNodes[i].InnerText = myList[i].ToString();
            }
            xmlDocument.Save(xmlPath);
            xmlDocument = null;
            GC.Collect();
        }
        /// <summary>
        /// 读取节点的值
        /// </summary>
        /// <param name="xmlPath">文件路径</param>
        /// <param name="nodeName">节点名</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static string ReadNodeValue(string xmlPath, string nodeName, string value)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(xmlPath);
            string node = nodeName + value;
            XmlNode xmlNode = xmlDocument.SelectSingleNode("//" + node);
            string str = xmlNode.InnerText;
            xmlDocument = null;
            GC.Collect();
            return str;

        }
        /// <summary>
        /// 读取节点下子节点的值
        /// </summary>
        /// <param name="xmlPath">文件路径</param>
        /// <param name="nodeName">节点名</param>
        /// <param name="value">值</param>
        /// <param name="childNodeName">子节点名</param>
        /// <returns></returns>
        public static string ReadNodeValue(string xmlPath, string nodeName, string value, string childNodeName)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(xmlPath);
            string node = nodeName + value.ToString();
            XmlNode xmlNode = xmlDocument.SelectSingleNode("//" + node + "//" + childNodeName);
            string str = xmlNode.InnerText;
            xmlDocument = null;
            GC.Collect();
            return str;
        }
        public static void StringToXml2(string xmlpath, int storeNo, string nodeName, string nodeValue)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(xmlpath);
            string node = "state" + storeNo.ToString();
            XmlNode xmlNode = xmlDocument.SelectSingleNode("//taskNo//" + node);
            xmlNode.RemoveAll();
            // 创建新的节点并设置其值
            XmlElement newNode = xmlDocument.CreateElement("taskNo" + nodeName);
            newNode.InnerText = nodeValue;
            xmlNode.AppendChild(newNode);
            xmlDocument.Save(xmlpath);
            xmlDocument = null;
            GC.Collect();
        }
        /// <summary>
        /// 读取指定节点的第一级子节点名截取长度为6
        /// </summary>
        /// <param name="xmlPath">文件路径</param>
        /// <param name="nodeName">节点名</param>
        /// <param name="value">值</param>
        /// <param name="parentName">父节点名</param>
        /// <returns></returns>
        //public static string ReadNodeName(string xmlPath, string nodeName, string value, string parentName)
        //{
        //    XmlDocument xmlDocument = new XmlDocument();
        //    xmlDocument.Load(xmlPath);
        //    string node = nodeName + value;
        //    XmlNode xmlNode = xmlDocument.SelectSingleNode("//" + parentName + "//" + node);
        //    //return xmlNode.FirstChild.Name.Substring(6);
        //    return xmlNode.Name.Substring(5);
        //}
        public static string ReadNodeName(string xmlPath, string nodeName)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(xmlPath);
            XmlNode xmlNode = xmlDocument.SelectSingleNode("//taskNo" + nodeName);
            string str = xmlNode.ParentNode.Name.Substring(5);
            xmlDocument = null;
            GC.Collect();
            return str;
        }

        /// <summary>
        /// 读取指定节点的所有子节点的第一个子级的值写入list
        /// </summary>
        /// <param name="xmlPath">文件路径</param>
        /// <param name="nodeName">节点名</param>
        /// <returns></returns>
        public static List<string> ReadFirstChildToList(string xmlPath, string nodeName)
        {
            List<string> list = new List<string>();
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(xmlPath);
            XmlNode xmlNode = xmlDocument.SelectSingleNode("//" + nodeName);
            for (int i = 0; i < xmlNode.ChildNodes.Count; i++)
            {
                list.Add(xmlNode.ChildNodes[i].FirstChild.InnerText);
            }
            return list;
            xmlDocument = null;
            GC.Collect();
        }
        /// <summary>
        /// 更改指定节点名的节点值
        /// </summary>
        /// <param name="xmlpath">文件路径</param>
        /// <param name="nodeName"></param>
        /// <param name="nodeValue"></param>
        public static void WriteXmlNodeValue(string xmlpath, string nodeName, string nodeValue)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(xmlpath);
            XmlNode xmlNode = xmlDocument.SelectSingleNode("//" + nodeName);
            xmlNode.InnerText = nodeValue;
            xmlDocument.Save(xmlpath);
            xmlDocument = null;
            GC.Collect();
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
        /// <summary>
        /// 更改节点下子节点名称
        /// </summary>
        /// <param name="xmlpath">文件路径</param>
        /// <param name="nodeName">节点名</param>
        /// <param name="value1">值1</param>
        /// <param name="childNodeName">新的节点名</param>
        /// <param name="value2">值2</param>
        public static void WriteXmlNodeName(string xmlpath, string parentName, string nodeName, string value1, string childNodeName, string value2)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(xmlpath);
            XmlNode node = xmlDocument.SelectSingleNode("//" + parentName + "//" + nodeName + value1);
            node.RemoveAll();
            string newChildNodeName = childNodeName + value2;
            XmlNode newChildNode = xmlDocument.CreateElement(newChildNodeName);
            newChildNode.InnerText = value2;
            node.AppendChild(newChildNode);
            xmlDocument.Save(xmlpath);
            xmlDocument = null;
            GC.Collect();
        }
        /// <summary>
        /// 遍历节点下所有子节点，取子节点的第一级的值作比较，返回子节点的名称并截取长度为5
        /// </summary>
        /// <param name="xmlpath">文件路径</param>
        /// <param name="nodeName">节点名</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static string ReadNodeName(string xmlpath, string nodeName, string value)
        {
            string parameter = string.Empty;
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(xmlpath);
            XmlNode xmlNode = xmlDocument.SelectSingleNode("//" + nodeName);
            for (int i = 0; i < xmlNode.ChildNodes.Count; i++)
            {
                if (xmlNode.ChildNodes[i].FirstChild.InnerText == value)
                {
                    parameter = xmlNode.ChildNodes[i].Name.Substring(5);
                    break;
                }
            }
            string str = parameter;
            xmlDocument = null;
            GC.Collect();
            return str;
        }
    }
}
