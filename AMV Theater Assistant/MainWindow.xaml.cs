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
using System.Windows.Media;
using System.ComponentModel;

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
            LoadRegistrySettings();
            SetIEemulation();
            SetIEPageUpdate();
            DetectFullScreenControls();
        }

        void SetIEemulation()
        {
            var ieRegistryEmulation = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", true);
            if (ieRegistryEmulation == null)
            {
                Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION");
                ieRegistryEmulation = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", true);
            }
            ieRegistryEmulation.SetValue(AppDomain.CurrentDomain.FriendlyName, "11001", RegistryValueKind.DWord);
        }
        void SetIEPageUpdate()
        {
            var ieRegistryEmulation = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Internet Settings", true);
            if (ieRegistryEmulation != null)
            {
                ieRegistryEmulation.SetValue("SyncMode5", "4", RegistryValueKind.DWord);
            }
            
        }

        void LoadRegistrySettings()
        {
            var attRegSettings = Registry.CurrentUser.OpenSubKey(@"Software\AMV Theater Assistant", true);
            if (attRegSettings != null)
            {
                string[] regSettings = { "pageTitleLine1","title1Font","title1Size","enableTitleLine2","pageTitleLine2","title2Font","title2Size","panelInfoFont","panelInfoSize","presentersFont","presentersSize","videoInfoFont","videoInfoSize","upcoming1Font","upcoming1Size","upcoming2Font","upcoming2Size","alwaysOnTop","cssBGColor","cssTextColor","cssTextFont","cssTextSize","defaultPlayMonitor","defaultPlayMonitor","defaultPlayMonitor","defaultPlayMonitor","defaultInfoScreen" };
                foreach (string regSettingName in regSettings)
                {
                    var regSetting = attRegSettings.GetValue(regSettingName);
                    if (regSetting != null)
                    {
                        string regSettingString = regSetting.ToString();
                        if (regSettingString == "True" || regSettingString == "False")
                        {
                            Settings.Default[regSettingName] = Convert.ToBoolean(regSettingString);
                        }
                        else
                        {
                            Settings.Default[regSettingName] = regSettingString;
                        }
                    }
                }

                
            }
        }

        public void DetectFullScreenControls()
        {

            var mpcRegSettings = Registry.CurrentUser.OpenSubKey(@"Software\MPC-HC\MPC-HC\Settings", true);
            if (mpcRegSettings != null)
            {

                if (mpcRegSettings.GetValue("FullscreenSeparateControls") != null)
                {
                    if (mpcRegSettings.GetValue("FullscreenSeparateControls").ToString() != "0")
                    {
                        DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("Playback controls in this applicaion will not work when MPC-HC is fullscreen with Fullscreen Separate Controls enabled, it will need to be disabled in the Advanced Settings menu of MPC-HC or by using the Change Settings button in this application.", "Disable Fullscreen Separate Controls", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

            }
        }

        void SaveToRegistry()
        {
            var attRegSettings = Registry.CurrentUser.OpenSubKey(@"Software\AMV Theater Assistant", true);
            if (attRegSettings == null)
            {
                Registry.CurrentUser.CreateSubKey(@"Software\AMV Theater Assistant");
                attRegSettings = Registry.CurrentUser.OpenSubKey(@"Software\AMV Theater Assistant", true);
            }
            attRegSettings.SetValue("pageTitleLine1", Settings.Default["pageTitleLine1"], RegistryValueKind.String);
            attRegSettings.SetValue("title1Font", Settings.Default["title1Font"], RegistryValueKind.String);
            attRegSettings.SetValue("title1Size", Settings.Default["title1Size"], RegistryValueKind.DWord);
            attRegSettings.SetValue("enableTitleLine2", Settings.Default["enableTitleLine2"], RegistryValueKind.String);
            attRegSettings.SetValue("pageTitleLine2", Settings.Default["pageTitleLine2"], RegistryValueKind.String);
            attRegSettings.SetValue("title2Font", Settings.Default["title2Font"], RegistryValueKind.String);
            attRegSettings.SetValue("title2Size", Settings.Default["title2Size"], RegistryValueKind.DWord);
            attRegSettings.SetValue("panelInfoFont", Settings.Default["panelInfoFont"], RegistryValueKind.String);
            attRegSettings.SetValue("panelInfoSize", Settings.Default["panelInfoSize"], RegistryValueKind.DWord);
            attRegSettings.SetValue("presentersFont", Settings.Default["presentersFont"], RegistryValueKind.String);
            attRegSettings.SetValue("presentersSize", Settings.Default["presentersSize"], RegistryValueKind.DWord);
            attRegSettings.SetValue("videoInfoFont", Settings.Default["videoInfoFont"], RegistryValueKind.String);
            attRegSettings.SetValue("videoInfoSize", Settings.Default["VideoInfoSize"], RegistryValueKind.DWord);
            attRegSettings.SetValue("upcoming1Font", Settings.Default["upcoming1Font"], RegistryValueKind.String);
            attRegSettings.SetValue("upcoming1Size", Settings.Default["upcoming1Size"], RegistryValueKind.DWord);
            attRegSettings.SetValue("upcoming2Font", Settings.Default["upcoming2Font"], RegistryValueKind.String);
            attRegSettings.SetValue("upcoming2Size", Settings.Default["upcoming2Size"], RegistryValueKind.DWord);
            attRegSettings.SetValue("alwaysOnTop", Settings.Default["alwaysOnTop"], RegistryValueKind.String);
            attRegSettings.SetValue("cssBGColor", Settings.Default["cssBGColor"], RegistryValueKind.String);
            attRegSettings.SetValue("cssTextColor", Settings.Default["cssTextColor"], RegistryValueKind.String);
            attRegSettings.SetValue("cssTextFont", Settings.Default["cssTextFont"], RegistryValueKind.String);
            attRegSettings.SetValue("cssTextSize", Settings.Default["cssTextSize"], RegistryValueKind.DWord);
            attRegSettings.SetValue("defaultPlayMonitor", Settings.Default["defaultPlayMonitor"], RegistryValueKind.DWord);
            attRegSettings.SetValue("defaultInfoScreen", Settings.Default["defaultInfoScreen"], RegistryValueKind.DWord);
        }

        private bool DetectMissingFiles()
        {
            bool generatewebsite = false;
            string siteLocation = Settings.Default["siteLocation"].ToString();
            string[] customsitefiles = { @"\amvtt.css", @"\display.html", @"\index.html", @"\panelinfo.html", @"\javascript\jquery-2.2.4.js", @"\javascript\panelinfo.js" };
            string[] customsitefolders = { @"\fonts", @"\images", @"\javascript", @"\logos" };

            if (!Directory.Exists(siteLocation))
            {
                Directory.CreateDirectory(siteLocation);
                generatewebsite = true;
            }
            else
            {
                foreach (string folder in customsitefolders)
                {
                    if (!Directory.Exists(siteLocation + folder))
                        generatewebsite = true;
                }
                foreach (string file in customsitefiles)
                {
                    if (!System.IO.File.Exists(siteLocation + file))
                        generatewebsite = true;
                }
            }
            return generatewebsite;
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
                    ieRegistryEmulation.DeleteValue(AppDomain.CurrentDomain.FriendlyName);
                }
                
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadSettings();
        }

        private void LoadSettings()
        {
            Settings.Default["siteLocation"] = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\AMV Theater Assistant";
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
            cssTextSize.Text = Settings.Default["cssTextSize"].ToString();
            if (Convert.ToBoolean(Settings.Default["alwaysOnTop"]) == true) { Topmost = true; alwaysOnTop.IsChecked = true; }
            var brushcolor = new BrushConverter();
            cssBGColor.Fill = (Brush)brushcolor.ConvertFrom(Settings.Default["cssBGColor"]);
            cssTextColor.Fill = (Brush)brushcolor.ConvertFrom(Settings.Default["cssTextColor"]);
            DoThingsExists();
            LoadFontlist();
            LoadLogos();
        }

        private void DoThingsExists()
        {
            MonitorCount();
            string siteLocation = Settings.Default["siteLocation"].ToString();
            if (Directory.Exists(siteLocation))
                openSiteFolder.IsEnabled = true;
            else
                openSiteFolder.IsEnabled = false;
            if (System.IO.File.Exists(siteLocation + @"\display.html"))
            {
                updateInfo.IsEnabled = true;
                updateSiteStyling.IsEnabled = true;
            }
            else
            {
                updateInfo.IsEnabled = false;
                updateSiteStyling.IsEnabled = false;
            }
            if (Directory.Exists(siteLocation + @"\fonts"))
            {
                addFonts.IsEnabled = true;
                removeFonts.IsEnabled = true;
            }
            else
            {
                addFonts.IsEnabled = false;
                removeFonts.IsEnabled = false;
            }
            if (Directory.Exists(siteLocation + @"\logos"))
            {
                addLogos.IsEnabled = true;
                removeLogos.IsEnabled = true;
            }
            else
            {
                addLogos.IsEnabled = false;
                removeLogos.IsEnabled = false;
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
                }

                var mpcRegSettings = Registry.CurrentUser.OpenSubKey(@"Software\MPC-HC\MPC-HC\Settings", true);
                if (mpcRegSettings != null)
                {
                    MPCHC MPC = new MPCHC();
                    bool mpcWebRunning = MPC.DetectMPCSetting("EnableWebServer");
                    bool siteVisiblity = MPC.DetectMPCSetting("WebServerLocalhostOnly");
                    bool backupPlaylistVisibility = MPC.DetectMPCSetting("RememberPlaylistItems");
                    startfull.IsChecked = MPC.DetectMPCSetting("LaunchFullScreen");
                    exitfull.IsChecked = MPC.DetectMPCSetting("ExitFullscreenAtTheEnd");
                    backupPlaylist.IsEnabled = backupPlaylistVisibility;
                    enablewebserver.IsChecked = mpcWebRunning;
                    webcontrols.IsEnabled = mpcWebRunning;

                    if (System.IO.File.Exists(siteLocation + @"\display.html") && mpcWebRunning && !mpcRegSettings.GetValue("WebRoot").ToString().Contains("*"))
                    {
                        InfoScreen.IsEnabled = true;
                        if(Screen.AllScreens.Count() > 2)
                        {
                            showInfo.IsEnabled = true;
                            infoScreenSelection.IsEnabled = true;
                        }
                        else
                        {
                            showInfo.IsEnabled = false;
                            infoScreenSelection.IsEnabled = false;
                        }
                        
                    }
                    else
                    {
                        InfoScreen.IsEnabled = false;
                        showInfo.IsEnabled = false;
                        infoScreenSelection.IsEnabled = false;
                    }

                    string mpcPort = Settings.Default["mpcWebPort"].ToString();
                    if (mpcRegSettings.GetValue("WebServerPort") != null)
                    {
                        mpcPort = mpcRegSettings.GetValue("WebServerPort").ToString();
                        mpcWebPort.Text = mpcPort;
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
                    if (mpcRegSettings.GetValue("WebRoot").ToString().Contains("*"))
                        mpcSiteType.SelectedIndex = 0;
                    else
                        mpcSiteType.SelectedIndex = 1;
                    if (siteVisiblity)
                        mpcSiteVisibility.SelectedIndex = 0;
                    else
                        mpcSiteVisibility.SelectedIndex = 1;
                    setRecommended.IsEnabled = true;
                    mpcSettings.IsEnabled = true;
                }
                else
                {
                    IPAddress.Text = "MPC-HC Not Detected";
                }
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
           
            var mpcRegSettings = Registry.CurrentUser.OpenSubKey(@"Software\MPC-HC\MPC-HC\Settings", true);
            if (mpcRegSettings != null)
            {
                MPCHC MPC = new MPCHC();
                if (MPC.DetectMPCSetting("WebServerLocalhostOnly") || ip == "127.0.0.1")
                    ip = "localhost";
            }

            string ServerAddress = ip + ":" + mpcPort;
            IPAddress.Text = "Website: http://" + ServerAddress;
        }

        private void MonitorCount()
        {
            mpcPlayback.Items.Clear();
            infoScreenSelection.Items.Clear();
            foreach (var screen in System.Windows.Forms.Screen.AllScreens)
            {
                mpcPlayback.Items.Add(screen.DeviceName.Replace(@"\\.\DISPLAY","Monitor "));
                infoScreenSelection.Items.Add(screen.DeviceName.Replace(@"\\.\DISPLAY", "Monitor "));
            }
            int playscreennum = mpcPlayback.Items.IndexOf("Monitor " + Settings.Default["defaultPlayMonitor"]);
            var mpcRegSettings = Registry.CurrentUser.OpenSubKey(@"Software\MPC-HC\MPC-HC\Settings", true);
            if (mpcRegSettings != null)
            {
                string test = mpcRegSettings.GetValue("FullScreenMonitor").ToString();
                if (mpcRegSettings.GetValue("FullScreenMonitor") != null && mpcRegSettings.GetValue("FullScreenMonitor").ToString() != "")
                    playscreennum = mpcPlayback.Items.IndexOf(mpcRegSettings.GetValue("FullScreenMonitor").ToString().Replace(@"\\.\DISPLAY", "Monitor "));
            }


            if (playscreennum > 0)
            {
                mpcPlayback.SelectedIndex = playscreennum;
            }
            else
            {
                mpcPlayback.SelectedIndex = 0;
            }

            int infoscreennum = infoScreenSelection.Items.IndexOf("Monitor " + Settings.Default["defaultInfoScreen"].ToString());

            if (playscreennum == infoscreennum)
            {
                if (playscreennum < Screen.AllScreens.Count() - 1)
                    infoscreennum = playscreennum + 1;
                else
                {
                    if (playscreennum > 0)
                        infoscreennum = playscreennum - 1;
                    else
                        infoscreennum = 0;
                }

            }

            if (infoscreennum > 0)
            {
                infoScreenSelection.SelectedIndex = infoscreennum;
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
            cssFontSelection.Items.Clear();


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
                cssFontSelection.Items.Add(FontList[i]);
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

            cssFontSelection.SelectedIndex = FontList.FindIndex(x => x.Equals(Settings.Default["cssTextFont"].ToString()));
            if (cssFontSelection.SelectedIndex < 0) { cssFontSelection.SelectedIndex = FontList.FindIndex(x => x.Equals("Helvetica")); }
        }

        private void CreateWebsite(bool overwrite, bool updateinfo)
        {
            Website Website = new Website();
            Website.GenerateWebsite(overwrite);
            LoadFontlist();
            var Logos = logoList.Items.Cast<String>().ToList();
            if (!File.Exists(Settings.Default["siteLocation"] + @"\display.html") || overwrite)
                Website.CreateDisplayPage(Logos);
            if (!File.Exists(Settings.Default["siteLocation"] + @"\panelinfo.html") || updateinfo)
            {
                updateInfoPage();
            }
            DoThingsExists();
        }

        private void updateInfo_Click(object sender, RoutedEventArgs e)
        {
            SaveUserValues();
            bool generatewebsite = DetectMissingFiles();
            if (generatewebsite)
            {
                CreateWebsite(false, true);
            }
            else
            {
                updateInfoPage();
            }
        }

        private void updateInfoPage()
        {
            string PanelInfo = panelInfo.Text;
            bool IfPresenters = Convert.ToBoolean(presentedBy.IsChecked);
            string Presenters = panelPresenters.Text;
            bool IfPlaying = Convert.ToBoolean(showVideoInfo.IsChecked);
            string Playing = videoInfo.Text;
            bool IfUpcoming = Convert.ToBoolean(showNextPanel.IsChecked);
            string Upcoming1 = upcoming1.Text;
            string Upcoming2 = upcoming2.Text;


            Website Website = new Website();
            Website.UpdatePage(PanelInfo, IfPresenters, Presenters, IfPlaying, Playing, IfUpcoming, Upcoming1, Upcoming2);
        }

        private void internetOptions_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("InetCpl.Cpl");
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

        private void mpcSettigns_Click(object sender, RoutedEventArgs e)
        {
            DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("This will apply changes to MPC-HC settings and restart all instances of the application if it is currently open, would you like to continue?", "Apply Changes", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == System.Windows.Forms.DialogResult.Yes)
            {
                if (mpcSiteType.Text == "Built-In Site" || enablewebserver.IsChecked == false)
                {
                    if (showInfo.Content.ToString() == "Hide")
                    {
                        showInfoScreen.LoadWebsite("closeBrowser");
                        showInfo_Click(sender, e);
                    }

                }
                saveMonitorDefaults();
                if (mpcWebPort.Text != "") { Settings.Default["mpcWebPort"] = mpcWebPort.Text; }

                string WebServer = "0";
                string CustomSite = mpcSiteType.Text;
                string StartFull = "0";
                string ExitFull = "0";
                mpcWebPort.Text = Settings.Default["mpcWebPort"].ToString();
                string SiteVisibility = "0";
                if (enablewebserver.IsChecked == true) { WebServer = "1"; }
                if (startfull.IsChecked == true) { StartFull = "1"; }
                if (exitfull.IsChecked == true) { ExitFull = "1"; }
                if (mpcSiteVisibility.Text == "Localhost") { SiteVisibility = "1"; }

                MPCHC MPCSettings = new MPCHC();
                MPCSettings.ChangeMPCSettings(WebServer, CustomSite, StartFull, ExitFull, SiteVisibility);

                FindNetworkAddress(Settings.Default["mpcWebPort"].ToString());

                if (enablewebserver.IsChecked == true && CustomSite == "Custom Site")
                {
                    bool generatewebsite = DetectMissingFiles();

                    if (generatewebsite)
                    {
                        CreateWebsite(false, false);
                    }
                }

                DoThingsExists();
            }
        }

        private void removeLogos_Click(object sender, RoutedEventArgs e)
        {
            var selected = logoList.SelectedIndex;
            if (selected >= 0)
            {
                string logo = Settings.Default["siteLocation"].ToString() + @"\logos\" + logoList.SelectedValue.ToString();
                if (File.Exists(logo) == true)
                {
                    File.SetAttributes(logo, FileAttributes.Normal);
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
                    File.SetAttributes(font, FileAttributes.Normal);
                    File.Delete(font);
                }
                LoadFontlist();
                fontList.SelectedIndex = selected;
            }
        }

        private void updateSiteStyling_Click(object sender, RoutedEventArgs e)
        {
            bool generatewebsite = DetectMissingFiles();
            if (generatewebsite)
            {
                CreateWebsite(false, false);
            }
            Settings.Default["cssBGColor"] = "#" + cssBGColor.Fill.ToString().Substring(3);
            Settings.Default["cssTextColor"] = "#" + cssTextColor.Fill.ToString().Substring(3);
            Settings.Default["cssTextFont"] = cssFontSelection.Text;
            Settings.Default["cssTextSize"] = cssTextSize.Text;
            SaveToRegistry();

            string panelInfo = Settings.Default["siteLocation"] + @"\panelinfo.html";
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

            if (showInfoScreen != null)
                showInfoScreen.LoadWebsite("refresh");
        }



        private void alwaysOnTop_Checked(object sender, RoutedEventArgs e)
        {
            Topmost = true;
            Settings.Default["alwaysOnTop"] = Topmost;
            SaveToRegistry();
        }
        private void alwaysOnTop_UnChecked(object sender, RoutedEventArgs e)
        {
            Topmost = false;
            Settings.Default["alwaysOnTop"] = Topmost;
            SaveToRegistry();
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
            Settings.Default["videoInfoSize"] = videoInfoSize.Text;
            Settings.Default["upcoming1Font"] = upcoming1Font.Text;
            Settings.Default["upcoming1Size"] = upcoming1Size.Text;
            Settings.Default["upcoming2Font"] = upcoming2Font.Text;
            Settings.Default["upcoming2Size"] = upcoming2Size.Text;
            SaveToRegistry();
        }

        private void mpcControls_Click(object sender, RoutedEventArgs e)
        {
            MPCHC MPC = new MPCHC();
            string command = (sender as System.Windows.Controls.Button).Uid.ToString();
            MPC.ControlMPC(Convert.ToInt32(command));
        }
        private void volumeMixer_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("sndvol.exe");
        }

        private void backupPlaylist_Click(object sender, RoutedEventArgs e)
        {
            string playlist = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\MPC-HC\default.mpcpl";
            if (System.IO.File.Exists(playlist))
            {
                System.Windows.Forms.SaveFileDialog saveDialog = new System.Windows.Forms.SaveFileDialog();
                saveDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                saveDialog.Title = "Save Current Playlist Here";
                saveDialog.Filter = "MPC-HC playlist (*.mpcpl)|*.mpcpl";
                if (saveDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    try
                    {
                        File.Copy(playlist, saveDialog.FileName, true);
                    }
                    catch (IOException copyError)
                    {
                        DialogResult dialogResult = System.Windows.Forms.MessageBox.Show(copyError.Message, "Error Copying File", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MPCHC MPC = new MPCHC();
                if (MPC.DetectMPCSetting("RememberPlaylistItems"))
                {
                    DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("File does not exist, try adding files to the playlist and try again.", "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("Remember last playlist is not abled, please change that setting or use the change settings button in this appplication then try again.", "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }


            }

        }

        private void webcontrols_Click(object sender, RoutedEventArgs e)
        {
            MPCHC MPC = new MPCHC();
            bool IsMPCrunning = MPC.ControlMPC(0);
            bool WebServerEnabled = MPC.DetectMPCSetting("EnableWebServer");
            if (WebServerEnabled == true)
            {
                if (IsMPCrunning == true)
                {
                    ContentWindow webControlsForm = new ContentWindow();
                    webControlsForm.LoadWebsite("http://localhost:" + Settings.Default["mpcWebPort"].ToString() + "/controls.html");
                    webControlsForm.Width = 860;
                    webControlsForm.Height = 420;
                    webControlsForm.Text = "Web Controls";
                    webControlsForm.FormClosed += webcontrols_FormClosed;
                    webControlsForm.Show();
                    //Process.Start("http://localhost:" + Settings.Default["mpcWebPort"].ToString() + "/controls.html");
                    webcontrols.IsEnabled = false;
                }
            }
            else
            {
                webcontrols.IsEnabled = false;
                IPAddress.Text = "MPC Web Server Not Enabled";
            }

        }

        private void webcontrols_FormClosed(object sender, FormClosedEventArgs e)
        {
            DoThingsExists();
        }

        private void mpchc_Click(object sender, RoutedEventArgs e)
        {
            MPCHC MPC = new MPCHC();
            MPC.ControlMPC(1);
        }

        private void showInfo_Click(object sender, RoutedEventArgs e)
        {
            bool generatewebsite = DetectMissingFiles();
            if (generatewebsite)
            {
                CreateWebsite(false, false);
            }
            if (showInfoScreen == null)
            {
                MPCHC MPC = new MPCHC();
                bool IsMPCrunning = MPC.ControlMPC(0);

                if (IsMPCrunning == true &&  Screen.AllScreens.Count() > infoScreenSelection.SelectedIndex )
                {
                    saveMonitorDefaults();
                    showInfoScreen = new ContentWindow();
                    showInfoScreen.LoadInfoScreen(infoScreenSelection.SelectedIndex);
                    showInfoScreen.FormClosed += showInfoScreen_FormClosed;
                    showInfoScreen.BackColor = System.Drawing.Color.Black;
                    showInfoScreen.Opacity = 0;
                    
                    showInfoScreen.Show();
                    showInfo.Content = "Hide";
                }
                else
                {
                    if (Screen.AllScreens.Count() <= infoScreenSelection.SelectedIndex)
                        DoThingsExists();
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

        private void fadeVideo_Click(object sender, RoutedEventArgs e)
        {

            if (fadeVideo.Content.ToString() == "Fade Out")
            {
                MonitorCount();
                fadeVideo.Content = "Fade In";
                testScreen.IsEnabled = false;
                fillScreen(mpcPlayback.SelectedIndex, int.Parse(fadeVideoTime.Text), false);
            }
            else
            {
                blankVideo.fadeTimer(false, int.Parse(fadeVideoTime.Text));
            }
        }

        private void testScreen_Click(object sender, RoutedEventArgs e)
        {
            if (testScreen.Content.ToString() == "Test Playback Screen")
            {
                testScreen.Content = "Close Screen Test";
                fadeVideo.IsEnabled = false;
                fillScreen(mpcPlayback.SelectedIndex,500, true);
            }
            else
            {
                blankVideo.fadeTimer(false, 500);
            }
        }

        private void fillScreen(int screen, int timer, bool displaylogo)
        {
            blankVideo = new ContentWindow();
            blankVideo.Opacity = 0;
            blankVideo.CoverScreen(screen, displaylogo);
            blankVideo.fadeTimer(true, timer);
            blankVideo.FormClosed += fadeVideo_FormClosed;
            blankVideo.BackColor = System.Drawing.Color.Black;
            blankVideo.Show();
        }

        private void fadeVideo_FormClosed(object sender, FormClosedEventArgs e)
        {
            blankVideo = null;
            fadeVideo.Content = "Fade Out";
            fadeVideo.IsEnabled = true;
            testScreen.Content = "Test Playback Screen";
            testScreen.IsEnabled = true;
        }

        private void openSiteFolder_Click(object sender, RoutedEventArgs e)
        {
            string sitefolder = Settings.Default["siteLocation"].ToString();
            string windir = Environment.GetEnvironmentVariable("WINDIR");
            System.Diagnostics.Process prc = new System.Diagnostics.Process();
            prc.StartInfo.FileName = windir + @"\explorer.exe";
            prc.StartInfo.Arguments = sitefolder;
            prc.Start();
            
        }

        private void setRecommended_Click(object sender, RoutedEventArgs e)
        {
            startfull.IsChecked = true;
            exitfull.IsChecked = false;
            enablewebserver.IsChecked = true;
            mpcSiteType.SelectedIndex = 1;
            if (Screen.AllScreens.Count() > 2)
                mpcSiteVisibility.SelectedIndex = 0;
            else
                mpcSiteVisibility.SelectedIndex = 1;
        }

        private void refreshMonitors_Click(object sender, RoutedEventArgs e)
        {
            DoThingsExists();
        }

        private void saveMonitorDefaults()
        {
            Settings.Default["defaultInfoScreen"] = infoScreenSelection.Items[infoScreenSelection.SelectedIndex].ToString().Replace("Monitor ", "");
            Settings.Default["defaultPlayMonitor"] = mpcPlayback.Items[mpcPlayback.SelectedIndex].ToString().Replace("Monitor ", "");
            SaveToRegistry();
        }


        private void cssBGcolorButton_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog BGcolor = new ColorDialog();
            if (BGcolor.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var BGbrush = new SolidColorBrush(Color.FromRgb(BGcolor.Color.R,BGcolor.Color.G,BGcolor.Color.B));
                cssBGColor.Fill =  BGbrush;
                Settings.Default["cssBGcolor"] = "#" + BGcolor.Color.R.ToString("X2") + BGcolor.Color.G.ToString("X2") + BGcolor.Color.B.ToString("X2");
            }
        }

        private void cssTextColorButton_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog TextColor = new ColorDialog();
            if (TextColor.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var BGbrush = new SolidColorBrush(Color.FromRgb(TextColor.Color.R, TextColor.Color.G, TextColor.Color.B));
                cssTextColor.Fill = BGbrush;
                Settings.Default["cssTextColor"] = "#" + TextColor.Color.R.ToString("X2") + TextColor.Color.G.ToString("X2") + TextColor.Color.B.ToString("X2");
            }
        }

        private void defaultSettings_Click(object sender, RoutedEventArgs e)
        {
            DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("This will overwrite all saved setting for AMV Theater Assistant with the application defaults, would you like to continue?\n\nThis will not change any MPC-HC settings.", "Reset Settings", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == System.Windows.Forms.DialogResult.Yes)
            {
                Settings.Default.Reset();
                SaveToRegistry();
                LoadSettings();
            }
        }

        private void eraseSettings_Click(object sender, RoutedEventArgs e)
        {
            DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("This will erase all saved setting for AMV Theater Assistant from your system, would you like to continue?\n\nThis will not change any MPC-HC settings.", "Erase Settings", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == System.Windows.Forms.DialogResult.Yes)
            {
                Website Website = new Website();
                Website.RemoveThings(false);
                LoadSettings();
            }
        }

        private void eraseFilesSettings_Click(object sender, RoutedEventArgs e)
        {
            DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("This will erase all files and settings created by AMV Theater Assistant from your system, would you like to continue?\n\nThis will not change any MPC-HC settings.", "Erase Files & Settings", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == System.Windows.Forms.DialogResult.Yes)
            {
                if (showInfo.Content.ToString() == "Hide")
                    showInfo_Click(sender, e);
                showInfoScreen.LoadWebsite("closeBrowser");
                Website Website = new Website();
                Website.RemoveThings(true);
                LoadSettings();
            }
        }
    }
}
