using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;

namespace SqlProfiler {
  public static class ComparisonOperators {
    public const int Equal = 0;
    public const int NotEqual = 1;
    public const int GreaterThan = 2;
    public const int LessThan = 3;
    public const int GreaterThanOrEqual = 4;
    public const int LessThanOrEqual = 5;
    public const int Like = 6;
    public const int NotLike = 7;
  }

  public static class LogicalOperators {
    public const int AND = 0;
    public const int OR = 1;
  }

  public static class ProfilerEvents {
    /*
    select 'public static class '+replace(name,' ','')+'
    {
    }

    '
    from sys.trace_categories
    order by category_id


    select '/ *'+sc.name+'* / '+'public const int '+replace(replace(ev.name,' ',''),':','')+' = '+cast(trace_event_id as varchar)+';'
    from	sys.trace_categories sc inner join sys.trace_events ev on sc.category_id = ev.category_id
    order by sc.category_id,ev.trace_event_id
        */
    public static readonly string[] Names = new string[202] {
      "", "", "", "", "", "", "", "", "", "", "RPC:Completed", "RPC:Starting", "SQL:BatchCompleted",
      "SQL:BatchStarting", "Audit Login", "Audit Logout", "Attention", "ExistingConnection",
      "Audit Server Starts And Stops", "DTCTransaction", "Audit Login Failed", "EventLog", "ErrorLog", "Lock:Released",
      "Lock:Acquired", "Lock:Deadlock", "Lock:Cancel", "Lock:Timeout", "Degree of Parallelism (7.0 Insert)", "", "", "",
      "", "Exception", "SP:CacheMiss", "SP:CacheInsert", "SP:CacheRemove", "SP:Recompile", "SP:CacheHit", "Deprecated",
      "SQL:StmtStarting", "SQL:StmtCompleted", "SP:Starting", "SP:Completed", "SP:StmtStarting", "SP:StmtCompleted",
      "Object:Created", "Object:Deleted", "", "", "SQLTransaction", "Scan:Started", "Scan:Stopped", "CursorOpen",
      "TransactionLog", "Hash Warning", "", "", "Auto Stats", "Lock:Deadlock Chain", "Lock:Escalation", "OLEDB Errors",
      "", "", "", "", "", "Execution Warnings", "Showplan Text (Unencoded)", "Sort Warnings", "CursorPrepare",
      "Prepare SQL", "Exec Prepared SQL", "Unprepare SQL", "CursorExecute", "CursorRecompile",
      "CursorImplicitConversion", "CursorUnprepare", "CursorClose", "Missing Column Statistics",
      "Missing Join Predicate", "Server Memory Change", "UserConfigurable:0", "UserConfigurable:1",
      "UserConfigurable:2", "UserConfigurable:3", "UserConfigurable:4", "UserConfigurable:5", "UserConfigurable:6",
      "UserConfigurable:7", "UserConfigurable:8", "UserConfigurable:9", "Data File Auto Grow", "Log File Auto Grow",
      "Data File Auto Shrink", "Log File Auto Shrink", "Showplan Text", "Showplan All", "Showplan Statistics Profile",
      "", "RPC Output Parameter", "", "Audit Database Scope GDR Event", "Audit Schema Object GDR Event",
      "Audit Addlogin Event", "Audit Login GDR Event", "Audit Login Change Property Event",
      "Audit Login Change Password Event", "Audit Add Login to Server Role Event", "Audit Add DB User Event",
      "Audit Add Member to DB Role Event", "Audit Add Role Event", "Audit App Role Change Password Event",
      "Audit Statement Permission Event", "Audit Schema Object Access Event", "Audit Backup/Restore Event",
      "Audit DBCC Event", "Audit Change Audit Event", "Audit Object Derived Permission Event", "OLEDB Call Event",
      "OLEDB QueryInterface Event", "OLEDB DataRead Event", "Showplan XML", "SQL:FullTextQuery", "Broker:Conversation",
      "Deprecation Announcement", "Deprecation Final Support", "Exchange Spill Event",
      "Audit Database Management Event", "Audit Database Object Management Event",
      "Audit Database Principal Management Event", "Audit Schema Object Management Event",
      "Audit Server Principal Impersonation Event", "Audit Database Principal Impersonation Event",
      "Audit Server Object Take Ownership Event", "Audit Database Object Take Ownership Event",
      "Broker:Conversation Group", "Blocked process report", "Broker:Connection", "Broker:Forwarded Message Sent",
      "Broker:Forwarded Message Dropped", "Broker:Message Classify", "Broker:Transmission", "Broker:Queue Disabled",
      "Broker:Mirrored Route State Changed", "", "Showplan XML Statistics Profile", "", "Deadlock graph",
      "Broker:Remote Message Acknowledgement", "Trace File Close", "", "Audit Change Database Owner",
      "Audit Schema Object Take Ownership Event", "", "FT:Crawl Started", "FT:Crawl Stopped", "FT:Crawl Aborted",
      "Audit Broker Conversation", "Audit Broker Login", "Broker:Message Undeliverable", "Broker:Corrupted Message",
      "User Error Message", "Broker:Activation", "Object:Altered", "Performance statistics", "SQL:StmtRecompile",
      "Database Mirroring State Change", "Showplan XML For Query Compile", "Showplan All For Query Compile",
      "Audit Server Scope GDR Event", "Audit Server Object GDR Event", "Audit Database Object GDR Event",
      "Audit Server Operation Event", "", "Audit Server Alter Trace Event", "Audit Server Object Management Event",
      "Audit Server Principal Management Event", "Audit Database Operation Event", "",
      "Audit Database Object Access Event", "TM: Begin Tran starting", "TM: Begin Tran completed",
      "TM: Promote Tran starting", "TM: Promote Tran completed", "TM: Commit Tran starting",
      "TM: Commit Tran completed", "TM: Rollback Tran starting", "TM: Rollback Tran completed",
      "Lock:Timeout (timeout > 0)", "Progress Report: Online Index Operation", "TM: Save Tran starting",
      "TM: Save Tran completed", "Background Job Error", "OLEDB Provider Information", "Mount Tape", "Assembly Load",
      "", "XQuery Static Type", "QN: Subscription", "QN: Parameter table", "QN: Template"
    };

    public static class Cursors {
      /*Cursors*/
      public const int CursorOpen = 53;

      /*Cursors*/
      public const int CursorPrepare = 70;

      /*Cursors*/
      public const int CursorExecute = 74;

      /*Cursors*/
      public const int CursorRecompile = 75;

      /*Cursors*/
      public const int CursorImplicitConversion = 76;

      /*Cursors*/
      public const int CursorUnprepare = 77;

