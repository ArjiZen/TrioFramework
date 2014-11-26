/*==============================================================*/
/* DBMS name:      ORACLE Version 9i                            */
/* Created on:     2012/2/9 9:42:59                             */
/*==============================================================*/


drop table SYS_BUSINESS_OPERATE_LOG cascade constraints;

drop table SYS_LOGON_LOG cascade constraints;

drop table SYS_SERVICE_CALL_LOG cascade constraints;

drop table SYS_URL_REQUEST_LOG cascade constraints;

/*==============================================================*/
/* Table: SYS_BUSINESS_OPERATE_LOG                              */
/*==============================================================*/
create table SYS_BUSINESS_OPERATE_LOG  (
   BUSINESS_OPERATE_LOG_ID VARCHAR2(128)                   not null,
   APPLICATION          VARCHAR2(128),
   MODULE               VARCHAR2(128),
   ACTOR                VARCHAR2(128),
   ACTION               VARCHAR2(128),
   ACTION_CONTENT       VARCHAR2(512),
   ACTION_TIME          DATE,
   IS_SUCCESS           SMALLINT,
   EXCEPTION_TYPE       VARCHAR2(128),
   EXCEPTION_MSG        VARCHAR2(4000),
   constraint PK_SYS_BUSINESS_OPERATE_LOG primary key (BUSINESS_OPERATE_LOG_ID)
);

comment on table SYS_BUSINESS_OPERATE_LOG is
'业务操作日志';

comment on column SYS_BUSINESS_OPERATE_LOG.BUSINESS_OPERATE_LOG_ID is
'BUSINESS_OPERATE_LOG_ID';

comment on column SYS_BUSINESS_OPERATE_LOG.APPLICATION is
'APPLICATION';

comment on column SYS_BUSINESS_OPERATE_LOG.MODULE is
'MODULE';

comment on column SYS_BUSINESS_OPERATE_LOG.ACTOR is
'ACTOR';

comment on column SYS_BUSINESS_OPERATE_LOG.ACTION is
'ACTION';

comment on column SYS_BUSINESS_OPERATE_LOG.ACTION_CONTENT is
'ACTION_CONTENT';

comment on column SYS_BUSINESS_OPERATE_LOG.ACTION_TIME is
'ACTION_TIME';

comment on column SYS_BUSINESS_OPERATE_LOG.IS_SUCCESS is
'IS_SUCCESS';

comment on column SYS_BUSINESS_OPERATE_LOG.EXCEPTION_TYPE is
'EXCEPTION_TYPE';

comment on column SYS_BUSINESS_OPERATE_LOG.EXCEPTION_MSG is
'EXCEPTION_MSG';

/*==============================================================*/
/* Table: SYS_LOGON_LOG                                         */
/*==============================================================*/
create table SYS_LOGON_LOG  (
   ID                   VARCHAR2(128)                   not null,
   USER_ID              VARCHAR2(128),
   ACCOUNT              VARCHAR2(128),
   LOGON_TIME           DATE,
   LOGON_IP             VARCHAR2(128),
   REQUEST_URL          VARCHAR2(4000),
   LOGOUT_TIME          DATE,
   constraint PK_SYS_LOGON_LOG primary key (ID)
);

comment on table SYS_LOGON_LOG is
'登录日志';

comment on column SYS_LOGON_LOG.ID is
'ID';

comment on column SYS_LOGON_LOG.USER_ID is
'登录用户ID';

comment on column SYS_LOGON_LOG.ACCOUNT is
'登录用户账号';

comment on column SYS_LOGON_LOG.LOGON_TIME is
'登录时间';

comment on column SYS_LOGON_LOG.LOGON_IP is
'用户登录IP';

comment on column SYS_LOGON_LOG.REQUEST_URL is
'访问请求的URL';

comment on column SYS_LOGON_LOG.LOGOUT_TIME is
'注销时间';

