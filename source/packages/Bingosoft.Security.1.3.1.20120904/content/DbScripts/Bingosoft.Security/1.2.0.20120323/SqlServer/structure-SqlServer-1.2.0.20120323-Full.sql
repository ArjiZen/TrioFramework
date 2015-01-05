/*==============================================================*/
/* DBMS name:      Microsoft SQL Server 2005                    */
/* Created on:     2011/8/11 15:04:46                           */
/*==============================================================*/


if exists (select 1
            from  sysobjects
           where  id = object_id('v_sec_manage_role')
            and   type = 'V')
   drop view v_sec_manage_role
go

if exists (select 1
            from  sysobjects
           where  id = object_id('v_sec_valid_org')
            and   type = 'V')
   drop view v_sec_valid_org
go

if exists (select 1
            from  sysobjects
           where  id = object_id('v_sec_workflow_role')
            and   type = 'V')
   drop view v_sec_workflow_role
go

/*==============================================================*/
/* Table: SEC_Log                                               */
/*==============================================================*/
create table SEC_Log (
   Id                   varchar(38)          not null,
   UserId               uniqueidentifier     null,
   LogType              varchar(30)          null default 'auth'
      constraint CKC_LOGTYPE_SEC_LOG check (LogType is null or (LogType in ('login','auth','operation'))),
   UserName             varchar(50)          null,
   OperationName        varchar(50)          null,
   OperationTime        datetime             null,
   Description          varchar(3000)        null,
   constraint PK_SEC_LOG primary key (Id)
)
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '安全日志表',
   'user', @CurrentUser, 'table', 'SEC_Log'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '日志标识',
   'user', @CurrentUser, 'table', 'SEC_Log', 'column', 'Id'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '用户标识',
   'user', @CurrentUser, 'table', 'SEC_Log', 'column', 'UserId'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '日志类型',
   'user', @CurrentUser, 'table', 'SEC_Log', 'column', 'LogType'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '操作用户',
   'user', @CurrentUser, 'table', 'SEC_Log', 'column', 'UserName'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '操作名称',
   'user', @CurrentUser, 'table', 'SEC_Log', 'column', 'OperationName'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '操作时间',
   'user', @CurrentUser, 'table', 'SEC_Log', 'column', 'OperationTime'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '日志明细',
   'user', @CurrentUser, 'table', 'SEC_Log', 'column', 'Description'
go

/*==============================================================*/
/* Index: IDX_SEC_Log_Type                                      */
/*==============================================================*/
create index IDX_SEC_Log_Type on SEC_Log (
LogType ASC
)
go

/*==============================================================*/
/* Index: IDX_SEC_Log_UserId                                    */
/*==============================================================*/
create index IDX_SEC_Log_UserId on SEC_Log (
UserId ASC
)
go

/*==============================================================*/
/* Index: IDX_SEC_Log_UserName                                  */
/*==============================================================*/
create index IDX_SEC_Log_UserName on SEC_Log (
UserName ASC
)
go

/*==============================================================*/
/* Index: IDX_SEC_Log_OperationName                             */
/*==============================================================*/
create index IDX_SEC_Log_OperationName on SEC_Log (
OperationName ASC
)
go

/*==============================================================*/
/* Index: IDX_SEC_Log_OperationTime                             */
/*==============================================================*/
create index IDX_SEC_Log_OperationTime on SEC_Log (
OperationTime ASC
)
go

/*==============================================================*/
/* Table: SEC_OrgRole                                           */
/*==============================================================*/
create table SEC_OrgRole (
   OrgId                uniqueidentifier     not null,
   RoleId               varchar(38)          not null,
   CreatedBy            varchar(38)          not null,
   CreatedDate          datetime             not null,
   LastUpdatedBy        varchar(38)          not null,
   LastUpdatedDate      datetime             not null,
   constraint PK_SEC_ORGROLE primary key nonclustered (OrgId, RoleId)
)
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '组织角色表，存储组织在组织上充当的角色。',
   'user', @CurrentUser, 'table', 'SEC_OrgRole'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '组织标识',
   'user', @CurrentUser, 'table', 'SEC_OrgRole', 'column', 'OrgId'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '具备的角色（只能是管理类角色和混合类角色）',
   'user', @CurrentUser, 'table', 'SEC_OrgRole', 'column', 'RoleId'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '创建人',
   'user', @CurrentUser, 'table', 'SEC_OrgRole', 'column', 'CreatedBy'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '创建时间',
   'user', @CurrentUser, 'table', 'SEC_OrgRole', 'column', 'CreatedDate'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '最后修改人',
   'user', @CurrentUser, 'table', 'SEC_OrgRole', 'column', 'LastUpdatedBy'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '最后修改时间',
   'user', @CurrentUser, 'table', 'SEC_OrgRole', 'column', 'LastUpdatedDate'
go

/*==============================================================*/
/* Index: Inx_OrgRole_Org                                       */
/*==============================================================*/
create index Inx_OrgRole_Org on SEC_OrgRole (
OrgId ASC
)
go

/*==============================================================*/
/* Index: Inx_OrgRole_Role                                      */
/*==============================================================*/
create index Inx_OrgRole_Role on SEC_OrgRole (
RoleId ASC
)
go

