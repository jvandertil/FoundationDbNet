namespace FoundationDbNet.Native.Networking
{
    using System;
    using System.Threading;

    internal static class FdbNetwork
    {
        private const int MaxApiVersion = 520;

        private static readonly object SyncRoot = new object();

        private static int _selectedApiVersion = 0;
        private static int _started = 0;
        private static int _stopped = 0;

        public static void Setup(int apiVersion)
        {
            lock (SyncRoot)
            {
                if (_selectedApiVersion != 0
                    && _selectedApiVersion == apiVersion)
                {
                    return;
                }

                if (_selectedApiVersion != 0
                    && _selectedApiVersion != apiVersion)
                {
                    throw new InvalidOperationException($"ApiVersion already specified as '{_selectedApiVersion}', can only be set once.");
                }

                NativeMethods
                    .fdb_select_api_version_impl(apiVersion, MaxApiVersion)
                    .EnsureSuccess();

                NativeMethods
                    .fdb_setup_network()
                    .EnsureSuccess();

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

            NativeMethods.fdb_run_network()
                .EnsureSuccess();
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

                NativeMethods.fdb_stop_network()
                    .EnsureSuccess();

                Interlocked.Increment(ref _stopped);
            }
        }
    }
}
