using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using NBagOfTricks;


namespace Slog.Test
{
    public partial class TestHost : Form
    {
        readonly Logger _logger1 = LogManager.CreateLogger("TestLogger1");
        readonly Logger _logger2 = LogManager.CreateLogger("TestLogger2");

        /// <summary>Init slog.</summary>
        public TestHost()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);
            InitializeComponent();

            Location = new(20, 20);

            rtbLog.Font = new Font("Consolas", 10);

            LogManager.MinLevelFile = Level.Trace;
            LogManager.MinLevelNotif = Level.Info;
            LogManager.Log += LogManager_Log;
            LogManager.Run(@"C:\Dev\repos\Slog\Test\slog.log.txt", 1000);
        }

        /// <summary>Clean up.</summary>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            LogManager.Stop();

            base.OnFormClosing(e);
        }

        /// <summary>Something to show.</summary>
        void LogManager_Log(object? sender, LogEventArgs e)
        {
            // May come from a different thread.
            this.InvokeIfRequired(_ =>
            {
                string s = $"> {e.Message}";
                rtbLog.AppendText(s);
                rtbLog.AppendText(Environment.NewLine);
                rtbLog.ScrollToCaret();
            });
        }

        /// <summary>Debug</summary>
        void Doit_Click(object sender, EventArgs e)
        {
            // TLOG_CONTEXT_S(CMN_CStateMachine::ProcessEvent);

            _logger1.LogInfo("11111");

            _logger2.LogDebug("22222");

            _logger1.Log(Level.Trace, "33333 - should not appear in ui!!");

            try
            {
                int x = 0;
                int y = 100 / x;
            }
            catch (Exception ex)
            {
                _logger2.Log(ex);
            }
        }

        /// <summary>Debug</summary>
        void Again_Click(object sender, EventArgs e)
        {
            LogManager.MinLevelNotif = Level.Trace;

            _logger1.Log(Level.Trace, "44444 - should appear in ui!!");

        }
    }
}