      /*Cursors*/
      public const int CursorClose = 78;
    }

    public static class Database {
      /*Database*/
      public const int DataFileAutoGrow = 92;

      /*Database*/
      public const int LogFileAutoGrow = 93;

      /*Database*/
      public const int DataFileAutoShrink = 94;

      /*Database*/
      public const int LogFileAutoShrink = 95;

      /*Database*/
      public const int DatabaseMirroringStateChange = 167;
    }

    public static class ErrorsAndWarnings {
      /*Errors and Warnings*/
      public const int Attention = 16;

      /*Errors and Warnings*/
      public const int EventLog = 21;

      /*Errors and Warnings*/
      public const int ErrorLog = 22;

      /*Errors and Warnings*/
      public const int Exception = 33;

      /*Errors and Warnings*/
      public const int HashWarning = 55;

      /*Errors and Warnings*/
      public const int ExecutionWarnings = 67;

      /*Errors and Warnings*/
      public const int SortWarnings = 69;

      /*Errors and Warnings*/
      public const int MissingColumnStatistics = 79;

      /*Errors and Warnings*/
      public const int MissingJoinPredicate = 80;

      /*Errors and Warnings*/
      public const int ExchangeSpillEvent = 127;

      /*Errors and Warnings*/
      public const int Blockedprocessreport = 137;

      /*Errors and Warnings*/
      public const int UserErrorMessage = 162;

      /*Errors and Warnings*/
      public const int BackgroundJobError = 193;
    }

    public static class Locks {
      /*Locks*/
      public const int LockReleased = 23;

      /*Locks*/
      public const int LockAcquired = 24;

      /*Locks*/
      public const int LockDeadlock = 25;

      /*Locks*/
      public const int LockCancel = 26;

      /*Locks*/
      public const int LockTimeout = 27;

      /*Locks*/
      public const int LockDeadlockChain = 59;

      /*Locks*/
      public const int LockEscalation = 60;

      /*Locks*/
      public const int Deadlockgraph = 148;

      /*Locks*/
      public const int LockTimeout100 = 189;
    }

    public static class Objects {
      /*Objects*/
      public const int ObjectCreated = 46;

      /*Objects*/
      public const int ObjectDeleted = 47;

      /*Objects*/
      public const int ObjectAltered = 164;
    }

    public static class Performance {
      /*Performance*/
      public const int DegreeofParallelism70Insert = 28;

      /*Performance*/
      public const int AutoStats = 58;

      /*Performance*/
      public const int ShowplanTextUnencoded = 68;

      /*Performance*/
      public const int ShowplanText = 96;

      /*Performance*/
      public const int ShowplanAll = 97;

      /*Performance*/
      public const int ShowplanStatisticsProfile = 98;

      /*Performance*/
      public const int ShowplanXML = 122;

      /*Performance*/
      public const int SQLFullTextQuery = 123;

      /*Performance*/
      public const int ShowplanXMLStatisticsProfile = 146;

      /*Performance*/
      public const int Performancestatistics = 165;

      /*Performance*/
      public const int ShowplanXMLForQueryCompile = 168;

      /*Performance*/
      public const int ShowplanAllForQueryCompile = 169;
    }

    public static class Scans {
      /*Scans*/
      public const int ScanStarted = 51;

      /*Scans*/
      public const int ScanStopped = 52;
    }

    public static class SecurityAudit {
      /*Security Audit*/
      public const int AuditLogin = 14;

      /*Security Audit*/
      public const int AuditLogout = 15;

      /*Security Audit*/
      public const int AuditServerStartsAndStops = 18;

      /*Security Audit*/
      public const int AuditLoginFailed = 20;

      /*Security Audit*/
      public const int AuditDatabaseScopeGDREvent = 102;

      /*Security Audit*/
      public const int AuditSchemaObjectGDREvent = 103;

      /*Security Audit*/
      public const int AuditAddloginEvent = 104;

      /*Security Audit*/
      public const int AuditLoginGDREvent = 105;

      /*Security Audit*/
      public const int AuditLoginChangePropertyEvent = 106;

      /*Security Audit*/
      public const int AuditLoginChangePasswordEvent = 107;

      /*Security Audit*/
      public const int AuditAddLogintoServerRoleEvent = 108;

      /*Security Audit*/
      public const int AuditAddDBUserEvent = 109;

      /*Security Audit*/
      public const int AuditAddMembertoDBRoleEvent = 110;

      /*Security Audit*/
      public const int AuditAddRoleEvent = 111;

      /*Security Audit*/
      public const int AuditAppRoleChangePasswordEvent = 112;

      /*Security Audit*/
      public const int AuditStatementPermissionEvent = 113;

      /*Security Audit*/
      public const int AuditSchemaObjectAccessEvent = 114;

      /*Security Audit*/
      public const int AuditBackupRestoreEvent = 115;

      /*Security Audit*/
      public const int AuditDBCCEvent = 116;

      /*Security Audit*/
      public const int AuditChangeAuditEvent = 117;

      /*Security Audit*/
      public const int AuditObjectDerivedPermissionEvent = 118;

      /*Security Audit*/
      public const int AuditDatabaseManagementEvent = 128;

      /*Security Audit*/
      public const int AuditDatabaseObjectManagementEvent = 129;

      /*Security Audit*/
      public const int AuditDatabasePrincipalManagementEvent = 130;

      /*Security Audit*/
      public const int AuditSchemaObjectManagementEvent = 131;

      /*Security Audit*/
      public const int AuditServerPrincipalImpersonationEvent = 132;

      /*Security Audit*/
      public const int AuditDatabasePrincipalImpersonationEvent = 133;

      /*Security Audit*/
      public const int AuditServerObjectTakeOwnershipEvent = 134;

      /*Security Audit*/
      public const int AuditDatabaseObjectTakeOwnershipEvent = 135;

      /*Security Audit*/
      public const int AuditChangeDatabaseOwner = 152;

      /*Security Audit*/
      public const int AuditSchemaObjectTakeOwnershipEvent = 153;

      /*Security Audit*/
      public const int AuditBrokerConversation = 158;

      /*Security Audit*/
      public const int AuditBrokerLogin = 159;

      /*Security Audit*/
      public const int AuditServerScopeGDREvent = 170;

      /*Security Audit*/
      public const int AuditServerObjectGDREvent = 171;

      /*Security Audit*/
      public const int AuditDatabaseObjectGDREvent = 172;

      /*Security Audit*/
      public const int AuditServerOperationEvent = 173;

      /*Security Audit*/
      public const int AuditServerAlterTraceEvent = 175;

      /*Security Audit*/
      public const int AuditServerObjectManagementEvent = 176;

      /*Security Audit*/
      public const int AuditServerPrincipalManagementEvent = 177;

