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
using NBagOfTricks;


namespace NBagOfTricks.SimpleIpc
{
    /// <summary>Possible outcomes.</summary>
    public enum ServerStatus { Ok, Message, Error }

    public class ServerEventArgs : EventArgs
    {
        public ServerStatus Status { get; set; } = ServerStatus.Ok;
        public string Message { get; set; } = "";
    }

    public class Server : IDisposable
    {
        /// <summary>Named pipe name.</summary>
        readonly string _pipeName;

        /// <summary>The server thread.</summary>
        Thread _thread = null;

        /// <summary>Flag to unblock the listen and end the thread.</summary>
        bool _running = true;

        /// <summary>Something happened. Client will have to take care of thread issues.</summary>
        public event EventHandler<ServerEventArgs> ServerEvent = null;

        /// <summary>The canceller.</summary>
        readonly ManualResetEvent _cancelEvent = new ManualResetEvent(false);

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
            _log = new MpLog(logfn, "SERVER");
        }

        /// <summary>
        /// Run it.
        /// </summary>
        public void Start()
        {
            _thread = new Thread(ServerThread);
            _thread.Start();
        }

        /// <summary>
        /// Kill the server.
        /// </summary>
        /// <returns></returns>
        public bool Kill()
        {
            bool ok = true;

            _log.Write($"Kill()");

            _running = false;
            _cancelEvent.Set();

            _log.Write($"Shutting down");
            _thread.Join();
            _log.Write($"Thread ended");
            _thread = null;

            return ok;
        }

        /// <summary>
        /// Required.
        /// </summary>
        public void Dispose()
        {
            _cancelEvent.Dispose();
        }

        /// <summary>
        /// Listen for client messages. Interruptible by setting _cancelEvent.
        /// </summary>
        void ServerThread()
        {
            var buffer = new byte[1024];
            var index = 0;

            _log.Write($"thread started");

            while (_running)
            {
                using (var stream = new NamedPipeServerStream(_pipeName, PipeDirection.In, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous))
                using (AutoResetEvent connectEvent = new AutoResetEvent(false))
                {
                    Exception eserver = null;

                    try
                    {
                        _log.Write($"before BeginWaitForConnection()");
                        stream.BeginWaitForConnection(ar =>
                        {
                            try
                            {
                                // This is running in a new thread.
                                _log.Write($"before EndWaitForConnection()");
                                stream.EndWaitForConnection(ar);
                                _log.Write($"after EndWaitForConnection() - client connected");

                                // A client wants to tell us something.
                                bool done = false;
                                int retries = 0;

                                while(!done)
                                {
                                    if(retries++ < 10)
                                    {
                                        var numRead = stream.Read(buffer, index, buffer.Length - index);
                                        _log.Write($"num read:{numRead}");

                                        if (numRead > 0)
                                        {
                                            index += numRead;

                                            // Full string arrived?
                                            int terminator = Array.IndexOf(buffer, (byte)'\n');

                                            if (terminator >= 0)
                                            {
                                                done = true;

                                                // Make buffer into a string.
                                                string msg = new UTF8Encoding().GetString(buffer, 0, terminator);

                                                _log.Write($"got message:{msg}");

                                                // Process the line.
                                                ServerEvent?.Invoke(this, new ServerEventArgs() { Message = msg, Status = ServerStatus.Message });

                                                // Reset buffer.
                                                index = 0;
                                            }
                                        }

                                        if(!done)
                                        {
                                            // Wait a bit.
                                            Thread.Sleep(50);
                                        }
                                    }
                                    else
                                    {
                                        // Timed out waiting for client.
                                        _log.Write($"Timed out waiting for client eol", true);
                                        ServerEvent?.Invoke(this, new ServerEventArgs() { Message = $"Timed out waiting for client eol", Status = ServerStatus.Error });
                                        done = true;
                                    }
                                }
                            }
                            catch (Exception er)
                            {
                                // Pass any exceptions back to the main thread for handling.
                                eserver = er;
                            }

                            // Signal completion. Blows up on shutdown - not sure why.
                            if (!connectEvent.SafeWaitHandle.IsInvalid && !connectEvent.SafeWaitHandle.IsClosed)
                            {
                                connectEvent.Set();
                            }

                        }, null);
                    }
                    catch (Exception ee)
                    {
                        eserver = ee;
                    }

                    // Wait for events of interest.
                    int sig = -1;
                    if (!connectEvent.SafeWaitHandle.IsInvalid &&
                        !connectEvent.SafeWaitHandle.IsClosed &&
                        !_cancelEvent.SafeWaitHandle.IsInvalid &&
                        !_cancelEvent.SafeWaitHandle.IsClosed)
                    {
                        sig = WaitHandle.WaitAny(new WaitHandle[] { connectEvent, _cancelEvent });
                    }

                    if (sig == 1)
                    {
                        _log.Write($"shutdown sig");
                        _running = false;
                    }
                    else if (eserver != null)
                    {
                        _log.Write($"exception:{eserver}", true);
                        throw eserver; // rethrow
                    }
                    // else done with this stream.
                }
            }

            _log.Write($"thread ended");
        }
    }
}
