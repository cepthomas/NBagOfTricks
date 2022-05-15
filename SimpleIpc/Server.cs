using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace NBagOfTricks.SimpleIpc
{
    /// <summary>Possible states/outcomes.</summary>
    enum ConnectionStatus
    {
        Idle,           // Not connected, waiting.
        Receiving,      // Connected, collecting string.
        ValidMessage,   // Good message completed.
        Error           // Bad thing happened.
    }

    /// <summary>Per connection.</summary>
    class ConnectionState
    {
        public byte[] Buffer { get; set; } = new byte[1024];
        public int BufferIndex { get; set; } = 0;
        public ConnectionStatus Status { get; set; } = ConnectionStatus.Idle;
        public string Message { get; set; } = "";
    }

    /// <summary>Notify client of some connection event.</summary>
    public class ServerEventArgs : EventArgs
    {
        public string Message { get; set; } = "";
        public bool Error { get; set; } = false;
    }

    public sealed class Server : IDisposable
    {
        /// <summary>Named pipe name.</summary>
        readonly string _pipeName;

        /// <summary>The server thread.</summary>
        Thread? _thread = null;

        /// <summary>Flag to unblock the listen and end the thread.</summary>
        bool _running = true;

        /// <summary>Something happened. Client will have to take care of thread issues.</summary>
        public event EventHandler<ServerEventArgs>? ServerEvent;

        /// <summary>The canceller.</summary>
        readonly ManualResetEvent _cancelEvent = new(false);

        /// <summary>My logger.</summary>
        readonly MpLog _log;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="pipeName"></param>
        /// <param name="logfn"></param>
        public Server(string pipeName, string logfn)
        {
            _pipeName = pipeName;
            _log = new(logfn, "SERVER");
        }

        /// <summary>
        /// Run it.
        /// </summary>
        public void Start()
        {
            _thread = new(ServerThread);
            _thread.Start();
        }

        /// <summary>
        /// Stop the server - called from main.
        /// </summary>
        /// <returns></returns>
        public bool Stop()
        {
            bool ok = true;

            _log.Write($"Stop()");

            _running = false;
            _cancelEvent.Set();

            _log.Write($"Shutting down");
            _thread?.Join();
            _log.Write($"Thread ended");
            _thread = null;

            return ok;
        }

        /// <summary>
        /// Required.
        /// </summary>
        public void Dispose()
        {
            if(_running)
            {
                Stop();
            }

            _cancelEvent.Dispose();
        }

        /// <summary>
        /// Listen for client messages. Interruptible by setting _cancelEvent.
        /// </summary>
        void ServerThread()
        {
            _log.Write($"Main thread started");

            try
            {
                while (_running)
                {
                    using (NamedPipeServerStream stream = new(_pipeName, PipeDirection.In, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous))
                    using (AutoResetEvent connectEvent = new(false))
                    {
                        ServerEventArgs evt = new();

                        _log.Write($"BeginWaitForConnection()");

                        AsyncCallback callBack = new(ProcessClient);

                        ConnectionState cst = new();

                        stream.BeginWaitForConnection(callBack, cst);

                        // Check for events of interest.
                        int sig = WaitHandle.WaitAny(new WaitHandle[] { connectEvent, _cancelEvent });
                        switch (sig)
                        {
                            case 0:
                                // Normal, ignore.
                                break;

                            case 1:
                                _log.Write($"Normal stop signal");
                                _running = false;
                                break;

                            default:
                                _log.Write($"Unknown wait result:{sig}");
                                _running = false;
                                break;
                        }

                        ///// The actual worker callback.
                        void ProcessClient(IAsyncResult ar)
                        {
                            // This is running in a new thread. Wait for something to show up.
                            if(ar is not null && ar.AsyncState is not null)
                            {
                                ConnectionState state = (ConnectionState)(ar.AsyncState);

                                try
                                {
                                    _log.Write($"EndWaitForConnection()");
                                    stream.EndWaitForConnection(ar);
                                    _log.Write($"Client wants to tell us something");

                                    state.Status = ConnectionStatus.Receiving;

                                    while (state.Status == ConnectionStatus.Receiving)
                                    {
                                        // The total number of bytes read into the buffer or 0 if the end of the stream has been reached.
                                        var numRead = stream.Read(state.Buffer, state.BufferIndex, state.Buffer.Length - state.BufferIndex);
                                        _log.Write($"num read:{numRead}");

                                        if (numRead > 0)
                                        {
                                            state.BufferIndex += numRead;

                                            // Full string arrived?
                                            int terminator = Array.IndexOf(state.Buffer, (byte)'\n');
                                            if (terminator >= 0)
                                            {
                                                // Make buffer into a string.
                                                string msg = new UTF8Encoding().GetString(state.Buffer, 0, terminator);

                                                _log.Write($"Got message:{msg}");

                                                // Process the line.
                                                evt.Message = msg;
                                                evt.Error = false;

                                                // Reset.
                                                state.BufferIndex = 0;
                                                state.Status = ConnectionStatus.ValidMessage;
                                            }
                                        }

                                        // Wait a bit.
                                        Thread.Sleep(50);
                                    }
                                }
                                catch (ObjectDisposedException er)
                                {
                                    state.Status = ConnectionStatus.Error;
                                    evt.Message = $"Client pipe is closed: {er.Message}";
                                    evt.Error = true;
                                }
                                catch (IOException er)
                                {
                                    state.Status = ConnectionStatus.Error;
                                    evt.Message = $"Client pipe connection has been broken: {er.Message}";
                                    evt.Error = true;
                                }
                                catch (Exception er)
                                {
                                    state.Status = ConnectionStatus.Error;
                                    evt.Message = $"Client pipe unknown exception: {er.Message}";
                                    evt.Error = true;
                                }

                                // Hand back what we captured.
                                ServerEvent?.Invoke(this, evt);
                            }

                            // Signal completion.
                            connectEvent.Set();
                        }
                        ///// End of ProcessClient() callback
                    }
                }
            }
            catch (Exception ee)
            {
                // General server error.
                ServerEvent?.Invoke(this, new ServerEventArgs()
                {
                    Message = $"Unknown server exception: {ee.Message}",
                    Error = true
                });
            }
    
            _log.Write($"Main thread ended");
        }
    }
}
