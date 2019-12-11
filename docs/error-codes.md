# Starcounter Error Codes

This article lists the error codes that Starcounter uses, along with their severity and descriptions of what they mean.

## Categories

- `0` -  Unspecified.
- `1000` -  General.
- `2000` -  StartupAndShutdown.
- `3000` -  IO.
- `4000` -  Database.
- `5000` -  Session.
- `6000` -  Internal.
- `7000` -  Sql.
- `8000` -  TransactionAbort.
- `10000` -  Management.
- `11000` -  Installer.
- `12000` -  Development.
- `13000` -  NetworkGateway.
- `14000` -  Apps.
- `15000` -  MetaTypes.

## Error Codes

Category | Code | Name | Description
-------- | ---- | ---- | -----------
0 | 999 | `ScErrUnspecified` | An unspecified error caused the operation to fail. 
1000 | 1001 | `ScErrBadArguments` | One or more arguments was invalid. 
1000 | 1002 | `ScErrCodeNotEnhanced` | Code not enhanced. 
1000 | 1003 | `ScErrOutOfMemory` | Operation failed because needed memory couldn't be allocated. 
1000 | 1004 | `ScErrThreadNotAttached` | The operation failed because the current thread wasn't attached to the scheduler. 
1000 | 1005 | `ScErrNotSupported` | The requested operation is not supported on the specific object. 
1000 | 1006 | `ScErrNotAWorkerThread` | The operation failed because the calling thread isn't a worker thread. 
1000 | 1007 | `ScErrThreadNotDetached` | The operation failed because the current thread was attached to the scheduler and detached thread was expected. 
1000 | 1008 | `ScErrThreadYieldBlocked` | The operation failed because it required the thread to yield but yield was blocked. 
1000 | 1009 | `ScErrCounterMismatch` | Generic error code for counter mismatch. 
1000 | 1010 | `ScErrCounterOverflow` | Generic error code for counter overflow. 
1000 | 1011 | `ScErrStackOverflow` | A stack overflow has been detected. 
1000 | 1012 | `ScErrWaitTimeout` | The wait timed out before the object was signaled. 
1000 | 1013 | `ScErrMaxThreadsReached` | The operation failed because the maximum number of worker threads has been reached and the operation required the scheduler to create additional worker threads. 
1000 | 1014 | `ScErrCantCreateUuid` | Failed to generate an UUID. 
1000 | 1016 | `ScErrMutexNotOwned` | Operation failed because of an attempt to release ownership of a mutex not owned by the current thread. 
1000 | 1017 | `ScErrWouldBlock` | A non blocking operation failed because if would be required to block. 
1000 | 1018 | `ScErrUnresposiveThreadStall` | A cooperatively scheduled worker thread has been unresponsive for an unacceptably long period of time. 
1000 | 1019 | `ScErrThreadsBlockedStall` | All threads available to the scheduler has been blocked for an unacceptably long period of time. 
1000 | 1020 | `ScErrTaskCleanupIncomplete` | A task still held resource when terminating that should have been released before the task terminated. 
1000 | 1021 | `ScErrUnyieldingThreadStall` | A cooperatively scheduled worker thread has been unwilling to yield for an unacceptably long period of time. 
1000 | 1022 | `ScErrThreadAutoDetached` | Use of reattach function for manually detached threads on auto detached thread. 
1000 | 1023 | `ScErrThreadNotAutoDetached` | Use of reattach function for auto detached threads on manually detached thread. 
1000 | 1024 | `ScErrDebugVerifyFailed` | Generic error code indicating that debug verification failed. 
1000 | 1025 | `ScErrInvalidOperation` | Operation invalid for the object's current state. 
1000 | 1026 | `ScErrInputQueueFull` | Operation failed because the target input queue was full. 
1000 | 1027 | `ScErrNotImplemented` | The method, operation or feature is not implemented. 
1000 | 1028 | `ScErrEnvVariableNotAccessible` | Access to an environment variable was not permitted due to the current security settings. 
1000 | 1029 | `ScErrBinDirEnvNotFound` | The environment variable holding the path to the Starcounter installation directory was not found. 
1000 | 1030 | `ScErrBadCommandLineFormat` | The format of the command-line was invalid. 
1000 | 1031 | `ScErrBadCommandLineSyntax` | Command-line arguments didn't match the defined syntax. 
1000 | 1032 | `ScErrBadPostSharpLicense` | The license key used to initialize the PostSharp library was invalid. 
1000 | 1033 | `ScErrWrongErrorMessageFormat` | Parsing an error message string failed due to an incorrect message format. 
1000 | 1034 | `ScErrOperationCancelled` | The operation was cancelled. 
1000 | 1035 | `ScErrSharedInterfaceOpenDbShm` | Opening the managed segment with the segment_name failed. Invalid segment or does not exist. 
1000 | 1036 | `ScErrFindChunks` | Could not find the chunks shared memory object. 
1000 | 1037 | `ScErrFindSharedChunkPool` | Could not find the shared_chunk_pool shared memory object. 
1000 | 1038 | `ScErrFindCommonSchedInterface` | Could not find the common_scheduler_interface shared memory object. 
1000 | 1039 | `ScErrFindSchedulerInterfaces` | Could not find the scheduler_interfaces shared memory object. 
1000 | 1040 | `ScErrFindCommonClientInterface` | Could not find the common_client_interface shared memory object. 
1000 | 1041 | `ScErrFindClientInterfaces` | Could not find the client_interfaces shared memory object. 
1000 | 1042 | `ScErrFindChannels` | Could not find the channels shared memory object. 
1000 | 1043 | `ScErrOpenSchedulerWorkEvent` | Failed to open a scheduler work event. 
1000 | 1044 | `ScErrOpenClientWorkEvent` | Failed to open a client work event. 
1000 | 1045 | `ScErrOpenSchedNumPoolNotEmptyEv` | Failed to open a scheduler number pool not empty event. 
1000 | 1046 | `ScErrOpenSchedNumPoolNotFullEv` | Failed to open a scheduler number pool not full event. 
1000 | 1047 | `ScErrFormatActiveDbUpdatedEvName` | Failed to format the active databases updated event name. 
1000 | 1048 | `ScErrConvertActiveDbUpdatedEvMBS` | Failed to convert active databases updated event name to multi-byte string. 
1000 | 1049 | `ScErrOpenActiveDbUpdatedEv` | Failed to open the active databases updated event. 
1000 | 1050 | `ScErrIPCMonitorEraseDatabaseName` | The IPC monitor failed to erase a database name. 
1000 | 1051 | `ScErrBoostInterprocessException` | The IPC monitor trying to open segment boost::interprocess::interprocess_exception caught. 
1000 | 1052 | `ScErrTryOpenSegmentUnknownExcept` | The IPC monitor trying to open segment unknown exception caught. 
1000 | 1053 | `ScErrGotClientProcessTypeNotDb` | The IPC monitor expected database process but got client process type. 
1000 | 1054 | `ScErrGotUnknownProcessTypeNotDb` | The IPC monitor expected database process but got unknown process type. 
1000 | 1055 | `ScErrOpenProcessFailedRegDbProc` | The IPC monitor is trying to register a database process but OpenProcess() failed. 
1000 | 1056 | `ScErrIPCMonInsertDbNameActiveDbR` | The IPC monitor failed to insert database name into active databases register. 
1000 | 1057 | `ScErrIPCMonitorInsertSegmentName` | The IPC monitor failed to insert segment name into the monitor interface. 
1000 | 1058 | `ScErrIPCMonFailedNotifyScheduler` | The IPC monitor failed to notify the scheduler on the given channel. 
1000 | 1059 | `ScErrOpenDbShmSegToNotifyClients` | The IPC monitor failed to open the database shared memory segment and is unable to notify the clients. 
1000 | 1060 | `ScErrIPCMonOpenDbShmSegUnknownEx` | The IPC monitor caught an unknown exception while trying to open database IPC shared memory segments. 
1000 | 1061 | `ScErrIPCMonUnknownProessTypeExit` | The IPC monitor got an unknown proess type exit. 
1000 | 1062 | `ScErrOpenProcessFailedRegCliProc` | The IPC monitor is trying to register a client process but OpenProcess() failed. 
1000 | 1063 | `ScErrTheOwnerIDMatchButNotThePID` | The IPC monitor unregistration failed. The owner ID matches but not the PID. 
1000 | 1064 | `ScErrThePIDMatchButNotTheOwnerID` | The IPC monitor unregistration failed. The PID matches but not the owner ID. 
1000 | 1065 | `ScErrCleanupToOpenTheDBIPCShmSeg` | The IPC monitor cleanup failed. Could not open the IPC database shared memory segment. 
1000 | 1066 | `ScErrIPCMonitorCleanupWaitFailed` | The IPC monitor was not notified. Wait failed. 
1000 | 1067 | `ScErrOutOfDedicatedAddressSpace` | Operation failed because needed memory couldn't be allocated within dedicated address space 
1000 | 1068 | `ScErrMemoryLimitReached` | Operation failed because limit reached on limited memory resource. 
1000 | 1069 | `ScErrUnhandledException` | An unhandled exception was detected. 
1000 | 1070 | `ScErrUnexpectedInternalError` | An unexpected internal error was detected. 
1000 | 1071 | `ScErrTestAssertionFailure` | Assertion used in a test application failed. 
1000 | 1072 | `ScErrOperationPending` | Operation completion pending. 
2000 | 2001 | `ScErrAppAlreadyStarted` | A Starcounter process with the specified name is already started. 
2000 | 2002 | `ScErrBadBaseDirectoryConfig` | Base directory (baseDirectory) missing from config or invalid. 
2000 | 2003 | `ScErrBadTempDirectoryConfig` | Temp directory (tempDirectory) missing from config or invalid. 
2000 | 2004 | `ScErrBadImageDirectoryConfig` | Image directory (imageDirectory) missing from config or invalid. 
2000 | 2005 | `ScErrBadTLogDirectoryConfig` | The transaction log directory (transactionLogDirectory) is missing from config or invalid. 
2000 | 2012 | `ScErrCantInitializeClr` | Unable to initialize the CLR. 
2000 | 2013 | `ScErrCantLoadHostEnvironment` | Unable to load host environment. 
2000 | 2014 | `ScErrLogModuleNotFound` | The operation failed because sccorelog.dll or one of the functions it exports couldn't be loaded. 
2000 | 2015 | `ScErrImageStorageMismatch` | The properties of the different disk drives used to store the image files don't have compatible configuration (do they have the same sector sizes?). 
2000 | 2016 | `ScErrImageStorageIncompatible` | The properties of the disk drive doesn't match the database image (is the disk drive is configured for a different sector size than the image is built for?). 
2000 | 2017 | `ScErrWrongDatabaseVersion` | The database files are build for a different Starcounter version and are not compatible with the current version. 
2000 | 2018 | `ScErrCantGetPrivilegesNeeded` | Could not acquire the privileges needed for the operation to complete successfully. 
2000 | 2022 | `ScErrDatabaseFileMismatch` | Attempt to load database files from different databases was detected. 
2000 | 2023 | `ScErrDatabaseLangUnsupported` | One or more of the languages the database is configured for isn't supported by the current system. 
2000 | 2027 | `ScErrCodeLoaderError` | Code loder error. 
2000 | 2029 | `ScErrTypesLoadError` | Error inspecting assembly. 
2000 | 2030 | `ScErrWeavingError` | At least one error has been detected during weaving of the application code. 
2000 | 2031 | `ScErrInvalidServerName` | The server name is too long (max 31 chars) or contains invalid characters. 
2000 | 2032 | `ScErrShutdownTimedOut` | Shutdown timed out: Stuck in user code. 
2000 | 2035 | `ScErrBadSchedCountConfig` | Too many virtual CPUs configured. 
2000 | 2036 | `ScErrWrongTLogVersion` | Wrong transaction log version. 
2000 | 2037 | `ScErrBadRebuildConfig` | Bad unload/reload configuration. 
2000 | 2043 | `ScErrBadTLogSizeConfig` | The specified transaction log size is either smaller then the minimum size, larger then the maximum size or not aligned with the disk sector size. 
2000 | 2044 | `ScErrBadDiskSectorSizeConfig` | The specified disk sector size is not compatible with storage. 
2000 | 2045 | `ScErrShrinkImageNotSupported` | The specified new image size is smaller then the current image size. Making the image smaller is not allowed. 
2000 | 2046 | `ScErrBadImageSizeConfig` | The specified image size is either smaller then the minimum size, larger then the maximum size or not aligned with the database block size. 
2000 | 2047 | `ScErrDatabaseAlreadyExists` | A database with the specified name already exists. 
2000 | 2048 | `ScErrModuleNotFound` | The operation failed because an expected DLL module couldn't be loaded. 
2000 | 2049 | `ScErrParsingAppArgs` | When parsing the application arguments to scdbs.exe, the system routine parsing it reported an error. 
2000 | 2050 | `ScErrTooFewAppArgs` | The number of arguments passed to scdbs.exe did not include all arguments required. 
2000 | 2051 | `ScErrAppArgsSemanticViolation` | All arguments passed to scdbs.exe did not confirm to the constraints, or some of them were found incompatible. 
2000 | 2052 | `ScErrUnloadEmptyDatabase` | The -unload directive specified as a server application argument can not be used when the database is empty and not initialized. 
2000 | 2053 | `ScErrRldPopulatedDatabase` | The -reload directive specified as a server application argument can not be used when the database is allready populated. 
2000 | 2054 | `ScErrFuncExportNotFound` | The operation failed because an expected function export from a module dependent upon was not found. 
2000 | 2055 | `ScErrBadBackupDirectoryConfig` | Bad backup directory config. Either the configured directory does not exist or the directory path is to long. 
2000 | 2056 | `ScErrBadLogDirectoryConfig` | Bad log directory config. Either the configuration does not exist or the configured directory isn't valid. 
2000 | 2057 | `ScErrCompareEmptyDatabase` | The server was started, instructed to execute a compare of application and core schemas, and when the database was examined, the server detected it to not being bound to any previous application, i.e. containing no schema. 
2000 | 2058 | `ScErrTLogNotInitialized` | An operation failed due to the transaction/redo log not being initialized. 
2000 | 2059 | `ScErrAcquireDbControlTimeOut` | Attempt to acquire control of the database environment timed out. 
2000 | 2060 | `ScErrCreateActMonBufTimeOut` | Attempt to create activity monitor shared buffer timed out. 
2000 | 2061 | `ScErrDbStateAccessDenied` | Allocating shared memory for database state failed. Access was denied. 
2000 | 2063 | `ScErrBadImageFileCksumLoad` | Checksum mismatch detected in image file when image file was loaded into memory. 
2000 | 2064 | `ScErrClientBackendAlreadyLoaded` | Client application communication library was already loaded. 
2000 | 2065 | `ScErrClientBackendNotLoaded` | Client application communication library failed to load. 
2000 | 2066 | `ScErrClientBackendWrongPath` | Client application communication library was loaded from the wrong path. 
2000 | 2067 | `ScErrStartSharedMemoryMonitor` | Problems starting shared memory monitor process. 
2000 | 2068 | `ScErrWeaverProjectFileNotFound` | A project file the weaver engine depends on could not be found. Either it was deleted or the program hosting the weaver did not resolve its path to the correct installation directory. 
2000 | 2069 | `ScErrConstructDbShmParamName` | The database intends to open the database shared memory parameters, but failed when trying to concatenate the database name prefix with the database name. 
2000 | 2070 | `ScErrDbDataDirPathInvalidMBChar` | The database intends to open the shared memory object with the database shared memory parameters, but failed because of an illegal char in the db_data_dir_path in the psetup parameter. 
2000 | 2071 | `ScErrDbShmParamNameInvalidWChar` | The database intends to open the shared memory object with the database shared memory parameters, but failed because a wide character that cannot be converted into a multibyte character was encountered. 
2000 | 2075 | `ScErrConstructSegmentNameSeqNo` | Failed to construct the segment name with sequence number. 
2000 | 2076 | `ScErrSegmentNameInvalidWChar` | The database intends to get the sequence number and append it to the segment name, but failed because a wide character that cannot be converted into a multibyte character was encountered. 
2000 | 2077 | `ScErrConstrMonitorInterfaceName` | The database intends to register with the monitor, but it failed because constructing the monitor interface name would result in buffer overflow. 
2000 | 2078 | `ScErrDbMapMonitorInterfaceInShm` | The database intends to register with the monitor, but failed to map the monitor interface in shared memory. 
2000 | 2080 | `ScErrDBAcquireOwnerID` | The database tried to register with the monitor, but failed to acquire an owner ID. 
2000 | 2081 | `ScErrCConstructDbShmParamName` | The client intends to open the database shared memory parameters, but failed when trying to concatenate the database name prefix with the database name. 
2000 | 2082 | `ScErrCConstrMonitorInterfaceName` | The client intends to register with the monitor, but failed because constructing the monitor interface name would result in buffer overflow. 
2000 | 2083 | `ScErrCAcquireOwnerID` | The client tried to register with the monitor, but failed to acquire an owner ID. 
2000 | 2084 | `ScErrClientOpenDbShmSegment` | The client intends to open the database shared memory segment, but failed because it is not initialized yet. 
2000 | 2085 | `ScErrCConstrDbShmSegmentName` | The client intends to open the database shared memory segment, but failed because constructing the name of the database shared memory segment would result in buffer overflow. 
2000 | 2086 | `ScErrClientAcquireClientNumber` | The client failed to initialize because it could not acquire a client number. 
2000 | 2087 | `ScErrUnknownExceptThrownInSetup` | An unknown exception was thrown by the database when trying something in the call to setup during the initialization phase. 
2000 | 2088 | `ScErrDbOpenMonitorInterface` | The database intends to register with the monitor, but it failed to open the monitor interface. 
2000 | 2089 | `ScErrDbCreateDbShmParameters` | The database failed to create the database shared memory parameters file. 
2000 | 2090 | `ScErrCreateDbShmParameters` | Failed to create the database shared memory parameters file. 
2000 | 2091 | `ScErrOpenDbShmParameters` | Failed to open the database shared memory parameters file. 
2000 | 2092 | `ScErrMapDbShmParametersInShm` | Failed to map the database shared memory parameters in shared memory. 
2000 | 2094 | `ScErrOpenMonitorInterface` | Failed to open monitor interface. 
2000 | 2095 | `ScErrMapMonitorInterfaceInShm` | Failed to map monitor interface in shared memory. 
2000 | 2096 | `ScErrDbMapDbShmParametersInShm` | The database failed to map the database shared memory parameters in shared memory. 
2000 | 2097 | `ScErrCOpenDbShmParameters` | The client failed to open the database shared memory parameters file. 
2000 | 2098 | `ScErrCMapDbShmParametersInShm` | The client failed to map the database shared memory parameters in shared memory. 
2000 | 2099 | `ScErrDbOpenDebugFile` | The database failed to open the debug file. 
2000 | 2100 | `ScErrDbReleaseOwnerID` | The database tried to unregister with the monitor, but failed to release its owner ID. 
2000 | 2101 | `ScErrCReleaseOwnerID` | The client tried to unregister with the monitor, but failed to release its owner ID. 
2000 | 2102 | `ScErrDbAcquireOwnerIDTimeout` | The database tried to register with the monitor, but failed to acquire an owner ID because a timeout occurred. 
2000 | 2103 | `ScErrCAcquireOwnerIDTimeout` | The client tried to register with the monitor, but failed to acquire an owner ID because a timeout occurred. 
2000 | 2104 | `ScErrDbReleaseOwnerIDTimeout` | The database tried to unregister with the monitor, but failed to release its owner ID because a timeout occurred. 
2000 | 2105 | `ScErrCReleaseOwnerIDTimeout` | The client tried to unregister with the monitor, but failed to release its owner ID because a timeout occurred. 
2000 | 2106 | `ScErrBadSchedIdSupplied` | Trying to queue a job on scheduler with incorrect id. 
2000 | 2107 | `ScErrCAcquireOwnerIDTimeout2` | The client tried to register with the monitor, but failed to acquire an owner ID because a timeout occurred. 
2000 | 2108 | `ScErrDbAlreadyStarted` | A Starcounter database with the specified name is already started. 
2000 | 2109 | `ScErrBadDatabaseConfig` | Invalid configuration prevents the database from being accessible. 
2000 | 2110 | `ScErrStartNetworkGateway` | Problems starting network gateway process. 
2000 | 2111 | `ScErrBadChunksNumberConfig` | Shared memory chunks number is missing in database configuration file or has invalid value. 
2000 | 2112 | `ScErrTLogStorageIncompatible` | The properties of the disk drive doesn't match the transaction log (is the disk drive is configured for a different sector size than the transaction log is built for?). 
2000 | 2113 | `ScErrBadServiceConfig` | Service configuration is invalid. 
2000 | 2114 | `ScErrKilledByProcessManager` | Process was killed by process manager. 
2000 | 2115 | `ScErrBadServerConfig` | Server configuration is invalid. 
2000 | 2116 | `ScErrInvalidGlobalSegmentShmObj` | Invalid Global Segment Shared Memory Object. 
2000 | 2117 | `ScErrServerInitUnknownException` | Unknown exception thrown during initialization of server. 
2000 | 2118 | `ScErrServerPortInitInvalidShmObj` | Server port initialization failed, invalid shared memory object. 
2000 | 2119 | `ScErrServerPortInitInvalidMapReg` | Server port initialization failed, invalid mapped region. 
2000 | 2120 | `ScErrServerPortUnknownException` | Server port initialization failed, unknown exception caught. 
2000 | 2121 | `ScErrCantCreateIPCMonitorDir` | IPC monitor failed to create IPC monitor directory. 
2000 | 2122 | `ScErrIPCMonitorRequiredArguments` | IPC monitor failed to start, required arguments are missing. 
2000 | 2123 | `ScErrFormatIPCMonitorCleanupEv` | IPC monitor failed to format the ipc_monitor_cleanup_event_name. 
2000 | 2124 | `ScErrConvertIPCMonCleanupEvMBS` | IPC monitor failed to convert the IPC monitor cleanup event name to multi-byte string. 
2000 | 2125 | `ScErrCreateIPCMonitorCleanupEv` | IPC monitor failed to create the IPC monitor cleanup event. 
2000 | 2126 | `ScErrIPCMFormatActiveDbUpdatedEv` | IPC monitor failed to format the active databases updated event name. 
2000 | 2127 | `ScErrIPCMConvActiveDbUpdatedEvMB` | IPC monitor failed to convert active databases updated event name to multi-byte string. 
2000 | 2128 | `ScErrCreateActiveDbUpdatedEv` | IPC monitor failed to failed to create the active databases updated event. 
2000 | 2129 | `ScErrInvalidIPCMonInterfacShmObj` | IPC monitor invalid monitor_interface shared memory object. 
2000 | 2130 | `ScErrInvalidIPCMonInterfacMapReg` | IPC monitor invalid monitor_interface mapped_region. 
2000 | 2131 | `ScErrIPCMonitorOpenProcessToken` | IPC monitor OpenProcessToken() failed. 
2000 | 2132 | `ScErrIPCMonLookupPrivilegeValue` | IPC monitor LookupPrivilegeValue() failed. 
2000 | 2133 | `ScErrIPCMonAdjustTokenPrivileges` | IPC monitor AdjustTokenPrivileges() failed. 
2000 | 2134 | `ScErrIPCMonSetSeDebugPrivilege` | IPC monitor failed to set SeDebugPrivilege. 
2000 | 2135 | `ScErrIPCMonitorCreateActiveDbDir` | IPC monitor can't create monitor active databases directory. 
2000 | 2136 | `ScErrIPCMonitorDelActiveDbFile` | IPC monitor can't delete monitor active databases file. 
2000 | 2137 | `ScErrBoostIPCExecption` | Boost interprocess exception thrown in client_port. 
2000 | 2138 | `ScErrClientPortUnknownException` | Unknown exception thrown in client_port. 
2000 | 2139 | `ScErrIPCMonitorUnknownException` | Unknown exception thrown in IPC monitor. 
2000 | 2140 | `ScErrWeaverFailedLoadFile` | The weaver failed to load a binary user code file. 
2000 | 2141 | `ScErrScServiceIsAlreadyRunning` | An instance of scservice is already running in the same session. 
2000 | 2142 | `ScErrScServiceFailedCreateLck` | scservice tried to create a mutex named scservice_is_running_lock but CreateMutex() returned NULL. 
2000 | 2143 | `ScErrWeaverFailedStrongNameAsm` | A locally deployed strong-named assembly was detected. 
2000 | 2144 | `ScErrBadGatewayWorkersNumberConfig` | Gateway workers number is missing in configuration file or has invalid value. 
2000 | 2145 | `ScErrBadSessionsDefaultTimeout` | Sessions default timeout is missing in configuration file or has invalid value. 
2000 | 2146 | `ScErrBadApplicationName` | Application name can only contain letters, numbers and underscores. 
2000 | 2147 | `ScErrWeaverFailedResolveReference` | The weaver failed to resolve a reference to an assembly. 
2000 | 2148 | `ScErrBadInternalSystemPort` | Internal system port is missing in configuration file or has invalid value. 
2000 | 2149 | `ScErrCantBindAppWithPrivateData` | The application can not start because it contains at least one database class with private data. See separate errors with ID "ScErrNonPublicFieldNotExposed". 
2000 | 2150 | `ScErrInvokeApplicationHost` | Failed to create/invoke the application host. See IApplicationHost documentation on its constraints. 
2000 | 2151 | `ScErrApplicationAlreadyHosted` | Unable to start the application host because the host is already running. 
2000 | 2152 | `ScErrAppWithoutSchema` | The application assembly did not contain expected resource stream with schema. 
3000 | 3001 | `ScErrInvalidHandle` | The specified handle is invalid. 
3000 | 3002 | `ScErrNoUserAdapterReferences` | Attempt to release a user reference to an adapter failed because no user references was registered. 
3000 | 3003 | `ScErrBadChannelOptionValue` | Operation to set an option failed because the value of an option couldn't be set to the specified value. 
3000 | 3006 | `ScErrCantLockMemory` | The operation couldn't be completed because memory couldn't be locked. 
3000 | 3010 | `ScErrChannelClosed` | Operation couldn't be completed because the channel has been closed. 
3000 | 3011 | `ScErrChannelReading` | Operation failed because an read has been posted and pending completion. 
3000 | 3012 | `ScErrConnectTimedOut` | Connection timed out because the remote party didn't respond within an acceptable abount of time. 
3000 | 3013 | `ScErrConnectionRefused` | A connection to a remote endpoint could not be established because the target system actively refused it. 
3000 | 3014 | `ScErrEndPointInvalid` | Tried to bind to an invalid endpoint 
3000 | 3015 | `ScErrEndPointOccupied` | The endpoint is occupied 
3000 | 3016 | `ScErrEndPointUnreachable` | The endpoint couldn't be reached. 
3000 | 3018 | `ScErrListenerClosed` | Operation couldn't be completed because the listener has been closed. 
3000 | 3019 | `ScErrReadBufferExceeded` | Channel read operation failed because the message received was larger than allowed. 
3000 | 3020 | `ScErrToManyAdapterReferences` | Operation could not be completed because there the limit of references to the accessed I/O adapter has been reached. 
3000 | 3021 | `ScErrUnwritableMessage` | Operation couldn't be completed because the contents of the message couldn't be sent through a channel. 
3000 | 3022 | `ScErrLogsConnectionClosed` | Connection to log server closed. 
3000 | 3023 | `ScErrUnknownChannelOption` | Operation failed because no option with the specified name was availible. 
3000 | 3024 | `ScErrUnreadableMessage` | Received a malformed message 
3000 | 3025 | `ScErrObjectNotOwned` | An operation failed because the object that only allows access by the owning thread wasn't owned by the current thread. 
3000 | 3026 | `ScErrCantCreateProcessShare` | The memory file used to share server information already existed and the process could therefore not claim ownership of the object. 
3000 | 3027 | `ScErrCantOpenProcessShare` | The memory file used to share server information could not be opened. Most likely an indication that the server with the specified name isn't online. 
3000 | 3029 | `ScErrCantCreateAdapter` | Operation could not complete because of failure to create an I/O adapter. Occurs when the maximum number of I/O adapters has been reached. 
3000 | 3031 | `ScErrNotEnoughDiskSpace` | Operation failed due to insufficient disk space. 
3000 | 3032 | `ScErrBadProcessShareVersion` | The memory file used to share server information was of an incompatible version. 
3000 | 3033 | `ScErrCTimeoutPushRequestMessage` | A timeout occurred because the client could not push a request message to the channels in queue, because it is full. The database have not popped any request message from the channel within the timeout period. 
3000 | 3034 | `ScErrCTimeoutPopResponseMessage` | A timeout occurred because the client could not pop a response message from the channels out queue, because it is empty. The database have not pushed any response message to the channel within the timeout period. 
3000 | 3035 | `ScErrDatabaseIsNotLoaded` | Database connect failed because the database was not loaded. 
3000 | 3036 | `ScErrAcquireLinkedChunks` | Not enough free shared memory. Enough memory chunks could not be acquired to hold the whole message. 
3000 | 3037 | `ScErrIncorrectNetworkProtocolUsage` | Current network protocol is not used correctly. Please consult documentation for more details. 
3000 | 3038 | `ScErrCantPushToChannel` | Channel is full, can't push. 
4000 | 4002 | `ScErrStringHeapOutOfSpace` | No more space in string heap. 
4000 | 4003 | `ScErrNoTransactionAttached` | Operation must be performed in the context of a valid transaction 
4000 | 4008 | `ScErrNotAnExtensionOfType` | The specified extension is not an extension of the specific type. 
4000 | 4009 | `ScErrCantCreateSchema` | Unable to create database schema 
4000 | 4011 | `ScErrNotProperRelationAttr` | Indicates that an relation property wasn't properly declared. 
4000 | 4012 | `ScErrUnsupportedAttributeType` | The data type is not supported. The data type of a field or property in an Entity class is not supported by the database. 
4000 | 4013 | `ScErrToManyAttributes` | A type had to many attributes. 
4000 | 4016 | `ScErrDeletePending` | The operation failed because a delete is pending (issued but not finalized). 
4000 | 4019 | `ScErrObjectNotAType` | Instantiation of an object failed because the object wasn't a registered type. 
4000 | 4020 | `ScErrReadOnlyDatabase` | The database is read-only. 
4000 | 4022 | `ScErrNoTransRefToRelease` | An attempt to release an reference that wasn't registered was intercepted. 
4000 | 4024 | `ScErrToManyTransactionRefs` | Too many references registered by the transaction. 
4000 | 4025 | `ScErrObjectIllegalDurability` | A change in the durability of an object was requested, but the object was in a state where the durability couldn't be changed. 
4000 | 4028 | `ScErrCantOpenImageFile` | One of database the image files of the database could not be opened. 
4000 | 4029 | `ScErrCantReadImageFile` | An error occured when attempting to read from an image file. 
4000 | 4030 | `ScErrCantWriteImageFile` | An error occured when attempting to write to an image file. 
4000 | 4031 | `ScErrTransactionLockedOnThread` | Can't modify the state of the transaction. 
4000 | 4033 | `ScErrNotProperCommitHookType` | The commit hook type is properly declared (doesn't adhere to the constraints of commit hook types). 
4000 | 4034 | `ScErrErrorInHookCallback` | The operation failed because an error was detected in a hook callback function. 
4000 | 4035 | `ScErrIteratorNotOwned` | A cursor operation failed because the cursor wasn't owned by the current thread and transaction. 
4000 | 4036 | `ScErrPluginCodeViolation` | A violation in plugin code was detected. 
4000 | 4037 | `ScErrPluginMissingDefaultCtor` | A plugin type lacks a default constructor. 
4000 | 4038 | `ScErrPluginInvocationException` | An exception was not handled by a plugin callback or a plugin constructor. 
4000 | 4039 | `ScErrInvalidHookTarget` | The target declared foor a commit hooks is not valid hook. 
4000 | 4040 | `ScErrTransactionAlreadyBound` | The transaction is already attached to another thread. 
4000 | 4041 | `ScErrWeavingFailed` | General weaver error. 
4000 | 4042 | `ScErrMissingDefinition` | An entity class in the deployed application was not part of the previously created core database schema. 
4000 | 4043 | `ScErrRefForbiddenUserCode` | User code referenced a type, field or method decorated with the HideFromApplications attribute 
4000 | 4044 | `ScErrIllegalAttributeAssign` | The field may not be explicitly assigned. 
4000 | 4046 | `ScErrDbClassCantBeGeneric` | A database class was declared as a generic, which is not allowed. 
4000 | 4047 | `ScErrIllegalFinalizer` | A database class contained a finalizer, something currently not supported. 
4000 | 4048 | `ScErrIllegalTypeRefDecl` | A database class contained a field declaration where the field was named __typeRef. 
4000 | 4049 | `ScErrToComplexCtor` | The instance initializer was too complex. Assign values inside the instance constructor instead. 
4000 | 4050 | `ScErrFieldRedeclaration` | A database class declared a persistent field that has already been declared in one of the parent classes. 
4000 | 4051 | `ScErrIllegalExtCtor` | Extension classes may only have default constructors. 
4000 | 4052 | `ScErrIllegalExtCreation` | May not explicitly create instances of extension classes. 
4000 | 4053 | `ScErrIllegalExtCtorBody` | The constructor in an extension class contained user code. 
4000 | 4054 | `ScErrExtNotSealed` | An extension class that wasn't declared as "sealed" was found. 
4000 | 4055 | `ScErrKindWrongName` | A Kind class named something other than "Kind" was found. 
4000 | 4056 | `ScErrKindMissingConcept` | A Kind class missing an enclosing concept class was found. 
4000 | 4057 | `ScErrKindIllegalParent` | A Kind class was derived from an inappropriate base class. 
4000 | 4058 | `ScErrKindMissingCtor` | A Kind class missing a default constructor was found. 
4000 | 4059 | `ScErrKindMissingParent` | Classes named "Kind" declared within Society Object concept classes must inherit its parent Kind. 
4000 | 4060 | `ScErrKindWrongVisibility` | Kind classes must at least have the "protected" visibility. 
4000 | 4061 | `ScErrFieldComplexInit` | Too complex field initializer in entity class. Initialize it in the constructor instead. 
4000 | 4062 | `ScErrFieldRefMethod` | Starcounter can't handle the reference parameters in a certain method. 
4000 | 4066 | `ScErrSynNoTarget` | A field was declared as a synonym to a non-existent field. 
4000 | 4067 | `ScErrSynTypeMismatch` | Mismatch between a synonym's type and the synonym's target's type. 
4000 | 4068 | `ScErrSynVisibilityMismatch` | A synonym was found that was more visible than its target. 
4000 | 4069 | `ScErrSynReadOnlyMismatch` | A non-readonly synonym to a readonly field was found. 
4000 | 4070 | `ScErrSynTargetNotPersistent` | A synonym targeted a non-persistent field. 
4000 | 4071 | `ScErrSynPrivateTarget` | Can't declare synonyms to private fields. 
4000 | 4077 | `ScErrPersPropNoTarget` | Field not found. 
4000 | 4078 | `ScErrTypeNameDuplicate` | Two classes of the same full name were found. 
4000 | 4083 | `ScErrObjectDoesntExist` | A transaction tried to access an object that has been deleted (or which for some reason doesn't exist). 
4000 | 4084 | `ScErrTransactionNotOwned` | A transaction operation failed because the transaction wasn't owned by the current virtual processor. 
4000 | 4085 | `ScErrAttrNoKernelField` | An entity class in the deployed application declared a field not part of the previously created core database schema. 
4000 | 4086 | `ScErrUnloadFailed` | The unload routine failed to execute. 
4000 | 4087 | `ScErrReloadFailed` | The database reload routine failed to execute. 
4000 | 4089 | `ScErrTransactionScopeOwned` | Context not accessible because it's owned by a scope. 
4000 | 4090 | `ScErrUnhandledTransactConflict` | The operation failed because of an unhandled transaction conflict. 
4000 | 4091 | `ScErrBinaryValueExceedsMaxSize` | Binary data exceeds maximum size. 
4000 | 4093 | `ScErrReadOnlyTransaction` | The transaction is readonly and cannot be changed to write-mode. 
4000 | 4099 | `ScErrRenameMissingSource` | A renamed token (class/field) specifies a previous name not part of the old schema. 
4000 | 4101 | `ScErrMemoryManagerIsDead` | The memory manager has terminated unexpectedly. 
4000 | 4103 | `ScErrNotTheSameDatabase` | When reloading the database; the memory manager has detected that the image files represents another database the the one in memory, or another generation of it. 
4000 | 4104 | `ScErrCantWriteTransactionLog` | An error occured when writing to the redo log. 
4000 | 4105 | `ScErrCantOpenDumpFile` | Reload failed because the process was unable to open the dump file. 
4000 | 4106 | `ScErrCantOpenTransactionLog` | Unable to open transaction log. 
4000 | 4107 | `ScErrCantExpandImageFile` | Could not expand image file. 
4000 | 4108 | `ScErrCantCreateImageFile` | Could not create image file. 
4000 | 4109 | `ScErrCantCreateTransactionLog` | Could not create transaction log. 
4000 | 4111 | `ScErrUnloadFailedDumpExists` | The unload routine failed to execute because a dump with the specified name already exists. 
4000 | 4112 | `ScErrInvalidReloadInstructSet` | When initializing the reload, the reload instruction set unexpectedly was proved invalid. 
4000 | 4113 | `ScErrReloadBugV1NullableAI63` | When rebuilding (dump ver 1), a known and un-recoverable error was detected. Please contact Starcounter for guidance. 
4000 | 4114 | `ScErrReloadUnsupFieldConversion` | Unsupported field conversion during rebuild. 
4000 | 4115 | `ScErrReloadConversionOverflow` | Conversion overflow during rebuild. 
4000 | 4116 | `ScErrDumpVersionNotSupported` | When rebuilding, a version of the dump was not supported. 
4000 | 4117 | `ScErrCantFindImageFile` | One or both of the image files are missing. 
4000 | 4118 | `ScErrCantAccessImageFile` | One or both of the image files aren't accessible by the process. 
4000 | 4119 | `ScErrImageFileLocked` | One or both of the image files are locked by another process. 
4000 | 4120 | `ScErrCantFindTransactionLog` | The transaction log is missing. 
4000 | 4121 | `ScErrCantAccessTransactionLog` | The transaction log isn't accessible by the process. 
4000 | 4122 | `ScErrTransactionLogLocked` | The transaction log is locked by another process. 
4000 | 4123 | `ScErrCantAccessDumpFile` | Reload failed because the dump file weren't accessible by the process. 
4000 | 4124 | `ScErrDumpFileLocked` | Reload failed because the dump file was locked by another process. 
4000 | 4125 | `ScErrReloadConstraintViolation` | The reload completed successfully but reload data violated one or more constraints. 
4000 | 4126 | `ScErrFileTransNotSupported` | File transactions cannot be used on this platform version. 
4000 | 4127 | `ScErrTypeBaseDeviation` | An entity type declared a base class that did not match the base class previously bound to the core database schema. 
4000 | 4128 | `ScErrFieldSignatureDeviation` | A database type declared a persistent field that did not match the signature of the attribute in the core schema. 
4000 | 4129 | `ScErrSchemaDeviation` | Schema deviations were found when comparing the deployed application to the core schema in the database. Individual deviations detected have been logged. To find these logs, use the activity ID property of this message as a reference. 
4000 | 4130 | `ScErrExtFieldMissingCore` | An extension class in the deployed application declared a field not part of the previously created core database schema. This is caused by addition of fields, inproper renaming of fields or if the extended class of the extension was altered. 
4000 | 4131 | `ScErrIndexDeclarationDeviation` | An index was declared but either not part of the core database schema or the signature of that index did not match the signature of the index in the core. 
4000 | 4132 | `ScErrMissingEntityClass` | A database definition in the current core database schema have no corresponding entity class in the deployed application. 
4000 | 4133 | `ScErrMissingPersistentField` | A database attribute in the current core database schema have no corresponding persistent field in the deployed application. 
4000 | 4134 | `ScErrMissingExtensionClass` | In the core database schema, an entity class was extended by an extension class that have no corresponding class in the deployed application. 
4000 | 4135 | `ScErrIndexDeclarationMissing` | An index was defined in the core database schema but it has no corresponding index declaration in the deployed application. 
4000 | 4136 | `ScErrHookCallbackNotBound` | At least one commit hook part of the deployed application defined a callback that was not previously bound to the core database definition of the target being hooked. 
4000 | 4137 | `ScErrHookCallbackNotInstalled` | At least one hook part of the deployed application was missing. The core keeps track of what hooks and callbacks are installed and when a core definition was inspected, a callback was previously bound but no longer part of the deployed application. 
4000 | 4139 | `ScErrIteratorClosed` | A interator operation failed because the iterator was closed. 
4000 | 4141 | `ScErrCantBackupOutOfMemory` | The checkpoint image file could not be backed up because of failure to allocate needed resources. 
4000 | 4142 | `ScErrCantBackupAlreadyExists` | The checkpoint image file could not be backed up because a backup with the same timestamp already existed. 
4000 | 4143 | `ScErrCantBackupDiskFull` | The checkpoint image file could not be backed up because there wasn't enough room om the target disk. 
4000 | 4144 | `ScErrCantBackupUnexpError` | The checkpoint image file could not be backed up as a result of an OS error. 
4000 | 4145 | `ScErrCantVerifyBackupFile` | Backup file could not be verified. 
4000 | 4146 | `ScErrCantFindBackupFile` | Backup file could not be found. 
4000 | 4147 | `ScErrCantAccessBackupFile` | Backup file isn't accessible by the process. 
4000 | 4148 | `ScErrVerifyImageMagicFailed` | Verification of image file against magic number failed. The file could not be identified as an image file. 
4000 | 4149 | `ScErrVerifyBackupMagicFailed` | Verification of backup file against magic number failed. The file could not be identified as a backup file. 
4000 | 4150 | `ScErrCantFindMainTransLogFile` | When inspecting a set of physical redo log files in a multi-file transaction log setup, the server was not able to find the main redo log, indexed 0 (zero). The reason for this indicates that the main redo log file have been tampered with on the server, either it has been deleted or moved. 
4000 | 4151 | `ScErrTransLogFileCountMismatch` | When inspecting a set of physical redo log files in a multi-file transaction log setup, the server found that the count expected did not match the number of redo logs found. The reason for this indicates that physical files have been tampered with on the server, usually that one of more files have been deleted or moved. 
4000 | 4152 | `ScErrTransLogCantFindRefFile` | The physical redo log file referenced by the image opened was not part of the set of file identified as logs. This is most likely caused by the file being renamed or possibly deleted. 
4000 | 4153 | `ScErrLBinaryValueExceedMaxSize` | Large binary data exceeds maximum size. 
4000 | 4154 | `ScErrStringValueExceedsMaxSize` | String data exceeds maximum size. 
4000 | 4155 | `ScErrStringConversionFailed` | An error occured when converting string to or from native format. 
4000 | 4156 | `ScErrSearchKeyExceedsMaxSize` | Search key data exceeds maximum allowed size. 
4000 | 4157 | `ScErrOnlyDuringSchemaUpdate` | Operation is only supported during schema update. 
4000 | 4158 | `ScErrReattachFailedBadDbState` | A thread detached during database operation and failed to reattach leaving the server process in an inconsistent state. 
4000 | 4159 | `ScErrCantInitCheckpOutOfMemory` | Checkpoint process could not be initialized because of failure to allocated needed resources from OS. 
4000 | 4160 | `ScErrCantInitCheckpUnexpError` | Checkpoint process could not be initialized because of an unexpected OS error. 
4000 | 4161 | `ScErrNotDuringSchemaUpdate` | Operation is not supported during schema update. 
4000 | 4162 | `ScErrCommitPending` | The operation failed because a transaction commit is pending (issued but not finalized). 
4000 | 4163 | `ScErrOutOfThreadBuffer` | The operation failed because the maximum size of the thread buffer was exceeded. 
4000 | 4164 | `ScErrCodeGenerationFailed` | Code generation failed. 
4000 | 4166 | `ScErrIndexOnTypeNotSupported` | Index on type of specified attribute not supported. 
4000 | 4167 | `ScErrToManyAttributesOnIndex` | The number of attributes specified exceeds the maximum number of attributes supported in a combined index. 
4000 | 4168 | `ScErrNoAttributesOnIndex` | No attributes specified on index creation. 
4000 | 4169 | `ScErrInvalidIndexSortMask` | Invalid sort mask specified on index creation. 
4000 | 4170 | `ScErrDefinitionToLarge` | The size of the defintion exceeded the maximum definition size. 
4000 | 4173 | `ScErrReloadFailStringConvert` | String conversion failed during rebuild. 
4000 | 4174 | `ScErrInvalidObjectAccess` | The operation failed because of an invalid object access. 
4000 | 4177 | `ScErrSchemaCodeMismatch` | Operation failed because input does not match schema. 
4000 | 4178 | `ScErrSetupDynCodeEnvFailed` | Failed to setup dynamic code environment. 
4000 | 4179 | `ScErrLoadDynamicCodeFailed` | Failed to load a dynamically generated library. 
4000 | 4180 | `ScErrCantReadTransactionLog` | An error occured when reading from the redo log. 
4000 | 4181 | `ScErrPersPropWrongCoreRef` | A PersistentProperty declaration referenced a field in the core that was not found. 
4000 | 4182 | `ScErrToManyOpenIterators` | Too many open iterators registered with the current scheduler. 
4000 | 4183 | `ScErrCantGenerateDynLibName` | Unable to generate a name for dynamically generated library. 
4000 | 4184 | `ScErrNotARepTransaction` | Operation is only allowed by a REP transaction. 
4000 | 4185 | `ScErrNotIfRepTransaction` | Operation is not allowed by a REP transaction. 
4000 | 4186 | `ScErrCantCreateDbMemoryFile` | The memory manager was unable to create a memory file to store database memory. 
4000 | 4187 | `ScErrNamedIndexAlreadyExists` | An index with the specified name already exists. 
4000 | 4188 | `ScErrMetadataClassDelete` | Metadata objects cannot be deleted. 
4000 | 4189 | `ScErrCantInitFlushOutOfMemory` | Flush process could not be initialized because of failure to allocated needed resources from OS. 
4000 | 4190 | `ScErrCantInitFlushUnexpError` | Flush process could not be initialized because of an unexpected OS error. 
4000 | 4191 | `ScErrBadMemoryCksumCheckp` | Checksum mismatch detected in database memory on checkpoint. 
4000 | 4192 | `ScErrBadImageFileCksumCheckp` | Checksum mismatch detected in image file on checkpoint. 
4000 | 4193 | `ScErrBadImageFileCksumBackup` | Checksum mismatch detected in image file on backup. 
4000 | 4194 | `ScErrImageFileAlreadyExists` | A new image file could not be created because a file with the specific name already exists. 
4000 | 4195 | `ScErrImageExePageSizeMismatch` | The image page size does not match the page size of the executable. 
4000 | 4196 | `ScErrBackupExePagesizeMismatch` | The backup page size does not match the page size of the executable. 
4000 | 4197 | `ScErrOutputBufferToSmall` | Operation failed because the output buffer was to small for the output. 
4000 | 4198 | `ScErrIndexNameTooLong` | Index creation failed because the specified index name was too long. 
4000 | 4199 | `ScErrNotWithinATransaction` | Operation failed because a transaction was attached to the thread and the operation was not allowed whtin a transaction. 
4000 | 4200 | `ScErrInvalidIndexName` | Index creation failed because the specified index name was of an invalid format. 
4000 | 4201 | `ScErrConnectInsideDatabase` | Connect failed because it was called from code running inside a database. 
4000 | 4202 | `ScErrClientEntityTypeUnknown` | The SQL statement referenced a class/table whose code type was not found on the client. Missing type: "{0}". Please add the assembly defining this type using Db.Current.EnableClientAssembly(Assembly), for example using Db.Current.EnableClientAssembly(typeof({0}).Assembly).{1}SQL: "{2}". 
4000 | 4203 | `ScErrClientBackendNotInitialized` | A method call failed because the client backend was not yet established. 
4000 | 4204 | `ScErrAssemblyNotPreparedForClient` | The assembly was not compiled/weaved for access from a client application. 
4000 | 4205 | `ScErrInvalidClientConnectString` | The database connection string is incorrect. 
4000 | 4206 | `ScErrInstantiateBindingNoType` | The type- or extenstion binding "{0}" can not be instantiated because the code type is not known. Make sure the code type is assigned using the CodeType property. 
4000 | 4207 | `ScErrInstantiateAbstractBinding` | The type- or extenstion binding "{0}" can not be instantiated because it represents a type that is declared abstract. 
4000 | 4208 | `ScErrMaxNumberOfTablesExceeded` | Unable to create a new table because the maximum number of tables would be exceeded. 
4000 | 4209 | `ScErrTableNameTooLong` | Create or alter table failed because the specified table name was too long. 
4000 | 4210 | `ScErrInvalidTableName` | Create or alter table failed because the specified table name was of an invalid format. 
4000 | 4211 | `ScErrColumnNameTooLong` | Create or alter table failed because the specified column name was too long. 
4000 | 4212 | `ScErrInvalidColumnName` | Create or alter table failed because the specified column name was of an invalid format. 
4000 | 4213 | `ScErrColumnTypeMustBeNullable` | Create or alter table failed because a column of a type that requires it to be nullable was not specified as nullable. 
4000 | 4214 | `ScErrNamedTableAlreadyExists` | Create table failed because a table with the specified name already exists. 
4000 | 4215 | `ScErrAlreadyConnectedToOtherDb` | Connecting failed because another database is already connected. 
4000 | 4216 | `ScErrTableAlreadyDropped` | Failed to drop a table because it has already been dropped. 
4000 | 4217 | `ScErrDbTerminatedGracefully` | The database process terminated gracefully. 
4000 | 4218 | `ScErrDbTerminatedUnexpectedly` | The database process terminated unexpectedly. 
4000 | 4219 | `ScErrUnknownDbState` | Unknown database state. 
4000 | 4220 | `ScErrEntityClassNotPublic` | The class is not public. Entity classes must be declared with public visibility. 
4000 | 4221 | `ScErrCommitNotPending` | The operation failed because a transaction commit is not pending. 
4000 | 4222 | `ScErrCantResetAbort` | Transaction abort could not be reset. 
4000 | 4224 | `ScErrCheckpWaitForLogAborted` | Wait for log writer to write log entries to disk before completing checkpoint was aborted. 
4000 | 4225 | `ScErrCantWriteImageFileDiskFull` | Unable to write to image file because the write would expand the file and there is no space available on disk. 
4000 | 4226 | `ScErrBadColumnType` | An unknown column type was specified when creating a table. 
4000 | 4227 | `ScErrTransactionBound` | The operation failed because the transaction was bound to a thread. 
4000 | 4228 | `ScErrTableDropped` | Failed alter table because it has been dropped. 
4000 | 4229 | `ScErrIndexNotFound` | Index requested not found. 
4000 | 4230 | `ScErrTableNotFound` | Table with specified name not found. 
4000 | 4232 | `ScErrSystemTable` | Table can't be altered or dropped because it is a system table. 
4000 | 4233 | `ScErrSystemIndex` | Index can't be altered or dropped because it is a system index. 
4000 | 4234 | `ScErrCheckpointAborted` | Checkpoint was aborted. 
4000 | 4239 | `ScErrCheckpWaitForSpaceAborted` | Wait for disk space to become available before completing checkpoint was aborted. 
4000 | 4240 | `ScErrAssemblySpecNotFound` | The assembly specification type was not found in the given assembly. 
4000 | 4241 | `ScErrBackingRetreivalFailed` | Unable to retrieve well-known metadata from a transformed binary. Check inner exceptions and/or logs to find more information. 
4000 | 4242 | `ScErrBackingDbIndexTypeNotFound` | The database class index backing type was not found in the given assembly. 
4000 | 4243 | `ScErrNotATypeSpecificationType` | The given .NET type was not considered a valid type specification type. 
4000 | 4244 | `ScErrTypeSpecIllegalConstruct` | A type specification construct was missing or had an illegal signature. 
4000 | 4245 | `ScErrAtLeastOneIndexOnTable` | Unable to drop index because at least one (not inherited) index is required on a table. 
4000 | 4246 | `ScErrCLRDecToX6DecRangeError` | The CLR Decimal cannot be converted to a Starcounter X6 decimal without data loss. Range error. 
4000 | 4247 | `ScErrPropertyNameEqualsField` | A property in a database class can not have the same name as a public/protected field in the same class, or in a base class, not even if they differ in casing. 
4000 | 4248 | `ScErrCantExpandTransLog` | An error occured when attempting to expand transaction log. 
4000 | 4249 | `ScErrCantExpandTransLogBadFile` | Transaction log could not be expanded because a faulty log element file was found in the log directory. 
4000 | 4250 | `ScErrCantExpandTransLogDiskFull` | Transaction log could not be expanded because of a failure to allocate disk space. 
4000 | 4251 | `ScErrCantExpandTransLogNoMemory` | ransaction log could not be expanded because of a failure to allocate disk memory. 
4000 | 4252 | `ScErrCantInitTransLogWriter` | An error occured when initializing transaction log writer. 
4000 | 4254 | `ScErrNoTransLogSpaceAvailable` | Transaction could not be committed because of a lack of available log space. 
4000 | 4255 | `ScErrTransLogSpaceCantBeExpanded` | Transaction could not be committed because of a lack of available log space and log space can't be expanded. 
4000 | 4256 | `ScErrTransLogWriterNotAvailable` | Transaction log writer is not available. 
4000 | 4257 | `ScErrCantLoadTransLogWriterLib` | Transaction log writer client library could not be loaded. 
4000 | 4258 | `ScErrIllegalTransientTarget` | The Transient attribute is not applicable to the target code construct. 
4000 | 4259 | `ScErrFailingEntrypoint` | The user code entrypoint method raised an exception. 
4000 | 4260 | `ScErrTransactionClosed` | An operation failed because transaction has been closed. 
4000 | 4261 | `ScErrFieldsDifferInCaseOnly` | A field in a database class can not have the same name as a another field in the same class, or in a base class, not even if they differ in casing. 
4000 | 4262 | `ScErrPropertyDifferInCaseOnly` | A property in a database class can not have the same name as a another property in the same class, or in a base class, not even if they differ in casing. 
4000 | 4263 | `ScErrForbiddenFieldInitializer` | Initializing fields in database classes must be done inside a constructor. Field initializers are not supported. 
4000 | 4264 | `ScErrApplicationCantBeResolved` | The application could not be resolved based on the given arguments or context. 
4000 | 4265 | `ScErrDatabaseMemberReservedName` | The name of a database class field or property clashed with a name reserved by Starcounter. 
4000 | 4266 | `ScErrCantExecuteDDLTransactLocked` | Failed to execute DDL statement, since a transaction cannot be created for the execution. This can happen if the statement was called in a user-specified transaction scope. 
4000 | 4267 | `ScErrTransactionSizeLimitReached` | Transaction size limit reached. 
4000 | 4268 | `ScErrSystemIndexOnNonSystemTable` | System index can't be added to a regular table. 
4000 | 4269 | `ScErrNotASystemTransaction` | Operation is only allowed within the context of a system transaction. 
4000 | 4270 | `ScErrNotANonSystemTransaction` | Operation is not allowed within the context of a system transaction. 
4000 | 4271 | `ScErrThreadDetachBlocked` | Attempt to detach thread when not allowed to detach thread. 
4000 | 4272 | `ScErrValueUndefined` | Value is undefined. 
4000 | 4273 | `ScErrRecordNotFound` | Record with specified id not found. 
4000 | 4274 | `ScErrTypeAlreadyLoaded` | A type with the same name was already loaded, from a different assembly. 
4000 | 4275 | `ScErrUnloadTableNoClass` | Unloading a table fails, since its class is not loaded. An application, where the class is declared, needs to be loaded before unload is called. 
4000 | 4276 | `ScErrToManyIndexes` | Maximum index count for table exceeded when creating index. 
4000 | 4277 | `ScErrColumnNameNotUnique` | Create table failed because one or more columns shared the same name token as another column. 
4000 | 4278 | `ScErrAmbiguousImplicitTransaction` | Ambiguous implicit transaction. Please put code inside a transaction scope. 
4000 | 4279 | `ScErrTokenNotFound` | Internal error: token for the given name is not found. 
4000 | 4280 | `ScErrInvalidTypeReference` | The [Type] attribute is not valid on the given target. 
4000 | 4281 | `ScErrInvalidInheritsReference` | The [Inherits] attribute is not valid on the given target. 
4000 | 4282 | `ScErrInvalidTypeName` | The [TypeName] attribute is not valid on the given target. 
4000 | 4283 | `ScErrDatabaseMemberNotPublic` | Fields/properties of this type must be declared public. 
4000 | 4284 | `ScErrIllegalTypeObject` | Object cannot be used as a type. 
4000 | 4285 | `ScErrNonPublicFieldNotExposed` | The database field was not public and no public property exist that expose it. 
4000 | 4286 | `ScErrTransactionAlreadyOwned` | The transaction is already owned by someone else and cannot be claimed. 
4000 | 4287 | `ScErrTransactionModifiedButNotReferenced` | Data in the transaction is created or updated, but the transaction is not referenced and will be discarded. 
4000 | 4293 | `ScErrDataManagerUnavailable` | Failed to connect to data manager is unavailable. Occurs when data manager is not started or not yet properly initialized. 
4000 | 4294 | `ScErrDataManagerChannelInUse` | Failed to connect to data manager because designated channel is in use. Occurs if a previous client is still connected or not yet properly disconnected. 
4000 | 4295 | `ScErrDataManagerUnresponsive` | Communication with data manager failed because data manager is unresponsive. 
4000 | 4296 | `ScErrDataManagerDisconnected` | Communication with data manager failed because client was disconnected by data manager. 
4000 | 4297 | `ScErrDataManagerIncoherent` | Communication with data manager failed because client is unable to understand the data manager. 
4000 | 4298 | `ScErrLogWriterStillRunning` | Could not proceed because log writer is still running and won't shut down. 
4000 | 4299 | `ScErrRecordIDLimitReached` | Record ID limit is reached. No more records can be inserted in the database. 
4000 | 4300 | `ScErrColumnNotFound` | Column with specified name not found. 
4000 | 4301 | `ScErrTransactionLogPositionNotFound` | Specified position not found in the transaction log. 
4000 | 4302 | `ScErrHandleTableLimitReached` | Unable to allocate a handle table since the maximum number of handles tables are reached. 
4000 | 4303 | `ScErrIllegalSynonymElement` | Illegal element for the SynonymousTo attribute. 
4000 | 4304 | `ScErrForbiddenDatabaseName` | Invalid database name. 
4000 | 4305 | `ScErrCantRunSharedAppHost` | Illegal attempt to run the shared app code host. 
4000 | 4306 | `ScErrNonPublicCodeProperty` | Only public code properties in database classes are allowed. 
4000 | 4307 | `ScErrFieldInDatabaseType` | Database types can not declare instance fields. 
4000 | 4308 | `ScErrEntityClassNotAbstract` | The class is not abstract. Entity classes must be declared abstract. 
4000 | 4309 | `ScErrAutoPropNotAbstract` | The property is not abstract. Auto-implemented properties in entity classes must be declared abstract. 
4000 | 4310 | `ScErrPropertyNotSupported` | The property is not supported. Consult Starcounter documentation on declaring properties in database classes. 
4000 | 4311 | `ScErrNestedTransactionNotSupported` | An attempt was made to initialize a new transaction within the scope of an active transaction. Creating such nested transactions is not supported. Consider reusing the already active transaction or running a new transaction outside the already active transaction's scope. 
5000 | 5002 | `ScErrInvalidSessionId` | Specified session id not valid. 
5000 | 5003 | `ScErrTransNotRegisteredWithSesn` | The specific transaction is not registered with the session. 
5000 | 5004 | `ScErrMaxRegisteredTransReached` | Unable to register transaction with session because the maximum number of transactions per session has already been reached. 
5000 | 5005 | `ScErrSessionInsideDatabase` | Operations on sessions are only available from clients, not code running inside a database. 
5000 | 5006 | `ScErrSessionManagerDied` | Scheduler session manager has died. 
5000 | 5007 | `ScErrAnotherSessionActive` | Another session for this thread is already active. 
5000 | 5008 | `ScErrAcquireSessionTimeout` | Exclusive access to the session could not be obtained. 
6000 | 6002 | `ScErrUnexpectedDiskAccessErr` | A call to an operating system level disk access API failed to execute. 
6000 | 6003 | `ScErrUnexpectedFileAccessErr` | A call (other than read/write) to an operating system level file access API failed to execute. 
6000 | 6005 | `ScErrTransactionLogCorrupt` | Recovery detected the transaction log to be corrupt, recovery can't be completed. 
6000 | 6006 | `ScErrUnexpectedImageFileError` | An unexpected file I/O error occurred when writing to or reading from the image file. 
6000 | 6007 | `ScErrUnexpectedImageMMapError` | An unexpected error was detected when allocating memory for the database image. 
6000 | 6008 | `ScErrUnexpectedWaitError` | An unexpected error was detected when waiting for a waitable object. 
6000 | 6009 | `ScErrUnexpErrorOpenImageFile` | An unexpected file I/O error occurred when attempting to open an image file. 
6000 | 6010 | `ScErrUnexpErrorReadImageFile` | An unexpected file I/O error occurred when attempting to read from an image file. 
6000 | 6011 | `ScErrUnexpErrorWriteImageFile` | An unexpected file I/O error occurred when attempting to write to an image file. 
6000 | 6012 | `ScErrUnexpErrorInitSharedObject` | An unexpected error occured when attempting to initialize a shared object. 
6000 | 6013 | `ScErrUnexpErrorCreateTransLog` | An unexpected file I/O error occurred when attempting to create the redo log. 
6000 | 6014 | `ScErrUnexpErrorOpenTransLog` | An unexpected file I/O error occurred when attempting to open the redo log. 
6000 | 6015 | `ScErrUnexpErrorReadTransLog` | An unexpected file I/O error occurred when attempting to read from the redo log. 
6000 | 6016 | `ScErrUnexpErrorWriteTransLog` | An unexpected file I/O error occurred when attempting to write to the redo log. 
6000 | 6017 | `ScErrUnexpErrorOpenDumpFile` | An unexpected file I/O error occurred when attempting to open a dump file (SCD or SCDX). 
6000 | 6018 | `ScErrUnexpErrorReadDumpFile` | An unexpected file I/O error occurred when attempting to read a dump file (SCD or SCDX). 
6000 | 6019 | `ScErrUnexpErrorWriteDumpFile` | An unexpected file I/O error occurred when attempting to write to dump file (SCD or SCDX). 
6000 | 6020 | `ScErrUnexpErrorExpandImageFile` | An unexpected file I/O error occurred when attempting to expand an image file. 
6000 | 6021 | `ScErrUnexpErrorCopyImageFile` | An unexpected file I/O error occurred when attempting to copy an image file. 
6000 | 6022 | `ScErrUnexpErrorCreateImageFile` | An unexpected file I/O error occurred when attempting to create an image file. 
6000 | 6024 | `ScErrImageFileCorrupt` | The image file is corrupt. 
6000 | 6026 | `ScErrUnexpErrorCreateProcess` | An unexpected error occured when attempting to create a child process. 
6000 | 6027 | `ScErrUnexpErrorNameDumpFile` | An unexpected file I/O error occurred when attempting to rename dump file from temporary to final name. 
6000 | 6028 | `ScErrVerifyDumpMagicFailed` | When rebuilding, verifying the dump against the magic number failed. 
6000 | 6029 | `ScErrUnexpErrorGetDumpTmpName` | An unexpected error occured when creating a temporary file name for dump. 
6000 | 6030 | `ScErrUnexpErrorCreateDumpFile` | An unexpected file I/O error occurred when attempting to create dump file (SCD or SCDX). 
6000 | 6031 | `ScErrMemoryManagerIncoherent` | The server can't understand the memory manager. 
6000 | 6032 | `ScErrUnexpChannelReadError` | An unexpected error was detected when initiating or completing a channel read. 
6000 | 6033 | `ScErrUnexpChannelWriteError` | An unexpected error was detected when initiating or completing a channel write. 
6000 | 6034 | `ScErrUnexpChannelAcceptError` | An unexpected error was detected when initiating or completing a channel accept. 
6000 | 6035 | `ScErrUnexpChannelConnectError` | An unexpected error was detected when initiating or completing a channel connect. 
6000 | 6036 | `ScErrUnexpChannelCloseError` | An unexpected error was detected when initiating or completing a channel close. 
6000 | 6037 | `ScErrUnexpListenerCreateError` | An unexpected error was detected when creating a listener. 
6000 | 6038 | `ScErrUnexpListenerCloseError` | An unexpected error was detected when initiating or completing a listener close. 
6000 | 6039 | `ScErrUnexpErrorOpenFileLog` | An unexpected error was detected when opening the server file/trace log. 
6000 | 6041 | `ScErrUnexpErrInitBackupProc` | An unexpected OS error occurred when attempting to initialize backup process. 
6000 | 6042 | `ScErrUnexpErrBackupImageFile` | An unexpected file I/O error occurred when attempting to backup an image file. 
6000 | 6043 | `ScErrUnexpErrOpenBackupFile` | An unexpected file I/O error occurred when attempting to open a backup file. 
6000 | 6044 | `ScErrUnexpErrFindingTransLog` | An unexpected error occurred when attempting to find the redo log(s). 
6000 | 6045 | `ScErrUnexpErrTransLogCountZero` | An unexpected error occurred when reading the internal COUNT property of the physical main redo log. The value was 0 (zero). 
6000 | 6046 | `ScErrUnexpErrIllegalTLogSectRef` | The reference to the place where the next transaction should be written in the redo log, fetched from the image when loaded, was out of bounds with respect to the sector count of the given file. 
6000 | 6047 | `ScErrUnexpErrInitCheckpProc` | An unexpected OS error occurred when attempting to initialize checkpoint process. 
6000 | 6048 | `ScErrUnexpErrorCopyCodeFile` | An unexpected file I/O error occurred when attempting to copy code generation base file. 
6000 | 6049 | `ScErrUnexpErrorOpenCodeFile` | An unexpected file I/O error occurred when attempting to open code generation file. 
6000 | 6050 | `ScErrUnexpErrorWriteCodeFile` | An unexpected file I/O error occurred when attempting to write to code generation file. 
6000 | 6051 | `ScErrUnexpErrorExecuteCompile` | An unexpected error occurred when attempting to compile code generation output. 
6000 | 6052 | `ScErrUnexpectedCompilerError` | An unexpected error occurred was returned by the compiler when attempting to compile code generation output. 
6000 | 6053 | `ScErrUnexpErrorCreateCodeDir` | An unexpected error occurred when attempting to create directory for dynamically generated code. 
6000 | 6054 | `ScErrUnexpErrorLoadCodeFile` | An unexpected error occurred when attempting to load dynamically generated code. 
6000 | 6055 | `ScErrUnexpErrorAllocResource` | An unexpected error occured when attempting to allocate a OS resource. 
6000 | 6056 | `ScErrUnexpErrorFreeResource` | An unexpected error occured when attempting to release a OS resource. 
6000 | 6057 | `ScErrUnexpErrorSetWaitEvent` | An unexpected error occured when attempting set, reset or wait for an OS event. 
6000 | 6058 | `ScErrUnexpErrorCreateDbMemFile` | An unexpected error was detected when creating a memory file for database data. 
6000 | 6059 | `ScErrUnexpMemoryCksumCheckp` | Checksum mismatch detected in database memory on checkpoint. 
6000 | 6060 | `ScErrUnexpImageFileCksumLoad` | Checksum mismatch detected in image file on load. 
6000 | 6061 | `ScErrUnexpImageFileCksumCheckp` | Checksum mismatch detected in image file on checkpoint. 
6000 | 6062 | `ScErrUnexpImageFileCksumBackup` | Checksum mismatch detected in image file on backup. 
6000 | 6063 | `ScErrUnexpErrInitFlushProc` | An unexpected OS error occurred when attempting to initialize image write process. 
6000 | 6064 | `ScErrUnexpErrSetDllDirectory` | An unexpected OS error occurred when attempting to initialize image write process. 
6000 | 6065 | `ScErrUriMatcherCodeGeneratorFailure` | Exception occurred inside URI matcher code generator. 
6000 | 6066 | `ScErrUnhandledWeaverException` | An unhandled, unidentified (non-Starcounter) exception occurred in the weaver. 
6000 | 6067 | `ScErrUnexpPlatformAPIError` | An unexpected error occured calling a platform API function. 
6000 | 6068 | `ScErrTupleTooBig` | TupleWriter cannot write tuple, since it is too big. 
6000 | 6069 | `ScErrUnexpFasterThanJson` | An unexpected error occured in FasterThanJson library. 
6000 | 6070 | `ScErrNoTupleWriteSave` | TupleWriter cannot safely write tuple. 
6000 | 6071 | `ScErrTupleValueTooBig` | Input value cannot fit the given tuple lenght. 
6000 | 6072 | `ScErrTupleOutOfRange` | Value is out of range of the tuple. 
6000 | 6073 | `ScErrUnexpEnumeratorDispose` | Unexpected error during enumerator dispose. 
6000 | 6074 | `ScErrUnexpDbMetadataMapping` | Unexpected error during mapping different meta-data, which represent database classes and properties. 
6000 | 6075 | `ScErrUnknownNetworkProtocol` | Current network protocol is unknown. 
6000 | 6076 | `ScErrTupleIncomplete` | TupleWriter cannot safely seal tuple, since not all values were written. 
6000 | 6077 | `ScErrWeaverCantUseCache` | The weaver was unable to weave the application because of an internal problem with the weaver cache. Run the weaver with the --nocache option to remedy. 
6000 | 6078 | `ScErrMaxHandlersReached` | Maximum number of handlers reached. 
6000 | 6079 | `ScErrHandlerInfoExceedsLimits` | Handler info exceeds size limits. 
6000 | 6080 | `ScErrUnexpectedResponse` | An internal node call returned an unexpected response. 
6000 | 6081 | `ScErrColumnHasNoProperty` | The column is not exposed with a property, it can not be accessed. 
6000 | 6082 | `ScErrUnexpectedConnectTimeout` | Unexpected connect timeout. 
7000 | 7000 | `ScErrSqlInternalError` | Unexpected internal error in SQL module. 
7000 | 7001 | `ScErrSqlExportSchemaFailed` | SQL module was unable to write a schema file to the current temp directory. 
7000 | 7002 | `ScErrSqlStartProcessFailed` | SQL module was unable to start external process scsqlparser.exe. 
7000 | 7003 | `ScErrSqlVerifyProcessFailed` | SQL module was unable to verify external process scsqlparser.exe. 
7000 | 7004 | `ScErrSqlProcessQueryFailed` | External process scsqlparser.exe was unable to process the current SQL query. 
7000 | 7006 | `ScErrQueryOptimInternalError` | Unexpected internal error in query optimization module. 
7000 | 7007 | `ScErrQueryExecInternalError` | Unexpected internal error in query execution module. 
7000 | 7008 | `ScErrSqlDuplicatedIdentifier` | More than one public identifier (class name, property name etc.) with the same case insensitive representation. 
7000 | 7009 | `ScErrConnConnectToDatabase` | Can't establish connection to specified database. 
7000 | 7010 | `ScErrConnGetQueryID` | Error fetching unique SQL query identifier. 
7000 | 7011 | `ScErrConnGetQueryResults` | Error fetching SQL query results. 
7000 | 7013 | `ScErrConnGetNextResultsPage` | Error fetching SQL query results next page. 
7000 | 7014 | `ScErrConnCloseEnum` | Error closing open SQL enumerator. 
7000 | 7015 | `ScErrConnInitSqlFunctions` | Error initializing managed SQL function pointers. 
7000 | 7016 | `ScErrSingleObjectProjection` | Only single object projection is allowed for SQL queries on the client. 
7000 | 7017 | `ScErrQueryStringTooLong` | The length of the query string exceeds the maximal length allowed. 
7000 | 7019 | `ScErrOffsetKeyOutOfProcessFetch` | In out of database execution offset key can be obtained only for queries with FETCH statement. 
7000 | 7020 | `ScErrQueryWrongParamType` | Incorrect SQL parameter/variable type. 
7000 | 7021 | `ScErrSQLIncorrectSyntax` | Incorrect SQL syntax. 
7000 | 7022 | `ScErrSQLNotImplemented` | Not implemented SQL feature. 
7000 | 7023 | `ScErrSQLNotSupported` | Not supported SQL feature. 
7000 | 7024 | `ScErrUnexpErrSprintfSQLSyntax` | An unexpected error occurred in unmanaged SQL parser when attempting to format message for SQL syntax error. 
7000 | 7025 | `ScErrUnexpSQLParser` | An unexpected error occurred in SQL Parser. 
7000 | 7026 | `ScErrSQLUnknownName` | Specified name of class, property or other object name does not exist in the database. 
7000 | 7027 | `ScErrUnexpSQLScanner` | An unexpected error occurred in scanner of SQL Parser. 
7000 | 7028 | `ScErrInvalidOffsetKey` | Offset key is invalid for given query. 
7000 | 7029 | `ScErrUnsupportLiteral` | Literals are not supported in the query. 
7000 | 7030 | `ScErrUnsupportAggregate` | Aggregates are not supported in the query. 
7000 | 7031 | `ScErrQueryResultTypeMismatch` | Expected generic result type does not match with query result type. 
7000 | 7032 | `ScErrTooManyIndexColumns` | Too many columns for compound index. 
7000 | 7033 | `ScErrInvalidCurrent` | Calling current is invalid, since enumerator has not started or has already finished. 
7000 | 7034 | `ScErrUnexpErrUnavailable` | Internal error in SQL Processor. Error cannot be retrieved. 
7000 | 7035 | `ScErrUnexpMetadata` | Internal error in unmanaged meta-data population. 
7000 | 7036 | `ScErrSqlInternalBufferSmall` | Internal error in native SQL processor that allocated buffer is too small. 
7000 | 7037 | `ScErrSqlUnknownType` | The type is unknown. The name of the table or database class specified in the query is unknown and cannot be matched to a database type. 
7000 | 7038 | `ScErrSqlUnknownColumn` | The column is unknown. The name of the column or property specified in the query is unknown and cannot be matched to a database column. 
7000 | 7039 | `ScErrSqlAmbiguousType` | The type is ambiguous. The name of the table or database class specified in the query matches several database types. 
7000 | 7040 | `ScErrSqlColumnTypeMismatch` | Type of column value does not match actual column type. 
7000 | 7041 | `ScErrRange` | Range error: value cannot be converted without loss. 
7000 | 7042 | `ScErrInvalidStatement` | Given SQL statement is of unexpected type. Another API function should be called. 
7000 | 7043 | `ScErrMetadataHasntBeenCreated` | Meta-data hasn't been created or created incompletely for this database. 
7000 | 7044 | `ScErrQueryIdNotFoundInCache` | Given Query ID wasn't found in cache. 
8000 | 8001 | `ScErrConstraintViolationAbort` | A transaction was aborted because a constraint was violated. 
8000 | 8002 | `ScErrTransactionConflictAbort` | A transaction was aborted because it was in conflict with another transaction. 
8000 | 8005 | `ScErrOutOfMemoryAbort` | The transaction was aborted because of a failure to allocate memory. 
8000 | 8006 | `ScErrInvalidObjectAccessAbort` | The transaction was aborted because of an invalid object access. 
8000 | 8007 | `ScErrSchemaCodeMismatchAbort` | The transaction was aborted because pending writes didn't match the schema. 
8000 | 8008 | `ScErrInconsistentCodeAbort` | The transaction was aborted because code is inconsistent. 
8000 | 8009 | `ScErrPrototypeDeleteAbort` | The transaction was aborted because of an attempt to delete a prototype. 
8000 | 8010 | `ScErrOutOfFileStorageAbort` | The transaction was aborted because of a failure to allocate file storage space. 
8000 | 8012 | `ScErrOutOfSharedStorageAbort` | The transaction was aborted because of a failure to allocate shared storage memory. 
8000 | 8013 | `ScErrUnexpInternalErrorAbort` | The transaction was aborted because of an unexpected internal error. 
8000 | 8014 | `ScErrCodeConstrViolationAbort` | The transaction was aborted because code constraint violation. That is, the calling code did something is wasn't supposed to. 
8000 | 8016 | `ScErrOutOfLogSpaceAbort` | The transaction was aborted because space could not be allocated for the transaction log entry. 
8000 | 8017 | `ScErrOutOfLogMemoryAbort` | The transaction was aborted because of a failure to allocate transaction log buffer space. 
8000 | 8034 | `ScErrExternalAbort` | The transaction has been aborted by the framework because of an external event that suggested that the transaction should be considered invalid. 
8000 | 8035 | `ScErrNotASystemTransactionAbort` | Transaction was aborted because of an attempt by a non-system transaction to update a system table. 
8000 | 8036 | `ScErrMemoryLimitReachedAbort` | The transaction was aborted because limit reached on limited memory resource. 
8000 | 8037 | `ScErrInvalidOperationAbort` | The transaction was aborted because an invalid operation attempted left the transaction in an inconsistent state. 
8000 | 8038 | `ScErrTransactionMergedAbort` | The transaction was aborted after being merged into another transaction. 
8000 | 8039 | `ScErrRecordTooLargeAbort` | The transaction was aborted because record generated exeeded the maximum allowed record size. 
8000 | 8040 | `ScErrLayoutLimitReachedAbort` | The transaction was aborted because of an attempt to create a new record layout while the limit of the number of record layouts had been reached. 
8000 | 8041 | `ScErrIndexLimitReachedAbort` | The transaction was aborted because of an attempt to create a new index while the limit of the number of indexes had been reached. 
8000 | 8048 | `ScErrSetIndexLimitReachedAbort` | The transaction was aborted because of an attempt to create a new index while the limit of the number of indexes on the specific set had been reached. 
8000 | 8049 | `ScErrCantMaintainSnapshotAbort` | The transaction was aborted because the snapshot the transaction was referencing couldn't be maintained. 
10000 | 10001 | `ScErrServerNotFound` | A server with the specified identity was not found. 
10000 | 10002 | `ScErrDatabaseNotFound` | A database with the specified name was not found. 
10000 | 10003 | `ScErrServerNotRunning` | The administrator server is not running. 
10000 | 10004 | `ScErrCantStartDatabase` | Attempting to start the database failed. 
10000 | 10005 | `ScErrCantDeleteMaintenanceDir` | When recovering a previous, not finished maintenance operation, the maintenance directory could not be deleted. 
10000 | 10006 | `ScErrCantDeleteJournalWhenFiles` | When recovering a previous, not finished maintenance operation, the server refused to delete the maintenance journal because there were still files in the maintenance directory. 
10000 | 10007 | `ScErrDatabaseEngineTerminated` | The server detected that the user code host executable (sccode.exe) unexpectedly terminated during a management operation. 
10000 | 10008 | `ScErrUpdateFailedToUnload` | When the server tried to unload the database during a code library update, unloading failed. 
10000 | 10009 | `ScErrUpdateFailed` | Failed to update the database user code library. 
10000 | 10010 | `ScErrUpdateFailedToCreateFiles` | The server failed to create new database files during a code library update. 
10000 | 10011 | `ScErrServerCommandFailed` | The operation "{0}" failed. 
10000 | 10012 | `ScErrDatabaseDeleteFailed` | Failed to delete the database. 
10000 | 10013 | `ScErrDatabaseNotStopped` | {0} failed because the database was not stopped. 
10000 | 10014 | `ScErrWrongDatabaseEngineConfig` | The database configuration specified a database engine not part of the configured engines known by the server. 
10000 | 10015 | `ScErrConfEngineExeNotFound` | The engine executable file was not found in the location as specified in the servers engine configuration. 
10000 | 10016 | `ScErrUnexpectedCommandException` | {0} failed due to an unexpected problem. 
10000 | 10017 | `ScErrCodeHostProcessRefusedStop` | When asked to shut down, the user code process actively refused it. 
10000 | 10018 | `ScErrCodeHostProcessNotExited` | When asked to shut down, the user code process agreed to shut down, but the process didn't exit gracefully in time. 
10000 | 10019 | `ScErrExecutableNotFound` | The executable file cound not be found. 
10000 | 10020 | `ScErrServerNotAvailable` | Unable to communicate with the administrator server. 
10000 | 10021 | `ScErrNetworkGatewayTerminated` | The server detected that the network gateway executable (scnetworkgateway.exe) unexpectedly terminated. 
10000 | 10022 | `ScErrIPCMonitorTerminated` | The server detected that the interprocess monitor executable (scipcmonitor.exe) unexpectedly terminated. 
10000 | 10023 | `ScErrCommandPreconditionFailed` | The server detected a precondition that was not met; the command was therefore not executed. 
10000 | 10024 | `ScErrDatabaseEngineNotRunning` | The database engine is not running. 
10000 | 10025 | `ScErrExecutableAlreadyRunning` | The executable is already running. 
10000 | 10026 | `ScErrExecutableNotRunning` | The executable is not running. 
10000 | 10027 | `ScErrDbProcNotSignaling` | The database data process, when started, didn't signal it was operational in time. 
10000 | 10028 | `ScErrDbProcTerminated` | The server detected that the database data process unexpectedly terminated during a management operation. 
10000 | 10029 | `ScErrEngineProcFailedStart` | The database engine process failed to start due to an unexpected internal error. The full error detail can be found in the server log. 
10000 | 10030 | `ScErrDeleteDbRenameConfig` | Failed to rename the database configuration file when asked to delete the database. 
10000 | 10031 | `ScErrDatabaseRunning` | The database is running. 
10000 | 10032 | `ScErrDeleteDbFilesPostponed` | At least one database-related file could not be deleted. Deleting will be retried until it succeeds or is cancelled. 
10000 | 10033 | `ScErrAutoReportFailedCollect` | Failed to collect reported error notifications from the notification file. 
10000 | 10034 | `ScErrAutoReportFailedSend` | Failed to send a set of collected reported error notifications. 
10000 | 10035 | `ScErrMissingApplicationName` | The name of the application was not specified. 
10000 | 10036 | `ScErrApplicationAlreadyRunning` | An application with the given name is already running. 
10000 | 10037 | `ScErrApplicationNotRunning` | No application with the given name is running. 
10000 | 10038 | `ScErrServerNotSignaling` | The admin server is taken longer than normal to start; it didn't signal it was ready in time. 
10000 | 10039 | `ScErrApplicationNotAnExecutable` | The application is not an executable. 
10000 | 10040 | `ScErrProcessExitCodeNotAvailable` | The exit code of the terminating process was not available. 
10000 | 10041 | `ScErrTooManyDatabases` | Creating a new database failed. The maximum number of databases that can coexist on a given server have been reached. 
10000 | 10042 | `ScErrFileInInstallationNotFound` | Unable to find file expected to be part of installation. 
10000 | 10043 | `ScErrHostProcessNotRunning` | The specified code host process is not running. 
11000 | 11002 | `ScErrInstallerVs2010NotFound` | Visual Studio 2010 is not found in the system. This component is required by Starcounter Visual Studio 2010 developer's integration. 
11000 | 11003 | `ScErrInstallerVStudioIsRunning` | One or more instances of Microsoft Visual Studio (devenv.exe) are running. Please shut them down and press OK. 
11000 | 11004 | `ScErrInstallerDXOrVideoCard` | Proper Microsoft DirectX version is not installed in the system. This component is required by Starcounter activity monitor component. 
11000 | 11005 | `ScErrInstallerDxDiagProblem` | DirectX diagnostic utility has exited with the error code. 
11000 | 11006 | `ScErrInstallerCorruptedSetupFile` | Can't read keys from INI settings file. 
11000 | 11007 | `ScErrInstallerCantReadSettingValue` | Can't read value from INI settings file for a certain key. 
11000 | 11008 | `ScErrInstallerVSSafeImportsRegistry` | Can't access the Visual Studio safe imports path in the registry. This component is required by Starcounter Visual Studio integration. 
11000 | 11009 | `ScErrInstallerSetupVsIntegration` | The Visual Studio integration setup returned an error. 
11000 | 11010 | `ScErrInstallerVstudio2008` | Running Visual Studio 2008 for Starcounter integration installation has failed. 
11000 | 11011 | `ScErrInstallerRegedit` | Running regedit for Starcounter integration installation has failed. 
11000 | 11012 | `ScErrInstallerProcessTimeout` | One of the processes was not finished within allowed time interval. 
11000 | 11013 | `ScErrInstallerFirewallException` | Problem adding Windows Firewall exception. 
11000 | 11014 | `ScErrInstallerWrongServersNumber` | The number of installed servers differs from expected. Please properly uninstall previous Starcounter instance before starting a new installation. 
11000 | 11015 | `ScErrInstallerAborted` | Installation process has been aborted. 
11000 | 11016 | `ScErrInstallerInternalProblem` | General installer internal exception type. 
11000 | 11017 | `ScErrInstallerProcessWrongErrorCode` | One of the external processes didn't exit with correct exit code. 
11000 | 11018 | `ScErrInstallerProcessTaskNotAccomp` | One of the external processes didn't accomplish the indended task. 
11000 | 11019 | `ScErrInstallerVs2010NotInitialized` | The Visual Studio 2010 IDE was never initialized. Please start Visual Studio once before you install. 
11000 | 11020 | `ScErrInstallerAlreadyStarted` | Another Starcounter setup instance is running. 
11000 | 11026 | `ScErrVSIXEngineNotFound` | The installer could not find the VSIX installer engine executable. 
11000 | 11027 | `ScErrVSIXPackageNotFound` | The installer could not find the VSIX deployment package. 
11000 | 11028 | `ScErrVSIXEngineCouldNotStart` | The installer failed to launch the VSIX installer engine. 
11000 | 11029 | `ScErrVSIXEngineTimedOut` | The VSIX installer engine did not complete in time. 
11000 | 11030 | `ScErrVSIXEngineFailed` | The VSIX installer engine failed to install or uninstall the deployment package. 
11000 | 11031 | `ScErrInstallerVs2012NotFound` | Visual Studio 2012 is not installed. This component is required by Starcounter Visual Studio 2012 developer's integration. 
11000 | 11032 | `ScErrInstallerSameDirectories` | At least two components have equal installation directories. All components should be installed in different directories. 
12000 | 12001 | `ScErrUnableToDeployment` | Unable to deploy code library. The code library could not be deployed due to previous problems. 
12000 | 12002 | `ScErrUnexpectedDeploymentExcept` | Unable to deploy code library. The code library could not be deployed due to an unexpected condition. Please restart Visual Studio using the /log switch or consult the help link to find additional help. 
12000 | 12003 | `ScErrCodeLibaryPathNoSet` | Can't find code library file. The path to the code library archive was not set. Check your projects "ScArchive"-related properties or consult the help link to find additional help. 
12000 | 12004 | `ScErrCodeLibaryFileNotFound` | Can't find code library file. The path to the code library archive referenced a file that was not found. Check your projects "ScArchive"-related properties or consult the help link to find additional help. 
12000 | 12005 | `ScErrWrongCodeLibaryExtension` | Can't find code library file. The path to the code library archive referenced a file type not supported. Check your projects "ScArchiveExtension" property or consult the help link to find additional help. 
12000 | 12006 | `ScErrCodeLibInputDirNotFound` | The input directory does not exist. 
12000 | 12007 | `ScErrCodeLibFailedNewCacheDir` | The cache directory could not be created. 
12000 | 12008 | `ScErrWeaverFileNotFound` | The specified file seem not to exist. 
12000 | 12009 | `ScErrWeaverFileNotSupported` | The specified files extension is not supported. 
12000 | 12010 | `ScErrDebugFailedConnectToServer` | Debugging the executable failed because Visual Studio was unable to connect to the server (a.k.a Starcounter Administrator). Usually indicates that the server is not running. 
12000 | 12011 | `ScErrDebugFailedServerErrorStarting` | Debugging the executable failed because the server (a.k.a Starcounter Administrator) returned an error when trying to run the code. Consult the server log for more detailed information. 
12000 | 12012 | `ScErrDebugFailedReported` | The debug sequence failed. Consult the Visual Studio error list for more information. 
12000 | 12013 | `ScErrDebuggerAlreadyAttached` | Unable to attach a debugger to the code host process. A debugger is already attached. 
12000 | 12014 | `ScErrDebugDbHigherPrivilege` | Unable to attach the debugger to the database process because the database runs with a higher privilege than Visual Studio. 
12000 | 12015 | `ScErrDebugNoDbProcess` | Unable to attach the debugger to the database process because the database process was not found. 
12000 | 12016 | `ScErrDebugSequenceFailUnexpect` | The debugging sequence failed with an unexpected error. Consult the Visual Studio activity log for additional information. 
12000 | 12017 | `ScErrBinaryNotFoundWhenDebug` | Could not find the executable file to run. Is the project configured to build? 
12000 | 12018 | `ScErrCLITemplateNotFound` | A source code template with the specified name could not be found. 
12000 | 12019 | `ScErrAppDeployedEditionLibrary` | At least one file considered a Starcounter edition library was found locally deployed with an application, i.e. residing in the same directory as the application file. 
12000 | 12020 | `ScErrCantAccessAppResourceDir` | Specified application resource directory is not accessible. Please check it existence and correct access rights. 
12000 | 12021 | `ScErrAppResourceDirIsOnNetworkDrive` | Seems that specified application resource directory is mapped to a network drive. 
12000 | 12022 | `ScErrJsonWithConstructor` | Typed JSON code-behind classes does not support custom instance constructors. 
12000 | 12023 | `ScErrJsonMappedMoreThanOnce` | The code-behind class defines multiple mapping attributes. 
12000 | 12024 | `ScErrJsonRootHasCustomMapping` | The code-behind class is named as the root JSON object, but also define a mapping. 
12000 | 12025 | `ScErrJsonClassNotPartial` | Typed JSON code-behind classes must be partial. 
12000 | 12026 | `ScErrJsonClassIsGeneric` | Typed JSON code-behind classes can not be generic. 
12000 | 12027 | `ScErrJsonWithMultipleRoots` | Found more than once code-behind class that was mapped to the root JSON object. 
12000 | 12028 | `ScErrJsonStaticInputHandler` | Input handler methods can not be static. 
12000 | 12029 | `ScErrJsonAbstractInputHandler` | Input handler methods can not be abstract. 
12000 | 12030 | `ScErrJsonInputHandlerBadParameterCount` | The input handler has to many parameters. 
12000 | 12031 | `ScErrJsonInputHandlerGeneric` | Input handler methods can not be generic. 
12000 | 12032 | `ScErrJsonInputHandlerRefParam` | Input handler parameter should not be defined as a reference (ref) parameter. 
12000 | 12033 | `ScErrJsonInputHandlerNotVoid` | Input handler methods must have void return type. 
12000 | 12034 | `ScErrJsonInvalidInstanceTypeAssignment` | Assignment of instancetype for a TypedJSON member must follow the pattern: 'DefaultTemplate.{memberpath}.InstanceType = typeof({type});'. 
12000 | 12035 | `ScErrJsonUnsupportedInstanceTypeAssignment` | Unsupported assignment of instancetype. 
12000 | 12036 | `ScErrJsonDuplicateReuse` | Reuse of a TypedJSON object is specified both in the json file and code-behind. The value from code-behind will be used. 
12000 | 12037 | `ScErrJsonDuplicateNamespace` | Namespace is specified both in the json file and code-behind. The value from code-behind will be used. 
12000 | 12038 | `ScErrJsonAmbiguousDatatypeIExplicitBound` | A datatype specified as a generic parameter for IExplicitBound interface is ambiguous. Please use the fully qualified name including namespace. 
12000 | 12039 | `ScErrJsonInvalidBindAssignment` | Assignment of binding for a TypedJSON member must follow the pattern: 'DefaultTemplate.{memberpath}.Bind = "{binding-path}";'. 
12000 | 12040 | `ScErrAlreadyWeaved` | The given assembly is already weaved. 
12000 | 12041 | `ScErrContainDatabaseTypes` | Forbidden declaration of database type(s). 
13000 | 13001 | `ScErrRequestOnUnregisteredUri` | HTTP request made on unregistered URI. 
13000 | 13002 | `ScErrParsingMethodAndUri` | Error while parsing HTTP method and URI. 
13000 | 13003 | `ScErrHandlerAlreadyRegistered` | This handler has already been registered. 
13000 | 13004 | `ScErrHandlerNotFound` | This given handler is not found. 
13000 | 13005 | `ScErrBadGatewayConfig` | Gateway configuration is invalid. 
13000 | 13006 | `ScErrNetworkPortIsOccupied` | Gateway tried to bind a socket on network port that is occupied. 
13000 | 13007 | `ScErrNetworkGatewayAlreadyRunning` | An instance of network gateway is already running. 
14000 | 14001 | `ScErrAppsHttpParserIncompleteHeaders` | HTTP request has incomplete headers. 
14000 | 14002 | `ScErrAppsHttpParserIncorrect` | HTTP contains incorrect data. 
14000 | 14003 | `ScErrJsonPropertyNotFound` | The property is not defined in the template. 
14000 | 14004 | `ScErrJsonValueWrongType` | The value does not correspond to the template property type. 
14000 | 14005 | `ScErrJsonUnexpectedEndOfContent` | End of content reached unexpectedly 
14000 | 14006 | `ScErrCreateDataBindingForJson` | Cannot create binding between typed json and dataobject. 
14000 | 14007 | `ScErrInvalidJsonForInput` | Invalid json found when populating an typed json object. 
14000 | 14008 | `ScErrMissingDataTypeBindingJson` | Cannot create binding between typed json and dataobject. The datatype needs to be specified. 
14000 | 14009 | `ScErrDuplicateDataTypeJson` | Datatype cannot be specified in the json file if a code-behind class exists. The datatype needs to be set as generic argument on the inheritance on the class. 
14000 | 14010 | `ScErrSessionJsonNotRoot` | The json is not a rootobject (have no parent) and cannot be used as sessiondata. 
14000 | 14011 | `ScErrJsonSetOnOtherSession` | The json is already connected to another session. 
14000 | 14012 | `ScErrDataBindingForJsonException` | An exception occurred when a databinding for a property in json was used. 
14000 | 14013 | `ScErrInvalidOperationDataOnEmptyJson` | A dataobject was set on a TypedJSON object or array that does not define any properties. 
15000 | 15001 | `ScErrBaseTypeNotFound` | There is no type in the database for the given base type name. 
15000 | 15002 | `ScErrDataTypeNotFound` | There is no type in the database for the given column data type name. 
15000 | 15003 | `ScErrTypeNotFound` | There is no type in the database for the given type name. 
15000 | 15004 | `ScErrTypeColumnNotFound` | There is no column in the database for the given type and column name. 
15000 | 15005 | `ScErrIndexNotFoundInType` | There is no given index in the database for the given type. 
15000 | 15006 | `ScErrDropTypeNotEmpty` | Type cannot be dropped, since it is not empty. 
15000 | 15007 | `ScErrMetalayerBufferTooSmall` | Internal metalayer buffer is too small. 
15000 | 15008 | `ScErrColumnsArentUnique` | Column definitions aren't unique. 
15000 | 15009 | `ScErrSchemasDoNotMatch` | Schema of given type does not match with schema of the existing type. 
15000 | 15010 | `ScErrMetaschemaVersionMismatch` | The version of the metaschema in the database does not match the metaschema version in Starcounter's binary. 
15000 | 15011 | `ScErrIndexExistsOnColumn` | The column is a part of an index. 
15000 | 15012 | `ScErrTableIsReferenced` | There are references to the table. 
15000 | 15013 | `ScErrTableHasChildren` | There are tables inheriting this table. 
15000 | 15014 | `ScErrColumnIsMapped` | There is a mapped property on this column. 
15000 | 15015 | `ScErrMapperFormatNotSupported` | The requested mapper format is not supported. 
15000 | 15016 | `ScErrRecordHasNoIdentity` | There is no identity associated with this record. 
15000 | 15017 | `ScErrIdentityHasNoRecord` | There is no record associated with this identity and type. 
15000 | 15018 | `ScErrMapperScLLVMError` | An ScLLVM error occurred while compiling or linking the mapper. 
15000 | 15019 | `ScErrMapperIoError` | An I/O error occurred while accessing the mapper's source code. 
15000 | 15020 | `ScErrMappingInconsistent` | The operation cannot be executed because at least one mapping is in an inconsistent state. 
15000 | 15021 | `ScErrVdbNotFound` | The virtual database does not exist. 
15000 | 15022 | `ScErrIllegalMapping` | An illegal mapping configuration was requested. 
15000 | 15023 | `ScErrMapperAbort` | The operation was aborted at the request of a mapper. 
15000 | 15024 | `ScErrColumnIsInherited` | This column is inherited from a base table. 

### `ScErrCodeNotEnhanced`, `1002`

Runtime error occuring when accessing code that needs to be enhanced in order to work, but for some reason hasn&#39;t,

### `ScErrThreadNotAttached`, `1004`

This can either indicate that the thread is temporarily detached from the scheduler or that it isn&#39;t a worker thread. Only certain operations differs between the two.

### `ScErrMaxThreadsReached`, `1013`

This error can occur when a large number of worker threads are blocked.

### `ScErrUnresposiveThreadStall`, `1018`

A cooperatively scheduled worker thread is considered unresponsive when it hasn&#39;t yielded for more then a minute but this error isn&#39;t generated unless the thread has been unresponsive for a user configured period of time (2 minutes by default). If a thread is detected to be unresponsive but not long enough for the error to be generated, a warning is generated instead.
The error is likely caused by an infinite loop or a deadlock but could also be caused by the thread doing something that doesn&#39;t give it the opportunity to yield for a very long period of time (which also is a real problem since the stall timeout is configured in minutes).

### `ScErrThreadsBlockedStall`, `1019`

The error is likely an indication of a deadlock or a highly contested lock held for a very long period of time.

### `ScErrTaskCleanupIncomplete`, `1020`

Error currently only raised when exclusive environment ownership hasn&#39;t been released when task terminated. This should not occur.

### `ScErrOperationCancelled`, `1034`

General error code representing a cancelled operation.

### `ScErrBadBaseDirectoryConfig`, `2002`

Applies if the specified directory doesn&#39;t exists or is invalid (for example if the specified path indicates a file), or if a base directory isn&#39;t specified when one is required.

### `ScErrBadTempDirectoryConfig`, `2003`

Applies if the specified directory doesn&#39;t exists or is invalid (for example if the specified path indicates a file).

### `ScErrBadImageDirectoryConfig`, `2004`

Applies if the specified directory doesn&#39;t exists or is invalid (for example if the specified path indicates a file), or if a image directory isn&#39;t specified when one is required.

### `ScErrBadTLogDirectoryConfig`, `2005`

Applies if the specified directory doesn&#39;t exists or is invalid (for example if the specified path indicates a file), or if a transaction log directory isn&#39;t specified when one is required.

### `ScErrLogModuleNotFound`, `2014`

Most likely caused by a faulty installation.

### `ScErrCantGetPrivilegesNeeded`, `2018`

This error code is obsolete in current version.

### `ScErrDatabaseFileMismatch`, `2022`

This means that one of the image files, the transaction log or all three files were created for different databases.

### `ScErrCodeLoaderError`, `2027`

General error produced by the code loader component. This error indicates that an operation during startup requiered some code to be accessed via the .NET reflection engine and that the loader cound not satisfy the request because of an error fetching the needed data. This usually indicates version mismatches or missing pe-files in the runtime library. The error lines up with one (or several) more specific errors. The later contains more detailed information about what specific code that couldnt load and can be found in the error log. This error ends up in the critical log (causing the database to fail startup). A corresponding and more specific error at the same time is written to the error log, describing what assembly failed and how to get even more error information.

### `ScErrTypesLoadError`, `2029`

The code loader requested to iterate types in a given (loaded) assembly, but this operation resulted in an error. This usually indicates some kind of reference error where one assembly or module references (direct or indirect) a type that was implied for the former to load properly. Results in a code loader error in the critical error log. See about this above. In companion with this error, Starcounter writes out what assembly failed to load. This might be enough for the developer, but if it isn&#39;t, even more information is availible. To get this additional and extended error information, the error in the error log comes equipped with a reference number (in the form of a GUID). This reference number can be used to find the information lining up with the given exception. To search for this information by reference, consult the Starcounter.Notice log. In the complementary log the following can be found:
- Number of types declared in the failing assembly, and how many of these that wheren&#39;t possible to load.
- Which of the above types that was possible to load (and from that, it should be clear which ones could not)
- The full set of assemblies currently loaded into the database.
- Exception messages (including file name of the target) for each type in the faulty assembly, indicating the broken link.
- Fusion log for each exception message above.

### `ScErrWeavingError`, `2030`

All specific weaver related errors use this general weaving error as an umbrella to communicate their existence to the user. That is, this general weaving error will be found in the critical logging branch of errors and all specific errors will be found in the extended error logging system (the error log). When this error is found in the critical error log, consult the error log for a corresponding ScErrWeavingFailed error. That error will head a list of more specific errors that are the real source for this error.

### `ScErrInvalidServerName`, `2031`

Indicates that the specified server name is to long (a server name is currently limited to 31 characters) or contains invalid characters.

### `ScErrShutdownTimedOut`, `2032`

The shutdown thread got stuck in user code and was aborted.

### `ScErrBadSchedCountConfig`, `2035`

Indicates that the configured number of virtual CPUs wasn&#39;t supported. Currently no more than 4 virtual CPUs is supported in the x64 version and no more then a 1 virtual CPU is supported in the x86 version.

### `ScErrWrongTLogVersion`, `2036`

The transaction log file is versioned and must match the version of the loading transaction logging runtime library when the database is started. If versions differ, this error will be in the error log and the database will fail to load.

### `ScErrBadRebuildConfig`, `2037`

Configuration given for either an unload operation or a reload counterpart violated some constraint. What type of operation (unload/reload) the error originated from will be indicated by the context, a parachute error for respective routine.
In current version, all configuration is done internally so this error should not occur. In previous versions it could occur because of bad settings in configuration for unload/reload.

### `ScErrModuleNotFound`, `2048`

Most likely caused by a faulty installation.

### `ScErrParsingAppArgs`, `2049`

Consult the system error code to find out more about what failed.

### `ScErrTooFewAppArgs`, `2050`

Consult the documentation to see what arguments Starcounter encforces, accepts and what constraints apply.

### `ScErrAppArgsSemanticViolation`, `2051`

Consult the documentation to see what arguments Starcounter encforces, accepts and what constraints apply.

### `ScErrFuncExportNotFound`, `2054`

Most likely caused by a faulty installation.

### `ScErrBadBackupDirectoryConfig`, `2055`

Backup directory is currently the same as output directory.

### `ScErrTLogNotInitialized`, `2058`

The transaction/redo log is considered uninitialized after it has been created and before it has successfully been started at least once.

### `ScErrAcquireDbControlTimeOut`, `2059`

Error occurs if unable to acquire the shared object used to control ownership of the database environment on startup.
Could indicate that the memory manager is unresponsive (scdata.exe). Try again after terminating this process.

### `ScErrCreateActMonBufTimeOut`, `2060`

Error occurs if unable create activity monitor shared buffer timed out.
Could indicate that the activity monitor is unresponsive (scactmon.exe) and has therefore not released the buffer. Try again after terminating the process.

### `ScErrDbStateAccessDenied`, `2061`

Check so that the user that runs the database or server has the priviledge to create/open globally named objects, &quot;SeCreateGlobalPrivilege&quot;.
Corresponds to windows error ERROR_ACCESS_DENIED.

### `ScErrClientBackendAlreadyLoaded`, `2064`

When the client runtime was trying to load the library that communicates with Starcounter databases, a library with the same name (&quot;sccoredb&quot;) was found already loaded.

### `ScErrClientBackendNotLoaded`, `2065`

When the client runtime was trying to load the library that communicates with Starcounter databases, the library was not found loaded after initialization.

### `ScErrClientBackendWrongPath`, `2066`

When the client runtime was trying to load the library that communicates with Starcounter databases, the library was found loaded from the wrong path.

### `ScErrStartSharedMemoryMonitor`, `2067`

Shared memory monitor process is started by each Starcounter server and is used to correctly free clients&#39; shared memory resources.

### `ScErrDbOpenMonitorInterface`, `2088`

If there is no monitor (scipcmonitor.exe) running in the same terminal server session as the database process, this is likely the cause of the error. If the database is to be started in session 0, it may help to start the Starcounter System Server service. If the database is to be started in session 1, it may help to start the Starcounter Administrator.

### `ScErrBadSchedIdSupplied`, `2106`

You can queue a job on scheduler with id from 0 to n-1, where n is a number of logical processors in the system.

### `ScErrBadDatabaseConfig`, `2109`

Generic error indicating that the configuration of a database is invalid preventing the database from being accessible.

### `ScErrStartNetworkGateway`, `2110`

Network gateway process is started by each Starcounter server and is used to provide communication for Starcounter with outside world.

### `ScErrBadServiceConfig`, `2113`

The service configuration file doesn&#39;t exist, isn&#39;t accessible or isn&#39;t formatted correctly.

### `ScErrBadServerConfig`, `2115`

The server configuration file doesn&#39;t exist, isn&#39;t accessible or isn&#39;t formatted correctly.

### `ScErrApplicationAlreadyHosted`, `2151`

This error usually indicate an app tries to start a host when the app is is either already running in the shared host, or has already started the host previously.

### `ScErrInvalidHandle`, `3001`

Usually implies that the object referenced by the handle has been freed.

### `ScErrBadChannelOptionValue`, `3003`

Error is raised when configuring a channel. This is usually done using values from named port configuration (I/O handler configuration) but if so, configuration should already have been validated and this error should not occur.

### `ScErrChannelReading`, `3011`

A new read operation can not be posted and certain options can not be set until the operation has completed.

### `ScErrConnectionRefused`, `3013`

This is usually the result when communication could be established with the remote system but the current system wasn&#39;t excepted as a client. An example is when trying to connect to a specific port and address and the addressed system was found but no one was accepting connection requests on that port.

### `ScErrEndPointInvalid`, `3014`

Operation failed because the channel couldn&#39;t be bound to the specified end point since it wasn&#39;t a valid end point.

### `ScErrEndPointOccupied`, `3015`

Operation failed because the a channel couldn&#39;t be bound to the specified end point since it already was occupied.

### `ScErrEndPointUnreachable`, `3016`

A connection couldn&#39;t be established because the specified end point couldn&#39;t be reached. When this error occurs it&#39;s probably because the current machine isn&#39;t connected to a network or only connected to a local network.

### `ScErrUnwritableMessage`, `3021`

This error occurs when trying to write a message containing 0 value bytes when 0 bytes is used as message terminator.

### `ScErrUnreadableMessage`, `3024`

Operation couldn&#39;t be completed because the contents of a message being read from by a channel couldn&#39;t be interpreted by the channel. Indicates that a message of an invalid format was passed to the process. The channel is closed when this error is detected since the input stream must be considered corrupted.

### `ScErrCantCreateProcessShare`, `3026`

Another process holds the memory file open. Could be a conflict with a client application reading server statistics although if the client application is properly implemented this is highly unlikely. In this case the issues should be resolved by trying again to start the server.

### `ScErrStringHeapOutOfSpace`, `4002`

String heap is used to store type and field names in the kernel. The maximum string heap size if fixed so the only way to resolve this is reduce the number of types and fields store in the database.

### `ScErrNoTransactionAttached`, `4003`

Operation failed because no transaction was attached to the thread when the operation only could be executed within the context of a transaction (this applies to most database operations). Note that this error can occur even if the transaction isn&#39;t activly detached from the running thread, should another thread force-claim the ownership of the transaction (by for example committing or rolling back the transaction).

### `ScErrCantCreateSchema`, `4009`

Indicates that schema creation failed. The reason why schema creation failed is logged before the error is raised.

### `ScErrDeletePending`, `4016`

If this error occurs it indicates a bug in the managed binding.

### `ScErrNoTransRefToRelease`, `4022`

Probably indicates that the reference counter is out of sync.

### `ScErrToManyTransactionRefs`, `4024`

Only a limited number of references can be registered (the reference counter is a 16-bit integer).

### `ScErrCantOpenImageFile`, `4028`

The most likely case of this error is that the image file does not exists in the configured director or is locked by another process. The reason for why the image file couldn&#39;t be opened is logged seperatly.

### `ScErrCantReadImageFile`, `4029`

The reason for the failure is logged seperatly.&quot;

### `ScErrCantWriteImageFile`, `4030`

The reason for the failure is logged seperatly.&quot;

### `ScErrTransactionLockedOnThread`, `4031`

Transaction failed because the current transaction was locked on the thread. This error occures if trying to change the current transaction of modify the state of the current transaction in a context where this isn&#39;t allowed (for example when executing a trigger).

### `ScErrErrorInHookCallback`, `4034`

This error code is for example set when there is an error in a commit hook which in turn causes the commit to fail.

### `ScErrPluginCodeViolation`, `4036`

This could either be a declarative violation or one what was caused during runtime due to a callback function that let an exception slip loose. Individual codes for plugin violations are logged separatly; this is the common error that is propagated to user code.

### `ScErrPluginInvocationException`, `4038`

Note that errors occuring due to the runtime (code access for ex) should be treated separatly if possible.

### `ScErrInvalidHookTarget`, `4039`

Hooks that implements custom filtering of targets must obey the constraints of targets as specified by the hooking documentation.

### `ScErrTransactionAlreadyBound`, `4040`

Indicates an attempt to attach a transaction to a thread when the transaction is already attached to another thread.

### `ScErrWeavingFailed`, `4041`

Information error connecting a sample of errors (always at least one) that is the cause of a general weaver error. Weaving errors use a reference approach, meaning that this error will inform the user about a unique reference that can be used to connect all errors and notices that are the underlying source for this error. With that reference it will be easy to find out more specific error information, assisting in correcting the problems that was discovered.

### `ScErrMissingDefinition`, `4042`

During schema deviation detection, a violation was found, indicating that the deployed database application code structure was different than the metadata found in the core.
To remedy this symptom, a database rebuild must be performed.

### `ScErrRefForbiddenUserCode`, `4043`

Maps to ATV01

### `ScErrIllegalAttributeAssign`, `4044`

Illegal field assignment. This error will be the result of user code assigning a construct in Starcounter that doesnt allow explicit assignment. Examples includes reference lists and sequence number fields. (Maps to ATV03)

### `ScErrDbClassCantBeGeneric`, `4046`

Maps to DCV01.

### `ScErrIllegalFinalizer`, `4047`

Maps to DCV02.

### `ScErrIllegalTypeRefDecl`, `4048`

This name has been reserved by Starcounter. (Maps to DCV03)

### `ScErrToComplexCtor`, `4049`

The &quot;initialization&quot; part of the constructor (i.e., the part before the base constructor is called) should be simple enough:
- May not declare lexical scopes (impossible in C#)
- May not declare exception handlers (impossible in C#)
- May not contain branching instructions (like constructs &lt;condition&gt;? &lt;true&gt; : &lt;false&gt; in C#).
Maps to DCV04.

### `ScErrFieldRedeclaration`, `4050`

Maps to DCV06.

### `ScErrIllegalExtCtor`, `4051`

An extension class was discovered and that class declared a constructor other than the default contructor. Extension classes are not allowed to declare any other constructors than the default contructor. (Maps to ECV01)

### `ScErrIllegalExtCreation`, `4052`

Code that explicitly instantiated a Starcounter extension class was discovered. This is not allowed. (Maps to ECV02)

### `ScErrIllegalExtCtorBody`, `4053`

This is currently not allowed. Note: I can&#39;t really remember why we have this limitation? We should remove it if it&#39;s there for no particular reason. (Maps to ECV03)

### `ScErrExtNotSealed`, `4054`

Maps to ECV04.

### `ScErrKindWrongName`, `4055`

A Society Object class was discovered as a kind (by extending Something.Kind) but it was named something other than &quot;Kind&quot;.
Note: As long as we provide a tight integration with Society Objects and we validate their code constrains in modules that targets their framework, I guess it is fair to have a set of error codes for those violations as well. However, these might be considered to be removed/moved in the future. (Maps to KCV02)

### `ScErrKindMissingConcept`, `4056`

All kind classes must be declared inside a Society Object concept class. A kind was found that wasn&#39;t. (Maps to KCV03)

### `ScErrKindIllegalParent`, `4057`

A kind class belonging to concept A must be derived from the closest declared kind class of A&#39;s base concept. A class that violated this was found. (Maps to KCV04)

### `ScErrKindMissingCtor`, `4058`

A kind class must have a default constructor, either an explicit one or the one provided by the compiler. (Maps to KCV05)

### `ScErrKindMissingParent`, `4059`

Maps to KCV06.

### `ScErrKindWrongVisibility`, `4060`

Maps to KCV09.

### `ScErrFieldComplexInit`, `4061`

A persistent field was equipped with an initialization statement (for example &quot;public int = 0;&quot;), but the statement was to complex for the current version of the code weaver to interpret. Consult weaver documentation about what initialization constructs are supported, and watch out for later versions of Starcounter, being able to support more complex initializations. (Maps to PFV02)

### `ScErrFieldRefMethod`, `4062`

A few restrictions regarding methods with reference parameters of persistent fields currently applies. When this error occurs, look at the method signature of the target method (part of the error message) and see if it can be implemented differently until there is support for more complex field-by-reference cases in later versions of the database. For more information about this, contact Starcounter. (Maps to PFV21)

### `ScErrSynNoTarget`, `4066`

A field was declared as a synonym, but the field it was declared as a synonym for could not be located by the loader. Make sure your synonym targets are accessible as fields in the current class (or a baseclass) and that they are also persistent. (Maps to PFV06)

### `ScErrSynTypeMismatch`, `4067`

There was a mismatch between a synonym and the target it specified. A synonym target must always be assignable for the synonym field and if and of the fields are instrict fields, the type must be a perfect match. (Maps to PFV07)

### `ScErrSynVisibilityMismatch`, `4068`

When a synonym references a target in another type that its own, the synonym is not allowed to have a wider visibility than the target. (Maps to PFV08)

### `ScErrSynReadOnlyMismatch`, `4069`

When a synonym references a target in another type that its own, the synonym must be readonly if the target is readonly. (Maps to PFV09)

### `ScErrSynTargetNotPersistent`, `4070`

When a synonym references a target, the target must be a persistent field and nothing else. (Maps to PFV12)

### `ScErrSynPrivateTarget`, `4071`

When a synonym references a target, and the target is in another type, the target must never have the private visibility. (Maps to PFV20)

### `ScErrPersPropNoTarget`, `4077`

A persistent property (in a known assembly) was declared. It supplied a reference to a target database field, but that field was not found when queried for by the loader. (Maps to PPV02)

### `ScErrTypeNameDuplicate`, `4078`

A class was being discovered by the analyzer and while analyzing, the analyzer detected that a type with the same name (full name) was allready discovered. (Maps to DCV07)

### `ScErrObjectDoesntExist`, `4083`

Indicates an attempt to access an object that for some reason doesn&#39;t exist. This error is raised if accessing or attempting to modify an object that has been deleted in another transaction before the start of the current transaction. This error is also used when attempting to access removed objects that exist outside the scope of transactions (like an index) or when accessing removed entity object without any notion of transactions (applies to certain metadata functions).

### `ScErrAttrNoKernelField`, `4085`

During schema deviation detection, a violation was found, indicating that the deployed database application code structure was different than the metadata found in the core.
To remedy this symptom, a database rebuild must be performed.

### `ScErrUnhandledTransactConflict`, `4090`

This error is raised by transaction scopes if they are unable to restart a conflicted transaction.

### `ScErrBinaryValueExceedsMaxSize`, `4091`

Maximum size allowed for binary data is 4080 bytes (4096 - 16).

### `ScErrRenameMissingSource`, `4099`

When specifying renaming instructions for classes and fields, Starcounter forces them to be part of the legacy schema, the one the renaming applies to. There are several probable causes for this error, all of them relating to bad specification of the previous name, and they will be listed on the documentation, helping developers understand what might have caused the error to be raised.

### `ScErrCantAccessImageFile`, `4118`

Check so that the user that runs the server has read and write access to the file and that the file isn&#39;t read-only.
Corresponds to windows error ERROR_ACCESS_DENIED.

### `ScErrImageFileLocked`, `4119`

Corresponds to windows error ERROR_SHARING_VIOLATION.

### `ScErrCantAccessTransactionLog`, `4121`

Check so that the user that runs the server has read and write access to the file and that the file isn&#39;t read-only.
Corresponds to windows error ERROR_ACCESS_DENIED.

### `ScErrTransactionLogLocked`, `4122`

Corresponds to windows error ERROR_SHARING_VIOLATION.

### `ScErrCantAccessDumpFile`, `4123`

Check so that the user that runs the server has read and write access to the file and that the file isn&#39;t read-only.
Corresponds to windows error ERROR_ACCESS_DENIED.

### `ScErrDumpFileLocked`, `4124`

Corresponds to windows error ERROR_SHARING_VIOLATION.

### `ScErrReloadConstraintViolation`, `4125`

The log will also contain which constraint or constraints that was violated.

### `ScErrFileTransNotSupported`, `4126`

Minimum version for File transactions is 6 (Windows Vista/Server 2008).

### `ScErrTypeBaseDeviation`, `4127`

During schema deviation detection, a violation was found, indicating that the deployed database application code structure was different than the metadata found in the core.
To remedy this symptom, a database rebuild must be performed.

### `ScErrFieldSignatureDeviation`, `4128`

During schema deviation detection, a violation was found, indicating that the deployed database application code structure was different than the metadata found in the core.
To remedy this symptom, a database rebuild must be performed.
This error code is used for deviating signatures on persistent fields declared in regular entity classes as well as in Starcounter extension classes.

### `ScErrSchemaDeviation`, `4129`

During schema deviation detection, at least one violation was found, indicating that the deployed database application code structure was different than the metadata found in the core.
To remedy this symptom, a database rebuild must be performed.

### `ScErrExtFieldMissingCore`, `4130`

During schema deviation detection, a violation was found, indicating that the deployed database application code structure was different than the metadata found in the core.
To remedy this symptom, a database rebuild must be performed.

### `ScErrIndexDeclarationDeviation`, `4131`

During schema deviation detection, a violation was found, indicating that the deployed database application code structure was different than the metadata found in the core.
To remedy this symptom, a database rebuild must be performed.

### `ScErrMissingEntityClass`, `4132`

During schema deviation detection, a violation was found, indicating that the deployed database application code structure was different than the metadata found in the core.
The reason for this error is that a class was either removed or improperly renamed. To remedy the symptom, a database rebuild must be performed.

### `ScErrMissingPersistentField`, `4133`

During schema deviation detection, a violation was found, indicating that the deployed database application code structure was different than the metadata found in the core.
The reason for this error is that a field was either removed or improperly renamed. To remedy the symptom, a database rebuild must be performed.

### `ScErrMissingExtensionClass`, `4134`

During schema deviation detection, a violation was found, indicating that the deployed database application code structure was different than the metadata found in the core.
The reason for this error is that an extension class was either removed, improperly renamed or was redefined to extend another class than it extended originally. To remedy the symptom, a database rebuild must be performed.

### `ScErrIndexDeclarationMissing`, `4135`

During schema deviation detection, a violation was found, indicating that the deployed database application code structure was different than the metadata found in the core.
The reason for this error is that an index previously defined have been removed/renamed. To remedy this symptom, a database rebuild must be performed.

### `ScErrHookCallbackNotBound`, `4136`

During schema deviation detection, a violation was found, indicating that the deployed database application code structure was different than the metadata found in the core.
The reason for this error is that either a new hook was added or that a callback on an existing hook was added. To remedy this symptom, a database rebuild must be performed.

### `ScErrHookCallbackNotInstalled`, `4137`

During schema deviation detection, a violation was found, indicating that the deployed database application code structure was different than the metadata found in the core.
The reason for this error is that a hook either was removed completly, referenced another target than it was originally, or that a particular callback of such a hook was removed. To remedy this symptom, a database rebuild must be performed.

### `ScErrCantBackupUnexpError`, `4144`

The reason for why the image file couldn&#39;t be backed up is logged seperatly.

### `ScErrStringValueExceedsMaxSize`, `4154`

The maximum size is set so that it&#39;s unlikely to be exceeded. If storing a larger string is needed then it will have to be stored as a large binary.
The maximum size of a string in characters is affected by contents of the string and varies between different collations. This since it&#39;s the size of the string packed that is restricted.

### `ScErrStringConversionFailed`, `4155`

This error may indicate a bug in the string formatter or data corruption.

### `ScErrReattachFailedBadDbState`, `4158`

Can occur either when processing commit hooks or a delete hook. Caused either by the thread ending the hook operation manually detached (which should not occur) or if an out of memory or similar occurs while attempting to reattach an auto detached thread.

### `ScErrCantInitCheckpUnexpError`, `4160`

The reason for why the checkpoint process could not be initialized is logged seperatly.

### `ScErrCommitPending`, `4162`

If this error occurs it indicates a bug in the managed binding.

### `ScErrCodeGenerationFailed`, `4164`

The error is usually due to a previous I/O error. If so that error will have been written to log.

### `ScErrDefinitionToLarge`, `4170`

The size of a definition depends on the number and type of attributes and the number and type indexes. Reducing the number of attributes and/or indexes might help resolve this problem.

### `ScErrInvalidObjectAccess`, `4174`

Occurs when transaction attempts to access an object that isn&#39;t accessible from the specific transaction.
This could mean that the object either didn&#39;t exist when the transaction started, was deleted within the scope of the transaction or was created within the scope of another transaction not yet committed.
It could also mean that the referenced object isn&#39;t accessible to the current transaction because it belongs to another container.
Only simple reads generates this error. If the error is detected in a write the transaction is aborted. See ScErrInvalidObjectAccessAbort.

### `ScErrSchemaCodeMismatch`, `4177`

Occurs when, for example, a string attribute is accessed as an integer.

### `ScErrLoadDynamicCodeFailed`, `4179`

The error is usually due to a previous I/O error. If so that error will have been written to log.

### `ScErrPersPropWrongCoreRef`, `4181`

A persistent property (in a known assembly) was declared. It specified a mapping to a database field in the core, but that field was not found when the class and it&#39;s ancestors where analyzed.

### `ScErrToManyOpenIterators`, `4182`

Only a limited number of open iterators is allowed at any given time. To avoid this error be sure to close iterators (releasing the result set) when done with them and don&#39;t keep too many iterators referenced at any given time.

### `ScErrCantCreateDbMemoryFile`, `4186`

This error occurs when the memory manager was unable to create a memory file for storing database data either when loading image or expanding database memory. If the failure is caused by an OS error code is logged seperatly. The error could also occur because the maximum number of memory files kept by the memory manager has been reached.
If the memory manager fails to create a memory file because it was unable to allocate sufficient memory from the OS then a ScErrOutOfMemory will be raised.

### `ScErrCantInitFlushUnexpError`, `4190`

The reason for why the checkpoint process could not be initialized is logged seperatly.

### `ScErrConnectInsideDatabase`, `4201`

Connecting to databases using Db.Connect is currently only supported from client applications. When executing inside a database, a connect is not needed since the code is implicitly connected to the running database.

### `ScErrClientEntityTypeUnknown`, `4202`

To use entity types in a client application, the client runtime must be aware of their code types. All the entity types in a given assembly is instantly known the first type one of them is referenced, as they are implicitly registering themself with the runtime. However, if a type is used in an SQL statement and has not previously been referenced by it&#39;s code type, the query will fail with this error.
To remedy this, you can explicitly add the entity types in a given assembly by issuing Db.Current.EnableClientAssembly(Assembly).

### `ScErrClientBackendNotInitialized`, `4203`

This error usually indicates that a client has not successfully called Db.Connect before it used a Starcounter method that needed to access something in the database.
Example of methods that can cause this error if they are called without the client first have successfully called Db.Connect is: Db.SQL, SqlResult.GetEnumerator, new T where T is an entity type.

### `ScErrAssemblyNotPreparedForClient`, `4204`

To use Starcounter binaries in a client application, the build system must prepare the code for client access during compilation. This is normally done in Visual Studio, under the project property page &quot;Starcounter&quot; in a Starcounter Library project. Make sure you set the &quot;Enable access from external process&quot; to TRUE.

### `ScErrBackingRetreivalFailed`, `4241`

A general backing retreival error, used when no more specific backing error applies. Exceptions with this error will carry more detailed information (such as postfixes describing the case, and inner exceptions) and we are strict when logging this when it occurs in a real (i.e. non-testing) context.

### `ScErrCantExpandTransLog`, `4248`

The reason for the failure is logged seperatly.&quot;

### `ScErrToManyIndexes`, `4276`

Keep in mind that this is evaluated using the total amount of indexes on a table, including inherited indexes. Create table where base table because of failure to add default index for this reason.

### `ScErrColumnNameNotUnique`, `4277`

Note that in case the table definition inherits another table definition the name of a defined column must not share the name of a inherited column.

### `ScErrLogWriterStillRunning`, `4298`

Database load can not proceed while log writer is still running and for some reason it won&#39;t shut down within an expected time frame. This could be because is stalled or because flushing pending log writes takes longer then expected.

### `ScErrRecordIDLimitReached`, `4299`

Database preconfigured maximum record ID has been reached. No more records can be inserted in the database.

### `ScErrTransactionLogPositionNotFound`, `4301`

Position passed is not found in the transaction log. Most likely appropriate transaction log files has been removed due to retention policy.

### `ScErrForbiddenDatabaseName`, `4304`

The database name must follow this [a-z,A-Z,0-9] naming scheme.

### `ScErrCantRunSharedAppHost`, `4305`

The shared app code host is managed by Starcounter and can not be explicitly executed by user code.

### `ScErrInvalidSessionId`, `5002`

The session with the id has either expired or never existed.

### `ScErrUnexpErrorGetDumpTmpName`, `6029`

Name is created using API function GetTempFileName.

### `ScErrUnexpErrorExecuteCompile`, `6051`

This error is probably due to a faulty installation. Likely the used compiler is not properly installed.

### `ScErrUnexpectedCompilerError`, `6052`

This error should not occur. Please contact Starcounter support.

### `ScErrSqlExportSchemaFailed`, `7001`

The schema file is used by external process scsqlparser.exe.

### `ScErrSqlDuplicatedIdentifier`, `7008`

Since the SQL is case insensitive such case insensitive ambiguity can not be allowed.

### `ScErrConstraintViolationAbort`, `8001`

A constraint violation is most likely caused by a bug, restarting a transaction that&#39;s aborted for this reason is therefore not advisable since it&#39;s very likely that the same error will occur again and again each time the transaction commits.

### `ScErrTransactionConflictAbort`, `8002`

Accessing an object that has been deleted before the transaction started could also cause the current transaction to abort because of a perceived conflict. This can happen if an object is accessed within another transaction then the transaction the object was fetched in and the object at that time has been deleted. The reason for this is that the database engine can&#39;t determine when the object was deleted and therefore assumes that the object existed when the transaction started because of the assumption that the reference was fetched within the current transaction. To avoid this one should only access objects fetched within the context of the current transaction is possible.

### `ScErrInvalidObjectAccessAbort`, `8006`

Occurs when transaction attempts to access an object that isn&#39;t accessible from the specific transaction.
This could mean that the object either didn&#39;t exist when the transaction started, was deleted within the scope of the transaction or was created within the scope of another transaction not yet committed.
It could also mean that the referenced object isn&#39;t accessible to the current transaction because it belongs to another container.
Simple read operations that fails because of an invalid object access does not abort the transaction but instead generates a ScErrInvalidObjectAccess error. error.

### `ScErrSchemaCodeMismatchAbort`, `8007`

Occurs when for example a string attribute was written to as an integer. Should not occur unless there is a problem with the binding.

### `ScErrInconsistentCodeAbort`, `8008`

Occurs when for example an attribute is written as an attribute of a certain type and later read or written as an attribute of a different type.

### `ScErrCodeConstrViolationAbort`, `8014`

This is most likely due to a bug in database binding, not in user code.

### `ScErrServerNotRunning`, `10003`

This error is used when a management client, such as star.exe, can not communicate properly with the admin server and decide it is because the admin server process is not running.
See also: ScErrServerNotAvailable

### `ScErrServerCommandFailed`, `10011`

This error is used as a general error for all server commands that don&#39;t provide a custom, more specific one.
Example message when expanded: &quot;The operation &quot;Starting database &#39;MyDatabase&#39;&quot; failed.&quot;.

### `ScErrUnexpectedCommandException`, `10016`

The server detected that a command failed with an exception that was not an exception based on a Starcounter error code (and hence unexpected). The exception details are logged along with this error.
Example message when expanded: &quot;Starting database &#39;MyDatabase&#39; failed due to an unexpected problem&quot;.

### `ScErrExecutableNotFound`, `10019`

Error used mainly by the server library when asked to run an executable and the executable file could not be found where specified. The error message should include the full path used (if possible).

### `ScErrServerNotAvailable`, `10020`

This error is used when a management client, such as star.exe, can not communicate properly with the admin server and the exact reason why doesn&#39;t map one to one to any other, more specific Starcounter error (such as ScErrServerNotRunning).
When constructing and logging/showing an error message based on this code, make sure to supply more low-level, detailed info such as socket level error codes, etc, to make support for this easier.
See also: ScErrServerNotRunning

### `ScErrCommandPreconditionFailed`, `10023`

This error allows server command processors to report about a condition failing to be met based on a given fingerprint. The server design for fingerprint matching is closely related and largely influenced by the HTTP Etag- and conditional invocation concepts and hence, this error can be considered the equivivalent to HTTP status 412.

### `ScErrDatabaseEngineNotRunning`, `10024`

Indicates a management operation was attempted but could not succeed because the database engine was not running, as expected too.

### `ScErrExecutableAlreadyRunning`, `10025`

Indicates an executable is already loaded in a certain database engine and that state was considered worth reporting as an error.

### `ScErrDatabaseRunning`, `10031`

Indicates a management operation was attempted but could not succeed because the database was running. The operation might succeed if the database is first stopped, and the operation is issued again.

### `ScErrWeaverFileNotFound`, `12008`

Occurs when the weaver executable is given a file that can not be found.

### `ScErrWeaverFileNotSupported`, `12009`

Occurs when the weaver executable is given a file with an extension that is not supported.

### `ScErrDebugFailedConnectToServer`, `12010`

Occurs when the Visual Studio extension runs the debugging sequence, in order to debug a Starcounter module, and the server (i.e. Starcounter Administrator) does not answer to the request being sent over the pipe. Usually indicates that the server is not running.

### `ScErrDebugFailedServerErrorStarting`, `12011`

Occurs when the Visual Studio extension runs the debugging sequence, in order to debug a Starcounter module, and the server (i.e. Starcounter Administrator) reports an error in response to the request to weave and host the module inside the user code process.

### `ScErrDebugFailedReported`, `12012`

Used internally to communicate the failure of the debug sequence using an exception. Indicates the debug sequence has failed, but that the raising component has logged detailed error messages to the error list and hence, no such action must be taken by the catching party.

### `ScErrDebuggerAlreadyAttached`, `12013`

Indicates the debugging of a Starcounter application failed b/c attaching the debugger was not possible, since a debugger was already attached to the database code host process the about-to-be-debugged application targets.

### `ScErrDebugDbHigherPrivilege`, `12014`

To resolve this, either restart Visual Studio and run it as an administrator, or make sure the database runs in non-elevated mode.

### `ScErrDebugNoDbProcess`, `12015`

This indicate the database process terminated in between it was started and when Visual Studio tried attaching a debugger to it.

### `ScErrBadGatewayConfig`, `13005`

The gateway configuration file doesn&#39;t exist, isn&#39;t accessible or isn&#39;t formatted correctly.

### `ScErrJsonPropertyNotFound`, `14003`

Error code used when parsing a Json document against a well-defined template and a given property name is not found in the template.

### `ScErrJsonValueWrongType`, `14004`

Error code used when parsing a Json document against a well-defined template and a given property value is not valid according to the type of the property. For example, when doing &quot;Name&quot;:false and &quot;Name&quot; is a string.

### `ScErrJsonUnexpectedEndOfContent`, `14005`

Error code used when populating a typed json object using plain json as input and the end of the content is reached before the end of the json.

_THIS FILE IS AUTOMATICALLY GENERATED. DO NOT EDIT._
_Generated on: UTC 2019-12-11 09:35:03_