using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using Bingosoft.TrioFramework.DB;

namespace Bingosoft.TrioFramework.Workflow.Core.Models
{
    /// <summary>
    /// 流程附件类型
    /// </summary>
    [Table("WF_AttachType")]
    public class WorkflowAttachType
    {
        #region Properties
        /// <summary>
        /// 附件类型
        /// </summary>
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// 附件名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        #endregion

        /// <summary>
        /// 添加新的类型
        /// </summary>
        public void AddNew()
        {
            using (var db = DBFactory.Get<WorkflowAttachTypeContext>())
            {
                var entry = db.Entry(this);
                entry.State = EntityState.Added;
                db.SaveChanges();
            }
        }

        /// <summary>
        /// 获取附件类型
        /// </summary>
        /// <param name="id">附件类型id</param>
        /// <returns></returns>
        public static WorkflowAttachType Find(int id)
        {
            using (var db = DBFactory.Get<WorkflowAttachTypeContext>())
            {
                var query = (from t in db.Types
                             where t.Id == id
                             select t).FirstOrDefault();
                return query;
            }
        }
    }

    /// <summary>
    /// 流程附件数据库操作类
    /// </summary>
    public class WorkflowAttachTypeContext : DbContextBase
    {
        #region ctor

        public WorkflowAttachTypeContext()
        {

        }

        public WorkflowAttachTypeContext(DbConnection conn)
            : base(conn)
        {

        }

        #endregion

        /// <summary>
        /// 附件类型
        /// </summary>
        public DbSet<WorkflowAttachType> Types { get; set; }
    }

    /// <summary>
    /// 流程附件表
    /// </summary>
    [Table("WF_Attachments")]
    public class WorkflowAttachment
    {
        /// <summary>
        /// 实例化工作流附件
        /// </summary>
        public WorkflowAttachment()
        {
            this.IsCanDeleted = false;
            this.IsCanDownload = false;
        }

        #region Properties

        /// <summary>
        /// 文件Id
        /// </summary>
        [Key]
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
        /// 附件类型Id
        /// </summary>
        public int FileTypeId { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }

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
        public string FileSizeText { get; set; }

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
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 创建时间字符串格式
        /// </summary>
        [NotMapped]
        public string CreateTimeStr
        {
            get { return CreateTime.ToString("MM-dd HH:mm:ss"); }
        }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 删除者
        /// </summary>
        public string DeletedBy { get; set; }

        /// <summary>
        /// 删除时间
        /// </summary>
        public DateTime? DeletedTime { get; set; }

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

        /// <summary>
        /// 附件类型
        /// </summary>
        [ForeignKey("FileTypeId")]
        public WorkflowAttachType FileType { get; set; }

        #region for business

        /// <summary>
        /// 是否允许下载
        /// </summary>
        [NotMapped]
        public bool IsCanDownload { get; set; }
        /// <summary>
        /// 是否允许删除
        /// </summary>
        [NotMapped]
        public bool IsCanDeleted { get; set; }
        /// <summary>
        /// 是否可以查看
        /// </summary>
        [NotMapped]
        public bool IsCanView { get; set; }

        #endregion

        #endregion

        /// <summary>
        /// 获取文件大小的文本表达形式
        /// </summary>
        /// <param name="fileSize">文件大小</param>
        /// <returns></returns>
        private string GetFileSizeStr(long fileSize)
        {
            var units = new string[] { "GB", "MB", "KB" };
            var sizes = new long[] { 1024 * 1024 * 1024, 1024 * 1024, 1024 };
            var finalSize = sizes[sizes.Length - 1];
            for (int level = 0; level < sizes.Length; level++)
            {
                if (fileSize > sizes[level] || level == finalSize)
                {
                    return ((double)fileSize / sizes[level]).ToString("f2") + units[level];
                }
            }
            return "0KB";
        }

