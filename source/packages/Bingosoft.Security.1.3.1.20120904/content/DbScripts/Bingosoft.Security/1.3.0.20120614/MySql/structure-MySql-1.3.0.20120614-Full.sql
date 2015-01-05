/*==============================================================*/
/* DBMS name:      MySQL 5.0                                    */
/* Created on:     2012-3-6 13:13:19                            */
/*==============================================================*/

/*==============================================================*/
/* Table: SEC_Log                                               */
/*==============================================================*/
CREATE TABLE SEC_Log
(
   Id                   VARCHAR(38) NOT NULL COMMENT '日志标识',
   User_Id              VARCHAR(38) NOT NULL COMMENT '用户标识',
   Log_Type             VARCHAR(10) NOT NULL DEFAULT 'login' COMMENT '日志类型（login、operation、auth）',
   User_Name            VARCHAR(50) NOT NULL COMMENT '用户名称',
   Operation_Name       VARCHAR(50) NOT NULL COMMENT '操作名称',
   Operation_Time       DATETIME NOT NULL COMMENT '操作时间',
   Description          VARCHAR(3000) COMMENT '日志描述'
);

ALTER TABLE SEC_Log COMMENT '安全日志(包括登录日志、操作日志和授权日志)';

ALTER TABLE SEC_Log
   ADD PRIMARY KEY (Id);

/*==============================================================*/
/* Index: IDX_SEC_Log_Type                                      */
/*==============================================================*/
CREATE INDEX IDX_SEC_Log_Type ON SEC_Log
(
   Log_Type
);

/*==============================================================*/
/* Index: IDX_SEC_Log_UserId                                    */
/*==============================================================*/
CREATE INDEX IDX_SEC_Log_UserId ON SEC_Log
(
   User_Id
);

/*==============================================================*/
/* Index: IDX_SEC_Log_UserName                                  */
/*==============================================================*/
CREATE INDEX IDX_SEC_Log_UserName ON SEC_Log
(
   User_Name
);

/*==============================================================*/
/* Index: IDX_SEC_Log_OperationName                             */
/*==============================================================*/
CREATE INDEX IDX_SEC_Log_OperationName ON SEC_Log
(
   Operation_Name
);

/*==============================================================*/
/* Index: IDX_SEC_Log_OperationTime                             */
/*==============================================================*/
CREATE INDEX IDX_SEC_Log_OperationTime ON SEC_Log
(
   Operation_Time
);

/*==============================================================*/
/* Table: SEC_Org_Role                                          */
/*==============================================================*/
CREATE TABLE SEC_Org_Role
(
   Org_Id               VARCHAR(38) NOT NULL COMMENT '组织标识',
   Role_Id              VARCHAR(38) NOT NULL COMMENT '具备的角色（只能是管理类角色和混合类角色）',
   Created_By           VARCHAR(38) NOT NULL COMMENT '创建人',
   Created_Date         DATETIME NOT NULL COMMENT '创建时间',
   Last_Updated_By      VARCHAR(38) NOT NULL COMMENT '最后修改人',
   Last_Updated_Date    DATETIME NOT NULL COMMENT '最后修改时间'
);

ALTER TABLE SEC_Org_Role COMMENT '组织角色表，存储组织在组织上充当的角色。';

ALTER TABLE SEC_Org_Role
   ADD PRIMARY KEY (Org_Id, Role_Id);

/*==============================================================*/
/* Index: Inx_OrgRole_Org                                       */
/*==============================================================*/
CREATE INDEX Inx_OrgRole_Org ON SEC_Org_Role
(
   Org_Id
);

/*==============================================================*/
/* Index: Inx_OrgRole_Role                                      */
/*==============================================================*/
CREATE INDEX Inx_OrgRole_Role ON SEC_Org_Role
(
   Role_Id
);

/*==============================================================*/
/* Table: SEC_Organization                                      */
/*==============================================================*/
CREATE TABLE SEC_Organization
(
   Id                   VARCHAR(38) NOT NULL COMMENT '组织标识',
   Parent               VARCHAR(38) COMMENT '父组织标识',
   NAME                 VARCHAR(150) COMMENT '名称',
   CODE                 VARCHAR(1000) COMMENT '编码',
   LEVEL                INT COMMENT '层级',
   `Order`              INT COMMENT '顺序',
   TYPE                 INT COMMENT '类型（公司、子公司、部门、科室等）',
   STATUS               VARCHAR(10) NOT NULL DEFAULT 'enabled' COMMENT '状态',
   Created_By           VARCHAR(38) NOT NULL COMMENT '创建人',
   Created_Date         DATETIME NOT NULL COMMENT '创建时间',
   Last_Updated_By      VARCHAR(38) NOT NULL COMMENT '最后修改人',
   Last_Updated_Date    DATETIME NOT NULL COMMENT '最后修改时间'
);

