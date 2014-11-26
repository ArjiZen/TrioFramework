namespace Bingosoft.TrioFramework.Workflow.Business {
    /// <summary>
    /// 业务表单
    /// </summary>
    public abstract class BusinessForm {

        #region Properties
        /// <summary>
        /// 流程单号
        /// </summary>
        public string InstanceNo { get; set; }
        /// <summary>
        /// 流程标题
        /// </summary>
        public string Title { get; set; }

        #endregion

        /// <summary>
        /// 加载业务表单数据
        /// </summary>
        /// <param name="instanceNo">流程编号</param>
        /// <returns></returns>
        public abstract bool Load(string instanceNo);

        /// <summary>
        /// 更新表单中的流程编号
        /// </summary>
        /// <returns></returns>
        public abstract bool UpdateInstanceNo();

        /// <summary>
        /// 属性复制
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="from"></param>
        /// <param name="to"></param>
        protected void CopyTo<T>(T from, T to) {
            var type = typeof(T);
            var properties = type.GetProperties();
            foreach (var property in properties) {
                var value = property.GetValue(from, null);
                if (property.CanWrite) {
                    property.SetValue(to, value, null);
                }
            }
        }

        /// <summary>
        /// 验证表单
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public abstract bool Validate(out string message);
    }
}