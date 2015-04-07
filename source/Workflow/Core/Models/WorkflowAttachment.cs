using System;
using System.Collections.Generic;
using System.Linq;
using Bingosoft.Data;
using Bingosoft.Data.Attributes;
using Bingosoft.Security;

namespace Bingosoft.TrioFramework.Workflow.Core.Models {

    /// <summary>
    /// 流程附件表
    /// </summary>
    [Table("WF_Attachments")]
    public class WorkflowAttachment {
        /// <summary>
        /// 实例化工作流附件
        /// </summary>
        public WorkflowAttachment() {
            this.IsCanDeleted = false;
            this.IsCanDownload = false;
        }

        #region Properties

        /// <summary>
        /// 文件Id
        /// </summary>
        [PrimaryKey]
        public string FileId { get; set; }
        /// <summary>
        /// 所属流程编号
        /// </summary>
        public string InstanceNo { get; set; }
        /// <summary>
        /// 任务Id
        /// </summary>
        public int TaskId { get; set; }
        /// <summary>
        /// 所属环节名称
        /// </summary>
        public string ActivityName { get; set; }
        /// <summary>
        /// 附件类型
        /// </summary>
        public int FileType { get; set; }
        /// <summary>
        /// 附件类型
        /// </summary>
        public string AttachTypeStr { get; set; }

