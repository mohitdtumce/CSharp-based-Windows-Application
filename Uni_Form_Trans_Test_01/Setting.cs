using System;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Text;

[StructLayout(LayoutKind.Sequential)]
public struct RECT
{
    public int Left;
    public int Top;
    public int Right;
    public int Bottom;
}

namespace Uni_Form_Trans_Test_01
{
    public partial class Setting : Form
    {
        Form1 dino;
        About about;
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        private static extern IntPtr GetDesktopWindow();
        [DllImport("user32.dll")]
        private static extern IntPtr GetShellWindow();
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowRect(IntPtr hwnd, out RECT rc);
        [DllImport("user32.dll")]
        static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        public Setting()
        {
            InitializeComponent();
        }

        private void Setting_Load(object sender, EventArgs e)
        {
            dino = new Form1();
            dino.Show();
            about = new About();
            about.Hide();
            string address = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
            int val = Properties.Settings.Default.SIZE;
            trackBar1.Value = val;
            float size = 9 - val;
            size = size / 10f + 1;
            dino.ChangeSetting("SIZE", size);

            if (Properties.Settings.Default.STARTUP)
                checkBox2.Checked = true;

            if (Properties.Settings.Default.FADE_ON_HOVER)
                checkBox3.Checked = true;

            if (Properties.Settings.Default.HIDE_FULLSCREEN)
                checkBox4.Checked = true;

            Hide();

            if (Properties.Settings.Default.STARTUP)
                checkBox2_CheckedChanged(null, null);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.notifyIcon1.Dispose();
            this.Close();
            System.Windows.Forms.Application.Exit();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.notifyIcon1.Dispose();
            this.Close();
            System.Windows.Forms.Application.Exit();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            float size = 9 - trackBar1.Value;
            size = size / 10f + 1;
            Properties.Settings.Default.SIZE = trackBar1.Value;
            Properties.Settings.Default.Save();
            dino.ChangeSetting("SIZE", size);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.CheckState == CheckState.Checked)
                notifyIcon1.Visible = false;
            else
                notifyIcon1.Visible = true;
        }

        private void hideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string currentName = contextMenuStrip1.Items[1].Text;
            if (currentName == "Hide")
            {
                contextMenuStrip1.Items[1].Text = "Show";
                dino.ChangeSetting("HIDE|SHOW", 0);
            }
            else
            {
                dino.ChangeSetting("HIDE|SHOW", 1);
                contextMenuStrip1.Items[1].Text = "Hide";
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (checkBox2.CheckState == CheckState.Checked || sender == null)
            {
                string address = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
                address = address.Substring(8);
                address = address.Replace('/', '\\');
                key.SetValue("Dino", @address);
                Properties.Settings.Default.STARTUP = true;
            }
            else
            {
                key.DeleteValue("Dino", false);
                Properties.Settings.Default.STARTUP = false;
            }
            Properties.Settings.Default.Save();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.CheckState == CheckState.Checked)
            {
                Properties.Settings.Default.FADE_ON_HOVER = false;
            }
            else
            {
                Properties.Settings.Default.FADE_ON_HOVER = true;
            }
            dino.ChangeSetting("FADE_ON_HOVER");
            Properties.Settings.Default.Save();
        }


        private void flyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dino.ChangeSetting("FLY");
        }

        private void layDownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dino.ChangeSetting("LAY_DOWN");
        }

        private void sitDownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dino.ChangeSetting("SIT_DOWN");
        }

        private void danceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dino.ChangeSetting("DANCE");
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            about = new About();
            about.Show();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.HIDE_FULLSCREEN == false || contextMenuStrip1.Items[1].Text == "Show")
                goto skip;
            IntPtr desktopHandle;
            IntPtr shellHandle;

            desktopHandle = GetDesktopWindow();
            shellHandle = GetShellWindow();

            bool runningFullScreen = false;
            RECT appBounds;
            Rectangle screenBounds;
            IntPtr hWnd;
            StringBuilder className = new StringBuilder(256);
            hWnd = GetForegroundWindow();
            if (hWnd != null && !hWnd.Equals(IntPtr.Zero))
            {
                GetClassName(hWnd, className, 256);
                if (!(hWnd.Equals(desktopHandle) || hWnd.Equals(shellHandle)) && className.ToString() != "WorkerW" && className.ToString() != "Progman")
                {
                    GetWindowRect(hWnd, out appBounds);
                    screenBounds = Screen.FromHandle(hWnd).Bounds;
                    if ((appBounds.Bottom - appBounds.Top) == screenBounds.Height && (appBounds.Right - appBounds.Left) == screenBounds.Width)
                    {
                        runningFullScreen = true;
                    }
                }
            }

            if (runningFullScreen)
                dino.ChangeSetting("HIDE|SHOW", 0);
            else
                dino.ChangeSetting("HIDE|SHOW", 1);
            skip:
            {
                // Dummy Jump Statement
            }
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox4.CheckState == CheckState.Checked)
                Properties.Settings.Default.HIDE_FULLSCREEN = true;
            else
                Properties.Settings.Default.HIDE_FULLSCREEN = false;
            Properties.Settings.Default.Save();

        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
        }

        ~Setting()
        {
            this.notifyIcon1.Dispose();
            this.Close();
        }
    }
}
