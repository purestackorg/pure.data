using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Linq;
using System.Xml.Serialization;

namespace Pure.Data
{
    public class ExcelExportProvider
    {

        public static bool Export<T>(IDatabase db, List<T> objs, IClassMapper mapper, string filepath)
        {

            //using (System.IO.StringWriter stringWriter = new StringWriter(new StringBuilder()))
            //{
            //    XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<T>));
            //    xmlSerializer.Serialize(stringWriter, list);

            //    FileStream fs = new FileStream(file, FileMode.OpenOrCreate);
            //    StreamWriter sw = new StreamWriter(fs);
            //    sw.Write(stringWriter.ToString());
            //    sw.Close();
            //    fs.Close();

            //}

            string headtag = mapper.EntityType.Name;

            string str = ObjListToXml<T>(mapper, objs,  headtag);


            System.IO.File.WriteAllText(filepath, str );

            return true;
             
        }

        #region 实体类，XML互操作
        

        /// <summary>
        /// 实体类序列化成xml
        /// </summary>
        /// <param name="enitities">The enitities.</param>
        /// <param name="headtag">The headtag.</param>
        /// <returns></returns>
        public static string ObjListToXml<T>(IClassMapper mapper, List<T> enitities, string headtag)
        {
            StringBuilder sb = new StringBuilder();
            PropertyInfo[] propinfos = mapper.Properties.Select(p=>p.PropertyInfo).ToArray();// null;
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            sb.AppendLine("<" + headtag + ">");
            foreach (T obj in enitities)
            {
                //初始化propertyinfo
                //if (propinfos == null)
                //{
                //    Type objtype = obj.GetType();
                //    propinfos = objtype.GetProperties();
                //}

                sb.AppendLine("<item>");
                foreach (PropertyInfo propinfo in propinfos)
                {
                    sb.Append("<");
                    sb.Append(propinfo.Name);
                    sb.Append(">");
                    sb.Append(propinfo.GetValue(obj, null));
                    sb.Append("</");
                    sb.Append(propinfo.Name);
                    sb.AppendLine(">");
                }
                sb.AppendLine("</item>");
            }
            sb.AppendLine("</" + headtag + ">");
            return sb.ToString();
        }

        /// <summary>
        /// 使用XML初始化实体类容器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="typename">The typename.</param>
        /// <param name="xml">The XML.</param>
        /// <param name="headtag">The headtag.</param>
        /// <returns></returns>
        public static List<T> XmlToObjList<T>(string xml, string headtag)
            where T : new()
        {
            List<T> list = new List<T>();
            XmlDocument doc = new XmlDocument();
            PropertyInfo[] propinfos = null;
            doc.LoadXml(xml);
            XmlNodeList nodelist = doc.SelectNodes("/" + headtag + "/item");
            foreach (XmlNode node in nodelist)
            {
                T entity = new T();
                //初始化propertyinfo
                if (propinfos == null)
                {
                    Type objtype = entity.GetType();
                    propinfos = objtype.GetProperties();
                }
                //填充entity类的属性
                foreach (PropertyInfo propinfo in propinfos)
                {
                    XmlNode cnode = node.SelectSingleNode(propinfo.Name);
                    string v = cnode.InnerText;
                    if (v != null)
                        propinfo.SetValue(entity, Convert.ChangeType(v, propinfo.PropertyType), null);
                }
                list.Add(entity);
            }
            return list;
        }

        #endregion



    }
}
