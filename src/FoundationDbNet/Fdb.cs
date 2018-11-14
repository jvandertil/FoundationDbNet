namespace FoundationDbNet
{
    using System;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Threading.Tasks;
    using FoundationDbNet.Logging;
    using FoundationDbNet.Native.Futures;
    using FoundationDbNet.Native.Networking;
    using FoundationDbNet.Native.SafeHandles;

    public sealed class Fdb
    {
        private static readonly ILog Logger = LogProvider.For<Fdb>();

        public static Fdb Instance { get; } = new Fdb();

        private readonly object _syncRoot;
        private readonly Thread _networkThread;

        private Fdb()
        {
            _syncRoot = new object();

            _networkThread = new Thread(FdbNetwork.Start)
            {
                Name = "fdb-network-thread",
                IsBackground = true,
            };
        }

        private bool IsAlive => _networkThread.IsAlive;

        private bool IsOnNetworkThread => Thread.CurrentThread.ManagedThreadId == _networkThread.ManagedThreadId;

        public Task<IFdbConnection> OpenDefaultClusterAsync()
        {
            Logger.Info("Opening default cluster file.");

            return OpenClusterFileAsync(null);
        }

        public Task<IFdbConnection> OpenClusterAsync(string clusterFile)
        {
            if (string.IsNullOrWhiteSpace(clusterFile))
            {
                throw new ArgumentException("Cluster file not specified.", nameof(clusterFile));
            }

            LogOpeningClusterFile();

            return OpenClusterFileAsync(clusterFile);

            void LogOpeningClusterFile()
            {
                if (Logger.IsInfoEnabled())
                {
                    Logger.Info("Opening cluster file: '{clusterFile}'.", clusterFile);
                }
            }
        }

        public void SelectApiVersion(int apiVersion)
        {
            lock (_syncRoot)
            {
                FdbNetwork.Setup(apiVersion);

                // Start networking
                RegisterShutdownHook();
                _networkThread.Start();
            }
        }

        /// <summary>
        /// Terminates this instance.
        ///
        /// Note that all calls to any API provided by this library are invalid after calling this.
        /// </summary>
        public void Terminate()
        {
            if (IsOnNetworkThread)
            {
                // You can't call this method from the network thread. It might deadlock.
                throw new InvalidOperationException("You can not call this method from the network thread.");
            }

            Logger.Info("Terminating FoundationDB network loop.");

            lock (_syncRoot)
            {
                if (IsAlive)
                {
                    FdbNetwork.Stop();
                    _networkThread.Join();
                }
                else
                {
                    Logger.Info("FoundationDB network loop already stopped.");
                }
            }
        }

        private void RegisterShutdownHook()
        {
            var domain = AppDomain.CurrentDomain;

            if (domain.IsDefaultAppDomain())
            {
                Logger.Info("Registered shutdown hook on ProcessExit.");
                domain.ProcessExit += ShutdownEventHandler;
            }
            else
            {
                Logger.Info("Registered shutdown hook on DomainUnload.");
                domain.DomainUnload += ShutdownEventHandler;
            }

            void ShutdownEventHandler(object sender, EventArgs args) => Terminate();
        }

        private static Task<IFdbConnection> OpenClusterFileAsync(string clusterFile)
        {
            var result =
                fdb_create_cluster(clusterFile)
                    .SafeMap(h => new FdbClusterFuture(h))
                    .ToTask()
                    .ContinueWith<IFdbConnection>(
                        future => future.Result.SafeMap(h => new FdbConnection(h)),
                        TaskContinuationOptions.OnlyOnRanToCompletion);

            return result;
        }

        [DllImport(FdbConstants.FdbDll, CharSet = CharSet.Ansi)]
        private static extern FdbFutureHandle fdb_create_cluster(string clusterFilePath);
    }
}
