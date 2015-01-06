using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Web.Mvc;
using Bingosoft.Security;
using Bingosoft.TrioFramework.Attributes;
using Bingosoft.TrioFramework.Mvc.Models;
using Bingosoft.TrioFramework.Mvc.Workflow;
using Bingosoft.TrioFramework.Workflow.Business;
using Bingosoft.TrioFramework.Workflow.Core;
using Bingosoft.TrioFramework.Workflow.Core.Models;

namespace Bingosoft.TrioFramework.Mvc.Controllers {
	/// <summary>
	/// 工作流基类
	/// </summary>
	public class WorkflowController : BaseController {

		private const string ModuleName = "流程控制器";

		/// <summary>
		/// 实例化工作流基类
		/// </summary>
		public WorkflowController() {
			InitActivityHandler();
		}

		/// <summary>
		/// 初始化流程环节配置
		/// </summary>
		protected virtual void InitActivityHandler() {
			var currentClass = this.GetType();
			var subClasses = currentClass.GetNestedTypes();
			foreach (var subClass in subClasses) {
				if (subClass.IsSubclassOf(typeof(ActivityHandler))) {
					var attrs = subClass.GetCustomAttributes(typeof(ActivityAttribute), true);
					if (attrs.Length == 0 || !(attrs[0] is ActivityAttribute))
						continue;

					var attr = attrs[0] as ActivityAttribute;
					var activityName = attr.ActivityName + "_" + attr.Version;
					if (this.handlers.ContainsKey(activityName)) {
						throw new ArgumentException(string.Format("流程{0}中配置重名的ActivityHandler：{1}", currentClass.Name, activityName));
					}
					this.handlers.Add(activityName, (ActivityHandler)Activator.CreateInstance(subClass));
				}
			}
		}

		/// <summary>
		/// 环节自定义处理器
		/// </summary>
		protected readonly IDictionary<string, ActivityHandler> handlers = new Dictionary<string, ActivityHandler>();
		/// <summary>
		/// 表单Readonly字段标识
		/// </summary>
		protected const string VIEWDATA_READONLY = "Readonly";
		/// <summary>
		/// 表单环节名称字段标识
		/// </summary>
		protected const string VIEWDATA_ACTIVITYNAME = "ActivityName";

		/// <summary>
		/// 业务表单类型
		/// </summary>
		protected virtual Type BusinessForm {
			get { return null; }
		}

		#region Attachment

		/// <summary>
		/// 附件列表
		/// </summary>
		/// <param name="instanceNo">流程实例</param>
		/// <param name="activityName">当前环节名称</param>
		/// <param name="version">当前流程版本号</param>
		/// <returns></returns>
		[HttpPost]
		public JsonResult Attachments(string instanceNo, string activityName, string version) {
			var oInstance = instanceNo.Decrypt();
			var oVersion = version.Decrypt();
			var oActivityName = activityName.Decrypt();
			var handlerKey = oActivityName + "_" + oVersion;
			var loginUser = SecurityContext.User;
			var attachments = WorkflowAttachment.GetFiles(oInstance);
			if (this.handlers.ContainsKey(handlerKey)) {
				var handler = this.handlers[handlerKey];
				foreach (var attachment in attachments) {
					attachment.IsCanView = true;
					attachment.IsCanDownload = true;
					attachment.IsCanDeleted = oActivityName.Equals(attachment.ActivityName, StringComparison.OrdinalIgnoreCase) && !attachment.IsDisabled;

					attachment.IsCanDownload = handler.CanDownloadAttachment(attachment);
					attachment.IsCanDeleted = handler.CanDeleteAttachment(attachment);
					attachment.IsCanView = handler.CanViewAttachment(attachment);
				}
			} else {
				foreach (var attachment in attachments) {
					attachment.IsCanDownload = true;
					attachment.IsCanView = true;
					attachment.IsCanDeleted = (loginUser.Id.Equals(attachment.CreatorId, StringComparison.OrdinalIgnoreCase) && oActivityName.Equals(attachment.ActivityName, StringComparison.OrdinalIgnoreCase));
				}
			}
			// 隐藏不能被显示的附件
			for (int i = 0; i < attachments.Count; i++) {
				if (!attachments[i].IsCanView) {
					attachments.RemoveAt(i);
					i--;
				}
			}
			return new JsonResult().Succeed(new { Total = attachments.Count, List = attachments });
		}

