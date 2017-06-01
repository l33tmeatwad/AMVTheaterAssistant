using Microsoft.Win32;
using System;
using System.Windows.Forms;

namespace AMVTheaterAssistant
{
    public partial class ContentWindow : Form
    {
        public ContentWindow()
        {
            InitializeComponent();
            webBrowser1.Visible = false;

        }

        public void CoverScreen(int screen)
        {
            Text = "Cover Screen";
            Screen[] screens = Screen.AllScreens;
            var bounds = screens[screen].Bounds;

            Left = bounds.Left;
            Top = bounds.Top;
            Width = bounds.Width;
            Height = bounds.Height;
            TopMost = true;
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;

            
        }

        public void LoadInfoScreen(int screen)
        {
            Text = "Info Display";
            Screen[] screens = Screen.AllScreens;
            var bounds = screens[screen].Bounds;

            Left = bounds.Left;
            Top = bounds.Top;
            Width = bounds.Width;
            Height = bounds.Height;
            TopMost = true;
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;

            var mpcRegSettings = Registry.CurrentUser.OpenSubKey(@"Software\MPC-HC\MPC-HC\Settings", true);
            if (mpcRegSettings == null)
            {
                Registry.CurrentUser.CreateSubKey(@"Software\MPC-HC\MPC-HC\Settings");
                mpcRegSettings = Registry.CurrentUser.OpenSubKey(@"Software\MPC-HC\MPC-HC\Settings", true);
            }
            int webport = (int)mpcRegSettings.GetValue("WebServerPort");
            mpcRegSettings.Close();

            string infosite = "http://localhost:" + webport.ToString() + "/display.html";
            HorizontalScroll.Enabled = false;
            VerticalScroll.Enabled = false;

           
            
            LoadWebsite(infosite);

        }

        public void LoadWebsite(string site)
        {
            webBrowser1.Navigate(site);
        }
        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            webBrowser1.Visible = true;
            fadeTimer(true, 100);
        }

        Timer fadetimer;
        int fadems;
        public void fadeTimer(bool fadein, int ms)
        {
            fadems = ms;
            int interval = ms/50;
            
            if (interval > 0)
            {
                fadetimer = new Timer();
                fadetimer.Interval = interval;
                if (fadein)
                    fadetimer.Tick += new EventHandler(fadeIn);
                else
                    fadetimer.Tick += new EventHandler(fadeOut);
                fadetimer.Start();
            }
            else
            {
                if (fadein)
                    Opacity += 1.00;
                else
                    Close();
            }
        }

        void fadeIn(object sender, EventArgs e)
        {
            if (Opacity >= 1)
            {
                fadetimer.Stop();
                fadetimer.Dispose();
            }
            else
                Opacity += 0.02;
        }

        void fadeOut(object sender, EventArgs e)
        {
            if (Opacity <= 0)
            {
                fadetimer.Stop();
                Close();
            }
            else
                Opacity -= 0.02;
        }

        private void ContentWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                fadeTimer(false, fadems);
            }
        }
    }
}
