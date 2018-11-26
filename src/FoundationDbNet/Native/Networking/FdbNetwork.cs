namespace FoundationDbNet.Native.Networking
{
    using System;
    using System.Runtime.InteropServices;
    using System.Threading;
    using FoundationDbNet.Logging;

    internal static class FdbNetwork
    {
        private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();
        private static readonly object SyncRoot = new object();

        private static int _selectedApiVersion = 0;
        private static int _started = 0;
        private static int _stopped = 0;

        public static void Setup(int apiVersion)
        {
            if (apiVersion <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(apiVersion), apiVersion, "Should be > 0. See the documentation for valid values.");
            }

            Logger.Info("Setting FoundationDB API version to '{0}'.", apiVersion);

            if (_selectedApiVersion != 0
                && _selectedApiVersion == apiVersion)
            {
                Logger.Warn("Tried to set FoundationDB API to '{0}' twice. Only call this method once!", apiVersion);

                return;
            }

            lock (SyncRoot)
            {
                if (_selectedApiVersion != 0
                    && _selectedApiVersion != apiVersion)
                {
                    throw new InvalidOperationException($"ApiVersion already specified as '{_selectedApiVersion}', can only be set once.");
                }

                fdb_select_api_version_impl(apiVersion, GetMaxApiVersion())
                    .EnsureSuccess();
                Logger.Debug("Selected API version {0}.", apiVersion);

                fdb_setup_network()
                    .EnsureSuccess();
                Logger.Debug("FoundationDB network setup completed.");

                Interlocked.Exchange(ref _selectedApiVersion, apiVersion);
            }
        }

        public static void Start()
        {
            lock (SyncRoot)
            {
                if (_selectedApiVersion == 0)
                {
                    throw new InvalidOperationException("Can not start network loop, not initialized. Please call '" +
                                                        nameof(Setup) + "' before calling '" + nameof(Start) + "'.");
                }

                if (_stopped > 0)
                {
                    throw new InvalidOperationException("Can not start network loop once it has been stopped.");
                }

                if (_started > 0)
                {
                    throw new InvalidOperationException("Can not start network loop more than once.");
                }

                Interlocked.Increment(ref _started);
            }

            Logger.Info("Starting FoundationDB network loop. Current ManagedThreadId: {0}.", Thread.CurrentThread.ManagedThreadId);

            var result = fdb_run_network();

            if (result != FdbError.Success)
            {
                Logger.Fatal("FoundationDB network loop terminated with an error! Error code: {0}.", result);
                result.EnsureSuccess();
            }
            else
            {
                Logger.Info("FoundationDB network loop terminated with status code '{0}'.", result);
            }
        }

        public static void Stop()
        {
            lock (SyncRoot)
            {
                if (_started == 0)
                {
                    throw new InvalidOperationException("Can not stop network loop. Not started.");
                }

                if (_stopped > 0)
                {
                    return;
                }

                var result = fdb_stop_network();
                Interlocked.Increment(ref _stopped);

                result.EnsureSuccess();
            }
        }

        public static int GetMaxApiVersion()
        {
            return fdb_get_max_api_version();
        }

        [DllImport(FdbConstants.FdbDll)]
        private static extern FdbError fdb_select_api_version_impl(int runtimeVersion, int headerVersion);


        [DllImport(FdbConstants.FdbDll)]
        private static extern int fdb_get_max_api_version();

        [DllImport(FdbConstants.FdbDll)]
        private static extern FdbError fdb_setup_network();

        [DllImport(FdbConstants.FdbDll)]
        private static extern FdbError fdb_run_network();

        [DllImport(FdbConstants.FdbDll)]
        private static extern FdbError fdb_stop_network();
    }
}