      /*Security Audit*/
      public const int AuditDatabaseOperationEvent = 178;

      /*Security Audit*/
      public const int AuditDatabaseObjectAccessEvent = 180;
    }

    public static class Server {
      /*Server*/
      public const int ServerMemoryChange = 81;

      /*Server*/
      public const int TraceFileClose = 150;

      /*Server*/
      public const int MountTape = 195;
    }

    public static class Sessions {
      /*Sessions*/
      public const int ExistingConnection = 17;
    }

    public static class StoredProcedures {
      /*Stored Procedures*/
      public const int RPCCompleted = 10;

      /*Stored Procedures*/
      public const int RPCStarting = 11;

      /*Stored Procedures*/
      public const int SPCacheMiss = 34;

      /*Stored Procedures*/
      public const int SPCacheInsert = 35;

      /*Stored Procedures*/
      public const int SPCacheRemove = 36;

      /*Stored Procedures*/
      public const int SPRecompile = 37;

      /*Stored Procedures*/
      public const int SPCacheHit = 38;

      /*Stored Procedures*/
      public const int Deprecated = 39;

      /*Stored Procedures*/
      public const int SPStarting = 42;

      /*Stored Procedures*/
      public const int SPCompleted = 43;

      /*Stored Procedures*/
      public const int SPStmtStarting = 44;

      /*Stored Procedures*/
      public const int SPStmtCompleted = 45;

      /*Stored Procedures*/
      public const int RPCOutputParameter = 100;
    }

    public static class Transactions {
      /*Transactions*/
      public const int DTCTransaction = 19;

      /*Transactions*/
      public const int SQLTransaction = 50;

      /*Transactions*/
      public const int TransactionLog = 54;

      /*Transactions*/
      public const int TMBeginTranstarting = 181;

      /*Transactions*/
      public const int TMBeginTrancompleted = 182;

      /*Transactions*/
      public const int TMPromoteTranstarting = 183;

      /*Transactions*/
      public const int TMPromoteTrancompleted = 184;

      /*Transactions*/
      public const int TMCommitTranstarting = 185;

      /*Transactions*/
      public const int TMCommitTrancompleted = 186;

      /*Transactions*/
      public const int TMRollbackTranstarting = 187;

      /*Transactions*/
      public const int TMRollbackTrancompleted = 188;

      /*Transactions*/
      public const int TMSaveTranstarting = 191;

      /*Transactions*/
      public const int TMSaveTrancompleted = 192;
    }

    public static class TSQL {
      /*TSQL*/
      public const int SQLBatchCompleted = 12;

      /*TSQL*/
      public const int SQLBatchStarting = 13;

      /*TSQL*/
      public const int SQLStmtStarting = 40;

      /*TSQL*/
      public const int SQLStmtCompleted = 41;

      /*TSQL*/
      public const int PrepareSQL = 71;

      /*TSQL*/
      public const int ExecPreparedSQL = 72;

      /*TSQL*/
      public const int UnprepareSQL = 73;

      /*TSQL*/
      public const int SQLStmtRecompile = 166;

      /*TSQL*/
      public const int XQueryStaticType = 198;
    }

    public static class Userconfigurable {
      /*User configurable*/
      public const int UserConfigurable0 = 82;

      /*User configurable*/
      public const int UserConfigurable1 = 83;

      /*User configurable*/
      public const int UserConfigurable2 = 84;

      /*User configurable*/
      public const int UserConfigurable3 = 85;

      /*User configurable*/
      public const int UserConfigurable4 = 86;

      /*User configurable*/
      public const int UserConfigurable5 = 87;

      /*User configurable*/
      public const int UserConfigurable6 = 88;

      /*User configurable*/
      public const int UserConfigurable7 = 89;

      /*User configurable*/
      public const int UserConfigurable8 = 90;

      /*User configurable*/
      public const int UserConfigurable9 = 91;
    }

    public static class OLEDB {
      /*OLEDB*/
      public const int OLEDBErrors = 61;

      /*OLEDB*/
      public const int OLEDBCallEvent = 119;

      /*OLEDB*/
      public const int OLEDBQueryInterfaceEvent = 120;

      /*OLEDB*/
      public const int OLEDBDataReadEvent = 121;

      /*OLEDB*/
      public const int OLEDBProviderInformation = 194;
    }

    public static class Broker {
      /*Broker*/
      public const int BrokerConversation = 124;

      /*Broker*/
      public const int BrokerConversationGroup = 136;

      /*Broker*/
      public const int BrokerConnection = 138;

      /*Broker*/
      public const int BrokerForwardedMessageSent = 139;

      /*Broker*/
      public const int BrokerForwardedMessageDropped = 140;

      /*Broker*/
      public const int BrokerMessageClassify = 141;

      /*Broker*/
      public const int BrokerTransmission = 142;

      /*Broker*/
      public const int BrokerQueueDisabled = 143;

      /*Broker*/
      public const int BrokerMirroredRouteStateChanged = 144;

      /*Broker*/
      public const int BrokerRemoteMessageAcknowledgement = 149;

      /*Broker*/
      public const int BrokerMessageUndeliverable = 160;

      /*Broker*/
      public const int BrokerCorruptedMessage = 161;

      /*Broker*/
      public const int BrokerActivation = 163;
    }

    public static class Fulltext {
      /*Full text*/
      public const int FTCrawlStarted = 155;

      /*Full text*/
      public const int FTCrawlStopped = 156;

      /*Full text*/
      public const int FTCrawlAborted = 157;
    }

    public static class Deprecation {
      /*Deprecation*/
      public const int DeprecationAnnouncement = 125;

      /*Deprecation*/
      public const int DeprecationFinalSupport = 126;
    }

    public static class ProgressReport {
      /*Progress Report*/
      public const int ProgressReportOnlineIndexOperation = 190;
    }

    public static class CLR {
      /*CLR*/
      public const int AssemblyLoad = 196;
    }

    public static class QueryNotifications {
      /*Query Notifications*/
      public const int QNSubscription = 199;

      /*Query Notifications*/
      public const int QNParametertable = 200;

      /*Query Notifications*/
      public const int QNTemplate = 201;

      /*Query Notifications*/
      public const int QNDynamics = 202;
    }
  }

  public enum ProfilerColumnDataType {
    Long,
    DateTime,
    Byte,
    Int,
    String,
    Guid
  }

