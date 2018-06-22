﻿namespace FoundationDbNet
{
    using System;
    using System.Threading;
    using FoundationDbNet.Native;

    /// <summary>
    /// A Singleton holder class for configuring the FoundationDb basic client.
    ///
    /// Callers are expected to call at least <see cref="Initialize"/> before calling any other methods on this class.
    /// </summary>
    public sealed class Fdb : IDisposable
    {
        public static Fdb Instance { get; } = new Fdb();

        private readonly object _lockObj;
        private readonly Thread _networkThread;

        private int _selectedApiVersion;

        private Fdb()
        {
            _lockObj = new object();

            _networkThread = new Thread(NetworkProcessingLoop)
            {
                Name = "fdb-network-thread",
                IsBackground = true,
                Priority = ThreadPriority.AboveNormal
            };

            _selectedApiVersion = -1;
        }

        public bool Initialized { get; private set; }

        public bool IsAlive => _networkThread.IsAlive;

        private bool IsOnNetworkThread => Thread.CurrentThread.ManagedThreadId == _networkThread.ManagedThreadId;

        public void Initialize(int apiVersion)
        {
            if (apiVersion == _selectedApiVersion)
            {
                return;
            }

            lock (_lockObj)
            {
                if (_selectedApiVersion != -1)
                {
                    throw new InvalidOperationException(nameof(Initialize) +
                                                        " was called multiple times. This is not supported");
                }

                // Set fdb_c API version
                NativeMethods
                    .fdb_select_api_version_impl(apiVersion, FdbConstants.FdbApiVersion)
                    .EnsureSuccess();

                Interlocked.Exchange(ref _selectedApiVersion, apiVersion);

                // Setup network
                NativeMethods
                    .fdb_setup_network()
                    .EnsureSuccess();

                // Start networking

                Thread.BeginCriticalRegion();

                RegisterShutdownHook();
                _networkThread.Start();

                Thread.EndCriticalRegion();

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

            if (IsAlive)
            {
                NativeMethods.fdb_stop_network();
                _networkThread.Join();
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
            NativeMethods
                .fdb_run_network()
                .EnsureSuccess();
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