        private string fileName = string.Empty;
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName {
            get { return (this.IsDisabled ? "（已失效）" : "") + fileName; }
            set { fileName = value; }
        }
        /// <summary>
        /// 文件路径（文件服务器）
        /// </summary>
        public string FilePath { get; set; }
        /// <summary>
        /// 文件大小(kb)
        /// </summary>
        public long FileSize { get; set; }
        /// <summary>
        /// 文件大小的文本表达形式
        /// </summary>
        public string FileSizeStr {
            get { return GetFileSizeStr(FileSize); }
        }
        /// <summary>
        /// 创建人Id
        /// </summary>
        public string CreatorId { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string Creator { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedTime { get; set; }
        /// <summary>
        /// 创建时间字符串格式
        /// </summary>
        public string CreatedTimeStr {
            get { return CreatedTime.ToString("MM-dd HH:mm:ss"); }
        }
        /// <summary>
        /// 是否允许下载
        /// </summary>
        public bool IsCanDownload { get; set; }
        /// <summary>
        /// 是否允许删除
        /// </summary>
        public bool IsCanDeleted { get; set; }
        /// <summary>
        /// 是否可以查看
        /// </summary>
        public bool IsCanView { get; set; }
        /// <summary>
        /// 附件是否已失效
        /// </summary>
        public bool IsDisabled { get; set; }
        /// <summary>
        /// 操作人
        /// </summary>
        public string DisabledBy { get; set; }
        /// <summary>
        /// 失效时间
        /// </summary>
        public DateTime? DisabledTime { get; set; }

        #endregion

        /// <summary>
        /// 获取文件大小的文本表达形式
        /// </summary>
        /// <param name="fileSize">文件大小</param>
        /// <returns></returns>
        private string GetFileSizeStr(long fileSize) {
            var units = new string[] { "GB", "MB", "KB" };
            var sizes = new long[] { 1024 * 1024 * 1024, 1024 * 1024, 1024 };
            var finalSize = sizes[sizes.Length - 1];
            for (int level = 0; level < sizes.Length; level++) {
                if (fileSize > sizes[level] || level == finalSize) {
                    return ((double)fileSize / sizes[level]).ToString("f2") + units[level];
                }
            }
            return "0KB";
        }

        private static Dao _dao = Dao.Get();
        /// <summary>
        /// 保存流程附件
        /// </summary>
        /// <returns></returns>
        public bool Save() {
            if (string.IsNullOrEmpty(this.FileId)) {
                this.FileId = Guid.NewGuid().ToString();
                _dao.Insert<WorkflowAttachment>(this);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 删除附件记录（物理删除，仅在附件上传失败时调用）
        /// </summary>
        /// <returns></returns>
        public bool Delete() {
            var effectRows = _dao.Delete<WorkflowAttachment>(this.FileId);
            return effectRows > 0;
        }

        /// <summary>
        /// 标记为删除（逻辑删除）
        /// </summary>
        /// <returns></returns>
        public bool MarkedDeleted() {
            var loginUser = SecurityContext.User;
            var effectRows = _dao.ExecuteNonQuery("workflow.core.attachment.markdeleted", new {
                FileId = this.FileId,
                DeletedBy = loginUser.LoginId
            });
            return effectRows > 0;
        }

        /// <summary>
        /// 标记为删除（逻辑删除）
        /// </summary>
        /// <returns></returns>
        public bool MarkedDisabled() {
            var loginUser = SecurityContext.User;
            var effectRows = _dao.ExecuteNonQuery("workflow.core.attachment.markdisabled", new {
                FileId = this.FileId,
                DeletedBy = loginUser.LoginId
            });
            return effectRows > 0;
        }

        /// <summary>
        /// 根据文件id获取流程附件信息
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public static WorkflowAttachment Get(string fileId) {
            return _dao.Select<WorkflowAttachment>(fileId);
        }

        /// <summary>
        /// 获取该流程的所有附件
        /// </summary>
        /// <param name="instanceNo">流程编号</param>
        /// <returns></returns>
        public static IList<WorkflowAttachment> GetFiles(string instanceNo) {
            return _dao.QueryEntities<WorkflowAttachment>("workflow.core.attachment.load", new { InstanceNo = instanceNo });
        }

        /// <summary>
        /// 读取该流程的某个类型的附件信息
        /// </summary>
        /// <param name="instanceNo">流程单号</param>
        /// <param name="fileType">文件类型</param>
        /// <returns></returns>
        public static WorkflowAttachment Get(string instanceNo, int fileType) {
            return _dao.QueryEntity<WorkflowAttachment>("workflow.core.attachment.getbytypeid", new { InstanceNo = instanceNo, FileType = fileType });
        }

        /// <summary>
        /// 读取该流程中该类型的所有附件
        /// </summary>
        /// <param name="instanceNo">流程单号</param>
        /// <param name="fileType">文件类型</param>
        /// <returns></returns>
        public static WorkflowAttachment[] GetFiles(string instanceNo, int fileType) {
            return _dao.QueryEntities<WorkflowAttachment>("workflow.core.attachment.getfilesbytype", new { InstanceNo = instanceNo, FileType = fileType }).ToArray();
        }


        /// <summary>
        /// 判断该流程是否存在该类型的附件
        /// </summary>
        /// <param name="instanceNo">流程单号</param>
        /// <param name="fileTypeName">附件类型名称</param>
        /// <returns></returns>
        public static bool Exists(string instanceNo, string fileTypeName) {
            var exists = _dao.QueryScalar<int>("workflow.core.attachment.existsbyname", new { InstanceNo = instanceNo, FileTypeName = fileTypeName });
            return exists > 0;
        }

        /// <summary>
        /// 判断该流程是否存在该附件类型
        /// </summary>
        /// <param name="instanceNo">流程单号</param>
        /// <param name="fileTypes">多个附件类型</param>
        /// <returns></returns>
        public static bool Exists(string instanceNo, params int[] fileTypes) {
            var existsCounts = _dao.QueryScalar<int>("workflow.core.attachment.existsbyids", new { InstanceNo = instanceNo, FileType = fileTypes.Concat(',') });
            return (existsCounts == fileTypes.Length);
        }

        /// <summary>
        /// 判断该流程是否存在该附件类型
        /// </summary>
        /// <param name="instanceNo">流程单号</param>
        /// <param name="fileType">附件类型</param>
        /// <returns></returns>
        public static bool Exists(string instanceNo, int fileType) {
            var exists = _dao.QueryScalar<int>("workflow.core.attachment.existsbyid", new { InstanceNo = instanceNo, FileType = fileType });
            return exists > 0;
        }

        private static IDictionary<int, string> _attachTypeCache = null;
        /// <summary>
        /// 获取所有附件类型
        /// </summary>
        /// <returns></returns>
        public static IDictionary<int, string> GetAllAttachType() {
            if (_attachTypeCache == null) {
                _attachTypeCache = new Dictionary<int, string>();
                using (var reader = _dao.QueryReader("workflow.core.attachtype.getall")) {
                    while (reader.Read()) {
                        _attachTypeCache.Add(Convert.ToInt32(reader["Id"]), Convert.ToString(reader["Name"]));
                    }
                    reader.Close();
                }
            }
            return _attachTypeCache;
        }
    }
}