  public static class ProfilerEventColumns {
    public static readonly string[] ColumnNames = {
      "Dumy", "TextData", "BinaryData", "DatabaseID", "TransactionID", "LineNumber", "NTUserName", "NTDomainName",
      "HostName", "ClientProcessID", "ApplicationName", "LoginName", "SPID", "Duration", "StartTime", "EndTime",
      "Reads", "Writes", "CPU", "Permissions", "Severity", "EventSubClass", "ObjectID", "Success", "IndexID",
      "IntegerData", "ServerName", "EventClass", "ObjectType", "NestLevel", "State", "Error", "Mode", "Handle",
      "ObjectName", "DatabaseName", "FileName", "OwnerName", "RoleName", "TargetUserName", "DBUserName", "LoginSid",
      "TargetLoginName", "TargetLoginSid", "ColumnPermissions", "LinkedServerName", "ProviderName", "MethodName",
      "RowCounts", "RequestID", "XactSequence", "EventSequence", "BigintData1", "BigintData2", "GUID", "IntegerData2",
      "ObjectID2", "Type", "OwnerID", "ParentName", "IsSystem", "Offset", "SourceDatabaseID", "SqlHandle",
      "SessionLoginName", "PlanHandle"
    };

    public static readonly ProfilerColumnDataType[] ProfilerColumnDataTypes = {
      /*dummy*/
      ProfilerColumnDataType.String
      /*TextData*/,
      ProfilerColumnDataType.String
      /*BinaryData*/,
      ProfilerColumnDataType.Byte
      /*DatabaseID*/,
      ProfilerColumnDataType.Int
      /*TransactionID*/,
      ProfilerColumnDataType.Long
      /*LineNumber*/,
      ProfilerColumnDataType.Int
      /*NTUserName*/,
      ProfilerColumnDataType.String
      /*NTDomainName*/,
      ProfilerColumnDataType.String
      /*HostName*/,
      ProfilerColumnDataType.String
      /*ClientProcessID*/,
      ProfilerColumnDataType.Int
      /*ApplicationName*/,
      ProfilerColumnDataType.String
      /*LoginName*/,
      ProfilerColumnDataType.String
      /*SPID*/,
      ProfilerColumnDataType.Int
      /*Duration*/,
      ProfilerColumnDataType.Long
      /*StartTime*/,
      ProfilerColumnDataType.DateTime
      /*EndTime*/,
      ProfilerColumnDataType.DateTime
      /*Reads*/,
      ProfilerColumnDataType.Long
      /*Writes*/,
      ProfilerColumnDataType.Long
      /*CPU*/,
      ProfilerColumnDataType.Int
      /*Permissions*/,
      ProfilerColumnDataType.Long
      /*Severity*/,
      ProfilerColumnDataType.Int
      /*EventSubClass*/,
      ProfilerColumnDataType.Int
      /*ObjectID*/,
      ProfilerColumnDataType.Int
      /*Success*/,
      ProfilerColumnDataType.Int
      /*IndexID*/,
      ProfilerColumnDataType.Int
      /*IntegerData*/,
      ProfilerColumnDataType.Int
      /*ServerName*/,
      ProfilerColumnDataType.String
      /*EventClass*/,
      ProfilerColumnDataType.Int
      /*ObjectType*/,
      ProfilerColumnDataType.Int
      /*NestLevel*/,
      ProfilerColumnDataType.Int
      /*State*/,
      ProfilerColumnDataType.Int
      /*Error*/,
      ProfilerColumnDataType.Int
      /*Mode*/,
      ProfilerColumnDataType.Int
      /*Handle*/,
      ProfilerColumnDataType.Int
      /*ObjectName*/,
      ProfilerColumnDataType.String
      /*DatabaseName*/,
      ProfilerColumnDataType.String
      /*FileName*/,
      ProfilerColumnDataType.String
      /*OwnerName*/,
      ProfilerColumnDataType.String
      /*RoleName*/,
      ProfilerColumnDataType.String
      /*TargetUserName*/,
      ProfilerColumnDataType.String
      /*DBUserName*/,
      ProfilerColumnDataType.String
      /*LoginSid*/,
      ProfilerColumnDataType.Byte
      /*TargetLoginName*/,
      ProfilerColumnDataType.String
      /*TargetLoginSid*/,
      ProfilerColumnDataType.Byte
      /*ColumnPermissions*/,
      ProfilerColumnDataType.Int
      /*LinkedServerName*/,
      ProfilerColumnDataType.String
      /*ProviderName*/,
      ProfilerColumnDataType.String
      /*MethodName*/,
      ProfilerColumnDataType.String
      /*RowCounts*/,
      ProfilerColumnDataType.Long
      /*RequestID*/,
      ProfilerColumnDataType.Int
      /*XactSequence*/,
      ProfilerColumnDataType.Long
      /*EventSequence*/,
      ProfilerColumnDataType.Long
      /*BigintData1*/,
      ProfilerColumnDataType.Long
      /*BigintData2*/,
      ProfilerColumnDataType.Long
      /*GUID*/,
      ProfilerColumnDataType.Guid
      /*IntegerData2*/,
      ProfilerColumnDataType.Int
      /*ObjectID2*/,
      ProfilerColumnDataType.Long
      /*Type*/,
      ProfilerColumnDataType.Int
      /*OwnerID*/,
      ProfilerColumnDataType.Int
      /*ParentName*/,
      ProfilerColumnDataType.String
      /*IsSystem*/,
      ProfilerColumnDataType.Int
      /*Offset*/,
      ProfilerColumnDataType.Int
      /*SourceDatabaseID*/,
      ProfilerColumnDataType.Int
      /*SqlHandle*/,
      ProfilerColumnDataType.Byte
      /*SessionLoginName*/,
      ProfilerColumnDataType.String
      /*PlanHandle*/,
      ProfilerColumnDataType.Byte
    };

