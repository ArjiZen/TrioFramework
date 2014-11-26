/*==============================================================*/
/* DBMS name:      MySQL 5.0                                    */
/* Created on:     2012/3/7 14:12:30                            */
/*==============================================================*/


/*==============================================================*/
/* Table: SYS_BUSINESS_OPERATE_LOG                              */
/*==============================================================*/
create table SYS_BUSINESS_OPERATE_LOG
(
   BUSINESS_OPERATE_LOG_ID varchar(128) not null,
   APPLICATION          varchar(128),
   MODULE               varchar(128),
   ACTOR                varchar(128),
   ACTION               varchar(128),
   ACTION_CONTENT       varchar(512),
   ACTION_TIME          datetime,
   IS_SUCCESS           smallint,
   EXCEPTION_TYPE       varchar(128),
   EXCEPTION_MSG        varchar(4000)
);

alter table SYS_BUSINESS_OPERATE_LOG comment '业务操作日志';

alter table SYS_BUSINESS_OPERATE_LOG
   add primary key (BUSINESS_OPERATE_LOG_ID);

/*==============================================================*/
/* Table: SYS_LOGON_LOG                                         */
/*==============================================================*/
create table SYS_LOGON_LOG
(
   ID                   varchar(128) not null,
   USER_ID              varchar(128) comment '登录用户ID',
   ACCOUNT              varchar(128) comment '登录用户账号',
   LOGON_TIME           datetime comment '登录时间',
   LOGON_IP             varchar(128) comment '用户登录IP',
   REQUEST_URL          varchar(4000) comment '访问请求的URL',
   LOGOUT_TIME          datetime comment '注销时间'
);

alter table SYS_LOGON_LOG comment '登录日志';

alter table SYS_LOGON_LOG
   add primary key (ID);

/*==============================================================*/
/* Table: SYS_SERVICE_CALL_LOG                                  */
/*==============================================================*/
create table SYS_SERVICE_CALL_LOG
(
   SERVICE_CALL_LOG_ID  varchar(128) not null,
   APPLICATION          varchar(128),
   MODULE               varchar(128),
   CALL_INTERFACE       varchar(128),
   REQUEST_TIME         datetime,
   REQUEST_CONTENT      varchar(4000),
   REQUEST_CONTENT_LENGTH int,
   IS_SUCCESS           smallint,
   RESPONSE_TIME        datetime,
   RESPONSE_CONTENT     varchar(4000),
   RESPONSE_CONTENT_LENGTH int,
   EXCEPTION_TYPE       varchar(128),
   EXCEPTION_MSG        varchar(4000)
);

alter table SYS_SERVICE_CALL_LOG comment '服务调用日志';

alter table SYS_SERVICE_CALL_LOG
   add primary key (SERVICE_CALL_LOG_ID);

/*==============================================================*/
/* Table: SYS_URL_REQUEST_LOG                                   */
/*==============================================================*/
create table SYS_URL_REQUEST_LOG
(
   ID                   varchar(128) not null,
   USER_ID              varchar(128) comment '登录用户ID',
   ACCOUNT              varchar(128) comment '登录用户账号',
   REQUEST_URL          varchar(4000) comment '请求的URL',
   REQUEST_TIME         datetime comment '请求时间',
   CONSUMING_TIME       float comment '耗时'
);

alter table SYS_URL_REQUEST_LOG
   add primary key (ID);

