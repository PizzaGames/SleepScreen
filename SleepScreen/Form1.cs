using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SleepScreen
{
    public partial class FormSleepScreen : Form
    {
        private int SC_MONITORPOWER = 0xF170;
        private uint WM_SYSCOMMAND = 0x0112;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        public FormSleepScreen()
        {
            InitializeComponent();
        }


        private void FormSleepScreen_KeyUp(object sender, KeyEventArgs e)
        {

            switch (e.KeyCode)
            {
                case Keys.Escape:
                    {
                        WakeUpMonitor();
                        break;
                    };
                default:
                    {
                        SleepMonitor();
                        break;
                    }
            }
        }

        private void WakeUpMonitor()
        {
            SendMessage(this.Handle, WM_SYSCOMMAND, (IntPtr)SC_MONITORPOWER, IntPtr.Zero);
            Application.Exit();
        }

        private void SleepMonitor()
        {
            SendMessage(this.Handle, WM_SYSCOMMAND, (IntPtr)SC_MONITORPOWER, (IntPtr)2);
        }

        private void FormSleepScreen_Activated(object sender, EventArgs e)
        {
            SleepMonitor();
        }

        private void FormSleepScreen_Deactivate(object sender, EventArgs e)
        {
            WakeUpMonitor();
        }
    }
}