		/// <summary>
		/// 附件上传
		/// </summary>
		/// <param name="instanceNo">流程实例编号</param>
		/// <param name="taskId">任务id</param>
		/// <param name="fileType">附件类型</param>
		/// <returns></returns>
		[HttpPost]
		public JsonResult AttachmentUpload(string instanceNo, string taskId, int fileType) {
			var loginUser = SecurityContext.User;
			string oInstanceNo, oActivityName, oTaskId, handlerKey;
			// 解密字符串
			try {
				oInstanceNo = instanceNo.Decrypt();
				oTaskId = taskId.Decrypt();
				var engine = WorkflowEngine.Create();
				var instance = engine.LoadWorkflow(oInstanceNo, int.Parse(oTaskId));
				oActivityName = instance.CurrentActivity;
				handlerKey = oActivityName + "_" + instance.Version;
			} catch (DecryptException) {
				return Error(500, "获取附件上传参数时出错，可能数据已被修改，请尝试刷新页面");
			}
			if (Request.Files.Count == 0) {
				return Error(404, "请先选择需要上传的附件");
			}
			var file = Request.Files[0];
			if (file == null) {
				return Error(500, "请先选择需要上传的附件");
			}

			// 处理不同环节对附件的要求
			if (this.handlers.ContainsKey(handlerKey)) {
				var func = this.handlers[handlerKey];
				if (func != null) {
					var message = "";
					var canUpload = func.BeforeUploadAttachment(oInstanceNo, fileType, out message);
					if (!canUpload) {
						return new JsonResult() { ContentType = "text/html" }.Error(500, message);
					}
				}
			}

			// 在IE6下上传附件会获取到文件的全路径，在FF和Chrome下只获取到文件名，所以这里要对IE的情况做处理
			// C:\Users\Zeran\Desktop\xxx.xls
			var fileName = Path.GetFileName(file.FileName);
			var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
			var fileExtension = Path.GetExtension(fileName);
			// 生成存放在文件服务器上的路径，确保唯一
			var filePath = string.Format("{0}/Flow_{1}_{2}_{3}{4}", DateTime.Today.ToString("yyyy-MM-dd"), fileNameWithoutExtension, DateTime.Now.ToString("HHmmss"), new Random().Next(1000), fileExtension);

			var attachment = new WorkflowAttachment() {
				InstanceNo = oInstanceNo,
				ActivityName = oActivityName,
				TaskId = oTaskId.ToInt(),
				FileType = fileType,
				FileName = fileName,
				FilePath = filePath,
				FileSize = file.ContentLength,
				CreatorId = loginUser.Id,
				Creator = loginUser.Name,
				CreatedTime = DateTime.Now
			};
			try {
				if (attachment.Save()) {
					try {
						WebDavHelper.UploadFile(filePath, file.InputStream);

						// 处理不同环节对附件的要求
						if (this.handlers.ContainsKey(handlerKey)) {
							var func = this.handlers[handlerKey];
							if (func != null) {
								var message = "";
								func.AfterUploadAttachment(oInstanceNo, fileType, out message);
								if (!string.IsNullOrEmpty(message)) {
									return new JsonResult() { ContentType = "text/html" }.Error(500, message);
								}
							}
						}
						return new JsonResult() { ContentType = "text/html" }.Succeed();
					} catch (Exception ex) {
						attachment.Delete();
						return new JsonResult() { ContentType = "text/html" }.Error(500, ex.Message);
					}
				}
				return new JsonResult() { ContentType = "text/html" }.Error(500, "保存附件记录失败");
			} catch (Exception ex) {
				return new JsonResult() { ContentType = "text/html" }.Error(500, "保存附件记录失败，错误信息：" + ex.Message);
			}
		}

