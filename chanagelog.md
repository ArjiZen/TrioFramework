---
layout: post
title: "TrioFramework更新日志"
date: 2015-05-05 23:19:00
tags: TrioFramework
---
*说明：主要记录每个版本变更的内容，及旧版本升级到新版本时需要注意和变更的内容*

<!--more-->

# Component

+ ## Excel 

+ ### v1.1.0

    1. 增加导出Excel中多级表头的支持

+ ### v1.0.0

    1. 增加Excel导出功能的NPOI实现

---

# TrioFramework.Worklfow

+ ### v1.5.1

    1. 增加记录流程提交AutoFinished=1的堆栈信息

+ ### v1.5.0

    1. 重构ActivityHandler各自定义事件的参数
    1. BusinessForm增加类型转换方法ConvertTo
    1. 移除WorkflowController的TryParseForm方法

---

# TrioFramework

+ ### v1.6.0
    
    1. 由 TrioMessage 代替 JsonModel 作为WebApi通信传输对象
    2. Mvc增加引用资源文件自动追加版本标记的功能
        + 基于资源文件内容摘要的Md5版本标记生成
        + 基于资源文件最后修改时间的版本标记生成
    3. 在Global.ascx的Application_Start加入以下代码用于监控资源文件的变化并重新生成版本标记
    
```
protected void Application_Start()
{
    // 启动资源文件监控
    ResFileVerTokenMarker.Start();
}
protected void Application_End()
{
    // 停止资源文件监控
    ResFileVerTokenMarker.Stop();
}
```
    

+ ### v1.5.0

    1. 增加Excel组件配置项

+ ### v1.4.3

    1. 调整附件列表，按上传时间倒序排列

+ ### v1.4.2

    1. 修复iOffice待办推送的建单人字段错误的问题

    ```
    ALTER TABLE dbo.WF_PendingJob ADD Creator NVARCHAR(100)
    ALTER TABLE dbo.WF_PendingJob ADD CreatorId VARCHAR(100)
    ```

+ ### v1.4.1

    1. 增加iOffice待办推送失败次数限制

    ```
    ALTER TABLE dbo.WF_PendingJob ADD ErrorTimes INT DEFAULT(0)
    ```

+ ### v1.4.0

    1. ActivityHandler中 `BeforeUploadAttachment()` 和 `AfterUploadAttachment()` 上下文参数

    1. Web接口调用日志接口

    1. 流程的ActionMode增加退回项 `Rollback`

+ ### v1.3.1

    1. 个人常用意见 `PersonalOpinion` 类，包含读取、保存功能，读取常用意见时按使用次数倒序排列
    1. 流程提交时检查用户的处理意见，如果为用户的常用意见时，则将该常用意见的使用次数+1
    1. BaseController 增加 ModuleName 方便记录日志
    1. BaseController 增加 `JsonExecutor` 方法，封装try-catch及返回值，独立执行逻辑
    1. 中途意见 `MidwayOpinion` 类

**更新脚本**

```
-- 个人常用意见
CREATE TABLE [dbo].[WF_PersonalOpinions](
	[Id] [BIGINT] IDENTITY(1,1) NOT NULL,
	[UserId] [VARCHAR](50) NOT NULL,
	[Content] [NVARCHAR](500) NOT NULL,
	[UsedTimes] [INT] NOT NULL,
	[IsDeleted] [BIT] NOT NULL,
	[DeleteTime] [DATETIME] NULL,
 CONSTRAINT [PK_WF_PersonalOpinions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[WF_PersonalOpinions] ADD  CONSTRAINT [DF_WF_PersonalOpinions_UsedTimes]  DEFAULT ((0)) FOR [UsedTimes]
GO

ALTER TABLE [dbo].[WF_PersonalOpinions] ADD  CONSTRAINT [DF_WF_PersonalOpinions_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
```


```
-- 中途意见
CREATE TABLE [dbo].[WF_MidwayOpinions](
	[Id] [BIGINT] IDENTITY(1,1) NOT NULL,
	[InstanceNo] [VARCHAR](50) NOT NULL,
	[TaskId] [INT] NOT NULL,
	[Content] [NVARCHAR](MAX) NOT NULL,
	[CreatorId] [VARCHAR](50) NOT NULL,
	[Creator] [NVARCHAR](50) NOT NULL,
	[CreateTime] [DATETIME] NOT NULL,
	[IsDeleted] [BIT] NOT NULL,
	[DeleteTime] [DATETIME] NULL,
 CONSTRAINT [PK_WF_MidwayOpinions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[WF_MidwayOpinions] ADD  CONSTRAINT [DF_WF_MidwayOpinions_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
```

