/*==============================================================*/
/* DBMS name:      Microsoft SQL Server 2005                    */
/* Created on:     2012/2/9 12:15:55                            */
/*==============================================================*/


if exists (select 1
            from  sysobjects
           where  id = object_id('SYS_BusinessOperateLog')
            and   type = 'U')
   drop table SYS_BusinessOperateLog
go

if exists (select 1
            from  sysobjects
           where  id = object_id('SYS_LogonLog')
            and   type = 'U')
   drop table SYS_LogonLog
go

if exists (select 1
            from  sysobjects
           where  id = object_id('SYS_ServiceCallLog')
            and   type = 'U')
   drop table SYS_ServiceCallLog
go

if exists (select 1
            from  sysobjects
           where  id = object_id('SYS_UrlRequestLog')
            and   type = 'U')
   drop table SYS_UrlRequestLog
go

/*==============================================================*/
/* Table: SYS_BusinessOperateLog                                */
/*==============================================================*/
create table SYS_BusinessOperateLog (
   BusinessOperateLogId varchar(128)         not null,
   Application          varchar(128)         null,
   Module               varchar(128)         null,
   Actor                varchar(128)         null,
   Action               varchar(128)         null,
   ActionContent        varchar(512)         null,
   ActionTime           datetime             null,
   IsSuccess            bit                  null,
   ExceptionType        varchar(128)         null,
   ExceptionMsg         varchar(4000)        null,
   constraint PK_SYS_BUSINESSOPERATELOG primary key (BusinessOperateLogId)
)
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '业务操作日志',
   'user', @CurrentUser, 'table', 'SYS_BusinessOperateLog'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'BusinessOperateLogId',
   'user', @CurrentUser, 'table', 'SYS_BusinessOperateLog', 'column', 'BusinessOperateLogId'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'Application',
   'user', @CurrentUser, 'table', 'SYS_BusinessOperateLog', 'column', 'Application'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'Module',
   'user', @CurrentUser, 'table', 'SYS_BusinessOperateLog', 'column', 'Module'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'Actor',
   'user', @CurrentUser, 'table', 'SYS_BusinessOperateLog', 'column', 'Actor'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'Action',
   'user', @CurrentUser, 'table', 'SYS_BusinessOperateLog', 'column', 'Action'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'ActionContent',
   'user', @CurrentUser, 'table', 'SYS_BusinessOperateLog', 'column', 'ActionContent'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'ActionTime',
   'user', @CurrentUser, 'table', 'SYS_BusinessOperateLog', 'column', 'ActionTime'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'IsSuccess',
   'user', @CurrentUser, 'table', 'SYS_BusinessOperateLog', 'column', 'IsSuccess'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'ExceptionType',
   'user', @CurrentUser, 'table', 'SYS_BusinessOperateLog', 'column', 'ExceptionType'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'ExceptionMsg',
   'user', @CurrentUser, 'table', 'SYS_BusinessOperateLog', 'column', 'ExceptionMsg'
go

/*==============================================================*/
/* Table: SYS_LogonLog                                          */
/*==============================================================*/
create table SYS_LogonLog (
   Id                   varchar(128)         not null,
   UserId               varchar(128)         null,
   Account              varchar(128)         null,
   LogonTime            datetime             null,
   LogonIp              varchar(128)         null,
   RequestUrl           varchar(4000)        null,
   LogoutTime           datetime             null,
   constraint PK_SYS_LOGONLOG primary key (Id)
)
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '登录日志',
   'user', @CurrentUser, 'table', 'SYS_LogonLog'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'Id',
   'user', @CurrentUser, 'table', 'SYS_LogonLog', 'column', 'Id'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '登录用户ID',
   'user', @CurrentUser, 'table', 'SYS_LogonLog', 'column', 'UserId'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '登录用户账号',
   'user', @CurrentUser, 'table', 'SYS_LogonLog', 'column', 'Account'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '登录时间',
   'user', @CurrentUser, 'table', 'SYS_LogonLog', 'column', 'LogonTime'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '用户登录IP',
   'user', @CurrentUser, 'table', 'SYS_LogonLog', 'column', 'LogonIp'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '访问请求的URL',
   'user', @CurrentUser, 'table', 'SYS_LogonLog', 'column', 'RequestUrl'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '注销时间',
   'user', @CurrentUser, 'table', 'SYS_LogonLog', 'column', 'LogoutTime'
