using System;

namespace Bingosoft.TrioFramework.Workflow.Core.Extensions {
    /// <summary>
    /// 类型扩展方法
    /// </summary>
    public static class TypeExtension {
        /// <summary>
        /// 获取第一个该类型的特性
        /// </summary>
        /// <typeparam name="T">特性类型</typeparam>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static T GetFirstAttr<T>(this Type type) where T : Attribute {
            var attrType = typeof (T);
            var attributes = type.GetCustomAttributes(attrType, true);
            if (attributes.Length == 0) {
                throw new TypeLoadException("未能加载特性" + attrType.Name + "来自类型" + type.FullName);
            }
            return (T)attributes[0];
        }
    }
}
