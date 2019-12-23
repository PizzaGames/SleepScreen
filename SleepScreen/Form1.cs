using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

namespace SleepScreen
{
    public partial class FormSleepScreen : Form
    {
        private int SC_MONITORPOWER = 0xF170;
        private uint WM_SYSCOMMAND = 0x0112;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        private volatile bool PreventActivation = false;
        private Thread LastThread = null;

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
                        System.Media.SystemSounds.Beep.Play();
                        SleepMonitor();
                        break;
                    }
            }
        }

        private void WakeUpMonitor()
        {
            PreventActivation = false;

            CallSysWakeUpMonitor();
            System.Media.SystemSounds.Question.Play();
            Application.Exit();
        }

        private IntPtr CallSysWakeUpMonitor()
        {
            return SendMessage(this.Handle, WM_SYSCOMMAND, (IntPtr)SC_MONITORPOWER, IntPtr.Zero);
        }

        private void SleepMonitor()
        {
            if (PreventActivation)
            {
                return;
            }

            if (LastThread != null)
            {
                LastThread.Interrupt();
            }

            PreventActivation = true;

            LastThread = new Thread(() =>
            {
                try
                {
                    while (PreventActivation)
                    {
                        this.Invoke(new MethodInvoker(delegate
                                {
                                    CallSysSleepMonitor();
                                }
                            )
                        );

                        Thread.Sleep(1000);
                    }
                } 
                catch (ThreadInterruptedException e)
                {
                    WakeUpMonitor();
                }
            });
            LastThread.Start();
            
        }



        private void CallSysSleepMonitor()
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

        private void FormSleepScreen_Click(object sender, EventArgs e)
        {
            System.Media.SystemSounds.Beep.Play();
        }
    }
}
