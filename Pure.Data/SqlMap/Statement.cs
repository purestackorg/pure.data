﻿
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
        public SqlMapInfo SqlMapInfo { get;  set; }
        public static Statement Load(XmlElement statementNode, SqlMapInfo sqlMapInfo)
        {
            var statement = new Statement
            {
                Id = SqlMapManager.Instance.FormatSqlMapNameCase(statementNode.Attributes["Id"].Value) ,

                SqlTags = new List<ITag> { },
                SqlMapInfo = sqlMapInfo
            };

            string cacheId = statementNode.GetValueInXmlAttributes("Cache");
            if (!String.IsNullOrEmpty(cacheId))
            {
                var cache = sqlMapInfo.Caches.FirstOrDefault(m => m.Id == cacheId);
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
            //foreach (var include in includes)
            //{
            //    if (include.RefId == statement.Id)
            //    {
            //        throw new Exception(string.Format("Statement.Include tag's RefId can not be self statement.id:{0}", include.RefId));
            //    }
            //    var refStatement = smartSqlMap.Statements.FirstOrDefault(m => m.Id == include.RefId);
            //    if (refStatement != null)
            //    {
            //        include.Ref = refStatement;
            //    }
            //    else
            //    {
            //        //修复跨sqlmap的引用refId
            //        var refStatement2 = SqlMapManager.Instance.Statements.FirstOrDefault(p => p.Value.Id == include.RefId).Value;
            //        if (refStatement2 != null)
            //        {
            //            include.Ref = refStatement2;
            //        }
            //        else
            //        {
            //            throw new Exception(string.Format("Statement.Include tag can not Include statement.id:{0}", include.RefId));
            //        }


            //    }
            //}
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
            string lowerXmlNodeName = xmlNode.Name.ToLower();
            switch (lowerXmlNodeName)
            {
                case "#text":
                case "#cdata-section":
                    {
                        var bodyText = " " + xmlNode.GetInnerTextInXmlAttributes();
                        return new SqlText
                        {
                            BodyText = bodyText
                        };
                    }
                //case "OrderBy":
                case "orderby":
                    {
                        var bodyText = " " + xmlNode.GetInnerTextInXmlAttributes();
                        tag = new OrderBy
                        {
                            ChildTags = new List<ITag>(),

                            BodyText = bodyText
                        };
                        break;
                    }
                //case "Include":
                case "include":
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
                //case "If":
                case "if":
                    {
                        var Test = xmlNode.GetValueInXmlAttributes("Test");
                        tag = new IfTag
                        {
                            Test = Test,
                            Prepend = prepend,
                            Property = property,
                            ChildTags = new List<ITag>()
                        };
                        break;
                    }
                //case "IsEmpty":
                case "isempty":
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

                //case "IsEqual":
                case "isequal":
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
                //case "IsGreaterEqual":
                case "isgreaterequal":
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
                //case "IsGreaterThan":
                case "isgreaterthan":
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
                //case "IsLessEqual":
                case "islessequal":
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
                //case "IsLessThan":
                case "islessthan":
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
                //case "IsNotEmpty":
                case "isnotempty":
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
                //case "IsNotEqual":
                case "isnotequal":
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
                //case "IsNotNull":
                case "isnotnull":
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
                //case "IsNull":
                case "isnull":
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
                //case "IsTrue":
                case "istrue":
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
                //case "IsFalse":
                case "isfalse":
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
                //case "IsProperty":
                case "isproperty":
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
                //case "Switch":
                case "switch":
                    {
                        tag = new Switch
                        {
                            Property = property,
                            //Prepend = prepend,
                            ChildTags = new List<ITag>()
                        };
                        break;
                    }
                //case "Case":
                case "case":
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
                //case "Default":
                case "default":
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
                //case "Choose":
                case "choose":
                    {
                        tag = new ChooseTag
                        {
                            //Property = property,
                            //Prepend = prepend,
                            ChildTags = new List<ITag>()
                        };
                        break;
                    }
                case "when":
                    {
                        var Test = xmlNode.GetValueInXmlAttributes("Test");
                        //var switchNode = xmlNode.ParentNode;
                        //var switchProperty = switchNode.GetValueInXmlAttributes("Property");
                        //var switchPrepend = switchNode.GetValueInXmlAttributes("Prepend");
                        tag = new ChooseTag.ChooseWhenTag
                        {
                            Test = Test,
                            //Property = switchProperty,
                            //Prepend = switchPrepend,
                            ChildTags = new List<ITag>()
                        };
                        break;
                    }
                case "otherwise":
                    {

                        //var switchNode = xmlNode.ParentNode;
                        //var switchProperty = switchNode.GetValueInXmlAttributes("Property");
                        //var switchPrepend = switchNode.GetValueInXmlAttributes("Prepend");
                        tag = new ChooseTag.ChooseOtherwiseTag
                        {
                            //Property = switchProperty,
                            //Prepend = switchPrepend,
                            ChildTags = new List<ITag>()
                        };
                        break;
                    }

                case "trim":
                    {
                        var Prefix = xmlNode.GetValueInXmlAttributes("Prefix", "", false);
                        var Suffix = xmlNode.GetValueInXmlAttributes("Suffix", "", false);
                        var PrefixOverrides = xmlNode.GetValueInXmlAttributes("PrefixOverrides");
                        var SuffixOverrides = xmlNode.GetValueInXmlAttributes("SuffixOverrides");
                        tag = new TrimTag
                        {
                            Prefix = Prefix,
                            Suffix = Suffix,
                            PrefixOverrides = PrefixOverrides,
                            SuffixOverrides = SuffixOverrides,
                            ChildTags = new List<ITag>()
                        };
                        break;
                    }
                case "set":
                    {
                        tag = new SetTag()
                        {
                            ChildTags = new List<ITag>()
                        };
                        break;
                    }
                case "where":
                    {
                        tag = new WhereTag
                        {
                            ChildTags = new List<ITag>()
                        };
                        break;
                    }
                //case "Where":
                //case "where":
                //    {
                //        tag = new Where
                //        {
                //            ChildTags = new List<ITag>()
                //        };
                //        break;
                //    }
                //case "Dynamic":
                case "dynamic":
                    {
                        tag = new Dynamic
                        {
                            Prepend = prepend,
                            ChildTags = new List<ITag>()
                        };
                        break;
                    }
                //case "Variable":
                case "variable":
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
                //case "Bind":
                case "bind":
                    {
                        var Name = xmlNode.GetValueInXmlAttributes("Name");
                        var Value = xmlNode.GetValueInXmlAttributes("Value");
                        //var bodyText = xmlNode.GetInnerTextInXmlAttributes();
                        tag = new BindTag
                        {
                            Name = Name,
                            Value = Value,
                            Prepend = prepend,
                            Property = property,
                            ChildTags = new List<ITag>()
                        };
                        break;
                    }
                //case "Foreach":
                case "foreach":
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
                //case "Env":
                case "env":
                    {
                        var dbProvider = xmlNode.GetValueInXmlAttributes("DbProvider");
                        var DbType = xmlNode.GetValueInXmlAttributes("DbType");
                        tag = new Env
                        {
                            Prepend = prepend,
                            DbProvider = dbProvider,
                            DbType = DbType,
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
                return SqlMapManager.Instance.FormatSqlMapNameCase(string.Format("{0}.{1}", SqlMapInfo.Scope, Id)); //string.Format("{0}.{1}", SqlMapInfo.Scope, Id);
            }
        }
        public List<ITag> SqlTags { get; set; }

        public bool HasCache
        {
            get
            {
                return Cache != null;
            }
        }
        public SqlMapCache Cache { get; set; }

        ICacheProvider _CacheProvider = null;
        public ICacheProvider CacheProvider
        {
            get
            {
                if (Cache != null)
                {
                    if (_CacheProvider == null)
                    {
                        _CacheProvider = Cache.CreateCacheProvider(this);
                    }
                }

                return _CacheProvider;
            }
        }

        public String BuildSql(RequestContext context)
        {

            StringBuilder sqlStrBuilder = new StringBuilder();
            foreach (ITag tag in SqlTags)
            {
                if (tag.Type == TagType.Include)
                {
                   ReplaceIncludeTag(context, tag as Include);
                }
                sqlStrBuilder.Append(tag.BuildSql(context));
            }

            return sqlStrBuilder.ToString();
        }

        public Include ReplaceIncludeTag(RequestContext context, Include include)
        {
            if (include == null)
            {
                throw new Exception(string.Format("Statement.Include tag can not be null!"));
            }
            if (string.IsNullOrWhiteSpace(include.RefId))
            {
                throw new Exception(string.Format("Statement.Include tag's RefId can not be null!"));
            }

            include.RefId =     SqlMapManager.Instance.FormatSqlMapNameCase(include.RefId);

            if (include.RefId == context.SqlId)
            {
                throw new Exception(string.Format("Statement.Include tag's RefId can not be self statement.id:{0}", include.RefId));
            }

            //修复跨sqlmap的引用refId
            var refStatement2 = SqlMapManager.Instance.Statements.FirstOrDefault(p => p.Value.Id == include.RefId).Value;
            if (refStatement2 != null)
            {
                include.Ref = refStatement2; //替换为新的Tag
            }
            else
            {
                throw new Exception(string.Format("Statement.Include tag can not Include statement.id:{0}", include.RefId));
            }

            return include;

        }

    }
}
