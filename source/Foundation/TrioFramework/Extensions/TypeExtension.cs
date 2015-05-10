using System;
using System.Globalization;
using System.Reflection;

/// <summary>
/// 类型扩展方法
/// </summary>
// ReSharper disable once CheckNamespace
public static class TypeExtension
{
    /// <summary>
    /// 获取第一个该类型的特性
    /// </summary>
    /// <typeparam name="T">特性类型</typeparam>
    /// <param name="type">类型</param>
    /// <returns></returns>
    public static T GetFirstAttr<T>(this Type type) where T : Attribute
    {
        var attrType = typeof(T);
        var attributes = type.GetCustomAttributes(attrType, true);
        if (attributes.Length == 0)
        {
            throw new TypeLoadException("未能加载特性" + attrType.Name + "来自类型" + type.FullName);
        }
        return (T)attributes[0];
    }

    /// <summary>
    /// 反射并创建对象
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="type"></param>
    /// <param name="args">构造参数</param>
    /// <returns></returns>
    public static T Create<T>(this Type type, params object[] args)
    {
        return (T)type.Assembly.CreateInstance(type.FullName, true, BindingFlags.CreateInstance, null, args, null, null);
    }
}
