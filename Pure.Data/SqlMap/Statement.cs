
using Pure.Data.SqlMap.Tags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Pure.Data.SqlMap
{
    public class Statement
    {
        [XmlIgnore]
        public SqlMapInfo SmartSqlMap { get; private set; }
        public static Statement Load(XmlElement statementNode, SqlMapInfo smartSqlMap)
        {
            var statement = new Statement
            {
                Id = statementNode.Attributes["Id"].Value,

                SqlTags = new List<ITag> { },
                SmartSqlMap = smartSqlMap
            };

            string cacheId = statementNode.GetValueInXmlAttributes("Cache");
            if (!String.IsNullOrEmpty(cacheId))
            {
                var cache = smartSqlMap.Caches.FirstOrDefault(m => m.Id == cacheId);
                if (cache == null)
                {

                    throw new Exception(string.Format("SqlMap.Statement.Id:{0} can not find Cache.Id:{1}", statement.Id, cacheId));
                }
                statement.Cache = cache;
            }

            var tagNodes = statementNode.ChildNodes;
            string prepend, property;
            IList<Include> includes = new List<Include>();
            foreach (XmlNode tagNode in tagNodes)
            {
                var tag = LoadTag(tagNode, includes);
                if (tag != null) { statement.SqlTags.Add(tag); }
            }


            #region Init Include
            foreach (var include in includes)
            {
                if (include.RefId == statement.Id)
                {
                    throw new Exception(string.Format("Statement.Load Include.RefId can not be self statement.id:{0}", include.RefId));
                }
                var refStatement = smartSqlMap.Statements.FirstOrDefault(m => m.Id == include.RefId);
                if (refStatement != null)
                {
                    include.Ref = refStatement;
                }
                else
                {
                    //修复跨sqlmap的引用refId
                    var refStatement2 = SqlMapManager.Instance.Statements.FirstOrDefault(p => p.Value.Id == include.RefId).Value;
                    if (refStatement2 != null)
                    {
                        include.Ref = refStatement2;
                    }
                    else
                    {
                        throw new Exception(string.Format("Statement.Load can not find statement.id:{0}", include.RefId));
                    }


                }
            }
            #endregion

            #region Init Tag

            //foreach (XmlNode tagNode in tagNodes)
            //{
            //    prepend = "";
            //    property = "";
            //    if (tagNode.Attributes != null)
            //    {
            //        prepend = tagNode.GetValueInXmlAttributes("Prepend");
            //        property = tagNode.GetValueInXmlAttributes("Property");
            //    }

            //         switch (tagNode.Name)
            //    {
            //        case "Include":
            //            {
            //                var refId = tagNode.GetValueInXmlAttributes("RefId");

            //                var refStatement = smartSqlMap.Statements.FirstOrDefault(m => m.Id == refId);
            //                if (refStatement == null)
            //                {
            //                    throw new ArgumentException(string.Format("SqlMap.Statement.Load can not find statement.id:{0}", refId));
            //                }
            //                if (refId == statement.Id)
            //                {
            //                    throw new ArgumentException(string.Format("SqlMap.Statement.Load Include.RefId can not be self statement.id:{0}", refId));
            //                }
            //                statement.SqlTags.Add(new Include
            //                {
            //                    RefId = refId,
            //                    Ref = refStatement
            //                });
            //                break;
            //            }
            //        case "Switch":
            //            {
            //                var switchTag = new Switch
            //                {
            //                    Property = property,
            //                    Prepend = prepend,
            //                    Cases = new List<Switch.Case>()
            //                };
            //                var caseNodes = tagNode.ChildNodes;
            //                foreach (XmlNode caseNode in caseNodes)
            //                {
            //                    var caseCompareValue = caseNode.GetValueInXmlAttributes("CompareValue");
            //                    var caseBodyText = caseNode.InnerText.Replace("\n", "");
            //                    switchTag.Cases.Add(new Switch.Case
            //                    {
            //                        CompareValue = caseCompareValue,
            //                        BodyText = caseBodyText
            //                    });
            //                }
            //                statement.SqlTags.Add(switchTag);
            //                break;
            //            }
            //        default:
            //            {
            //                var tag = LoadTag(tagNode , null);
            //                if (tag != null) { statement.SqlTags.Add(tag); }
            //                break;
            //            };
            //    }
            //   }

            #endregion

            return statement;
        }

        public static ITag LoadTag(XmlNode xmlNode, IList<Include> includes)
        {
            ITag tag = null;
            bool isIn = !string.IsNullOrEmpty(xmlNode.GetValueInXmlAttributes("In"));
            var prepend = xmlNode.GetValueInXmlAttributes("Prepend");
            var property = xmlNode.GetValueInXmlAttributes("Property");
            var compareValue = xmlNode.GetValueInXmlAttributes("CompareValue");

            #region Init Tag
            switch (xmlNode.Name)
            {
                case "#text":
                case "#cdata-section":
                    {
                        var bodyText = " " + xmlNode.GetInnerTextInXmlAttributes() ;
                        return new SqlText
                        {
                            BodyText = bodyText
                        };
                    }
                case "OrderBy":
                    {
                        var bodyText = " " + xmlNode.GetInnerTextInXmlAttributes();
                        tag = new OrderBy
                        {
                            ChildTags = new List<ITag>(),
                            
                            BodyText = bodyText
                        };
                        break;
                    }
                case "Include":
                    {
                        var refId = xmlNode.GetValueInXmlAttributes("RefId");
                        var include_tag = new Include
                        {
                            RefId = refId
                        };
                        includes.Add(include_tag);
                        tag = include_tag;
                        break;
                    }
                case "IsEmpty":
                    {
                        tag = new IsEmpty
                        {
                            In = isIn,
                            Prepend = prepend,
                            Property = property,
                            ChildTags = new List<ITag>()
                        };
                        break;
                    }

                case "IsEqual":
                    {
                        tag = new IsEqual
                        {
                            In = isIn,
                            Prepend = prepend,
                            Property = property,
                            CompareValue = compareValue,
                            ChildTags = new List<ITag>()
                        };
                        break;
                    }
                case "IsGreaterEqual":
                    {
                        tag = new IsGreaterEqual
                        {
                            In = isIn,
                            Prepend = prepend,
                            Property = property,
                            CompareValue = compareValue,
                            ChildTags = new List<ITag>()

                        };
                        break;
                    }
                case "IsGreaterThan":
                    {
                        tag = new IsGreaterThan
                        {
                            In = isIn,
                            Prepend = prepend,
                            Property = property,
                            CompareValue = compareValue,
                            ChildTags = new List<ITag>()
                        };
                        break;
                    }
                case "IsLessEqual":
                    {
                        tag = new IsLessEqual
                        {
                            In = isIn,
                            Prepend = prepend,
                            Property = property,
                            CompareValue = compareValue,
                            ChildTags = new List<ITag>()
                        };
                        break;
                    }
                case "IsLessThan":
                    {
                        tag = new IsLessThan
                        {
                            In = isIn,
                            Prepend = prepend,
                            Property = property,
                            CompareValue = compareValue,
                            ChildTags = new List<ITag>()
                        };
                        break;
                    }
                case "IsNotEmpty":
                    {
                        tag = new IsNotEmpty
                        {
                            In = isIn,
                            Prepend = prepend,
                            Property = property,
                            ChildTags = new List<ITag>()
                        };
                        break;
                    }
                case "IsNotEqual":
                    {
                        tag = new IsNotEqual
                        {
                            In = isIn,
                            Prepend = prepend,
                            Property = property,
                            CompareValue = compareValue,
                            ChildTags = new List<ITag>()
                        };
                        break;
                    }
                case "IsNotNull":
                    {
                        tag = new IsNotNull
                        {
                            In = isIn,
                            Prepend = prepend,
                            Property = property,
                            ChildTags = new List<ITag>()
                        };
                        break;
                    }
                case "IsNull":
                    {
                        tag = new IsNull
                        {
                            In = isIn,
                            Prepend = prepend,
                            Property = property,
                            ChildTags = new List<ITag>()
                        };
                        break;
                    }
                case "IsTrue":
                    {
                        tag = new IsTrue
                        {
                            In = isIn,
                            Prepend = prepend,
                            Property = property,
                            ChildTags = new List<ITag>()
                        };
                        break;
                    }
                case "IsFalse":
                    {
                        tag = new IsFalse
                        {
                            In = isIn,
                            Prepend = prepend,
                            Property = property,
                            ChildTags = new List<ITag>()
                        };
                        break;
                    }
                case "IsProperty":
                    {
                        tag = new IsProperty
                        {
                            In = isIn,
                            Prepend = prepend,
                            Property = property,
                            ChildTags = new List<ITag>()
                        };
                        break;
                    }
                case "Switch":
                    {
                        tag = new Switch
                        {
                            Property = property,
                            //Prepend = prepend,
                            ChildTags = new List<ITag>()
                        };
                        break;
                    }
                case "Case":
                    {
                        var switchNode = xmlNode.ParentNode;
                        var switchProperty = switchNode.GetValueInXmlAttributes("Property");
                        var switchPrepend = switchNode.GetValueInXmlAttributes("Prepend");
                        tag = new Switch.Case
                        {
                            CompareValue = compareValue,
                            Property = switchProperty,
                            Prepend = switchPrepend,
                            ChildTags = new List<ITag>()
                        };
                        break;
                    }
                case "Default":
                    {
                        var switchNode = xmlNode.ParentNode;
                        var switchProperty = switchNode.GetValueInXmlAttributes("Property");
                        var switchPrepend = switchNode.GetValueInXmlAttributes("Prepend");
                        tag = new Switch.Defalut
                        {
                            Property = switchProperty,
                            Prepend = switchPrepend,
                            ChildTags = new List<ITag>()
                        };
                        break;
                    }
                case "Where":
                    {
                        tag = new Where
                        {
                            ChildTags = new List<ITag>()
                        };
                        break;
                    }
                case "Dynamic":
                    {
                        tag = new Dynamic
                        {
                            Prepend = prepend,
                            ChildTags = new List<ITag>()
                        };
                        break;
                    }
                case "Variable":
                    {
                        var bodyText = xmlNode.GetInnerTextInXmlAttributes();
                        tag = new Variable
                        {
                            BodyText = bodyText,
                            Prepend = prepend,
                            Property = property,
                            ChildTags = new List<ITag>()
                        };
                        break;
                    }
                case "Foreach":
                    {
                        var open = xmlNode.GetValueInXmlAttributes("Open");
                        var separator = xmlNode.GetValueInXmlAttributes("Separator");
                        var close = xmlNode.GetValueInXmlAttributes("Close");
                        var item = xmlNode.GetValueInXmlAttributes("Item");
                        var index = xmlNode.GetValueInXmlAttributes("Index");
                        tag = new Foreach
                        {
                            Prepend = prepend,
                            Property = property,
                            Open = open,
                            Close = close,
                            Separator = separator,
                            Item = item,
                            Index = index,
                            ChildTags = new List<ITag>()
                        };
                        break;
                    }
                case "Env":
                    {
                        var dbProvider = xmlNode.GetValueInXmlAttributes("DbProvider");
                        tag = new Env
                        {
                            Prepend = prepend,
                            DbProvider = dbProvider,
                            ChildTags = new List<ITag>()
                        };
                        break;
                    }
                case "#comment": { break; }
                default:
                    {
                        throw new Exception(string.Format("Statement.LoadTag unkonw tagName:{0}.", xmlNode.Name));
                    };
            }
            #endregion
            //加载组合查询条件查询
            foreach (XmlNode childNode in xmlNode)
            {
                ITag childTag = LoadTag(childNode, includes);
                if (childTag != null && tag != null)
                {
                    (tag as Tag).ChildTags.Add(childTag);
                }
            }
            return tag;
        }

        [XmlAttribute]
        public String Id { get; set; }

        public String FullSqlId
        {
            get
            {
                return string.Format("{0}.{1}", SmartSqlMap.Scope, Id);
            }
        }
        public List<ITag> SqlTags { get; set; }

        public bool HasCache { get {
            return Cache != null;
        } }
        public SqlMapCache Cache { get; set; }

        ICacheProvider _CacheProvider = null;
        public ICacheProvider CacheProvider { get {
            if (Cache != null)
            {
                if (_CacheProvider == null)
                {
                    _CacheProvider = Cache.CreateCacheProvider(this);
                }
            }

            return _CacheProvider;
        } }

        public String BuildSql(RequestContext context)
        {

            StringBuilder sqlStrBuilder = new StringBuilder();
            foreach (ITag tag in SqlTags)
            {
                sqlStrBuilder.Append(tag.BuildSql(context));
            }
        
            return sqlStrBuilder.ToString();
        }
    }
}
