using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Pure.Data
{
    public static class AssemblyHelper
    {


        #region 加载程序集
        /// <summary>
        /// 加载某个Bin目录下面: Assembly[] asm = GetAllAssembly("*.Web.dll").ToArray();
        /// </summary>
        /// <param name="dllName"></param>
        /// <returns></returns>
        public static List<Assembly> GetAllAssembly(string dllName, string dir = "")
        {
            List<string> pluginpath = FindPlugin(dllName);
            var list = new List<Assembly>();
            foreach (string filename in pluginpath)
            {
                try
                {
                    string asmname = Path.GetFileNameWithoutExtension(filename);
                    if (asmname != string.Empty)
                    {
                        Assembly asm = Assembly.LoadFrom(filename);
                        list.Add(asm);
                    }
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message);
                }
            }
            return list;
        }
        //查找所有插件的路径
        public static List<string> FindPlugin(string dllName, string dirDll = "")
        {
            List<string> pluginpath = new List<string>();
            string dir = "";
            if (string.IsNullOrEmpty(dirDll))
            {
                string path = GetBaseDirectory();
                if (path != null)
                {
                    if (path.TrimEnd('\\').LastIndexOf("\\bin") == -1)
                    {
                        dir = Path.Combine(path, "bin\\");
                    }
                    else
                    {
                        dir = path;
                    }
                }
                
            }
            else
            {
                dir = dirDll;
            }
            
            string[] dllList = Directory.GetFiles(dir, dllName);
            if (dllList.Length > 0)
            {
                pluginpath.AddRange(dllList.Select(item => Path.Combine(dir, item.Substring(dir.Length))));
            }
            return pluginpath;
        }
        #endregion


        public static IEnumerable<Type> GetLoadableTypes(this Assembly assembly)
        {

            if (assembly == null) throw new ArgumentNullException("assembly");
            try
            {

                return assembly.GetTypes();

            }
            catch (ReflectionTypeLoadException e)
            {

                return e.Types.Where(t => t != null);

            }

        }

        /// <summary>
        /// 得到当前应用程序的根目录
        /// </summary>
        /// <returns></returns>
        public static string GetBaseDirectory()
        {
            var baseDirectory = PathHelper.GetAppExecuteDirectory();
             
            return baseDirectory;

        }

        /// <summary>
        /// 扫描程序集找到实现了某个接口的第一个实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="searchpattern">文件名过滤</param>
        /// <returns></returns>
        public static T FindTypeByInterface<T>(string searchpattern = "*.dll") where T : class
        {

            var interfaceType = typeof(T);

            string domain = GetBaseDirectory();
            string[] dllFiles = Directory.GetFiles(domain, searchpattern, SearchOption.TopDirectoryOnly);

            foreach (string dllFileName in dllFiles)
            {

                foreach (Type type in Assembly.LoadFrom(dllFileName).GetLoadableTypes())
                {

                    if (interfaceType != type && interfaceType.IsAssignableFrom(type))
                    {

                        var instance = Activator.CreateInstance(type) as T;
                        return instance;

                    }

                }

            }

            return null;

        }


        public static IList<Stream> GetResourceStream(Assembly assembly, System.Linq.Expressions.Expression<Func<string, bool>> predicate)
        {

            List<Stream> result = new List<Stream>();

            foreach (string resource in assembly.GetManifestResourceNames())
            {

                if (predicate.Compile().Invoke(resource))
                {

                    result.Add(assembly.GetManifestResourceStream(resource));

                }

            }

            StreamReader sr = new StreamReader(result[0]);
            string r = sr.ReadToEnd();
            result[0].Position = 0;

            return result;

        }

        /// <summary>
        /// 扫描程序集找到继承了某基类的所有子类
        /// </summary>
        /// <param name="inheritType">基类</param>
        /// <param name="searchpattern">文件名过滤</param>
        /// <returns></returns>
        public static List<Type> FindTypeByInheritType(Type inheritType, string searchpattern = "*.dll")
        {

            var result = new List<Type>();
            Type attr = inheritType;

            string domain = GetBaseDirectory();
            string[] dllFiles = Directory.GetFiles(domain, searchpattern, SearchOption.TopDirectoryOnly);

            foreach (string dllFileName in dllFiles)
            {

                foreach (Type type in Assembly.LoadFrom(dllFileName).GetLoadableTypes())
                {

                    if (type.BaseType == inheritType)
                    {

                        result.Add(type);

                    }

                }

            }

            return result;

        }

        /// <summary>
        /// 扫描程序集找到带有某个Attribute的所有PropertyInfo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="searchpattern">文件名过滤</param>
        /// <returns></returns>
        public static Dictionary<PropertyInfo, T> FindAllPropertyByAttribute<T>(string searchpattern = "*.dll") where T : Attribute
        {

            var result = new Dictionary<PropertyInfo, T>();
            var attr = typeof(T);

            string domain = GetBaseDirectory();
            string[] dllFiles = Directory.GetFiles(domain, searchpattern, SearchOption.TopDirectoryOnly);

            foreach (string dllFileName in dllFiles)
            {

                foreach (Type type in Assembly.LoadFrom(dllFileName).GetLoadableTypes())
                {

                    foreach (var property in type.GetProperties())
                    {

                        var attrs = property.GetCustomAttributes(attr, true);

                        if (attrs.Length == 0)
                            continue;

                        result.Add(property, (T)attrs.First());

                    }

                }

            }


            return result;

        }

        /// <summary>
        /// 扫描程序集找到类型上带有某个Attribute的所有类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="searchpattern">文件名过滤</param>
        /// <returns></returns>
        public static Dictionary<string, List<T>> FindAllTypeByAttribute<T>(string searchpattern = "*.dll") where T : Attribute
        {

            var result = new Dictionary<string, List<T>>();
            Type attr = typeof(T);

            string domain = GetBaseDirectory();
            string[] dllFiles = Directory.GetFiles(domain, searchpattern, SearchOption.TopDirectoryOnly);

            foreach (string dllFileName in dllFiles)
            {

                foreach (Type type in Assembly.LoadFrom(dllFileName).GetLoadableTypes())
                {

                    var typeName = type.AssemblyQualifiedName;

                    var attrs = type.GetCustomAttributes(attr, true);
                    if (attrs.Length == 0)
                        continue;

                    result.Add(typeName, new List<T>());

                    foreach (T a in attrs)
                        result[typeName].Add(a);


                }

            }

            return result;

        }
    }

}