+ ### v1.3.0

    1. 获取流程最大实例编号，修改为获取当前日期的最大实例编号
    1. 流程传阅功能（流程在当前环节传阅给其他用户）
        1. 流程传阅接口 `WorkflowEngine.Create().PassAround()`
        1. ActivityHandler 中增加 `ResolvePassAroundActor()` 计算传阅用户事件
        1. WorkflowController 中增加 `PassAround()` 用于流程传阅
        1. **传阅和待阅的区别**
            1. 待阅是在流程提交的时候产生
            1. 传阅的时候，流程不会提交到下一步
    1.  BusinessException 业务异常类，用于ActivityHandler的子类中，用于输出业务类型异常（即不需要记录异常日志的消息）

+ ### v1.2.2

    1. `SecurityContext.Provider.Get()`支持根据loginid和userid设置用户

    1. 待阅人员计算优化，待阅人员选择 `TobeReadSelector` 增加`Add()`、`Clear()`，用于根据审核结果设置不同的待阅人员。

    1. 待办人员选择 `ApproveSelector` 优化。
        +  `AddUser()` -> `Add()`
        + `RemoveActivity()` -> `RemoveChoice()`
        + `ClearUser()` -> `Clear()`

    1. WorkflowEngine的 `RunWorkflow()` 接口移除 `tobeReadUsersId` 参数，将待阅用户id整合到ApproveResult对象的 `NextTobeReadUsers` 属性

+ ### v1.2.1

    1. ActivityHandler增加 `BeforeSign` 签收前自定义事件
    1. 流程签收功能，WorkflowItem增加 `IsSign` 是否签收字段
    1. 待办推送配置文件修改为 `apiUrl` 和 `jobUrl` 字段
    1. K2ActivityConfig中的RoleName的建单人修改为**建单用户**

**更新配置**

```
<pendingJob 
	isEnabled="true"
	isEnabledSMS="false" 
	apiUrl="http://tedstodo.sz.gmcc.net/webservice/pendingjobII.asmx"
	jobUrl="http://itsp.sz.gmcc.net/mas/Workflow/Form/View?appCode={appCode}&amp;instanceNo={instanceNo}&amp;taskId={taskId}"/>
```

  + 推送待办配置说明
    + apiUrl：待办接口地址
    + jobUrl：待办推送地址


**更新脚本**

```
ALTER TABLE dbo.WF_WorkflowItem ADD IsSign BIT DEFAULT(0)
```

+ ### v1.2.0（2015-01-05）

    1. WorkflowItem的TaskStatus增加待阅状态（状态码4）
        + 增加 ResolveTobeReadActor Handler，用于计算待阅人员

    1. 设置流程当前用户接口，用于单元测试及WebService的流程调用

```
var engine = WorkflowEngine.Create();
engine.SetCurrentUser("xxx");
```

1. 流程签收功能
    + WorkflowItem增加SignTime列
    + `WorkflowEngine.SignWorkflow()` 用于流程签收
    + K2WorkflowEngine增加SignWorkflow的接口实现

1. 推送待办相关功能
    + 新增WF_PendingJob表

1. 人员委托相关功能
    + 修改表WF_Delegates结构
        + 移除IsEnabled列，增加是否删除列及删除时间列
        + Id列改为自增列
    + 修改表WF_WorkflowItem结构
    + 增加被委托人Id和被委托人字段

**配置更新**

```
<?xml version="1.0" encoding="utf-8"?>
<configuration>>
    <configSections>
        <section name="trio" type="Bingosoft.TrioFramework.TrioSection, Bingosoft.TrioFramework"/>
    </configSections>
    <trio>
        <common 
            encryptKey="123456" 
            systemName="TrioFramework UnitTest" 
            systemId="TrioFramework" 
            systemAccount="TrioFramework" 
            systemPassword="pass@word1"/>
        <pendingJob 
            isEnabled="true"
            isEnabledSMS="false" 
            url="http://tedstodo.sz.gmcc.net/webservice/pendingjobII.asmx" />
        <workflow 
            provider="Bingosoft.TrioFramework.Workflow.K2Client.K2WorkflowEngine,Bingosoft.TrioFramework.Workflow.K2Client"
            isConnectK2="false" />
        <fileServer 
            server="http://fs.sz.gmcc.net/itsp-mas-dev" 
            userName="administrator"
            password="pass@word1" />
    </trio>
</configuration>
```

+ common 节点
    + encryptKey：用于加密的Key
    + systemName：系统名称
    + systemId：系统Id（用于待办或短信）
    + systemAccount：系统账号（用于待办或短信）
    + systemPassword：系统密码（用于待办和短信）

+ pendingJob 节点：
    + isEnabled：是否启用待办推送
    + isEnableSMS：是否启用待办短信提醒
    + url：WebService接口地址

+ workflow：
    + provider：K2接口实现
    + isConnectK2：是否连接K2服务器