    /*
    select 'public const int '+Name + '= '+cast(trace_column_id as varchar)+';'
    from sys.trace_columns
    order by trace_column_id
     */
    public const int TextData = 1;
    public const int BinaryData = 2;
    public const int DatabaseID = 3;
    public const int TransactionID = 4;
    public const int LineNumber = 5;
    public const int NTUserName = 6;
    public const int NTDomainName = 7;
    public const int HostName = 8;
    public const int ClientProcessID = 9;
    public const int ApplicationName = 10;
    public const int LoginName = 11;
    public const int SPID = 12;
    public const int Duration = 13;
    public const int StartTime = 14;
    public const int EndTime = 15;
    public const int Reads = 16;
    public const int Writes = 17;
    public const int CPU = 18;
    public const int Permissions = 19;
    public const int Severity = 20;
    public const int EventSubClass = 21;
    public const int ObjectID = 22;
    public const int Success = 23;
    public const int IndexID = 24;
    public const int IntegerData = 25;
    public const int ServerName = 26;
    public const int EventClass = 27;
    public const int ObjectType = 28;
    public const int NestLevel = 29;
    public const int State = 30;
    public const int Error = 31;
    public const int Mode = 32;
    public const int Handle = 33;
    public const int ObjectName = 34;
    public const int DatabaseName = 35;
    public const int FileName = 36;
    public const int OwnerName = 37;
    public const int RoleName = 38;
    public const int TargetUserName = 39;
    public const int DBUserName = 40;
    public const int LoginSid = 41;
    public const int TargetLoginName = 42;
    public const int TargetLoginSid = 43;
    public const int ColumnPermissions = 44;
    public const int LinkedServerName = 45;
    public const int ProviderName = 46;
    public const int MethodName = 47;
    public const int RowCounts = 48;
    public const int RequestID = 49;
    public const int XactSequence = 50;
    public const int EventSequence = 51;
    public const int BigintData1 = 52;
    public const int BigintData2 = 53;
    public const int GUID = 54;
    public const int IntegerData2 = 55;
    public const int ObjectID2 = 56;
    public const int Type = 57;
    public const int OwnerID = 58;
    public const int ParentName = 59;
    public const int IsSystem = 60;
    public const int Offset = 61;
    public const int SourceDatabaseID = 62;
    public const int SqlHandle = 63;
    public const int SessionLoginName = 64;
    public const int PlanHandle = 65;
  }

  public class ProfilerEvent {
    internal readonly Object[] Values = new object[66];
    internal ulong ColumnMask;
    internal ulong ColumnMaskHigh;

    internal void MarkSet(int columnId) {
      if (columnId < 64) ColumnMask |= 1UL << columnId;
      else ColumnMaskHigh |= 1UL << (columnId - 64);
    }

    /*
select 'case ProfilerEventColumns.'+Name + ':
'
+
case when row_number() over(partition by Type_Name order by trace_column_id desc) = 1 then

'		return Get'+
case Type_Name
        when 'text' then 'String'
        when 'int' then 'Int'
        when 'bigint' then 'Long'
        when 'nvarchar' then 'String'
        when 'datetime' then 'DateTime'
        when 'image' then 'Byte'
        when 'uniqueidentifier' then 'Guid'
    end
+'(idx);
'
else '' end
from sys.trace_columns
order by Type_Name,trace_column_id
     */

    public string GetFormattedData(int idx, string format) {
      switch (ProfilerEventColumns.ProfilerColumnDataTypes[idx]) {
        case ProfilerColumnDataType.Long:
          return GetLong(idx).ToString(format);
        case ProfilerColumnDataType.DateTime:
          var d = GetDateTime(idx);
          return 1 == d.Year ? "" : d.ToString(format);
        case ProfilerColumnDataType.Byte:
          return GetByte(idx).ToString();
        case ProfilerColumnDataType.Int:
          return GetInt(idx).ToString(format);
        case ProfilerColumnDataType.String:
          return GetString(idx);
        case ProfilerColumnDataType.Guid:
          return GetGuid(idx).ToString();
      }

      return null;
    }

    private int GetInt(int idx) {
      if (!ColumnIsSet(idx)) return 0;
      return Values[idx] is null ? 0 : (int) Values[idx];
    }

    private long GetLong(int idx) {
      if (!ColumnIsSet(idx)) return 0;
      return Values[idx] is null ? 0 : (long) Values[idx];
    }

    private string GetString(int idx) {
      if (!ColumnIsSet(idx)) return "";
      return Values[idx] is null ? "" : (string) Values[idx];
    }

    private byte[] GetByte(int idx) {
      return ColumnIsSet(idx) ? (byte[]) Values[idx] : new byte[1];
    }

    private DateTime GetDateTime(int idx) {
      return ColumnIsSet(idx) ? (DateTime) Values[idx] : new DateTime(0);
    }

    private Guid GetGuid(int idx) {
      return ColumnIsSet(idx) ? (Guid) Values[idx] : Guid.Empty;
    }

    public bool ColumnIsSet(int columnId) {
      return columnId < 64
        ? (ColumnMask & (1UL << columnId)) != 0
        : (ColumnMaskHigh & (1UL << (columnId - 64))) != 0;
    }


    /*
select 'public '+case Type_Name
        when 'text' then 'string'
        when 'int' then 'int'
        when 'bigint' then 'long'
        when 'nvarchar' then 'string'
        when 'datetime' then 'DateTime'
        when 'image' then 'byte[]'
        when 'uniqueidentifier' then 'GUID'
    end
+' '+Name + '{get{ return Get'+
case Type_Name
        when 'text' then 'String'
        when 'int' then 'Int'
        when 'bigint' then 'Long'
        when 'nvarchar' then 'String'
        when 'datetime' then 'DateTime'
        when 'image' then 'Byte'
        when 'uniqueidentifier' then 'Guid'
    end
+'(ProfilerEventColumns.'+Name+');}}'
from sys.trace_columns
order by trace_column_id

     *
     */

    public string TextData => GetString(ProfilerEventColumns.TextData);

    public byte[] BinaryData => GetByte(ProfilerEventColumns.BinaryData);

    public int DatabaseID => GetInt(ProfilerEventColumns.DatabaseID);

    public long TransactionID => GetLong(ProfilerEventColumns.TransactionID);

    public int LineNumber => GetInt(ProfilerEventColumns.LineNumber);

    public string NTUserName => GetString(ProfilerEventColumns.NTUserName);

    public string NTDomainName => GetString(ProfilerEventColumns.NTDomainName);

    public string HostName => GetString(ProfilerEventColumns.HostName);

    public int ClientProcessID => GetInt(ProfilerEventColumns.ClientProcessID);

    public string ApplicationName => GetString(ProfilerEventColumns.ApplicationName);

    public string LoginName => GetString(ProfilerEventColumns.LoginName);

    public int SPID => GetInt(ProfilerEventColumns.SPID);

    public long Duration => GetLong(ProfilerEventColumns.Duration);

    public DateTime StartTime => GetDateTime(ProfilerEventColumns.StartTime);

    public DateTime EndTime => GetDateTime(ProfilerEventColumns.EndTime);

    public long Reads => GetLong(ProfilerEventColumns.Reads);

    public long Writes => GetLong(ProfilerEventColumns.Writes);

    public int CPU => GetInt(ProfilerEventColumns.CPU);

    public long Permissions => GetLong(ProfilerEventColumns.Permissions);

    public int Severity => GetInt(ProfilerEventColumns.Severity);

    public int EventSubClass => GetInt(ProfilerEventColumns.EventSubClass);

    public int ObjectID => GetInt(ProfilerEventColumns.ObjectID);

    public int Success => GetInt(ProfilerEventColumns.Success);

    public int IndexID => GetInt(ProfilerEventColumns.IndexID);

