using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Ephemera.NBagOfTricks.SimpleIpc
{
    /// <summary>Possible outcomes.</summary>
    public enum ClientStatus { Ok, Timeout, Error }

    /// <summary>Companion client to server. This runs in a new process.</summary>
    public class Client
    {
        /// <summary>Pipe name.</summary>
        readonly string _pipeName;

        /// <summary>My logger.</summary>
        readonly MpLog? _log = null;

        /// <summary>Caller may be able to use this.</summary>
        public string Error { get; set; } = "";

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="pipeName">Pipe name to use.</param>
        /// <param name="logfn">Optional log.</param>
        public Client(string pipeName, string? logfn = null)
        {
            _pipeName = pipeName;
            if (logfn is not null)
            {
                _log = new(logfn, "CLIENT");
            }
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
                using var pipeClient = new NamedPipeClientStream(".", _pipeName, PipeDirection.Out);
                _log?.Write($"rcv:{s}");
                pipeClient.Connect(timeout);
                byte[] outBuffer = new UTF8Encoding().GetBytes(s + "\n");
                pipeClient.Write(outBuffer, 0, outBuffer.Length);
                pipeClient.WaitForPipeDrain();
                // Now exit.
            }
            catch (TimeoutException)
            {
                // Client can deal with this.
                _log?.Write($"timed out", true);
                res = ClientStatus.Timeout;
            }
            catch (Exception ex)
            {
                // Other error.
                _log?.Write($"{ex}", true);
                Error = ex.ToString();
                res = ClientStatus.Error;
            }

            return res;
        }
    }
}