/*==============================================================*/
/* Table: SEC_Organization                                      */
/*==============================================================*/
create table SEC_Organization (
   Id                   uniqueidentifier     not null,
   Parent               uniqueidentifier     null,
   Name                 varchar(150)         null,
   Code                 varchar(1000)        null,
   Level                int                  null,
   "Order"              int                  null,
   Type                 int                  null,
   Status               varchar(10)          not null default 'enabled'
      constraint CKC_STATUS_SEC_ORGA check (Status in ('enabled','disabled')),
   CreatedBy            varchar(38)          not null,
   CreatedDate          datetime             not null,
   LastUpdatedBy        varchar(38)          not null,
   LastUpdatedDate      datetime             not null,
   constraint PK_SEC_ORGANIZATION primary key (Id)
)
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '系统组织表',
   'user', @CurrentUser, 'table', 'SEC_Organization'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '组织标识',
   'user', @CurrentUser, 'table', 'SEC_Organization', 'column', 'Id'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '父组织标识',
   'user', @CurrentUser, 'table', 'SEC_Organization', 'column', 'Parent'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '名称',
   'user', @CurrentUser, 'table', 'SEC_Organization', 'column', 'Name'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '编码',
   'user', @CurrentUser, 'table', 'SEC_Organization', 'column', 'Code'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '层级',
   'user', @CurrentUser, 'table', 'SEC_Organization', 'column', 'Level'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '顺序',
   'user', @CurrentUser, 'table', 'SEC_Organization', 'column', 'Order'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '类型（公司、子公司、部门、科室等）',
   'user', @CurrentUser, 'table', 'SEC_Organization', 'column', 'Type'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '状态',
   'user', @CurrentUser, 'table', 'SEC_Organization', 'column', 'Status'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '创建人',
   'user', @CurrentUser, 'table', 'SEC_Organization', 'column', 'CreatedBy'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '创建时间',
   'user', @CurrentUser, 'table', 'SEC_Organization', 'column', 'CreatedDate'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '最后修改人',
   'user', @CurrentUser, 'table', 'SEC_Organization', 'column', 'LastUpdatedBy'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '最后修改时间',
   'user', @CurrentUser, 'table', 'SEC_Organization', 'column', 'LastUpdatedDate'
go

/*==============================================================*/
/* Index: Inx_sec_index_status                                  */
/*==============================================================*/
create index Inx_sec_index_status on SEC_Organization (
Status ASC
)
go

/*==============================================================*/
/* Table: SEC_Permission                                        */
/*==============================================================*/
create table SEC_Permission (
   Id                   varchar(38)          not null,
   Parent               varchar(38)          null,
   Code                 varchar(50)          not null,
   Name                 varchar(150)         not null,
   Url                  varchar(300)         null,
   Type                 varchar(10)          not null default 'Page'
      constraint CKC_TYPE_SEC_PERM check (Type in ('Module','Page','Element')),
   "Order"              int                  null default 1000,
   ElementId            varchar(50)          null,
   ElementBehaviour     varchar(30)          null
      constraint CKC_ELEMENTBEHAVIOUR_SEC_PERM check (ElementBehaviour is null or (ElementBehaviour in ('invisible','disabled'))),
   Description          varchar(1000)        null,
   CreatedBy            varchar(38)          not null,
   CreatedDate          datetime             not null,
   LastUpdatedBy        varchar(38)          not null,
   LastUpdatedDate      datetime             not null,
   constraint PK_SEC_PERMISSION primary key nonclustered (Id)
)
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '操作权限表',
   'user', @CurrentUser, 'table', 'SEC_Permission'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '操作标识',
   'user', @CurrentUser, 'table', 'SEC_Permission', 'column', 'Id'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '父操作标识',
   'user', @CurrentUser, 'table', 'SEC_Permission', 'column', 'Parent'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '操作代码',
   'user', @CurrentUser, 'table', 'SEC_Permission', 'column', 'Code'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '名称',
   'user', @CurrentUser, 'table', 'SEC_Permission', 'column', 'Name'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'Page对应的URL，或者页面元素所在的URL',
   'user', @CurrentUser, 'table', 'SEC_Permission', 'column', 'Url'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '模块、页面或者元素',
   'user', @CurrentUser, 'table', 'SEC_Permission', 'column', 'Type'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '顺序',
   'user', @CurrentUser, 'table', 'SEC_Permission', 'column', 'Order'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '受控页面元素的ID',
   'user', @CurrentUser, 'table', 'SEC_Permission', 'column', 'ElementId'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '页面元素默认行为',
   'user', @CurrentUser, 'table', 'SEC_Permission', 'column', 'ElementBehaviour'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '描述',
   'user', @CurrentUser, 'table', 'SEC_Permission', 'column', 'Description'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '创建人',
   'user', @CurrentUser, 'table', 'SEC_Permission', 'column', 'CreatedBy'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '创建时间',
   'user', @CurrentUser, 'table', 'SEC_Permission', 'column', 'CreatedDate'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '最后修改人',
   'user', @CurrentUser, 'table', 'SEC_Permission', 'column', 'LastUpdatedBy'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '最后修改时间',
   'user', @CurrentUser, 'table', 'SEC_Permission', 'column', 'LastUpdatedDate'
go

/*==============================================================*/
/* Index: FK_ParentPermission                                   */
/*==============================================================*/
create index FK_ParentPermission on SEC_Permission (
Parent ASC
)
go