        /// <summary>
        /// 保存流程附件
        /// </summary>
        /// <returns></returns>
        public void AddNew()
        {
            if (string.IsNullOrWhiteSpace(this.FileId))
            {
                this.FileId = Guid.NewGuid().ToString();
            }
            this.FileSizeText = GetFileSizeStr(this.FileSize);
            using (var db = DBFactory.Get<WorkflowAttachmentContent>())
            {
                db.Attachments.Add(this);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// 删除附件记录（物理删除，仅在附件上传失败时调用）
        /// </summary>
        /// <returns></returns>
        public void Delete()
        {
            using (var db = DBFactory.Get<WorkflowAttachmentContent>())
            {
                var entry = db.Entry(this);
                entry.State = EntityState.Deleted;
                db.SaveChanges();
            }
        }

        /// <summary>
        /// 标记为删除（逻辑删除）
        /// </summary>
        /// <returns></returns>
        public void MarkedDeleted()
        {
            using (var db = DBFactory.Get<WorkflowAttachmentContent>())
            {
                this.IsDeleted = true;
                this.DeletedTime = DateTime.Now;
                var entry = db.Entry(this);
                entry.State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        /// <summary>
        /// 标记为删除（逻辑删除）
        /// </summary>
        /// <returns></returns>
        public void MarkedDisabled()
        {
            using (var db = DBFactory.Get<WorkflowAttachmentContent>())
            {
                this.IsDisabled = true;
                this.DisabledTime = DateTime.Now;
                var entry = db.Entry(this);
                entry.State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        /// <summary>
        /// 根据文件id获取流程附件信息
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public static WorkflowAttachment Find(string fileId)
        {
            using (var db = DBFactory.Get<WorkflowAttachmentContent>())
            {
                var query = (from a in db.Attachments.Include(a => a.FileType)
                             where a.FileId.Equals(fileId, StringComparison.OrdinalIgnoreCase)
                             select a).FirstOrDefault();
                return query;
            }
        }

        /// <summary>
        /// 获取该流程的所有附件
        /// </summary>
        /// <param name="instanceNo">流程编号</param>
        /// <returns></returns>
        public static IList<WorkflowAttachment> FindAll(string instanceNo)
        {
            using (var db = DBFactory.Get<WorkflowAttachmentContent>())
            {
                var query = (from a in db.Attachments.Include(a => a.FileType)
                             where a.InstanceNo.Equals(instanceNo, StringComparison.OrdinalIgnoreCase)
                             select a).ToList();
                return query;
            }
        }

        /// <summary>
        /// 读取该流程的某个类型的附件信息
        /// </summary>
        /// <param name="instanceNo">流程单号</param>
        /// <param name="fileType">文件类型</param>
        /// <returns></returns>
        public static WorkflowAttachment Find(string instanceNo, int fileType)
        {
            using (var db = DBFactory.Get<WorkflowAttachmentContent>())
            {
                var query = (from a in db.Attachments.Include(a => a.FileType)
                             where a.InstanceNo.Equals(instanceNo, StringComparison.OrdinalIgnoreCase) && a.FileTypeId == fileType
                             select a).FirstOrDefault();
                return query;
            }
        }

        /// <summary>
        /// 读取该流程中该类型的所有附件
        /// </summary>
        /// <param name="instanceNo">流程单号</param>
        /// <param name="fileType">文件类型</param>
        /// <returns></returns>
        public static WorkflowAttachment[] FindAll(string instanceNo, int fileType)
        {
            using (var db = DBFactory.Get<WorkflowAttachmentContent>())
            {
                var query = (from a in db.Attachments.Include(a => a.FileType)
                             where a.InstanceNo.Equals(instanceNo, StringComparison.OrdinalIgnoreCase) && a.FileTypeId == fileType
                             select a).ToArray();
                return query;
            }
        }

        /// <summary>
        /// 判断该流程是否存在该附件类型
        /// </summary>
        /// <param name="instanceNo">流程单号</param>
        /// <param name="fileType">附件类型</param>
        /// <returns></returns>
        public static bool Exists(string instanceNo, int fileType)
        {
            using (var db = DBFactory.Get<WorkflowAttachmentContent>())
            {
                var query = (from a in db.Attachments.Include(a => a.FileType)
                             where a.InstanceNo.Equals(instanceNo, StringComparison.OrdinalIgnoreCase) && a.FileTypeId == fileType
                             select a).Any();
                return query;
            }
        }
    }

    /// <summary>
    /// 流程附件数据库操作类
    /// </summary>
    public class WorkflowAttachmentContent : DbContextBase
    {
        #region ctor
        public WorkflowAttachmentContent()
        {

        }

        public WorkflowAttachmentContent(DbConnection conn)
            : base(conn)
        {

        }
        #endregion

        /// <summary>
        /// 附件列表
        /// </summary>
        public DbSet<WorkflowAttachment> Attachments { get; set; }
    }
}