ALTER TABLE SEC_Organization COMMENT '系统组织表';

ALTER TABLE SEC_Organization
   ADD PRIMARY KEY (Id);

/*==============================================================*/
/* Index: Inx_sec_index_status                                  */
/*==============================================================*/
CREATE INDEX Inx_sec_index_status ON SEC_Organization
(
   STATUS
);

/*==============================================================*/
/* Table: SEC_PERMISSION_RESOURCE                               */
/*==============================================================*/
CREATE TABLE SEC_PERMISSION_RESOURCE
(
   Id                   VARCHAR(38) NOT NULL COMMENT '操作资源标识',
   Permission_Id        VARCHAR(38) NOT NULL COMMENT '操作标识',
   TYPE                 VARCHAR(30) NOT NULL COMMENT '资源类型，默认是Url，将来可能有其他类型',
   Resource             VARCHAR(300) NOT NULL COMMENT '资源',
   Created_By           VARCHAR(38) NOT NULL COMMENT '创建人',
   Created_Date         DATETIME NOT NULL COMMENT '创建时间',
   Last_Updated_By      VARCHAR(38) NOT NULL COMMENT '最后修改人',
   Last_Updated_Date    DATETIME NOT NULL COMMENT '最后修改时间'
);

ALTER TABLE SEC_PERMISSION_RESOURCE COMMENT '权限资源表';

ALTER TABLE SEC_PERMISSION_RESOURCE
   ADD PRIMARY KEY (Id);

/*==============================================================*/
/* Index: IDX_PERMISSION_RESOURCE_01                            */
/*==============================================================*/
CREATE INDEX IDX_PERMISSION_RESOURCE_01 ON SEC_PERMISSION_RESOURCE
(
   Resource
);

/*==============================================================*/
/* Table: SEC_Permission                                        */
/*==============================================================*/
CREATE TABLE SEC_Permission
(
   Id                   VARCHAR(38) NOT NULL COMMENT '操作标识',
   Parent               VARCHAR(38) COMMENT '父操作标识',
   CODE                 VARCHAR(50) NOT NULL COMMENT '操作代码',
   NAME                 VARCHAR(150) NOT NULL COMMENT '名称',
   Url                  VARCHAR(300) COMMENT 'Page对应的URL，或者页面元素所在的URL',
   TYPE                 VARCHAR(10) NOT NULL DEFAULT 'Page' COMMENT '模块、页面或者元素',
   Element_Id           VARCHAR(50) COMMENT '受控页面元素的ID',
   Element_Behaviour    VARCHAR(30) COMMENT '页面元素默认行为',
   `Order`              INT DEFAULT 1000 COMMENT '顺序',
   Description          VARCHAR(1000) COMMENT '描述',
   Created_By           VARCHAR(38) NOT NULL COMMENT '创建人',
   Created_Date         DATETIME NOT NULL COMMENT '创建时间',
   Last_Updated_By      VARCHAR(38) NOT NULL COMMENT '最后修改人',
   Last_Updated_Date    DATETIME NOT NULL COMMENT '最后修改时间'
);

ALTER TABLE SEC_Permission COMMENT '操作权限表';

ALTER TABLE SEC_Permission
   ADD PRIMARY KEY (Id);

/*==============================================================*/
/* Index: IDX_Permissions_Url                                   */
/*==============================================================*/
CREATE INDEX IDX_Permissions_Url ON SEC_Permission
(
   Url
);

/*==============================================================*/
/* Index: lDX_Permissions_Type                                  */
/*==============================================================*/
CREATE INDEX lDX_Permissions_Type ON SEC_Permission
(
   TYPE
);

/*==============================================================*/
/* Index: IDX_Permissions_Code                                  */
/*==============================================================*/
CREATE UNIQUE INDEX IDX_Permissions_Code ON SEC_Permission
(
   CODE
);