		/// <summary>
		/// 附件删除
		/// </summary>
		/// <param name="fileId">文件Id</param>
		/// <param name="activityName">当前环节</param>
		/// <param name="version">当前流程版本号</param>
		/// <returns></returns>
		/// <remarks>
		/// token为用户信息、附件id、有效期的加密字符串，用于防止附件的任意下载
		/// </remarks>
		[HttpPost]
		public ActionResult AttachmentDelete(string fileId, string activityName, string version) {
			var oActivityName = activityName.Decrypt();
			var oVersion = version.Decrypt();
			var handlerKey = oActivityName + "_" + oVersion;
			var attachment = WorkflowAttachment.Get(fileId);
			// 进行权限检查

			// 处理不同环节对附件的要求
			if (this.handlers.ContainsKey(handlerKey)) {
				var func = this.handlers[handlerKey];
				if (func != null) {
					var message = "";
					var canDelete = func.BeforeDeleteAttachment(attachment, out message);
					if (!canDelete) {
						return Error(302, message);
					}
				}
			}

			var isSuccess = attachment.MarkedDeleted();
			if (isSuccess) {
				return Success();
			} else {
				return Error(500, "附件删除失败");
			}
		}

		#endregion

		/// <summary>
		/// 表单渲染
		/// </summary>
		/// <param name="form">流程表单</param>
		/// <param name="activityName">环节名称</param>
		/// <returns></returns>
		[ChildActionOnly]
		public ActionResult Render(WorkflowForm form, string activityName) {
			var handlerKey = activityName + "_" + form.Instance.Version;
			var xForm = form.BusinessForm;
			if (this.handlers.ContainsKey(handlerKey)) {
				var isReadonly = form.CurrentActi != activityName
				                 || (form.CurrentActi.Equals(activityName, StringComparison.OrdinalIgnoreCase) && form.Status == FormStatus.Done);
				var viewResult = this.handlers[handlerKey].Render(form, xForm);
				if (viewResult != null) {
					viewResult.ViewData[VIEWDATA_READONLY] = isReadonly;
					viewResult.ViewData[VIEWDATA_ACTIVITYNAME] = activityName;
					return viewResult;
				}
			}
			return View("Empty");
		}

		/// <summary>
		/// 保存流程
		/// </summary>
		/// <param name="form"></param>
		/// <returns></returns>
		[HttpPost]
		public ActionResult Save(WorkflowForm form) {
			try {
				Thread.Sleep(1000);
				var xForm = LoadForm(form);
				// 验证标题
				var errorMessage = "";
				if (!xForm.BusinessForm.Validate(out errorMessage)) {
					return Error(301, errorMessage);
				}
				// 从表单中获取并设置流程实例的标题
				// 一般情况下，只有第一个环节的时候，表单中的Title才不会为空
				if (!string.IsNullOrEmpty(xForm.BusinessForm.Title)) {
					xForm.Instance.Title = xForm.BusinessForm.Title;
				}
				var handlerKey = xForm.CurrentActi + "_" + xForm.Instance.Version;
				if (this.handlers.ContainsKey(handlerKey)) {
					var func = this.handlers[handlerKey];
					if (func != null) {
						try {
							func.Save(xForm.ActionMode, xForm.BusinessForm);
						} catch (Exception ex) {
							Logger.LogError(ModuleName, "保存自定义表单数据时出错", ex, "");
							return Error(500, ex.Message);
						}
					}
				}
				// 表单保存成功后，再保存流程
				var engine = WorkflowEngine.Create();
				var engineSaved = engine.SaveWorkflow(xForm.Instance);
				if (!engineSaved) {
					return Error(500, "流程保存失败");
				}
				// 流程保存后，从流程实例中获取流程编号
				// 然后更新表单的流程编号
				xForm.InstanceNo = xForm.Instance.InstanceNo;
				xForm.BusinessForm.InstanceNo = xForm.Instance.InstanceNo;
				xForm.BusinessForm.UpdateInstanceNo();

				if (form.ActionMode == ActionMode.Submit) {
					// 流程提交
					// 计算默认的角色参与者（包括用户自定义的）
					var selector = new ApproveSelector(xForm.Instance);
					// 处理用户自定义参与者
					if (this.handlers.ContainsKey(handlerKey)) {
						var func = this.handlers[handlerKey];
						if (func != null) {
							try {
								func.ResolveActor(xForm.BusinessForm, selector);
							} catch (Exception ex) {
								Logger.LogError(ModuleName, "计算自定义环节参与者时出错", ex, "");
								return new JsonResult().Error(500, ex.Message);
							}
						}
					}
					xForm.Selector = selector;
					// 用于弹出审批页面
					return new JsonResult().Succeed(xForm.Selector.ToString());
				} else {
					// 保存流程时，返回新的流程参数，用于页面上的重定向和刷新页面（由草稿重定向到待办）
					return new JsonResult().Succeed(new { appCode = xForm.AppCode, instanceNo = xForm.InstanceNo, taskId = xForm.TaskId });
				}
			} catch (Exception ex) {
				Logger.LogError(ModuleName, "流程保存时出错", ex, form);
				return new JsonResult().Error(500, ex.Message);
			}
		}