/*==============================================================*/
/* Index: IDX_Permissions_Url                                   */
/*==============================================================*/
create index IDX_Permissions_Url on SEC_Permission (
Url ASC
)
go

/*==============================================================*/
/* Index: lDX_Permissions_Type                                  */
/*==============================================================*/
create index lDX_Permissions_Type on SEC_Permission (
Type ASC
)
go

/*==============================================================*/
/* Index: IDX_Permissions_Code                                  */
/*==============================================================*/
create unique index IDX_Permissions_Code on SEC_Permission (
Code ASC
)
go

/*==============================================================*/
/* Table: SEC_PermissionRule                                    */
/*==============================================================*/
create table SEC_PermissionRule (
   Id                   varchar(38)          not null,
   OperationId          varchar(38)          not null,
   Name                 varchar(150)         not null,
   Priority             int                  not null default 99,
   "Rule"               varchar(1000)        not null,
   Behaviour            varchar(30)          null
      constraint CKC_BEHAVIOUR_SEC_PERM check (Behaviour is null or (Behaviour in ('invisible','disabled'))),
   Description          varchar(1000)        null,
   CreatedBy            varchar(38)          not null,
   CreatedDate          datetime             not null,
   LastUpdatedBy        varchar(38)          not null,
   LastUpdatedDate      datetime             not null,
   constraint PK_SEC_PERMISSIONRULE primary key nonclustered (Id)
)
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '权限规则表',
   'user', @CurrentUser, 'table', 'SEC_PermissionRule'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '规则标识',
   'user', @CurrentUser, 'table', 'SEC_PermissionRule', 'column', 'Id'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '操作标识',
   'user', @CurrentUser, 'table', 'SEC_PermissionRule', 'column', 'OperationId'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '名称',
   'user', @CurrentUser, 'table', 'SEC_PermissionRule', 'column', 'Name'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '优先级',
   'user', @CurrentUser, 'table', 'SEC_PermissionRule', 'column', 'Priority'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '规则',
   'user', @CurrentUser, 'table', 'SEC_PermissionRule', 'column', 'Rule'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '行为',
   'user', @CurrentUser, 'table', 'SEC_PermissionRule', 'column', 'Behaviour'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '描述',
   'user', @CurrentUser, 'table', 'SEC_PermissionRule', 'column', 'Description'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '创建人',
   'user', @CurrentUser, 'table', 'SEC_PermissionRule', 'column', 'CreatedBy'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '创建时间',
   'user', @CurrentUser, 'table', 'SEC_PermissionRule', 'column', 'CreatedDate'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '最后修改人',
   'user', @CurrentUser, 'table', 'SEC_PermissionRule', 'column', 'LastUpdatedBy'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '最后修改时间',
   'user', @CurrentUser, 'table', 'SEC_PermissionRule', 'column', 'LastUpdatedDate'
go

/*==============================================================*/
/* Index: FK_SecurityRule_OperationId                           */
/*==============================================================*/
create index FK_SecurityRule_OperationId on SEC_PermissionRule (
OperationId ASC
)
go

/*==============================================================*/
/* Table: SEC_Role                                              */
/*==============================================================*/
create table SEC_Role (
   Id                   varchar(38)          not null,
   Name                 varchar(150)         not null,
   Description          varchar(1000)        null,
   Type                 tinyint              not null default 0
      constraint CKC_TYPE_SEC_ROLE check (Type in (0,1,2)),
   ParticipantType      tinyint              null default null
      constraint CKC_PARTICIPANTTYPE_SEC_ROLE check (ParticipantType is null or (ParticipantType in (0,1))),
   CreatedBy            varchar(38)          not null,
   CreatedDate          datetime             not null,
   LastUpdatedBy        varchar(38)          not null,
   LastUpdatedDate      datetime             not null,
   constraint PK_SEC_ROLE primary key nonclustered (Id)
)
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '系统角色表，对工作流而言是角色类型',
   'user', @CurrentUser, 'table', 'SEC_Role'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '角色标识',
   'user', @CurrentUser, 'table', 'SEC_Role', 'column', 'Id'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '角色名',
   'user', @CurrentUser, 'table', 'SEC_Role', 'column', 'Name'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '描述',
   'user', @CurrentUser, 'table', 'SEC_Role', 'column', 'Description'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '角色类型（管理类、流转类、混合类）',
   'user', @CurrentUser, 'table', 'SEC_Role', 'column', 'Type'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '参与者类型（用户、部门）',
   'user', @CurrentUser, 'table', 'SEC_Role', 'column', 'ParticipantType'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '创建人',
   'user', @CurrentUser, 'table', 'SEC_Role', 'column', 'CreatedBy'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '创建时间',
   'user', @CurrentUser, 'table', 'SEC_Role', 'column', 'CreatedDate'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '最后修改人',
   'user', @CurrentUser, 'table', 'SEC_Role', 'column', 'LastUpdatedBy'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '最后修改时间',
   'user', @CurrentUser, 'table', 'SEC_Role', 'column', 'LastUpdatedDate'
go

/*==============================================================*/
/* Index: Inx_Roles_Type                                        */
/*==============================================================*/
create index Inx_Roles_Type on SEC_Role (
Type ASC
)
go