/*==============================================================*/
/* Table: SYS_SERVICE_CALL_LOG                                  */
/*==============================================================*/
create table SYS_SERVICE_CALL_LOG  (
   SERVICE_CALL_LOG_ID  VARCHAR2(128)                   not null,
   APPLICATION          VARCHAR2(128),
   MODULE               VARCHAR2(128),
   CALL_INTERFACE       VARCHAR2(128),
   REQUEST_TIME         DATE,
   REQUEST_CONTENT      VARCHAR2(4000),
   REQUEST_CONTENT_LENGTH INTEGER,
   IS_SUCCESS           SMALLINT,
   RESPONSE_TIME        DATE,
   RESPONSE_CONTENT     VARCHAR2(4000),
   RESPONSE_CONTENT_LENGTH INTEGER,
   EXCEPTION_TYPE       VARCHAR2(128),
   EXCEPTION_MSG        VARCHAR2(4000),
   constraint PK_SYS_SERVICE_CALL_LOG primary key (SERVICE_CALL_LOG_ID)
);

comment on table SYS_SERVICE_CALL_LOG is
'服务调用日志';

comment on column SYS_SERVICE_CALL_LOG.SERVICE_CALL_LOG_ID is
'SERVICE_CALL_LOG_ID';

comment on column SYS_SERVICE_CALL_LOG.APPLICATION is
'APPLICATION';

comment on column SYS_SERVICE_CALL_LOG.MODULE is
'MODULE';

comment on column SYS_SERVICE_CALL_LOG.CALL_INTERFACE is
'CALL_INTERFACE';

comment on column SYS_SERVICE_CALL_LOG.REQUEST_TIME is
'REQUEST_TIME';

comment on column SYS_SERVICE_CALL_LOG.REQUEST_CONTENT is
'REQUEST_CONTENT';

comment on column SYS_SERVICE_CALL_LOG.REQUEST_CONTENT_LENGTH is
'REQUEST_CONTENT_LENGTH';

comment on column SYS_SERVICE_CALL_LOG.IS_SUCCESS is
'IS_SUCCESS';

comment on column SYS_SERVICE_CALL_LOG.RESPONSE_TIME is
'RESPONSE_TIME';

comment on column SYS_SERVICE_CALL_LOG.RESPONSE_CONTENT is
'RESPONSE_CONTENT';

comment on column SYS_SERVICE_CALL_LOG.RESPONSE_CONTENT_LENGTH is
'RESPONSE_CONTENT_LENGTH';

comment on column SYS_SERVICE_CALL_LOG.EXCEPTION_TYPE is
'EXCEPTION_TYPE';

comment on column SYS_SERVICE_CALL_LOG.EXCEPTION_MSG is
'EXCEPTION_MSG';

/*==============================================================*/
/* Table: SYS_URL_REQUEST_LOG                                   */
/*==============================================================*/
create table SYS_URL_REQUEST_LOG  (
   ID                   VARCHAR2(128)                   not null,
   USER_ID              VARCHAR2(128),
   ACCOUNT              VARCHAR2(128),
   REQUEST_URL          VARCHAR2(4000),
   REQUEST_TIME         DATE,
   CONSUMING_TIME       FLOAT,
   constraint PK_SYS_URL_REQUEST_LOG primary key (ID)
);

comment on table SYS_URL_REQUEST_LOG is
'SYS_URL_REQUEST_LOG';

comment on column SYS_URL_REQUEST_LOG.ID is
'ID';

comment on column SYS_URL_REQUEST_LOG.USER_ID is
'登录用户ID';

comment on column SYS_URL_REQUEST_LOG.ACCOUNT is
'登录用户账号';

comment on column SYS_URL_REQUEST_LOG.REQUEST_URL is
'请求的URL';

comment on column SYS_URL_REQUEST_LOG.REQUEST_TIME is
'请求时间';

comment on column SYS_URL_REQUEST_LOG.CONSUMING_TIME is
'耗时';

