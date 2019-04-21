using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PureGen
{
    public static class EnumHelpers
    {
        /// <summary>
        /// get all information of enum,include value,name and description
        /// </summary>
        /// <param name="enumName">the type of enumName</param>
        /// <returns></returns>
        public static List<dynamic> GetAllItems(this Type enumName)
        {
            List<dynamic> list = new List<dynamic>();
            // get enum fileds
            FieldInfo[] fields = enumName.GetFields();
            foreach (FieldInfo field in fields)
            {
                if (!field.FieldType.IsEnum)
                {
                    continue;
                }
                // get enum value
                int value = (int)enumName.InvokeMember(field.Name, BindingFlags.GetField, null, null, null);
                string text = field.Name;
                string description = string.Empty;
                object[] array = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (array.Length > 0)
                {
                    description = ((DescriptionAttribute)array[0]).Description;
                }
                else
                {
                    description = ""; //none description,set empty
                }
                //add to list
                dynamic obj = new ExpandoObject();
                obj.Value = value;
                obj.Text = text;
                obj.Description = description;
                list.Add(obj);
            }
            return list;
        }
    }
}