/*==============================================================*/
/* Table: SEC_RoleInheritance                                   */
/*==============================================================*/
create table SEC_RoleInheritance (
   ParentRoleId         varchar(38)          not null,
   ChildRoleId          varchar(38)          not null,
   CreatedBy            varchar(38)          not null,
   CreatedDate          datetime             not null,
   LastUpdatedBy        varchar(38)          not null,
   LastUpdatedDate      datetime             not null,
   constraint PK_SEC_ROLEINHERITANCE primary key nonclustered (ParentRoleId, ChildRoleId)
)
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '角色继承表，记录角色的父子关系',
   'user', @CurrentUser, 'table', 'SEC_RoleInheritance'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '父角色标识',
   'user', @CurrentUser, 'table', 'SEC_RoleInheritance', 'column', 'ParentRoleId'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '子角色标识',
   'user', @CurrentUser, 'table', 'SEC_RoleInheritance', 'column', 'ChildRoleId'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '创建人',
   'user', @CurrentUser, 'table', 'SEC_RoleInheritance', 'column', 'CreatedBy'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '创建时间',
   'user', @CurrentUser, 'table', 'SEC_RoleInheritance', 'column', 'CreatedDate'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '最后修改人',
   'user', @CurrentUser, 'table', 'SEC_RoleInheritance', 'column', 'LastUpdatedBy'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '最后修改时间',
   'user', @CurrentUser, 'table', 'SEC_RoleInheritance', 'column', 'LastUpdatedDate'
go

/*==============================================================*/
/* Index: FK_SEC_RoleInheri_Parent                              */
/*==============================================================*/
create index FK_SEC_RoleInheri_Parent on SEC_RoleInheritance (
ParentRoleId ASC
)
go

/*==============================================================*/
/* Index: FK_SEC_RoleInheri_Child                               */
/*==============================================================*/
create index FK_SEC_RoleInheri_Child on SEC_RoleInheritance (
ChildRoleId ASC
)
go

/*==============================================================*/
/* Table: SEC_RolePermission                                    */
/*==============================================================*/
create table SEC_RolePermission (
   RoleId               varchar(38)          not null,
   OperationId          varchar(38)          not null,
   RuleId               varchar(38)          null,
   CreatedBy            varchar(38)          not null,
   CreatedDate          datetime             not null,
   LastUpdatedBy        varchar(38)          not null,
   LastUpdatedDate      datetime             not null,
   constraint PK_SEC_ROLEPERMISSION primary key nonclustered (RoleId, OperationId)
)
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '角色权限表',
   'user', @CurrentUser, 'table', 'SEC_RolePermission'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '角色标识',
   'user', @CurrentUser, 'table', 'SEC_RolePermission', 'column', 'RoleId'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '操作标识',
   'user', @CurrentUser, 'table', 'SEC_RolePermission', 'column', 'OperationId'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '规则标识',
   'user', @CurrentUser, 'table', 'SEC_RolePermission', 'column', 'RuleId'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '创建人',
   'user', @CurrentUser, 'table', 'SEC_RolePermission', 'column', 'CreatedBy'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '创建时间',
   'user', @CurrentUser, 'table', 'SEC_RolePermission', 'column', 'CreatedDate'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '最后修改人',
   'user', @CurrentUser, 'table', 'SEC_RolePermission', 'column', 'LastUpdatedBy'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '最后修改时间',
   'user', @CurrentUser, 'table', 'SEC_RolePermission', 'column', 'LastUpdatedDate'
go

/*==============================================================*/
/* Table: SEC_User                                              */
/*==============================================================*/
create table SEC_User (
   Id                   uniqueidentifier     not null,
   Name                 varchar(50)          not null,
   Type                 varchar(10)          not null default 'local'
      constraint CKC_TYPE_SEC_USER check (Type in ('local','portal')),
   LoginId              varchar(30)          not null,
   Password             varchar(50)          null,
   Email                varchar(50)          null,
   MobilePhone          varchar(15)          null,
   IM                   varchar(50)          null,
   Telephone            varchar(15)          null,
   Sex                  char(1)              not null default 'U'
      constraint CKC_SEX_SEC_USER check (Sex in ('F','M','U')),
   Birthday             datetime             null,
   Status               varchar(10)          not null default 'enabled'
      constraint CKC_STATUS_SEC_USER check (Status in ('enabled','disabled')),
   OrgId                uniqueidentifier     not null,
   Rank                 varchar(50)          null,
   CreatedBy            varchar(38)          not null,
   CreatedDate          datetime             not null,
   LastUpdatedBy        varchar(38)          not null,
   LastUpdatedDate      datetime             not null,
   constraint PK_SEC_USER primary key nonclustered (Id),
   constraint AK_AK_USERLOGINID_SEC_USER unique (LoginId, Type)
)
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '系统用户表',
   'user', @CurrentUser, 'table', 'SEC_User'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '用户标识',
   'user', @CurrentUser, 'table', 'SEC_User', 'column', 'Id'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '姓名',
   'user', @CurrentUser, 'table', 'SEC_User', 'column', 'Name'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '类型',
   'user', @CurrentUser, 'table', 'SEC_User', 'column', 'Type'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '账户',
   'user', @CurrentUser, 'table', 'SEC_User', 'column', 'LoginId'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '密码',
   'user', @CurrentUser, 'table', 'SEC_User', 'column', 'Password'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '邮件',
   'user', @CurrentUser, 'table', 'SEC_User', 'column', 'Email'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '手机',
   'user', @CurrentUser, 'table', 'SEC_User', 'column', 'MobilePhone'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '即时通讯号',
   'user', @CurrentUser, 'table', 'SEC_User', 'column', 'IM'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '办公电话',
   'user', @CurrentUser, 'table', 'SEC_User', 'column', 'Telephone'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '性别',
   'user', @CurrentUser, 'table', 'SEC_User', 'column', 'Sex'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '生日',
   'user', @CurrentUser, 'table', 'SEC_User', 'column', 'Birthday'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '状态',
   'user', @CurrentUser, 'table', 'SEC_User', 'column', 'Status'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '所属组织',
   'user', @CurrentUser, 'table', 'SEC_User', 'column', 'OrgId'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '职位',
   'user', @CurrentUser, 'table', 'SEC_User', 'column', 'Rank'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '创建人',
   'user', @CurrentUser, 'table', 'SEC_User', 'column', 'CreatedBy'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '创建时间',
   'user', @CurrentUser, 'table', 'SEC_User', 'column', 'CreatedDate'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '最后修改人',
   'user', @CurrentUser, 'table', 'SEC_User', 'column', 'LastUpdatedBy'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '最后修改时间',
   'user', @CurrentUser, 'table', 'SEC_User', 'column', 'LastUpdatedDate'