go

/*==============================================================*/
/* Table: SYS_ServiceCallLog                                    */
/*==============================================================*/
create table SYS_ServiceCallLog (
   ServiceCallLogId     varchar(128)         not null,
   Application          varchar(128)         null,
   Module               varchar(128)         null,
   CallInterface        varchar(128)         null,
   RequestTime          datetime             null,
   RequestContent       varchar(4000)        null,
   RequestContentLength int                  null,
   IsSuccess            bit                  null,
   ResponseTime         datetime             null,
   ResponseContent      varchar(4000)        null,
   ResponseContentLength int                  null,
   ExceptionType        varchar(128)         null,
   ExceptionMsg         varchar(4000)        null,
   constraint PK_SYS_SERVICECALLLOG primary key (ServiceCallLogId)
)
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '服务调用日志',
   'user', @CurrentUser, 'table', 'SYS_ServiceCallLog'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'ServiceCallLogId',
   'user', @CurrentUser, 'table', 'SYS_ServiceCallLog', 'column', 'ServiceCallLogId'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'Application',
   'user', @CurrentUser, 'table', 'SYS_ServiceCallLog', 'column', 'Application'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'Module',
   'user', @CurrentUser, 'table', 'SYS_ServiceCallLog', 'column', 'Module'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'CallInterface',
   'user', @CurrentUser, 'table', 'SYS_ServiceCallLog', 'column', 'CallInterface'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'RequestTime',
   'user', @CurrentUser, 'table', 'SYS_ServiceCallLog', 'column', 'RequestTime'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'RequestContent',
   'user', @CurrentUser, 'table', 'SYS_ServiceCallLog', 'column', 'RequestContent'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'RequestContentLength',
   'user', @CurrentUser, 'table', 'SYS_ServiceCallLog', 'column', 'RequestContentLength'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'IsSuccess',
   'user', @CurrentUser, 'table', 'SYS_ServiceCallLog', 'column', 'IsSuccess'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'ResponseTime',
   'user', @CurrentUser, 'table', 'SYS_ServiceCallLog', 'column', 'ResponseTime'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'ResponseContent',
   'user', @CurrentUser, 'table', 'SYS_ServiceCallLog', 'column', 'ResponseContent'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'ResponseContentLength',
   'user', @CurrentUser, 'table', 'SYS_ServiceCallLog', 'column', 'ResponseContentLength'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'ExceptionType',
   'user', @CurrentUser, 'table', 'SYS_ServiceCallLog', 'column', 'ExceptionType'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'ExceptionMsg',
   'user', @CurrentUser, 'table', 'SYS_ServiceCallLog', 'column', 'ExceptionMsg'
go

/*==============================================================*/
/* Table: SYS_UrlRequestLog                                     */
/*==============================================================*/
create table SYS_UrlRequestLog (
   Id                   varchar(128)         not null,
   UserId               varchar(128)         null,
   Account              varchar(128)         null,
   RequestUrl           varchar(4000)        null,
   RequestTime          datetime             null,
   ConsumingTime        double precision     null,
   constraint PK_SYS_URLREQUESTLOG primary key (Id)
)
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'SYS_UrlRequestLog',
   'user', @CurrentUser, 'table', 'SYS_UrlRequestLog'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'Id',
   'user', @CurrentUser, 'table', 'SYS_UrlRequestLog', 'column', 'Id'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '登录用户ID',
   'user', @CurrentUser, 'table', 'SYS_UrlRequestLog', 'column', 'UserId'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '登录用户账号',
   'user', @CurrentUser, 'table', 'SYS_UrlRequestLog', 'column', 'Account'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '请求的URL',
   'user', @CurrentUser, 'table', 'SYS_UrlRequestLog', 'column', 'RequestUrl'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '请求时间',
   'user', @CurrentUser, 'table', 'SYS_UrlRequestLog', 'column', 'RequestTime'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '耗时',
   'user', @CurrentUser, 'table', 'SYS_UrlRequestLog', 'column', 'ConsumingTime'
go