    public int IntegerData => GetInt(ProfilerEventColumns.IntegerData);

    public string ServerName => GetString(ProfilerEventColumns.ServerName);

    public int EventClass => GetInt(ProfilerEventColumns.EventClass);

    public int ObjectType => GetInt(ProfilerEventColumns.ObjectType);

    public int NestLevel => GetInt(ProfilerEventColumns.NestLevel);

    public int State => GetInt(ProfilerEventColumns.State);

    public int Error => GetInt(ProfilerEventColumns.Error);

    public int Mode => GetInt(ProfilerEventColumns.Mode);

    public int Handle => GetInt(ProfilerEventColumns.Handle);

    public string ObjectName => GetString(ProfilerEventColumns.ObjectName);

    public string DatabaseName => GetString(ProfilerEventColumns.DatabaseName);

    public string FileName => GetString(ProfilerEventColumns.FileName);

    public string OwnerName => GetString(ProfilerEventColumns.OwnerName);

    public string RoleName => GetString(ProfilerEventColumns.RoleName);

    public string TargetUserName => GetString(ProfilerEventColumns.TargetUserName);

    public string DBUserName => GetString(ProfilerEventColumns.DBUserName);

    public byte[] LoginSid => GetByte(ProfilerEventColumns.LoginSid);

    public string TargetLoginName => GetString(ProfilerEventColumns.TargetLoginName);

    public byte[] TargetLoginSid => GetByte(ProfilerEventColumns.TargetLoginSid);

    public int ColumnPermissions => GetInt(ProfilerEventColumns.ColumnPermissions);

    public string LinkedServerName => GetString(ProfilerEventColumns.LinkedServerName);

    public string ProviderName => GetString(ProfilerEventColumns.ProviderName);

    public string MethodName => GetString(ProfilerEventColumns.MethodName);

    public long RowCounts => GetLong(ProfilerEventColumns.RowCounts);

    public int RequestID => GetInt(ProfilerEventColumns.RequestID);

    public long XactSequence => GetLong(ProfilerEventColumns.XactSequence);

    public long EventSequence => GetLong(ProfilerEventColumns.EventSequence);

    public long BigintData1 => GetLong(ProfilerEventColumns.BigintData1);

    public long BigintData2 => GetLong(ProfilerEventColumns.BigintData2);

    public Guid GUID => GetGuid(ProfilerEventColumns.GUID);

    public int IntegerData2 => GetInt(ProfilerEventColumns.IntegerData2);

    public long ObjectID2 => GetLong(ProfilerEventColumns.ObjectID2);

    public int Type => GetInt(ProfilerEventColumns.Type);

    public int OwnerID => GetInt(ProfilerEventColumns.OwnerID);

    public string ParentName => GetString(ProfilerEventColumns.ParentName);

    public int IsSystem => GetInt(ProfilerEventColumns.IsSystem);

    public int Offset => GetInt(ProfilerEventColumns.Offset);

    public int SourceDatabaseID => GetInt(ProfilerEventColumns.SourceDatabaseID);

    public byte[] SqlHandle => GetByte(ProfilerEventColumns.SqlHandle);

    public string SessionLoginName => GetString(ProfilerEventColumns.SessionLoginName);

    public byte[] PlanHandle => GetByte(ProfilerEventColumns.PlanHandle);
  }

  public class RawTraceReader {
    private delegate void SetEventDelegate(ProfilerEvent evt, int columnId);

    private DbDataReader _reader;
    private readonly byte[] _b16 = new byte[16];
    private readonly byte[] _b8 = new byte[8];
    private readonly byte[] _b2 = new byte[2];
    private readonly byte[] _b4 = new byte[4];
    private readonly SqlConnection _conn;

    public int TraceId { get; private set; }

    private readonly SetEventDelegate[] _delegates = new SetEventDelegate[66];

    private bool Read() {
      TraceIsActive = _reader.Read();
      return TraceIsActive;
    }

    public bool TraceIsActive { get; private set; }

    public void Close() {
      TraceIsActive = false;
      _reader?.Close();
    }

