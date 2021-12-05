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
    public enum ClientStatus { Ok, Timeout, Error }

    /// <summary>Companion client to server. This runs in a new process.</summary>
    public class Client
    {
        /// <summary>Pipe name.</summary>
        readonly string _pipeName;

        /// <summary>My logger.</summary>
        readonly MpLog _log;

        /// <summary>Caller may be able to use this.</summary>
        public string Error { get; set; } = "";

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="pipeName">Pipe name to use.</param>
        /// <param name="logfn"></param>
        public Client(string pipeName, string logfn)
        {
            _pipeName = pipeName;
            _log = new MpLog(logfn, "CLIENT");
        }

        /// <summary>
        /// Blocking send string.
        /// </summary>
        /// <param name="s">String to send.</param>
        /// <param name="timeout">Msec to wait for completion.</param>
        /// <returns></returns>
        public ClientStatus Send(string s, int timeout)
        {
            ClientStatus res = ClientStatus.Ok;

            try
            {
                using (var pipeClient = new NamedPipeClientStream(".", _pipeName, PipeDirection.Out))
                {
                    _log.Write($"1 s:{s}");
                    pipeClient.Connect(timeout);

                    _log.Write($"2");
                    byte[] outBuffer = new UTF8Encoding().GetBytes(s + "\n");

                    _log.Write($"3");
                    pipeClient.Write(outBuffer, 0, outBuffer.Length);

                    _log.Write($"4");
                    pipeClient.WaitForPipeDrain();

                    _log.Write($"5");
                    // Now exit.
                }
            }
            catch (TimeoutException)
            {
                // Client can deal with this.
                _log.Write($"timed out", true);
                res = ClientStatus.Timeout;
            }
            catch (Exception ex)
            {
                _log.Write($"{ex}", true);
                Error = ex.ToString();
                res = ClientStatus.Error;
            }

            return res;
        }
    }
}