/*==============================================================*/
/* Table: SEC_Permission_Rule                                   */
/*==============================================================*/
CREATE TABLE SEC_Permission_Rule
(
   Id                   VARCHAR(38) NOT NULL COMMENT '规则标识',
   Operation_Id         VARCHAR(38) NOT NULL COMMENT '操作标识',
   NAME                 VARCHAR(150) NOT NULL COMMENT '名称',
   Priority             INT NOT NULL DEFAULT 100 COMMENT '优先级',
   Rule                 VARCHAR(1000) NOT NULL COMMENT '规则',
   Behaviour            VARCHAR(30) COMMENT '行为',
   Description          VARCHAR(1000) COMMENT '描述',
   Is_Default           INT NOT NULL DEFAULT 0 COMMENT '是否缺省规则',
   Created_By           VARCHAR(38) NOT NULL COMMENT '创建人',
   Created_Date         DATETIME NOT NULL COMMENT '创建时间',
   Last_Updated_By      VARCHAR(38) NOT NULL COMMENT '最后修改人',
   Last_Updated_Date    DATETIME NOT NULL COMMENT '最后修改时间'
);

ALTER TABLE SEC_Permission_Rule COMMENT '权限规则表';

ALTER TABLE SEC_Permission_Rule
   ADD PRIMARY KEY (Id);

/*==============================================================*/
/* Table: SEC_Role                                              */
/*==============================================================*/
CREATE TABLE SEC_Role
(
   Id                   VARCHAR(38) NOT NULL COMMENT '角色标识',
   CODE                 VARCHAR(50) COMMENT '角色编码',
   NAME                 VARCHAR(150) NOT NULL COMMENT '角色名',
   Description          VARCHAR(1000) COMMENT '描述',
   TYPE                 SMALLINT NOT NULL DEFAULT 0 COMMENT '角色类型（管理类、流转类、混合类）',
   Participant_Type     SMALLINT DEFAULT NULL COMMENT '参与者类型（用户、部门）',
   Created_By           VARCHAR(38) NOT NULL COMMENT '创建人',
   Created_Date         DATETIME NOT NULL COMMENT '创建时间',
   Last_Updated_By      VARCHAR(38) NOT NULL COMMENT '最后修改人',
   Last_Updated_Date    DATETIME NOT NULL COMMENT '最后修改时间'
);

ALTER TABLE SEC_Role COMMENT '系统角色表，对工作流而言是角色类型';

ALTER TABLE SEC_Role
   ADD UNIQUE AK_Key_2 (CODE);

ALTER TABLE SEC_Role
   ADD PRIMARY KEY (Id);

/*==============================================================*/
/* Index: Inx_Roles_Type                                        */
/*==============================================================*/
CREATE INDEX Inx_Roles_Type ON SEC_Role
(
   TYPE
);

/*==============================================================*/
/* Table: SEC_Role_Inheritance                                  */
/*==============================================================*/
CREATE TABLE SEC_Role_Inheritance
(
   Parent_Role_Id       VARCHAR(38) NOT NULL COMMENT '父角色标识',
   Child_Role_Id        VARCHAR(38) NOT NULL COMMENT '子角色标识',
   Created_By           VARCHAR(38) NOT NULL COMMENT '创建人',
   Created_Date         DATETIME NOT NULL COMMENT '创建时间',
   Last_Updated_By      VARCHAR(38) NOT NULL COMMENT '最后修改人',
   Last_Updated_Date    DATETIME NOT NULL COMMENT '最后修改时间'
);

ALTER TABLE SEC_Role_Inheritance COMMENT '角色继承表，记录角色的父子关系';

ALTER TABLE SEC_Role_Inheritance
   ADD PRIMARY KEY (Parent_Role_Id, Child_Role_Id);

/*==============================================================*/
/* Table: SEC_Role_Permission                                   */
/*==============================================================*/
CREATE TABLE SEC_Role_Permission
(
   Role_Id              VARCHAR(38) NOT NULL COMMENT '角色标识',
   Operation_Id         VARCHAR(38) NOT NULL COMMENT '操作标识',
   Rule_Id              VARCHAR(38) COMMENT '规则标识',
   Created_By           VARCHAR(38) NOT NULL COMMENT '创建人',
   Created_Date         DATETIME NOT NULL COMMENT '创建时间',
   Last_Updated_By      VARCHAR(38) NOT NULL COMMENT '最后修改人',
   Last_Updated_Date    DATETIME NOT NULL COMMENT '最后修改时间'
);

ALTER TABLE SEC_Role_Permission COMMENT '角色权限表';

ALTER TABLE SEC_Role_Permission
   ADD PRIMARY KEY (Role_Id, Operation_Id);