go

/*==============================================================*/
/* Table: SEC_UserRole                                          */
/*==============================================================*/
create table SEC_UserRole (
   UserId               uniqueidentifier     not null,
   RoleId               varchar(38)          not null,
   CreatedBy            varchar(38)          not null,
   CreatedDate          datetime             not null,
   LastUpdatedBy        varchar(38)          not null,
   LastUpdatedDate      datetime             not null,
   constraint PK_SEC_USERROLE primary key nonclustered (UserId, RoleId)
)
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '用户角色表，存储用户在组织上充当的角色。',
   'user', @CurrentUser, 'table', 'SEC_UserRole'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '用户标识',
   'user', @CurrentUser, 'table', 'SEC_UserRole', 'column', 'UserId'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '角色标识（只能是管理类角色和混合类角色）',
   'user', @CurrentUser, 'table', 'SEC_UserRole', 'column', 'RoleId'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '创建人',
   'user', @CurrentUser, 'table', 'SEC_UserRole', 'column', 'CreatedBy'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '创建时间',
   'user', @CurrentUser, 'table', 'SEC_UserRole', 'column', 'CreatedDate'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '最后修改人',
   'user', @CurrentUser, 'table', 'SEC_UserRole', 'column', 'LastUpdatedBy'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '最后修改时间',
   'user', @CurrentUser, 'table', 'SEC_UserRole', 'column', 'LastUpdatedDate'
go

/*==============================================================*/
/* Index: Inx_UserRole_user                                     */
/*==============================================================*/
create index Inx_UserRole_user on SEC_UserRole (
UserId ASC
)
go

/*==============================================================*/
/* Index: Inx_UserRole_role                                     */
/*==============================================================*/
create index Inx_UserRole_role on SEC_UserRole (
RoleId ASC
)
go

/*==============================================================*/
/* Table: SEC_WorkDay                                           */
/*==============================================================*/
create table SEC_WorkDay (
   Day                  smalldatetime        not null,
   OrgId                uniqueidentifier     not null,
   IsWork               bit                  null default 0
      constraint CKC_ISWORK_SEC_WORK check (IsWork is null or (IsWork in (0,1))),
   Description          varchar(300)         null,
   constraint PK_SEC_WORKDAY primary key (Day, OrgId)
)
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '工作日表，定义特定组织机构的工作日情况',
   'user', @CurrentUser, 'table', 'SEC_WorkDay'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '工作日标识',
   'user', @CurrentUser, 'table', 'SEC_WorkDay', 'column', 'Day'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '组织标识',
   'user', @CurrentUser, 'table', 'SEC_WorkDay', 'column', 'OrgId'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '是否是工作日(0 非工作日 1 工作日)',
   'user', @CurrentUser, 'table', 'SEC_WorkDay', 'column', 'IsWork'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '备注',
   'user', @CurrentUser, 'table', 'SEC_WorkDay', 'column', 'Description'
go

/*==============================================================*/
/* Index: FK_Sec_Workday                                        */
/*==============================================================*/
create index FK_Sec_Workday on SEC_WorkDay (
OrgId ASC
)
go

/*==============================================================*/
/* Table: SEC_wf_OrgRole                                        */
/*==============================================================*/
create table SEC_wf_OrgRole (
   OrgId                uniqueidentifier     not null,
   WfRoleId             uniqueidentifier     not null,
   CreatedBy            varchar(38)          not null,
   CreatedDate          datetime             not null,
   LastUpdatedBy        varchar(38)          not null,
   LastUpdatedDate      datetime             not null,
   constraint PK_SEC_WF_ORGROLE primary key nonclustered (OrgId, WfRoleId)
)
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '部门参与者角色里对应的会签部门',
   'user', @CurrentUser, 'table', 'SEC_wf_OrgRole'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '组织标识',
   'user', @CurrentUser, 'table', 'SEC_wf_OrgRole', 'column', 'OrgId'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '流程角色标识',
   'user', @CurrentUser, 'table', 'SEC_wf_OrgRole', 'column', 'WfRoleId'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '创建人',
   'user', @CurrentUser, 'table', 'SEC_wf_OrgRole', 'column', 'CreatedBy'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '创建时间',
   'user', @CurrentUser, 'table', 'SEC_wf_OrgRole', 'column', 'CreatedDate'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '最后修改人',
   'user', @CurrentUser, 'table', 'SEC_wf_OrgRole', 'column', 'LastUpdatedBy'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '最后修改时间',
   'user', @CurrentUser, 'table', 'SEC_wf_OrgRole', 'column', 'LastUpdatedDate'