		/// <summary>
		/// 表单提交
		/// </summary>
		/// <param name="form"></param>
		/// <returns></returns>
		[HttpPost]
		public ActionResult Submit(WorkflowForm form) {
			try {
				Thread.Sleep(1500);

				if (TryUpdateModel(form)) {
					UpdateModel(form);
				}

				form.InstanceNo = form.InstanceNo.Decrypt();
				form.CurrentActi = form.CurrentActi.Decrypt();
				form.VersionStr = form.VersionStr.Decrypt();

				var handlerKey = form.CurrentActi + "_" + form.VersionStr;                

				// 处理流程提交前自定义事件
				if (this.handlers.ContainsKey(handlerKey)) {
					var func = this.handlers[handlerKey];
					if (func != null) {
						try {
							func.BeforeSubmit(form.InstanceNo, form.ApproveResult.Choice);
						} catch (Exception ex) {
							Logger.LogError(ModuleName, "流程提交前自定义事件出错", ex, "");
							return Error(500, ex.Message);
						}

						try {
							func.ResolveTobeReadActor(form.BusinessForm, form.TobeReadSelector);
						} catch (Exception ex) {
							Logger.LogError(ModuleName, "流程提交前计算待阅人员出错", ex, "");
							return Error(500, ex.Message);
						}
					}
				}

				var engine = WorkflowEngine.Create();
				var instance = engine.LoadWorkflow(form.InstanceNo, form.TaskId);
				var runSuccess = engine.RunWorkflow(instance, form.ApproveResult, form.TobeReadSelector.UserIds);
				if (runSuccess) {

					// 处理流程提交后自定义事件
					if (this.handlers.ContainsKey(handlerKey)) {
						var func = this.handlers[handlerKey];
						if (func != null) {
							try {
								func.AfterSubmit(form.InstanceNo, form.ApproveResult.Choice);
							} catch (Exception ex) {
								Logger.LogError(ModuleName, "流程提交后自定义事件出错", ex, form);
								return Error(500, ex.Message);
							}
						}
					}

					return Success();
				} else {
					return Error(500, "请稍候重试");
				}
			} catch (Exception ex) {
				Logger.LogError(ModuleName, "流程提交时出错", ex, form);
				return Error(500, ex.Message);
			}
		}

		/// <summary>
		/// 删除工单
		/// </summary>
		/// <param name="form"></param>
		/// <returns></returns>
		[HttpPost]
		public ActionResult Delete(WorkflowForm form) {
			try {
				Thread.Sleep(1000);

				form.InstanceNo = form.InstanceNo.Decrypt();
				form.CurrentActi = form.CurrentActi.Decrypt();

				var handlerKey = form.CurrentActi + "_" + form.VersionStr;  
				// 处理流程删除前自定义事件
				if (this.handlers.ContainsKey(handlerKey)) {
					var func = this.handlers[handlerKey];
					if (func != null) {
						try {
							func.BeforeDelete(form.InstanceNo);
						} catch (Exception ex) {
							Logger.LogError(ModuleName, "流程删除前自定义事件出错", ex, form);
							return Error(500, ex.Message);
						}
					}
				}

				var engine = WorkflowEngine.Create();
				var instance = engine.LoadWorkflow(form.InstanceNo, form.TaskId);
				var success = engine.DeleteWorkflow(instance);
				if (success) {
					// 处理流程删除后自定义事件
					if (this.handlers.ContainsKey(handlerKey)) {
						var func = this.handlers[handlerKey];
						if (func != null) {
							try {
								func.AfterDeleted(form.InstanceNo);
							} catch (Exception ex) {
								Logger.LogError(ModuleName, "流程删除后自定义事件出错", ex, form);
								return Error(500, ex.Message);
							}
						}
					}
					return Success();
				} else {
					return Error(500, "请稍后重试");
				}
			} catch (Exception ex) {
				Logger.LogError(ModuleName, "流程删除时出错", ex, form);
				return Error(500, ex.Message);
			}
		}