/*==============================================================*/
/* Table: SEC_User                                              */
/*==============================================================*/
CREATE TABLE SEC_User
(
   Id                   VARCHAR(38) NOT NULL COMMENT '用户标识',
   NAME                 VARCHAR(50) NOT NULL COMMENT '姓名',
   TYPE                 VARCHAR(10) NOT NULL DEFAULT 'local' COMMENT '类型',
   Login_Id             VARCHAR(30) NOT NULL COMMENT '账户',
   PASSWORD             VARCHAR(50) COMMENT '密码',
   Email                VARCHAR(50) COMMENT '邮件',
   Mobile_Phone         VARCHAR(15) COMMENT '手机',
   IM                   VARCHAR(50) COMMENT '即时通讯号',
   Telephone            VARCHAR(15) COMMENT '办公电话',
   Sex                  CHAR(1) NOT NULL DEFAULT 'U' COMMENT '性别',
   Birthday             DATETIME COMMENT '生日',
   STATUS               VARCHAR(10) NOT NULL DEFAULT 'enabled' COMMENT '状态',
   Org_Id               VARCHAR(38) NOT NULL COMMENT '所属组织',
   Rank                 VARCHAR(50) COMMENT '职位',
   Created_By           VARCHAR(38) NOT NULL COMMENT '创建人',
   Created_Date         DATETIME NOT NULL COMMENT '创建时间',
   Last_Updated_By      VARCHAR(38) NOT NULL COMMENT '最后修改人',
   Last_Updated_Date    DATETIME NOT NULL COMMENT '最后修改时间'
);

ALTER TABLE SEC_User COMMENT '系统用户表';

ALTER TABLE SEC_User
   ADD PRIMARY KEY (Id);

ALTER TABLE SEC_User
   ADD UNIQUE AK_AK_UserLoginID (Login_Id, TYPE);

/*==============================================================*/
/* Table: SEC_User_Role                                         */
/*==============================================================*/
CREATE TABLE SEC_User_Role
(
   User_Id              VARCHAR(38) NOT NULL COMMENT '用户标识',
   Role_Id              VARCHAR(38) NOT NULL COMMENT '角色标识（只能是管理类角色和混合类角色）',
   Created_By           VARCHAR(38) NOT NULL COMMENT '创建人',
   Created_Date         DATETIME NOT NULL COMMENT '创建时间',
   Last_Updated_By      VARCHAR(38) NOT NULL COMMENT '最后修改人',
   Last_Updated_Date    DATETIME NOT NULL COMMENT '最后修改时间'
);

ALTER TABLE SEC_User_Role COMMENT '用户角色表，存储用户在组织上充当的角色。';

ALTER TABLE SEC_User_Role
   ADD PRIMARY KEY (User_Id, Role_Id);

/*==============================================================*/
/* Index: Inx_UserRole_user                                     */
/*==============================================================*/
CREATE INDEX Inx_UserRole_user ON SEC_User_Role
(
   User_Id
);

/*==============================================================*/
/* Index: Inx_UserRole_role                                     */
/*==============================================================*/
CREATE INDEX Inx_UserRole_role ON SEC_User_Role
(
   Role_Id
);

ALTER TABLE SEC_Log ADD CONSTRAINT FK_Reference_24 FOREIGN KEY (User_Id)
      REFERENCES SEC_User (Id) ON DELETE RESTRICT ON UPDATE RESTRICT;

ALTER TABLE SEC_Org_Role ADD CONSTRAINT FK_SEC_ORGR_REFERENCE_SEC_ORG2 FOREIGN KEY (Org_Id)
      REFERENCES SEC_Organization (Id) ON DELETE RESTRICT ON UPDATE RESTRICT;

ALTER TABLE SEC_Org_Role ADD CONSTRAINT FK_Reference_14 FOREIGN KEY (Role_Id)
      REFERENCES SEC_Role (Id) ON DELETE RESTRICT ON UPDATE RESTRICT;

ALTER TABLE SEC_Organization ADD CONSTRAINT FK_Reference_11 FOREIGN KEY (Parent)
      REFERENCES SEC_Organization (Id) ON DELETE CASCADE ON UPDATE RESTRICT;

ALTER TABLE SEC_PERMISSION_RESOURCE ADD CONSTRAINT FK_Reference_34 FOREIGN KEY (Permission_Id)
      REFERENCES SEC_Permission (Id) ON DELETE RESTRICT ON UPDATE RESTRICT;

ALTER TABLE SEC_Permission ADD CONSTRAINT FK_SEC_PERM_PARENTPER_SEC_PERM FOREIGN KEY (Parent)
      REFERENCES SEC_Permission (Id) ON DELETE CASCADE ON UPDATE RESTRICT;