go

/*==============================================================*/
/* Index: Inx_wf_OrgRole_Org                                    */
/*==============================================================*/
create index Inx_wf_OrgRole_Org on SEC_wf_OrgRole (
OrgId ASC
)
go

/*==============================================================*/
/* Index: Inx_wf_OrgRole_Role                                   */
/*==============================================================*/
create index Inx_wf_OrgRole_Role on SEC_wf_OrgRole (
WfRoleId ASC
)
go

/*==============================================================*/
/* Table: SEC_wf_UserRole                                       */
/*==============================================================*/
create table SEC_wf_UserRole (
   UserId               uniqueidentifier     not null,
   WfRoleId             uniqueidentifier     not null,
   CreatedBy            varchar(38)          not null,
   CreatedDate          datetime             not null,
   LastUpdatedBy        varchar(38)          not null,
   LastUpdatedDate      datetime             not null,
   constraint PK_SEC_WF_USERROLE primary key nonclustered (WfRoleId, UserId)
)
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '用户流程角色',
   'user', @CurrentUser, 'table', 'SEC_wf_UserRole'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '用户标识',
   'user', @CurrentUser, 'table', 'SEC_wf_UserRole', 'column', 'UserId'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '流程角色标识',
   'user', @CurrentUser, 'table', 'SEC_wf_UserRole', 'column', 'WfRoleId'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '创建人',
   'user', @CurrentUser, 'table', 'SEC_wf_UserRole', 'column', 'CreatedBy'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '创建时间',
   'user', @CurrentUser, 'table', 'SEC_wf_UserRole', 'column', 'CreatedDate'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '最后修改人',
   'user', @CurrentUser, 'table', 'SEC_wf_UserRole', 'column', 'LastUpdatedBy'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '最后修改时间',
   'user', @CurrentUser, 'table', 'SEC_wf_UserRole', 'column', 'LastUpdatedDate'
go

/*==============================================================*/
/* Index: Inx_wf_UserRole_user                                  */
/*==============================================================*/
create clustered index Inx_wf_UserRole_user on SEC_wf_UserRole (
UserId ASC
)
go

/*==============================================================*/
/* Index: Inx_wf_UserRole_role                                  */
/*==============================================================*/
create index Inx_wf_UserRole_role on SEC_wf_UserRole (
WfRoleId ASC
)
go

/*==============================================================*/
/* Table: SEC_wf_role                                           */
/*==============================================================*/
create table SEC_wf_role (
   Id                   uniqueidentifier     not null,
   RoleId               varchar(38)          not null,
   OrgId                uniqueidentifier     not null,
   CreatedBy            varchar(38)          not null,
   CreatedDate          datetime             not null,
   LastUpdatedBy        varchar(38)          not null,
   LastUpdatedDate      datetime             not null,
   constraint PK_SEC_WF_ROLE primary key (Id),
   constraint AK_KEY_2_SEC_WF_R unique (RoleId, OrgId)
)
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '流程角色标识',
   'user', @CurrentUser, 'table', 'SEC_wf_role', 'column', 'Id'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '角色标识（只能是流转类角色和混合类角色）',
   'user', @CurrentUser, 'table', 'SEC_wf_role', 'column', 'RoleId'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '组织标识',
   'user', @CurrentUser, 'table', 'SEC_wf_role', 'column', 'OrgId'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '创建人',
   'user', @CurrentUser, 'table', 'SEC_wf_role', 'column', 'CreatedBy'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '创建时间',
   'user', @CurrentUser, 'table', 'SEC_wf_role', 'column', 'CreatedDate'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '最后修改人',
   'user', @CurrentUser, 'table', 'SEC_wf_role', 'column', 'LastUpdatedBy'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '最后修改时间',
   'user', @CurrentUser, 'table', 'SEC_wf_role', 'column', 'LastUpdatedDate'
go

/*==============================================================*/
/* Index: Inx_wf_role_org                                       */
/*==============================================================*/
create index Inx_wf_role_org on SEC_wf_role (
OrgId ASC
)
go

/*==============================================================*/
/* Index: Inx_wf_role_role                                      */
/*==============================================================*/
create index Inx_wf_role_role on SEC_wf_role (
RoleId ASC
)
go


/*==============================================================*/
/* View: v_sec_manage_role                                      */
/*==============================================================*/
create view v_sec_manage_role as
select * 
from SEC_Role
where Type in (0,2)
go

/*==============================================================*/
/* View: v_sec_valid_org                                        */
/*==============================================================*/
create view v_sec_valid_org as
select Id, Cast(Parent as varchar(36)) Parent, Name, Code, [Level],[Order],[Type]
from SEC_Organization
where Status = 'enabled'
go

/*==============================================================*/
/* View: v_sec_workflow_role                                    */
/*==============================================================*/
create view v_sec_workflow_role as
select * 
from SEC_Role
where Type in (1,2)
go