    public RawTraceReader(SqlConnection con) {
      _conn = con;
      SetEventDelegate evtInt = SetIntColumn;
      SetEventDelegate evtLong = SetLongColumn;
      SetEventDelegate evtString = SetStringColumn;
      SetEventDelegate evtByte = SetByteColumn;
      SetEventDelegate evtDateTime = SetDateTimeColumn;
      SetEventDelegate evtGuid = SetGuidColumn;
      /*
      select '_delegates[ProfilerEventColumns.'+Name+'] = evt'+
      case Type_Name
                              when 'text' then 'String'
                              when 'int' then 'Int'
                              when 'bigint' then 'Long'
                              when 'nvarchar' then 'String'
                              when 'datetime' then 'DateTime'
                              when 'image' then 'Byte'
                              when 'uniqueidentifier' then 'Guid'
                      end+';'

      from sys.trace_columns
      order by trace_column_id

       */
      _delegates[ProfilerEventColumns.TextData] = evtString;
      _delegates[ProfilerEventColumns.BinaryData] = evtByte;
      _delegates[ProfilerEventColumns.DatabaseID] = evtInt;
      _delegates[ProfilerEventColumns.TransactionID] = evtLong;
      _delegates[ProfilerEventColumns.LineNumber] = evtInt;
      _delegates[ProfilerEventColumns.NTUserName] = evtString;
      _delegates[ProfilerEventColumns.NTDomainName] = evtString;
      _delegates[ProfilerEventColumns.HostName] = evtString;
      _delegates[ProfilerEventColumns.ClientProcessID] = evtInt;
      _delegates[ProfilerEventColumns.ApplicationName] = evtString;
      _delegates[ProfilerEventColumns.LoginName] = evtString;
      _delegates[ProfilerEventColumns.SPID] = evtInt;
      _delegates[ProfilerEventColumns.Duration] = evtLong;
      _delegates[ProfilerEventColumns.StartTime] = evtDateTime;
      _delegates[ProfilerEventColumns.EndTime] = evtDateTime;
      _delegates[ProfilerEventColumns.Reads] = evtLong;
      _delegates[ProfilerEventColumns.Writes] = evtLong;
      _delegates[ProfilerEventColumns.CPU] = evtInt;
      _delegates[ProfilerEventColumns.Permissions] = evtLong;
      _delegates[ProfilerEventColumns.Severity] = evtInt;
      _delegates[ProfilerEventColumns.EventSubClass] = evtInt;
      _delegates[ProfilerEventColumns.ObjectID] = evtInt;
      _delegates[ProfilerEventColumns.Success] = evtInt;
      _delegates[ProfilerEventColumns.IndexID] = evtInt;
      _delegates[ProfilerEventColumns.IntegerData] = evtInt;
      _delegates[ProfilerEventColumns.ServerName] = evtString;
      _delegates[ProfilerEventColumns.EventClass] = evtInt;
      _delegates[ProfilerEventColumns.ObjectType] = evtInt;
      _delegates[ProfilerEventColumns.NestLevel] = evtInt;
      _delegates[ProfilerEventColumns.State] = evtInt;
      _delegates[ProfilerEventColumns.Error] = evtInt;
      _delegates[ProfilerEventColumns.Mode] = evtInt;
      _delegates[ProfilerEventColumns.Handle] = evtInt;
      _delegates[ProfilerEventColumns.ObjectName] = evtString;
      _delegates[ProfilerEventColumns.DatabaseName] = evtString;
      _delegates[ProfilerEventColumns.FileName] = evtString;
      _delegates[ProfilerEventColumns.OwnerName] = evtString;
      _delegates[ProfilerEventColumns.RoleName] = evtString;
      _delegates[ProfilerEventColumns.TargetUserName] = evtString;
      _delegates[ProfilerEventColumns.DBUserName] = evtString;
      _delegates[ProfilerEventColumns.LoginSid] = evtByte;
      _delegates[ProfilerEventColumns.TargetLoginName] = evtString;
      _delegates[ProfilerEventColumns.TargetLoginSid] = evtByte;
      _delegates[ProfilerEventColumns.ColumnPermissions] = evtInt;
      _delegates[ProfilerEventColumns.LinkedServerName] = evtString;
      _delegates[ProfilerEventColumns.ProviderName] = evtString;
      _delegates[ProfilerEventColumns.MethodName] = evtString;
      _delegates[ProfilerEventColumns.RowCounts] = evtLong;
      _delegates[ProfilerEventColumns.RequestID] = evtInt;
      _delegates[ProfilerEventColumns.XactSequence] = evtLong;
      _delegates[ProfilerEventColumns.EventSequence] = evtLong;
      _delegates[ProfilerEventColumns.BigintData1] = evtLong;
      _delegates[ProfilerEventColumns.BigintData2] = evtLong;
      _delegates[ProfilerEventColumns.GUID] = evtGuid;
      _delegates[ProfilerEventColumns.IntegerData2] = evtInt;
      _delegates[ProfilerEventColumns.ObjectID2] = evtLong;
      _delegates[ProfilerEventColumns.Type] = evtInt;
      _delegates[ProfilerEventColumns.OwnerID] = evtInt;
      _delegates[ProfilerEventColumns.ParentName] = evtString;
      _delegates[ProfilerEventColumns.IsSystem] = evtInt;
      _delegates[ProfilerEventColumns.Offset] = evtInt;
      _delegates[ProfilerEventColumns.SourceDatabaseID] = evtInt;
      _delegates[ProfilerEventColumns.SqlHandle] = evtByte;
      _delegates[ProfilerEventColumns.SessionLoginName] = evtString;
      _delegates[ProfilerEventColumns.PlanHandle] = evtByte;
    }

    private void SetGuidColumn(ProfilerEvent evt, int columnid) {
      _reader.GetBytes(2, 0, _b16, 0, 16);
      evt.Values[columnid] = new Guid(_b16);
      evt.MarkSet(columnid);
    }

    private void SetDateTimeColumn(ProfilerEvent evt, int columnid) {
      //2 byte - year
      //2 byte - month
      //2 byte - ???
      //2 byte - day
      //2 byte - hour
      //2 byte - min
      //2 byte - sec
      //2 byte - msec
      _reader.GetBytes(2, 0, _b16, 0, 16);
      var year = _b16[0] | _b16[1] << 8;
      var month = _b16[2] | _b16[3] << 8;
      var day = _b16[6] | _b16[7] << 8;
      var hour = _b16[8] | _b16[9] << 8;
      var min = _b16[10] | _b16[11] << 8;
      var sec = _b16[12] | _b16[13] << 8;
      var msec = _b16[14] | _b16[15] << 8;
      try {
        evt.Values[columnid] = new DateTime(year, month, day, hour, min, sec, msec);
      }
      catch (ArgumentOutOfRangeException) {
        return;
      }

      evt.MarkSet(columnid);
    }

    private void SetByteColumn(ProfilerEvent evt, int columnid) {
      var b = new byte[(int) _reader[1]];
      evt.Values[columnid] = b;
      evt.MarkSet(columnid);
    }

    private void SetStringColumn(ProfilerEvent evt, int columnid) {
      evt.Values[columnid] = Encoding.Unicode.GetString((byte[]) _reader[2]);
      evt.MarkSet(columnid);
    }

    private void SetIntColumn(ProfilerEvent evt, int columnid) {
      _reader.GetBytes(2, 0, _b4, 0, 4);
      evt.Values[columnid] = ToInt32(_b4);
      evt.MarkSet(columnid);
    }

    private void SetLongColumn(ProfilerEvent evt, int columnid) {
      _reader.GetBytes(2, 0, _b8, 0, 8);
      evt.Values[columnid] = ToInt64(_b8);
      evt.MarkSet(columnid);
    }

    private static long ToInt64(byte[] value) {
      var i1 = (value[0]) | (value[1] << 8) | (value[2] << 16) | (value[3] << 24);
      var i2 = (value[4]) | (value[5] << 8) | (value[6] << 16) | (value[7] << 24);
      return (uint) i1 | ((long) i2 << 32);
    }

    private static int ToInt32(byte[] value) {
      return (value[0]) | (value[1] << 8) | (value[2] << 16) | (value[3] << 24);
    }

    private static int ToInt16(byte[] value) {
      return (value[0]) | (value[1] << 8);
    }

    public void SetEvent(int eventId, params int[] columns) {
      using var cmd = new SqlCommand
        {Connection = _conn, CommandText = "sp_trace_setevent", CommandType = CommandType.StoredProcedure};
      cmd.Parameters.Add("@traceid", SqlDbType.Int).Value = TraceId;
      cmd.Parameters.Add("@eventid", SqlDbType.Int).Value = eventId;
      var p = cmd.Parameters.Add("@columnid", SqlDbType.Int);
      cmd.Parameters.Add("@on", SqlDbType.Bit).Value = 1;
      foreach (var i in columns) {
        p.Value = i;
        cmd.ExecuteNonQuery();
      }
    }