ALTER TABLE SEC_Permission_Rule ADD CONSTRAINT FK_SEC_SECU_BELONGTOO_SEC_PERM FOREIGN KEY (Operation_Id)
      REFERENCES SEC_Permission (Id) ON DELETE CASCADE ON UPDATE RESTRICT;

ALTER TABLE SEC_Role_Inheritance ADD CONSTRAINT FK_SEC_ROLE_INHERI_ROLE FOREIGN KEY (Parent_Role_Id)
      REFERENCES SEC_Role (Id) ON DELETE CASCADE ON UPDATE RESTRICT;

ALTER TABLE SEC_Role_Inheritance ADD CONSTRAINT FK_SEC_ROLE_INHERI_CHILD_ROLE FOREIGN KEY (Child_Role_Id)
      REFERENCES SEC_Role (Id) ON DELETE CASCADE ON UPDATE RESTRICT;

ALTER TABLE SEC_Role_Permission ADD CONSTRAINT FK_AssignToRole FOREIGN KEY (Role_Id)
      REFERENCES SEC_Role (Id) ON DELETE CASCADE ON UPDATE RESTRICT;

ALTER TABLE SEC_Role_Permission ADD CONSTRAINT FK_SEC_ROLE_BELONGTOP_SEC_PERM FOREIGN KEY (Operation_Id)
      REFERENCES SEC_Permission (Id) ON DELETE CASCADE ON UPDATE RESTRICT;

ALTER TABLE SEC_Role_Permission ADD CONSTRAINT FK_SEC_ROLE_PERMISSIO_SEC_SECU FOREIGN KEY (Rule_Id)
      REFERENCES SEC_Permission_Rule (Id) ON UPDATE RESTRICT;

ALTER TABLE SEC_User ADD CONSTRAINT FK_SEC_USER_REFERENCE_SEC_ORG1 FOREIGN KEY (Org_Id)
      REFERENCES SEC_Organization (Id) ON DELETE CASCADE ON UPDATE RESTRICT;

ALTER TABLE SEC_User_Role ADD CONSTRAINT FK_RoleRole FOREIGN KEY (Role_Id)
      REFERENCES SEC_Role (Id) ON DELETE CASCADE;

ALTER TABLE SEC_User_Role ADD CONSTRAINT FK_UserRole FOREIGN KEY (User_Id)
      REFERENCES SEC_User (Id) ON DELETE CASCADE;


/*支持工作流*/

CREATE TABLE SEC_WORKDAY
(
   DAY                  DATETIME NOT NULL COMMENT '工作日标识',
   ORG_ID               VARCHAR(38) NOT NULL COMMENT '组织标识',
   ID                   VARCHAR(38) COMMENT '组织标识',
   IS_WORK              SMALLINT DEFAULT 0 COMMENT '是否是工作日(0 非工作日 1 工作日)',
   DESCRIPTION          VARCHAR(300) COMMENT '备注',
   PRIMARY KEY (DAY, ORG_ID)
);

ALTER TABLE SEC_WORKDAY COMMENT '工作日表，定义特定组织机构的工作日情况';

CREATE INDEX FK_SEC_WORKDAY ON SEC_WORKDAY
(
   ORG_ID
);

CREATE TABLE SEC_WF_ORG_ROLE
(
   ORG_ID               VARCHAR(38) NOT NULL COMMENT '组织标识',
   WF_ROLE_ID           VARCHAR(38) NOT NULL COMMENT '流程角色标识',
   CREATED_BY           VARCHAR(38) NOT NULL COMMENT '创建人',
   CREATED_DATE         DATETIME NOT NULL COMMENT '创建时间',
   LAST_UPDATED_BY      VARCHAR(38) NOT NULL COMMENT '最后修改人',
   LAST_UPDATED_DATE    DATETIME NOT NULL COMMENT '最后修改时间',
   PRIMARY KEY (ORG_ID, WF_ROLE_ID)
);

ALTER TABLE SEC_WF_ORG_ROLE COMMENT '部门参与者角色里对应的会签部门';

CREATE INDEX INX_WF_ORGROLE_ORG ON SEC_WF_ORG_ROLE
(
   ORG_ID
);

CREATE INDEX INX_WF_ORGROLE_ROLE ON SEC_WF_ORG_ROLE
(
   WF_ROLE_ID
);

