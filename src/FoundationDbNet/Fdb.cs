namespace FoundationDbNet
{
    using System;
    using System.Threading;
    using FoundationDbNet.Native.Networking;

    /// <summary>
    /// A Singleton holder class for configuring the FoundationDb basic client.
    ///
    /// Callers are expected to call at least <see cref="Initialize"/> before calling any other methods on this class.
    /// </summary>
    public sealed class Fdb : IDisposable
    {
        public static Fdb Instance { get; } = new Fdb();

        private readonly object _syncRoot;
        private readonly Thread _networkThread;

        private Fdb()
        {
            _syncRoot = new object();

            _networkThread = new Thread(NetworkProcessingLoop)
            {
                Name = "fdb-network-thread",
                IsBackground = true,
                Priority = ThreadPriority.Normal
            };
        }

        public bool Initialized { get; private set; }

        public bool IsAlive => _networkThread.IsAlive;

        private bool IsOnNetworkThread => Thread.CurrentThread.ManagedThreadId == _networkThread.ManagedThreadId;

        public void Initialize(int apiVersion)
        {
            lock (_syncRoot)
            {
                FdbNetwork.Setup(apiVersion);

                // Start networking
                RegisterShutdownHook();
                _networkThread.Start();

                Initialized = true;
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

            lock (_syncRoot)
            {
                if (IsAlive)
                {
                    FdbNetwork.Stop();
                    _networkThread.Join();
                }
            }
        }

        /// <summary>
        /// Disposes this instance. See <see cref="Terminate"/>.
        /// </summary>
        public void Dispose()
        {
            Terminate();
        }

        private void NetworkProcessingLoop()
        {
            FdbNetwork.Start();
        }

        private void RegisterShutdownHook()
        {
            var domain = AppDomain.CurrentDomain;

            if (domain.IsDefaultAppDomain())
            {
                domain.ProcessExit += ShutdownEventHandler;
            }
            else
            {
                domain.DomainUnload += ShutdownEventHandler;
            }

            void ShutdownEventHandler(object sender, EventArgs args) => Terminate();
        }
    }
}
