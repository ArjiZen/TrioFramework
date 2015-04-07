using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Bingosoft.Data;

namespace Bingosoft.TrioFramework.Models {

    /// <summary>
    /// 字典项
    /// </summary>
    public class DictionaryItem {
        /// <summary>
        /// 字典编码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 字典文本
        /// </summary>
        public string Text { get; set; }
    }

    /// <summary>
    /// 字典集合
    /// </summary>
    public class DictionaryCollection : Collection<DictionaryItem> {
        /// <summary>
        /// 实例化字典集合
        /// </summary>
        public DictionaryCollection() {

        }
        /// <summary>
        /// 实例化字典集合
        /// </summary>
        /// <param name="list"></param>
        public DictionaryCollection(IList<DictionaryItem> list) {
            this.Clear();
            foreach (var dictionaryItem in list) {
                this.Add(dictionaryItem);
            }
        }
        /// <summary>
        /// 获取字典值
        /// </summary>
        /// <param name="code">字典编码</param>
        /// <returns></returns>
        public string this[string code] {
            get {
                var dict = this.FirstOrDefault(p => p.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
                if (dict == null) {
                    return null;
                } else {
                    return dict.Text;
                }
            }
        }

        /// <summary>
        /// 包含字典项变啊
        /// </summary>
        /// <param name="code">字典项编码</param>
        /// <returns></returns>
        public bool ContainsCode(string code) {
            return this.Any(p => p.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
        }


        private readonly static Dao _dao = Dao.Get();
        /// <summary>
        /// 根据字典编码获取字典项集合
        /// </summary>
        /// <param name="code">字典编码</param>
        /// <returns></returns>
        public static DictionaryCollection GetByCode(string code) {
			var list = _dao.QueryEntities<DictionaryItem>("trio.framework.dictionary.getbycode", new { Code = code });
            var collection = new DictionaryCollection(list);
            return collection;
        }
    }
}