+ fileServer：
    + server：文件服务器WebDAV地址
    + userName：登录的用户名
    + password：登录的密码

**脚本更新**

```
ALTER TABLE dbo.WF_WorkflowItem ADD SignTime DATETIME
```

```
CREATE TABLE [dbo].[WF_PendingJob]
(
            [Id] [bigint] IDENTITY(1,1) NOT NULL,
            [InstanceNo] [varchar](50)  NOT NULL,
            [TaskId] [int]  NOT NULL,
            [JobId] [varchar](50)  NULL,
            [JobTitle] [nvarchar](200)  NULL,
            [Url] [varchar](255)  NULL,
            [UserId] [varchar](100)  NULL,
            [UserName] [nvarchar](50)  NULL,
            [DoDelete] [bit]  NULL,
            [DoPush] [bit]  NULL,
            [DoFinish] [bit]  NULL,
            [CreateTime] [DATETIME] NOT NULL,
            [PushedTime] [datetime]  NULL,
            [FinishedTime] [datetime]  NULL,
            [DeletedTime] [datetime]  NULL,
            [Result] [nvarchar](MAX)  NULL
)
ALTER TABLE [dbo].[WF_PendingJob] ADD CONSTRAINT PK_WF_PENDINGJOB PRIMARY KEY  ([Id])
GO
```

```
ALTER TABLE dbo.WF_Delegates DROP COLUMN IsEnabled
ALTER TABLE dbo.WF_Delegates ADD IsDeleted BIT DEFAULT(0)
ALTER TABLE dbo.WF_Delegates ADD DeleteTime DATETIME
```

```
alter table [dbo].[WF_Delegates] drop constraint PK_WF_DELEGATES
alter table [dbo].[WF_Delegates] drop column Id
alter table [dbo].[WF_Delegates] add Id bigint Identity(1,1) 
alter table [dbo].[WF_Delegates] add constraint PK_WF_DELEGATES primary key  ([Id])
```

```
ALTER TABLE dbo.WF_WorkflowItem ADD MandataryId VARCHAR(50)
ALTER TABLE dbo.WF_WorkflowItem ADD Mandatary NVARCHAR(50)
```

+ ### v1.1.2（2014-12-7）

    1. Request.ToModel，实体中包含以下可空类型属性的赋值问题
        + DateTime?
        + Decimal?
        + int?
        + bool?
        + long?
        + short?

    1. ActivityHandler 增加`AfterSubmit`、`BeforeDelete`、`AfterDeleted`方法

+ ### v1.1.1

    1. 调整DictionaryCollection到TrioFramework项目中（原TrioFramework.Mvc项目），Mvc项目中保留DictionaryCollection的`ToSelectListItem()`扩展

    1. WorkflowInstance增加Description字段，读取当前流程定义中的描述

    1. RequestExtension.ToModel 中增加属性是否可写入的判断

+ ### v1.1.0

    1.  流程提交页面的默认负责人配置数据
	    1. 增加default节点数据
    1.  由系统审批的流程项在审批历史中显示为“自动处理” 
    1.  增加流程环节自定义处理的多版本支持
        1. ActivityAttribute增加Version属性，为空时默认为version.1
        1. WorkflowForm增加VersionStr属性，用于传输从界面上保存的当前流程实例版本号
        1. 修改ActivityHandler的Submit方法为Save 
        1. 优化ActivityHandler，调整Save方法和ResolveActor方法返回值为void，调整Render和Save方法的abstract  ＝> virtual
    1.  流程回退时的人员计算问题，修改为RefActivityName的方式计算退回环节的办理人
    1.  修改日志中当前系统名称的获取为Web.Config中配置

```
<appSettings>
       <add name='SystemName' value='' />
</appSettings>
```

1.  K2流程相关配置

```
<connectionString>
        <!--K2流程引擎服务器连接字符串-->
        <add name='K2HostServer' connectionString=''>
        <!--K2流程管理服务器连接字符串-->
        <add name='K2Managerment' connectionString=''>
</connectionString>
<appSettings>
        <!--流程引擎实例-->
        <add key='WorkflowEngine' value='Bingosoft.TrioFramework.Workflow.K2Client.K2WorkflowEngine,Bingosoft.TrioFramework.Workflow.K2Client'>
        <!--是否启用K2服务器-->
        <add key='K2Online' value='true/false'>
</appSettings>
```

+ ### v1.0.0

    1.  完成工作流框架的基本内容
    1.  基于K2工作流的设计，并实现本地流程记录的保存（已实现）和服务器的接口调用
    1.  基础框架：包含用于Rest接口的通信消息对象及Rest调用客户端
    1.  UI框架：基于MVC4的前端界面，抽象并封装了WorkflowController，提供流程公共行为