		/// <summary>
		/// 签收工单
		/// </summary>
		/// <param name="form"></param>
		/// <returns></returns>
		[HttpPost]
		public ActionResult Sign(WorkflowForm form) {
			try {
				Thread.Sleep(1000);

				form.InstanceNo = form.InstanceNo.Decrypt();
				form.CurrentActi = form.CurrentActi.Decrypt();

				var handlerKey = form.CurrentActi + "_" + form.VersionStr;  

				// 处理流程签收前自定义事件
				if (this.handlers.ContainsKey(handlerKey)) {
					var func = this.handlers[handlerKey];
					if (func != null) {
						try {
							func.BeforeSign(form.BusinessForm);
						} catch (Exception ex) {
							Logger.LogError(ModuleName, "流程签收前自定义事件出错", ex, form);
							return Error(500, ex.Message);
						}
					}
				}

				var engine = WorkflowEngine.Create();
				engine.SignWorkflow(form.InstanceNo, form.TaskId);
				return Success();
			} catch (Exception ex) {
				Logger.LogError(ModuleName, "流程签收时出错", ex, form);
				return Error(500, ex.Message);
			}
		}

		/// <summary>
		/// 流程提交
		/// </summary>
		/// <param name="form"></param>
		/// <returns></returns>
		[HttpPost]
		protected WorkflowForm LoadForm(WorkflowForm form) {
			WorkflowForm nform = null;
			// 更新流程数据
			if (TryUpdateModel(form)) {
				UpdateModel(form);

				var engine = WorkflowEngine.Create();
				if (!string.IsNullOrEmpty(form.InstanceNo)) {
					var instanceNo = form.InstanceNo.Decrypt();
					var instance = engine.LoadWorkflow(form.AppCode, instanceNo, form.TaskId);
					nform = WorkflowForm.Init(instance);
				} else {
					var instance = engine.CreateWorkflow(form.AppCode);
					nform = WorkflowForm.Init(instance);
				}
				nform.ActionModeStr = form.ActionModeStr;
			} else {
				throw new NullReferenceException("加载表单数据失败");
			}
			// 更新表单数据
			if (BusinessForm == null) {
				throw new TypeLoadException("未指定当前流程的业务实体类型，流程控制器未实现BusinessForm属性");
			}
			var bizform = (BusinessForm)Activator.CreateInstance(BusinessForm);
			if (!string.IsNullOrEmpty(form.InstanceNo)) {
				// 除新发起的流程外，需要重新加载业务表单
				bizform.Load(form.InstanceNo.Decrypt());
			}
			// 从界面上读取数据到业务表单实体
			bizform = (BusinessForm)Request.ToModel(bizform, HttpRequestExtension.ModelSource.Form);
			// 流程单号是以加密的形式保存在界面上，所以这里从界面上重新读取数据后，需要重新解密
			if (!string.IsNullOrEmpty(bizform.InstanceNo)) {
				bizform.InstanceNo = bizform.InstanceNo.Decrypt();
			}
			nform.BusinessForm = bizform;
			return nform;
		}

		/// <summary>
		/// 获取业务表单
		/// </summary>
		/// <typeparam name="T">业务表单类型</typeparam>
		/// <param name="bform">业务表单通用类</param>
		/// <param name="form">业务表单</param>
		/// <returns></returns>
		protected bool TryParseForm<T>(BusinessForm bform, out T form) {
			form = default(T);
			try {
				form = (T)Convert.ChangeType(bform, typeof(T));
				return true;
			} catch (InvalidCastException) {
				return false;
			}
		}
	}
}
