using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace Bingosoft.TrioFramework.Component.Excel
{
    /// <summary>
    /// Excel对象工厂类
    /// </summary>
    internal class WorkFactory
    {
        private readonly static object lockObj = new object();

        private static Assembly _assembly = null;
        /// <summary>
        /// 实现的程序集
        /// </summary>
        private static Assembly ImplementAssembly
        {
            get
            {
                if (_assembly == null)
                {
                    lock (lockObj)
                    {
                        if (_assembly == null)
                        {
                            _assembly = Assembly.Load(SettingProvider.Excel.Assembly);
                        }
                    }
                }
                return _assembly;
            }
        }

        /// <summary>
        /// 类型缓存
        /// </summary>
        private static IDictionary<string, Type> _cache = new Dictionary<string, Type>();

        /// <summary>
        /// 创建对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static T Create<T>()
        {
            var fullName = typeof(T).Name;
            if (_cache.ContainsKey(fullName))
            {
                return (T)ImplementAssembly.CreateInstance(_cache[fullName].FullName);
            }
            var types = ImplementAssembly.GetTypes();
            foreach (var type in types)
            {
                if (type.IsSubclassOf(typeof(T)))
                {
                    return (T)ImplementAssembly.CreateInstance(type.FullName);
                }
            }
            return Activator.CreateInstance<T>();
        }

        /// <summary>
        /// 新建工作簿
        /// </summary>
        /// <returns></returns>
        public static WorkBook CreateWorkbook()
        {
            return Create<WorkBook>();
        }

        /// <summary>
        /// 新建工作表
        /// </summary>
        /// <returns></returns>
        public static WorkSheet CreateWorkSheet()
        {
            return Create<WorkSheet>();
        }

        /// <summary>
        /// 创建表头对象
        /// </summary>
        /// <returns></returns>
        public static WorkHead CreateWorkHead()
        {
            return Create<WorkHead>();
        }

        /// <summary>
        /// 创建数据行
        /// </summary>
        /// <returns></returns>
        public static WorkDataRow CreateDataRow()
        {
            return Create<WorkDataRow>();
        }

        /// <summary>
        /// 创建单元格
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static WorkCell CreateWorkCell(string content)
        {
            Decimal d;
            if (Decimal.TryParse(content, out d))
            {
                var c = Create<WorkNumCell>();
                c.Content = content;
                return c;
            }
            DateTime t;
            if (DateTime.TryParse(content, out t))
            {
                var c = Create<WorkDateCell>();
                c.Content = content;
                return c;
            }
            Boolean b;
            if (Boolean.TryParse(content, out b))
            {
                var c = Create<WorkBoolCell>();
                c.Content = content;
                return c;
            }

            var wc = Create<WorkStrCell>();
            wc.Content = content;
            return wc;
        }
    }
}
