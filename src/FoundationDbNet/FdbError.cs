namespace FoundationDbNet
{
    using System;

    /// <summary>
    /// Contains the values of error codes returned by FoundationDB API calls.
    /// </summary>
    [Serializable]
    internal enum FdbError : int
    {
        Success = 0,
        OperationFailed = 1000,
        OperationTimedOut = 1004,
        TransactionTooOld = 1007,
        FutureVersionRequested = 1009,
        // not_committed
        CommitConflict = 1020,
        CommitUnknownResult = 1021,
        TransactionCancelled = 1025,
        TransactionTimedOut = 1031,
        TooManyWatches = 1032,
        WatchesDisabled = 1034,
        ClientInvalidOperation = 2000,
        NetworkAlreadySetup = 2009,
        InvalidDatabaseName = 2013,
        ApiVersionAlreadySet = 2201,
        UnknownError = 4000,
        InternalError = 4100
    }
}
