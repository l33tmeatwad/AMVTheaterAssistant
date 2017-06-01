using System;
using System.Windows;
using System.Collections.Generic;
using AMVTheaterAssistant.Properties;
using System.IO;
using Microsoft.Win32;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using System.Net;
using System.Windows.Forms;
using System.ComponentModel;
using System.Threading.Tasks;

namespace AMVTheaterAssistant
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ContentWindow showInfoScreen;
        ContentWindow blankVideo;
        public MainWindow()
        {
            InitializeComponent();
            SetIEemulation();
        }

        void SetIEemulation()
        {
            var ieRegistryEmulation = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", true);
            if (ieRegistryEmulation == null)
            {
                Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION");
                ieRegistryEmulation = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", true);
            }
            ieRegistryEmulation.SetValue(System.AppDomain.CurrentDomain.FriendlyName, "11001", RegistryValueKind.DWord);
        }

        void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            MessageBoxResult result = System.Windows.MessageBox.Show("Are you sure you want to close AMV Theater Assistant?", "AMV Theater Assistant", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
            else
            {
                var ieRegistryEmulation = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", true);
                if (ieRegistryEmulation != null)
                {
                    ieRegistryEmulation.DeleteValue(System.AppDomain.CurrentDomain.FriendlyName);
                }
                
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Settings.Default["siteLocation"] = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\AMVTheaterAssistant";
            Settings.Default.Save();

            pageTitleLine1.Text = Settings.Default["pageTitleLine1"].ToString();
            title1Size.Text = Settings.Default["title1Size"].ToString();
            enableTitleLine2.IsChecked = Convert.ToBoolean(Settings.Default["enableTitleLine2"]);
            pageTitleLine2.Text = Settings.Default["pageTitleLine2"].ToString();
            title2Size.Text = Settings.Default["title2Size"].ToString();
            panelInfoSize.Text = Settings.Default["panelInfoSize"].ToString();
            presentersSize.Text = Settings.Default["presentersSize"].ToString();
            videoInfoSize.Text = Settings.Default["videoInfoSize"].ToString();
            upcoming1Size.Text = Settings.Default["upcoming1Size"].ToString();
            upcoming2Size.Text = Settings.Default["upcoming2Size"].ToString();
            cssBGcolor.Text = Settings.Default["cssBGcolor"].ToString();
            cssTextColor.Text = Settings.Default["cssTextColor"].ToString();
            cssTextSize.Text = Settings.Default["cssTextSize"].ToString();
            enablecustom.IsChecked = Convert.ToBoolean(Settings.Default["mpcEnableCustom"]);
            startfull.IsChecked = Convert.ToBoolean(Settings.Default["mpcStartFull"]);
            if (Convert.ToBoolean(Settings.Default["alwaysOnTop"]) == true) { Topmost = true; alwaysOnTop.IsChecked = true; }

            DoThingsExists();
            LoadFontlist();
            LoadLogos();
            MonitorCount();
        }

        private void DoThingsExists()
        {
            string siteLocation = Settings.Default["siteLocation"].ToString();

            if (System.IO.File.Exists(siteLocation + @"\display.html"))
            {
                updateInfo.IsEnabled = true;
                updateInfoPage.IsEnabled = true;
            }
            if (Directory.Exists(siteLocation + @"\fonts"))
            {
                addFonts.IsEnabled = true;
                removeFonts.IsEnabled = true;
            }
            if (Directory.Exists(siteLocation + @"\logos"))
            {
                addLogos.IsEnabled = true;
                removeLogos.IsEnabled = true;
            }

            var mpcRegLocation = Registry.CurrentUser.OpenSubKey(@"Software\MPC-HC\MPC-HC", true);
            if (mpcRegLocation != null)
            {
                if (mpcRegLocation.GetValue("ExePath") != null)
                {
                    Settings.Default["mpcAppLocation"] = mpcRegLocation.GetValue("ExePath").ToString();
                    previous.IsEnabled = true;
                    play.IsEnabled = true;
                    pause.IsEnabled = true;
                    stop.IsEnabled = true;
                    next.IsEnabled = true;
                    stepback.IsEnabled = true;
                    stepforward.IsEnabled = true;
                    skipback3.IsEnabled = true;
                    skipback2.IsEnabled = true;
                    skipback1.IsEnabled = true;
                    skipforward1.IsEnabled = true;
                    skipforward2.IsEnabled = true;
                    skipforward3.IsEnabled = true;
                    mute.IsEnabled = true;
                    voldown.IsEnabled = true;
                    volup.IsEnabled = true;
                    always.IsEnabled = true;
                    whileplaying.IsEnabled = true;
                    fullscreen.IsEnabled = true;
                    playlist.IsEnabled = true;
                    playAll.IsEnabled = true;
                    playOne.IsEnabled = true;
                    audioprev.IsEnabled = true;
                    audionext.IsEnabled = true;
                    subprev.IsEnabled = true;
                    subnext.IsEnabled = true;
                    panic.IsEnabled = true;
                    mpchc.IsEnabled = true;
                    webserver.IsEnabled = true;
                    if (Screen.AllScreens.Count() > 1)
                    {
                        showInfo.IsEnabled = true;
                        infoScreenSelection.IsEnabled = true;
                    }
                }






            MPCHC MPC = new MPCHC();
            bool mpcWebRunning = MPC.DetectWebServer();
            if (mpcWebRunning == true)
            {
                webcontrols.IsEnabled = true;
                webserver.IsEnabled = false;
            }

            var mpcRegSettings = Registry.CurrentUser.OpenSubKey(@"Software\MPC-HC\MPC-HC\Settings", true);
            string mpcPort = Settings.Default["mpcWebPort"].ToString();
            if (mpcRegSettings.GetValue("WebServerPort") != null)
            {
                mpcPort = mpcRegSettings.GetValue("WebServerPort").ToString();
                mpcWebPort.Text = mpcPort;
                mpcSettings.IsEnabled = true;
            }
            else
            {
                mpcWebPort.Text = mpcPort;
            }
            if (mpcWebRunning == true)
            {
                FindNetworkAddress(mpcPort);
            }
            else
            {
                IPAddress.Text = "MPC Web Server Not Enabled";
            }
            mpcSettings.IsEnabled = true;

            }
            else
            {
                IPAddress.Text = "MPC-HC Not Detected";
            }

        }



        private void FindNetworkAddress(string mpcPort)
        {
            IPHostEntry localhost;
            string ip = "127.0.0.1";
            localhost = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ipa in localhost.AddressList)
            {
                if (ipa.AddressFamily.ToString() == "InterNetwork") ip = ipa.ToString();
            }

            if (ip != "127.0.0.1")
            {
                string ServerAddress = ip + ":" + mpcPort;
                IPAddress.Text = "MPC-HC Server Address: http://" + ServerAddress;
            }
        }

        private void MonitorCount()
        {
            foreach (var screen in System.Windows.Forms.Screen.AllScreens)
            {
                mpcPlayback.Items.Add(screen.DeviceName.Replace(@"\\.\DISPLAY","Monitor "));
                infoScreenSelection.Items.Add(screen.DeviceName.Replace(@"\\.\DISPLAY", "Monitor "));
            }
            int screennum = mpcPlayback.Items.IndexOf("Monitor " + Settings.Default["defaultPlayMonitor"].ToString());
            if (screennum > 0)
            {
                mpcPlayback.SelectedIndex = screennum;
            }
            else
            {
                mpcPlayback.SelectedIndex = 0;
            }

            screennum = infoScreenSelection.Items.IndexOf("Monitor " + Settings.Default["defaultInfoScreen"].ToString());
            if (screennum > 0)
            {
                infoScreenSelection.SelectedIndex = screennum;
            }
            else
            {
                infoScreenSelection.SelectedIndex = 0;
            }
        }

        private void LoadLogos()
        {
            string logoDir = Settings.Default["siteLocation"].ToString() + @"\logos";
            if (Directory.Exists(logoDir))
            {
                FindThings FindThings = new FindThings();
                List<string> imgList = new List<string>(FindThings.FindImportedLogos(logoDir));
                logoList.Items.Clear();
                for (int i = 0; i < imgList.Count; i++)
                {
                    logoList.Items.Add(imgList[i]);
                }

            }

        }

        private void LoadFontlist()
        {
            string fontDir = Settings.Default["siteLocation"].ToString() + @"\fonts";
            List<string> FontList = new List<string>(new string[] { "Arial", "Arial Black", "Georgia", "Helvetica", "Impact", "Times" });
            if (Directory.Exists(fontDir))
            {
                FindThings FindThings = new FindThings();
                List<string> customFontList = FindThings.FindImportedFonts(fontDir);
                fontList.Items.Clear();
                for (int i = 0; i < customFontList.Count; i++)
                {
                    fontList.Items.Add(customFontList[i]);
                }
                FontList.AddRange(FindThings.FindFontNames(customFontList));
            }

            title1Font.Items.Clear();
            title2Font.Items.Clear();
            panelInfoFont.Items.Clear();
            presentersFont.Items.Clear();
            videoInfoFont.Items.Clear();
            upcoming1Font.Items.Clear();
            upcoming2Font.Items.Clear();


            FontList.Sort();
            for (int i = 0; i < FontList.Count; i++)
            {
                title1Font.Items.Add(FontList[i]);
                title2Font.Items.Add(FontList[i]);
                panelInfoFont.Items.Add(FontList[i]);
                presentersFont.Items.Add(FontList[i]);
                videoInfoFont.Items.Add(FontList[i]);
                upcoming1Font.Items.Add(FontList[i]);
                upcoming2Font.Items.Add(FontList[i]);
            }
            title1Font.SelectedIndex = FontList.FindIndex(x => x.Equals(Settings.Default["title1Font"].ToString()));
            if (title1Font.SelectedIndex < 0) { title1Font.SelectedIndex = FontList.FindIndex(x => x.Equals("Helvetica")); }

            title2Font.SelectedIndex = FontList.FindIndex(x => x.Equals(Settings.Default["title2Font"].ToString()));
            if (title2Font.SelectedIndex < 0) { title2Font.SelectedIndex = FontList.FindIndex(x => x.Equals("Helvetica")); }

            panelInfoFont.SelectedIndex = FontList.FindIndex(x => x.Equals(Settings.Default["panelInfoFont"].ToString()));
            if (panelInfoFont.SelectedIndex < 0) { panelInfoFont.SelectedIndex = FontList.FindIndex(x => x.Equals("Helvetica")); }

            presentersFont.SelectedIndex = FontList.FindIndex(x => x.Equals(Settings.Default["presentersFont"].ToString()));
            if (presentersFont.SelectedIndex < 0) { presentersFont.SelectedIndex = FontList.FindIndex(x => x.Equals("Helvetica")); }

            videoInfoFont.SelectedIndex = FontList.FindIndex(x => x.Equals(Settings.Default["videoInfoFont"].ToString()));
            if (videoInfoFont.SelectedIndex < 0) { videoInfoFont.SelectedIndex = FontList.FindIndex(x => x.Equals("Helvetica")); }

            upcoming1Font.SelectedIndex = FontList.FindIndex(x => x.Equals(Settings.Default["upcoming1Font"].ToString()));
            if (upcoming1Font.SelectedIndex < 0) { upcoming1Font.SelectedIndex = FontList.FindIndex(x => x.Equals("Helvetica")); }

            upcoming2Font.SelectedIndex = FontList.FindIndex(x => x.Equals(Settings.Default["upcoming2Font"].ToString()));
            if (upcoming2Font.SelectedIndex < 0) { upcoming2Font.SelectedIndex = FontList.FindIndex(x => x.Equals("Helvetica")); }
        }

        private void CreateWebsite()
        {
            Website Website = new Website();
            Website.GenerateWebsite();
            var Logos = logoList.Items.Cast<String>().ToList();
            Website.CreateDisplayPage(Logos);
            DoThingsExists();
        }

        private void updateInfo_Click(object sender, RoutedEventArgs e)
        {
            string PanelInfo = panelInfo.Text;
            bool IfPresenters = Convert.ToBoolean(presentedBy.IsChecked);
            string Presenters = panelPresenters.Text;
            bool IfPlaying = Convert.ToBoolean(showVideoInfo.IsChecked);
            string Playing = videoInfo.Text;
            bool IfUpcoming = Convert.ToBoolean(showNextPanel.IsChecked);
            string Upcoming1 = upcoming1.Text;
            string Upcoming2 = upcoming2.Text;

            SaveUserValues();
            Website Website = new Website();
            Website.UpdatePage(PanelInfo, IfPresenters, Presenters, IfPlaying, Playing, IfUpcoming, Upcoming1, Upcoming2);
        }

        private void addFonts_Click(object sender, RoutedEventArgs e)
        {
            string filter = "Font files (*.ttf)|*.ttf|All files (*.*)|*.*";
            string fontLocation = Settings.Default["siteLocation"].ToString() + @"\fonts";

            AddThings AddThings = new AddThings();
            AddThings.AddFiles(filter, fontLocation);
            LoadFontlist();
        }

        private void addLogos_Click(object sender, RoutedEventArgs e)
        {
            string filter = "Image files (*.gif, *.jpg, *.jpeg, *.png)|*.gif; *jpg; *.jpeg; *.png|All files (*.*)|*.*";
            string fontLocation = Settings.Default["siteLocation"].ToString() + @"\logos";

            AddThings AddThings = new AddThings();
            AddThings.AddFiles(filter, fontLocation);
            LoadLogos();
        }

        private void generateSite_Click(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists(Settings.Default["siteLocation"].ToString()))
            {
                Directory.CreateDirectory(Settings.Default["siteLocation"].ToString());
            }

            CreateWebsite();
            LoadFontlist();
        }

        private void mpcSettigns_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default["defaultPlayMonitor"] = Convert.ToInt32(mpcPlayback.Items[mpcPlayback.SelectedIndex].ToString().Replace("Monitor ",""));
            Settings.Default["mpcStartFull"] = startfull.IsChecked;
            Settings.Default["mpcEnableCustom"] = enablecustom.IsChecked;
            if (mpcWebPort.Text != "") { Settings.Default["mpcWebPort"] = mpcWebPort.Text; }
            Settings.Default.Save();

            string StartFull = "0";
            string WebPort = Settings.Default["mpcWebPort"].ToString();
            mpcWebPort.Text = WebPort;
            string mpcSettingsLocation = @"Software\MPC-HC\MPC-HC\Settings";
            string mpcPlaylistLocation = @"Software\MPC-HC\MPC-HC\ToolBars\Playlist";
            string mpcPlaylistSize = @"Software\MPC-HC\MPC-HC\ToolBars\Playlist\State-SCBar-824";
            if (startfull.IsChecked == true) { StartFull = "1"; }

            MPCHC MPCSettings = new MPCHC();
            MPCSettings.ChangeMPCSettings(mpcSettingsLocation, StartFull, WebPort, mpcPlaylistLocation, mpcPlaylistSize);

            FindNetworkAddress(WebPort);
            MPCSettings.ControlMPC(2);
        }

        private void removeLogos_Click(object sender, RoutedEventArgs e)
        {
            var selected = logoList.SelectedIndex;
            if (selected >= 0)
            {
                string logo = Settings.Default["siteLocation"].ToString() + @"\logos\" + logoList.SelectedValue.ToString();
                if (File.Exists(logo) == true)
                {
                    File.Delete(logo);
                }
                LoadLogos();
                logoList.SelectedIndex = selected;
            }
        }

        private void removeFonts_Click(object sender, RoutedEventArgs e)
        {
            var selected = fontList.SelectedIndex;
            if (selected >= 0)
            {
                string font = Settings.Default["siteLocation"].ToString() + @"\fonts\" + fontList.SelectedValue.ToString();
                if (File.Exists(font) == true)
                {
                    File.Delete(font);
                }
                LoadFontlist();
                fontList.SelectedIndex = selected;
            }
        }

        private void updateInfoPage_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default["cssBGcolor"] = cssBGcolor.Text;
            Settings.Default["cssTextColor"] = cssTextColor.Text;
            Settings.Default["cssTextSize"] = cssTextSize.Text;
            string panelInfo = Settings.Default["siteLocation"] + @"\panelinfo.txt";
            string oldInfo = "";
            if (File.Exists(panelInfo) == true)
            {
                oldInfo = System.IO.File.ReadAllText(panelInfo);
            }
            Website Website = new Website();
            var logos = logoList.Items.Cast<String>().ToList();
            Website.CreateDisplayPage(logos);
            string[] PanelInfoTXT = { "Updating Logos & Styling", "<script>window.location.reload();</script>" };
            File.WriteAllLines(panelInfo, PanelInfoTXT);
            Thread.Sleep(1500);
            File.WriteAllText(panelInfo, oldInfo);
        }



        private void alwaysOnTop_Checked(object sender, RoutedEventArgs e)
        {
            Topmost = true;
            Settings.Default["alwaysOnTop"] = true;
            Settings.Default.Save();
        }
        private void alwaysOnTop_UnChecked(object sender, RoutedEventArgs e)
        {
            Topmost = false;
            Settings.Default["alwaysOnTop"] = false;
            Settings.Default.Save();
        }

        private void numTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            string[] nums = { "D1", "D2", "D3", "D4", "D5", "D6", "D7", "D8", "D9", "D0", "NumPad1", "NumPad2", "NumPad3", "NumPad4", "NumPad5", "NumPad6", "NumPad7", "NumPad8", "NumPad9", "NumPad0", "Tab" };
            if (!nums.Contains(e.Key.ToString()))
                e.Handled = true;
            if (nums.Contains(e.Key.ToString()))
                e.Handled = false;
            if (System.Windows.Forms.Control.ModifierKeys == System.Windows.Forms.Keys.Shift && e.Key.ToString() != "Tab")
                e.Handled = true;
        }

        private void nospace_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void SaveUserValues()
        {
            Settings.Default["pageTitleLine1"] = pageTitleLine1.Text;
            Settings.Default["title1Font"] = title1Font.Text;
            Settings.Default["title1Size"] = title1Size.Text;
            Settings.Default["enableTitleLine2"] = enableTitleLine2.IsChecked;
            Settings.Default["pageTitleLine2"] = pageTitleLine2.Text;
            Settings.Default["title2Font"] = title2Font.Text;
            Settings.Default["title2Size"] = title2Size.Text;
            Settings.Default["panelInfoFont"] = panelInfoFont.Text;
            Settings.Default["panelInfoSize"] = panelInfoSize.Text;
            Settings.Default["presentersFont"] = presentersFont.Text;
            Settings.Default["presentersSize"] = presentersSize.Text;
            Settings.Default["videoInfoFont"] = videoInfoFont.Text;
            Settings.Default["VideoInfoSize"] = videoInfoSize.Text;
            Settings.Default["upcoming1Font"] = upcoming1Font.Text;
            Settings.Default["upcoming1Size"] = upcoming1Size.Text;
            Settings.Default["upcoming2Font"] = upcoming2Font.Text;
            Settings.Default["upcoming2Size"] = upcoming2Size.Text;
            Settings.Default.Save();
        }

        private void mpchc_Click(object sender, RoutedEventArgs e)
        {
            MPCHC MPC = new MPCHC();
            MPC.ControlMPC(1);
        }

        private void webcontrols_Click(object sender, RoutedEventArgs e)
        {
            MPCHC MPC = new MPCHC();
            bool IsMPCrunning = MPC.ControlMPC(0);
            bool WebServerEnabled = MPC.DetectWebServer();
            if (WebServerEnabled == true)
            {
                if (IsMPCrunning == true)
                {
                    ContentWindow webControls = new ContentWindow();
                    webControls.LoadWebsite("http://localhost:" + Settings.Default["mpcWebPort"].ToString() + "/controls.html");
                    webControls.Width = 860;
                    webControls.Height = 420;
                    webControls.Text = "Web Controls";
                    webControls.FormClosed += webcontrols_FormClosed;
                    webControls.Show();
                    //Process.Start("http://localhost:" + Settings.Default["mpcWebPort"].ToString() + "/controls.html");
                    webcontrols.IsEnabled = false;
                }
            }
            else
            {
                webcontrols.IsEnabled = false;
                webserver.IsEnabled = true;
                IPAddress.Text = "MPC Web Server Not Enabled";
            }

        }

        private void webcontrols_FormClosed(object sender, FormClosedEventArgs e)
        {
            webcontrols.IsEnabled = true;
        }

        private void mpcControls_Click(object sender, RoutedEventArgs e)
        {
            MPCHC MPC = new MPCHC();
            string command = (sender as System.Windows.Controls.Button).Uid.ToString();
            MPC.ControlMPC(Convert.ToInt32(command));
        }

        private void webserver_Click(object sender, RoutedEventArgs e)
        {
            MPCHC MPC = new MPCHC();
            MPC.StartWebServer();
            DoThingsExists();
        }

        private void showInfo_Click(object sender, RoutedEventArgs e)
        {
            if (showInfoScreen == null)
            {
                MPCHC MPC = new MPCHC();
                bool IsMPCrunning = MPC.ControlMPC(0);

                if (IsMPCrunning == true)
                {
                    showInfoScreen = new ContentWindow();
                    showInfoScreen.LoadInfoScreen(infoScreenSelection.SelectedIndex);
                    showInfoScreen.FormClosed += showInfoScreen_FormClosed;
                    showInfoScreen.BackColor = System.Drawing.Color.Black;
                    showInfoScreen.Opacity = 0;
                    
                    showInfoScreen.Show();
                    showInfo.Content = "Hide";
                }
            }
            else
            {
                showInfoScreen.fadeTimer(false, 100);
            }
        }
        private void showInfoScreen_FormClosed(object sender, FormClosedEventArgs e)
        {
            showInfoScreen = null;
            showInfo.Content = "Show";
        }
        private void infoScreenSelection_SelectionChanged(object sender, EventArgs e)
        {
            Settings.Default["defaultInfoScreen"] = Convert.ToInt32(infoScreenSelection.Items[infoScreenSelection.SelectedIndex].ToString().Replace("Monitor ", ""));
            Settings.Default.Save();
        }

        private void fadeVideo_Click(object sender, RoutedEventArgs e)
        {

            if (fadeVideo.Content.ToString() == "Fade Out")
            {
                blankVideo = new ContentWindow();
                blankVideo.Opacity = 0;
                blankVideo.CoverScreen(mpcPlayback.SelectedIndex);
                blankVideo.fadeTimer(true, int.Parse(fadeVideoTime.Text));
                blankVideo.FormClosed += fadeVideo_FormClosed;
                blankVideo.BackColor = System.Drawing.Color.Black;
                blankVideo.Show();
                fadeVideo.Content = "Fade In";
            }
            else
            {
                blankVideo.fadeTimer(false, int.Parse(fadeVideoTime.Text));
            }
        }

        private void fadeVideo_FormClosed(object sender, FormClosedEventArgs e)
        {
            blankVideo = null;
            fadeVideo.Content = "Fade Out";
        }
    }
}
