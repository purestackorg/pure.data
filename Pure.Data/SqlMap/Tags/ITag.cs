
using System;

namespace Pure.Data.SqlMap.Tags
{

    public interface ITag
    {
        TagType Type { get; }
        String BuildSql(RequestContext context);
    }
    /// <summary>
    /// 标签类型
    /// </summary>
    public enum TagType
    {
        None,
        SqlText,
        If,
        IsEmpty,
        IsEqual,
        IsGreaterEqual,
        IsGreaterThan,
        IsLessEqual,
        IsLessThan,
        IsNotEmpty,
        IsNotEqual,
        IsNotNull,
        IsNull,
		IsTrue,
        IsFalse,
        IsProperty,
        Include,
        Switch,
        SwitchCase,
        SwitchDefault,
        Choose,
        ChooseWhen,
        ChooseOtherwise,
        Trim,
        Set,
        Where,
        Dynamic,
        For,
        Env,
        Variable,
        Bind,
        OrderBy
    }
}