alter table SEC_Log
   add constraint FK_SEC_LOG_REFERENCE_SEC_USER foreign key (UserId)
      references SEC_User (Id)
go

alter table SEC_OrgRole
   add constraint FK_SEC_ORGR_REFERENCE_SEC_ORG2 foreign key (OrgId)
      references SEC_Organization (Id)
go

alter table SEC_OrgRole
   add constraint FK_SEC_ORGR_REFERENCE_SEC_ROLE foreign key (RoleId)
      references SEC_Role (Id)
go

alter table SEC_Organization
   add constraint FK_SEC_ORGA_REFERENCE_SEC_ORGA foreign key (Parent)
      references SEC_Organization (Id)
go

alter table SEC_Permission
   add constraint FK_SEC_PERM_PARENTPER_SEC_PERM foreign key (Parent)
      references SEC_Permission (Id)
go

alter table SEC_PermissionRule
   add constraint FK_SEC_SECU_BELONGTOO_SEC_PERM foreign key (OperationId)
      references SEC_Permission (Id)
         on delete cascade
go

alter table SEC_RoleInheritance
   add constraint FK_SEC_ROLE_INHERI_ROLE foreign key (ParentRoleId)
      references SEC_Role (Id)
         on delete cascade
go

alter table SEC_RoleInheritance
   add constraint FK_SEC_ROLE_INHERI_CHILD_ROLE foreign key (ChildRoleId)
      references SEC_Role (Id)
go

alter table SEC_RolePermission
   add constraint FK_SEC_ROLE_ASSIGNTOR_SEC_ROLE foreign key (RoleId)
      references SEC_Role (Id)
         on delete cascade
go

alter table SEC_RolePermission
   add constraint FK_SEC_ROLE_BELONGTOP_SEC_PERM foreign key (OperationId)
      references SEC_Permission (Id)
         on delete cascade
go

alter table SEC_RolePermission
   add constraint FK_SEC_ROLE_PERMISSIO_SEC_SECU foreign key (RuleId)
      references SEC_PermissionRule (Id)
go

alter table SEC_User
   add constraint FK_SEC_USER_REFERENCE_SEC_ORG1 foreign key (OrgId)
      references SEC_Organization (Id)
         on delete cascade
go

alter table SEC_UserRole
   add constraint FK_SEC_USER_ROLEROLE_SEC_USER foreign key (UserId)
      references SEC_User (Id)
         on delete cascade
go

alter table SEC_UserRole
   add constraint FK_SEC_USER_USERROLE_SEC_ROLE foreign key (RoleId)
      references SEC_Role (Id)
         on delete cascade
go

alter table SEC_WorkDay
   add constraint FK_SEC_WORK_REFERENCE_SEC_ORGA foreign key (OrgId)
      references SEC_Organization (Id)
go

alter table SEC_wf_OrgRole
   add constraint FK_SEC_WF_O_REFERENCE_SEC_ORGA foreign key (OrgId)
      references SEC_Organization (Id)
go

alter table SEC_wf_OrgRole
   add constraint FK_SEC_WF_O_REFERENCE_SEC_WF_R foreign key (WfRoleId)
      references SEC_wf_role (Id)
         on delete cascade
go

alter table SEC_wf_UserRole
   add constraint FK_SEC_WF_U_REFERENCE_SEC_USER foreign key (UserId)
      references SEC_User (Id)
go

alter table SEC_wf_UserRole
   add constraint FK_SEC_WF_U_REFERENCE_SEC_WF_R foreign key (WfRoleId)
      references SEC_wf_role (Id)
         on delete cascade
go

alter table SEC_wf_role
   add constraint FK_SEC_WF_R_REFERENCE_SEC_ROLE foreign key (RoleId)
      references SEC_Role (Id)
         on delete cascade
go

alter table SEC_wf_role
   add constraint FK_SEC_WF_R_REFERENCE_SEC_ORGA foreign key (OrgId)
      references SEC_Organization (Id)
         on delete cascade
go


