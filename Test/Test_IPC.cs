using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NBagOfTricks;
using NBagOfTricks.SimpleIpc;
using NBagOfTricks.PNUT;


namespace NBagOfTricks.Test
{
    public class IPC_BASIC : TestSuite
    {
        //string TS_FORMAT = @"mm\:ss\.fff";
        readonly string PIPE_NAME = "058F684D-AF82-4FE5-BD1E-9FD031FE28CF";
        readonly string LogFileName = "test_ipc_log.txt";

        MpLog _log;

        public override void RunSuite()
        {
            UT_INFO("Tests simple IPC.");

            _log = new MpLog(LogFileName, "^^^^^^");

            // Server
            using (Server server = new Server(PIPE_NAME, LogFileName))
            {
                server.ServerEvent += Server_IpcEvent;
                server.Start();

                string s;

                void Server_IpcEvent(object sender, ServerEventArgs e)
                {
                    switch (e.Status)
                    {
                        case ServerStatus.Message:
                            s = e.Message;
                            // s should be "ABC123"
                            break;

                        case ServerStatus.Error:
                            _log.Write($"Server error:{e.Message}", true);
                            break;
                    }
                }

                // Client - !put in separate process!
                Client client = new Client(PIPE_NAME, LogFileName);
                var res = client.Send("ABC123", 1000);

                switch (res)
                {
                    case ClientStatus.Error:
                        _log.Write($"Client error:{client.Error}", true);
                        break;

                    case ClientStatus.Timeout:
                        _log.Write($"Client timeout", true);
                        break;
                }

                _log.Write($"sub thread exit {res}");
            }
        }
    }
}