CREATE TABLE SEC_WF_USER_ROLE
(
   USER_ID              VARCHAR(38) NOT NULL COMMENT '用户标识',
   WF_ROLE_ID           VARCHAR(38) NOT NULL COMMENT '流程角色标识',
   CREATED_BY           VARCHAR(38) NOT NULL COMMENT '创建人',
   CREATED_DATE         DATETIME NOT NULL COMMENT '创建时间',
   LAST_UPDATED_BY      VARCHAR(38) NOT NULL COMMENT '最后修改人',
   LAST_UPDATED_DATE    DATETIME NOT NULL COMMENT '最后修改时间',
   PRIMARY KEY (WF_ROLE_ID, USER_ID)
);

ALTER TABLE SEC_WF_USER_ROLE COMMENT '用户流程角色';

CREATE INDEX INX_WF_USERROLE_USER ON SEC_WF_USER_ROLE
(
   USER_ID
);

CREATE INDEX INX_WF_USERROLE_ROLE ON SEC_WF_USER_ROLE
(
   WF_ROLE_ID
);

CREATE TABLE SEC_WF_ROLE
(
   ID                   VARCHAR(38) NOT NULL COMMENT '流程角色标识',
   ORG_ID               VARCHAR(38) NOT NULL,
   ROLE_ID              VARCHAR(38) NOT NULL COMMENT '角色标识',
   CREATED_BY           VARCHAR(38) NOT NULL COMMENT '创建人',
   CREATED_DATE         DATETIME NOT NULL COMMENT '创建时间',
   LAST_UPDATED_BY      VARCHAR(38) NOT NULL COMMENT '最后修改人',
   LAST_UPDATED_DATE    DATETIME NOT NULL COMMENT '最后修改时间',
   PRIMARY KEY (ID),
   KEY AK_KEY_2 (ORG_ID, ROLE_ID)
);

CREATE INDEX INX_WF_ROLE_ORG ON SEC_WF_ROLE
(
   ORG_ID
);

CREATE INDEX INX_WF_ROLE_ROLE ON SEC_WF_ROLE
(
   ROLE_ID
);

ALTER TABLE SEC_WORKDAY ADD CONSTRAINT FK_REFERENCE_29 FOREIGN KEY (ID)
      REFERENCES SEC_ORGANIZATION (ID) ON DELETE RESTRICT ON UPDATE RESTRICT;

ALTER TABLE SEC_WF_ORG_ROLE ADD CONSTRAINT FK_REFERENCE_22 FOREIGN KEY (ORG_ID)
      REFERENCES SEC_ORGANIZATION (ID) ON DELETE RESTRICT ON UPDATE RESTRICT;

ALTER TABLE SEC_WF_ORG_ROLE ADD CONSTRAINT FK_REFERENCE_23 FOREIGN KEY (WF_ROLE_ID)
      REFERENCES SEC_WF_ROLE (ID) ON DELETE CASCADE ON UPDATE RESTRICT;

ALTER TABLE SEC_WF_USER_ROLE ADD CONSTRAINT FK_REFERENCE_21 FOREIGN KEY (WF_ROLE_ID)
      REFERENCES SEC_WF_ROLE (ID) ON DELETE CASCADE ON UPDATE RESTRICT;

ALTER TABLE SEC_WF_USER_ROLE ADD CONSTRAINT FK_REFERENCE_26 FOREIGN KEY (USER_ID)
      REFERENCES SEC_USER (ID) ON DELETE RESTRICT ON UPDATE RESTRICT;

ALTER TABLE SEC_WF_ROLE ADD CONSTRAINT FK_REFERENCE_7 FOREIGN KEY (ROLE_ID)
      REFERENCES SEC_ROLE (ID) ON DELETE RESTRICT ON UPDATE RESTRICT;

ALTER TABLE SEC_WF_ROLE ADD CONSTRAINT FK_REFERENCE_8 FOREIGN KEY (ORG_ID)
      REFERENCES SEC_ORGANIZATION (ID) ON DELETE RESTRICT ON UPDATE RESTRICT;


/* 框架用到的视图 */
CREATE
    VIEW `v_sec_manage_role` 
    AS
(SELECT * FROM SEC_Role WHERE TYPE IN (0,2));


CREATE 
    VIEW `v_sec_valid_org`
    AS
    (SELECT * FROM SEC_Organization WHERE STATUS = 'enabled');
    
CREATE
    VIEW `v_sec_workflow_role` 
    AS
(SELECT * FROM sec_role WHERE `type` IN(1,2));