/****** Object:  UserDefinedFunction [dbo].[fn_is_role_enheritable]    Script Date: 05/12/2011 13:03:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE function [dbo].[fn_is_role_enheritable](@roleId varchar(38))
returns int
as
begin
	declare @ret int;
	
	select @ret = 0;

	WITH F_USF_ROLE_INHERITANCE 
	AS 
	( 
	--递归开始
	SELECT UR.childRoleId
		           FROM SEC_RoleInheritance UR
	where UR.ParentRoleId = @roleId     
	UNION ALL 
	-- 递归成员 (RM): 
	-- 使用现有数据往下一层展开 
	SELECT UR.childRoleId
		           FROM F_USF_ROLE_INHERITANCE FUR	  
	INNER JOIN SEC_RoleInheritance UR ON FUR.childRoleId = UR.ParentRoleId 
	) 
	--从递归函数获取数据
	select @ret=count(*) from F_USF_ROLE_INHERITANCE;
	
	return (case when @ret < 4 then 1 else 0 end)
end;
GO

/****** Object:  UserDefinedFunction [dbo].[fn_get_sub_me_operation]    Script Date: 05/12/2011 13:03:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[fn_get_sub_me_role] (@parent varchar(38))
RETURNS TABLE
AS
RETURN 
(
    WITH F_SEC_RoleInheritance
AS 
( 
--递归开始
SELECT *
	           FROM SEC_RoleInheritance P
	           where P.ParentRoleId = @parent    
UNION ALL 
-- 递归成员 (RM): 
-- 使用现有数据往下一层展开 
SELECT UO.*
	           FROM F_SEC_RoleInheritance FUO	  
INNER JOIN SEC_RoleInheritance UO ON FUO.ChildRoleId = UO.ParentRoleId
) 
--从递归函数获取数据
SELECT *
FROM F_SEC_RoleInheritance
);
GO

/****** Object:  UserDefinedFunction [dbo].[fn_get_sub_me_operation]    Script Date: 05/12/2011 13:03:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[fn_get_sub_me_operation] (@parent varchar(38))
RETURNS TABLE
AS
RETURN 
(
    WITH F_USF_OPERATION 
AS 
( 
--递归开始
SELECT *
	           FROM SEC_PERMISSION P
	           where P.Id = @parent    
UNION ALL 
-- 递归成员 (RM): 
-- 使用现有数据往下一层展开 
SELECT UO.*
	           FROM F_USF_OPERATION FUO	  
INNER JOIN SEC_PERMISSION UO ON FUO.Id = UO.Parent 
) 
--从递归函数获取数据
SELECT *
FROM F_USF_OPERATION
);
GO

/****** Object:  UserDefinedFunction [dbo].[fn_get_sub_me_organization]    Script Date: 05/12/2011 13:03:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[fn_get_sub_me_organization] (@parent_org_id varchar(300))
RETURNS TABLE
AS
RETURN 
(
    WITH F_USF_ORGANIZATION 
AS 
( 
--递归开始
SELECT *
	           FROM SEC_ORGANIZATION UO
	           where UO.Id = @parent_org_id    
UNION ALL 
-- 递归成员 (RM): 
-- 使用现有数据往下一层展开 
SELECT UO.*
	           FROM F_USF_ORGANIZATION FUO	  
INNER JOIN SEC_ORGANIZATION UO ON FUO.Id = UO.Parent 
) 
--从递归函数获取数据
SELECT *
FROM F_USF_ORGANIZATION
);
GO

/****** Object:  UserDefinedFunction [dbo].[fn_get_sub_organization]    Script Date: 05/12/2011 13:03:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[fn_get_sub_organization] (@parent_org_id varchar(300))
RETURNS TABLE
AS
RETURN 
(
    WITH F_USF_ORGANIZATION 
AS 
( 
--递归开始
SELECT *
	           FROM SEC_ORGANIZATION UO
	           where UO.Parent = @parent_org_id    
UNION ALL 
-- 递归成员 (RM): 
-- 使用现有数据往下一层展开 
SELECT UO.*
	           FROM F_USF_ORGANIZATION FUO	  
INNER JOIN SEC_ORGANIZATION UO ON FUO.Id = UO.Parent 
) 
--从递归函数获取数据
SELECT *
FROM F_USF_ORGANIZATION
);
GO

/****** Object:  UserDefinedFunction [dbo].[fn_get_parent_me_organization]    Script Date: 05/12/2011 13:03:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].fn_get_parent_me_organization (@child_org_id varchar(300))
RETURNS TABLE
AS
RETURN 
(
    WITH F_USF_ORGANIZATION 
AS 
( 
--递归开始
SELECT *
	           FROM SEC_ORGANIZATION UO
	           where UO.Id = @child_org_id    
UNION ALL 
-- 递归成员 (RM): 
-- 使用现有数据往下一层展开 
SELECT UO.*
	           FROM F_USF_ORGANIZATION FUO	  
INNER JOIN SEC_ORGANIZATION UO ON FUO.Parent = UO.Id
) 
--从递归函数获取数据
SELECT *
FROM F_USF_ORGANIZATION
);
GO

/****** Object:  UserDefinedFunction [dbo].[fn_get_parent_me_role]    Script Date: 06/28/2011 15:57:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[fn_get_parent_me_role] (@child varchar(38))
RETURNS TABLE
AS
RETURN 
(
    WITH F_SEC_RoleInheritance
AS 
( 
--递归开始
SELECT *
	           FROM SEC_RoleInheritance c
	           where c.ChildRoleId = @child    
UNION ALL 
-- 递归成员 (RM): 
-- 使用现有数据往上一层查找 
SELECT UO.*
	           FROM F_SEC_RoleInheritance FUO	  
INNER JOIN SEC_RoleInheritance UO ON FUO.ParentRoleId = UO.ChildRoleId
) 
--从递归函数获取数据
SELECT *
FROM F_SEC_RoleInheritance
);
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE trigger [dbo].[tri_sec_organization]
on [dbo].[Sec_Organization]
after insert, update
as 
begin 
	Declare @Id as varchar(1000); 
    Declare @ParentId varchar(1000);
    Declare @ParentCode varchar(1000);
    Declare @ParentLevel int;

    --不支持同时插入和更新多条记录的情况 
    select @Id=Id, @ParentId=Parent from inserted;
    if @ParentId is null
    begin
		Update Sec_Organization Set Code = cast(Id as varchar(36)),[Level] = 1 Where Id = @Id;
    end
    else
    begin
		select @ParentCode=Code,@ParentLevel=[Level] from Sec_Organization where ID=@ParentId;
		Update Sec_Organization Set Code = @ParentCode+'.'+cast(Id as varchar(36)),[Level] = @ParentLevel+1 Where Id = @Id;
    end
end
;
GO