    public void SetFilter(int columnId, int logicalOperator, int comparisonOperator, long? value) {
      using var cmd = new SqlCommand
        {Connection = _conn, CommandText = "sp_trace_setfilter", CommandType = CommandType.StoredProcedure};
      cmd.Parameters.Add("@traceid", SqlDbType.Int).Value = TraceId;
      cmd.Parameters.Add("@columnid", SqlDbType.Int).Value = columnId;
      cmd.Parameters.Add("@logical_operator", SqlDbType.Int).Value = logicalOperator;
      cmd.Parameters.Add("@comparison_operator", SqlDbType.Int).Value = comparisonOperator;
      if (value is null) {
        cmd.Parameters.Add("@value", SqlDbType.Int).Value = DBNull.Value;
      }
      else {
        switch (columnId) {
          case ProfilerEventColumns.BigintData1:
          case ProfilerEventColumns.BigintData2:
          case ProfilerEventColumns.Duration:
          case ProfilerEventColumns.EventSequence:
          case ProfilerEventColumns.ObjectID2:
          case ProfilerEventColumns.Permissions:
          case ProfilerEventColumns.Reads:
          case ProfilerEventColumns.RowCounts:
          case ProfilerEventColumns.TransactionID:
          case ProfilerEventColumns.Writes:
          case ProfilerEventColumns.XactSequence:
            cmd.Parameters.Add("@value", SqlDbType.BigInt).Value = value;
            break;
          case ProfilerEventColumns.ClientProcessID:
          case ProfilerEventColumns.ColumnPermissions:
          case ProfilerEventColumns.CPU:
          case ProfilerEventColumns.DatabaseID:
          case ProfilerEventColumns.Error:
          case ProfilerEventColumns.EventClass:
          case ProfilerEventColumns.EventSubClass:
          case ProfilerEventColumns.Handle:
          case ProfilerEventColumns.IndexID:
          case ProfilerEventColumns.IntegerData:
          case ProfilerEventColumns.IntegerData2:
          case ProfilerEventColumns.IsSystem:
          case ProfilerEventColumns.LineNumber:
          case ProfilerEventColumns.Mode:
          case ProfilerEventColumns.NestLevel:
          case ProfilerEventColumns.ObjectID:
          case ProfilerEventColumns.ObjectType:
          case ProfilerEventColumns.Offset:
          case ProfilerEventColumns.OwnerID:
          case ProfilerEventColumns.RequestID:
          case ProfilerEventColumns.Severity:
          case ProfilerEventColumns.SourceDatabaseID:
          case ProfilerEventColumns.SPID:
          case ProfilerEventColumns.State:
          case ProfilerEventColumns.Success:
          case ProfilerEventColumns.Type:
            cmd.Parameters.Add("@value", SqlDbType.Int).Value = value;
            break;
          default:
            throw new Exception(String.Format("Unsupported column_id: {0}", columnId));
        }
      }

      cmd.ExecuteNonQuery();
    }

    public void SetFilter(int columnId, int logicalOperator, int comparisonOperator, string value) {
      using var cmd = new SqlCommand
        {Connection = _conn, CommandText = "sp_trace_setfilter", CommandType = CommandType.StoredProcedure};
      cmd.Parameters.Add("@traceid", SqlDbType.Int).Value = TraceId;
      cmd.Parameters.Add("@columnid", SqlDbType.Int).Value = columnId;
      cmd.Parameters.Add("@logical_operator", SqlDbType.Int).Value = logicalOperator;
      cmd.Parameters.Add("@comparison_operator", SqlDbType.Int).Value = comparisonOperator;
      if (value is null) {
        cmd.Parameters.Add("@value", SqlDbType.NVarChar).Value = DBNull.Value;
      }
      else {
        cmd.Parameters.Add("@value", SqlDbType.NVarChar, Math.Max(value.Length, 1)).Value = value;
      }

      cmd.ExecuteNonQuery();
    }


    public void CreateTrace() {
      using var cmd = new SqlCommand
        {Connection = _conn, CommandText = "sp_trace_create", CommandType = CommandType.StoredProcedure};
      cmd.Parameters.Add("@traceid", SqlDbType.Int).Direction = ParameterDirection.Output;
      cmd.Parameters.Add("@options", SqlDbType.Int).Value = 1;
      cmd.Parameters.Add("@trace_file", SqlDbType.NVarChar, 245).Value = DBNull.Value;
      cmd.Parameters.Add("@maxfilesize", SqlDbType.BigInt).Value = DBNull.Value;
      cmd.Parameters.Add("@stoptime", SqlDbType.DateTime).Value = DBNull.Value;
      cmd.Parameters.Add("@filecount", SqlDbType.Int).Value = DBNull.Value;
      cmd.Parameters.Add("@result", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
      cmd.ExecuteNonQuery();
      var result = (int) cmd.Parameters["@result"].Value;
      TraceId = result != 0 ? -result : (int) cmd.Parameters["@traceid"].Value;
    }

    private void ControlTrace(SqlConnection con, int status) {
      using var cmd = new SqlCommand {
        Connection = con, CommandText = "sp_trace_setstatus", CommandType = CommandType.StoredProcedure,
        CommandTimeout = 0
      };
      cmd.Parameters.Add("@traceid", SqlDbType.Int).Value = TraceId;
      cmd.Parameters.Add("@status", SqlDbType.Int).Value = status;
      cmd.ExecuteNonQuery();
    }

    public void CloseTrace(SqlConnection con) {
      ControlTrace(con, 2);
    }

    public void StopTrace(SqlConnection con) {
      ControlTrace(con, 0);
    }

    public void StartTrace() {
      ControlTrace(_conn, 1);
      GetReader();
      Read();
    }

    private void GetReader() {
      var cmd = new SqlCommand {
        Connection = _conn, CommandText = "sp_trace_getdata", CommandType = CommandType.StoredProcedure,
        CommandTimeout = 0
      };
      cmd.Parameters.Add("@traceid", SqlDbType.Int).Value = TraceId;
      cmd.Parameters.Add("@records", SqlDbType.Int).Value = 0;
      _reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
    }

    public ProfilerEvent Next() {
      if (!TraceIsActive) return null;
      var columnId = (int) _reader[0];
      //skip to begin of new event
      while (columnId != 65526 && Read() && TraceIsActive) {
        columnId = (int) _reader[0];
      }

      //start of new event
      if (columnId != 65526) return null;
      if (!TraceIsActive) return null;
      //get potential event class
      _reader.GetBytes(2, 0, _b2, 0, 2);
      var eventClass = ToInt16(_b2);

      //we got new event
      if (eventClass >= 0 && eventClass < 255) {
        var evt = new ProfilerEvent();
        evt.Values[27] = eventClass;
        evt.MarkSet(27);
        while (Read()) {
          columnId = (int) _reader[0];
          if (columnId > 65) return evt;
          _delegates[columnId]?.Invoke(evt, columnId);
        }
      }

      Read();
      return null;
    }
  }
}