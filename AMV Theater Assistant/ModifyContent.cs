﻿using System;
using System.Windows.Forms;
using AMVTheaterAssistant.Properties;
using Microsoft.Win32;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Drawing.Text;
using System.Text;

namespace AMVTheaterAssistant
{
    class Website
    {
        public void GenerateWebsite()
        {
            string sitefolder = Settings.Default["siteLocation"].ToString();
            if (File.Exists(sitefolder + @"\variables.html"))
            {
                File.Delete(sitefolder + @"\variables.html");
            }
            Directory.CreateDirectory(sitefolder + @"\fonts");
            Directory.CreateDirectory(sitefolder + @"\logos");
            File.WriteAllText(sitefolder + @"\index.html", Properties.Resources.index_html);
            string javafolder = sitefolder + @"\javascript";
            string imagesfolder = sitefolder + @"\images";
            Directory.CreateDirectory(javafolder);
            System.IO.File.WriteAllText(javafolder + @"\jquery-2.2.3.js", Properties.Resources.jquery_2_2_3);
            System.IO.File.WriteAllText(javafolder + @"\panelinfo.js", Properties.Resources.panelinfo);
            Directory.CreateDirectory(imagesfolder);
            AddThings AddThings = new AddThings();
            AddThings.SavePNG(Properties.Resources.logo, imagesfolder, @"\logo.png");
        }

        public void UpdatePage(string PanelInfo, bool IfPresenters, string Presenters, bool IfPlaying, string Playing, bool IfUpcoming, string Upcoming1, string Upcoming2)
        {

            string Title1 = Settings.Default["pageTitleLine1"].ToString();
            string Title1Font = Settings.Default["title1Font"].ToString();
            string Title1Size = Settings.Default["title1Size"].ToString();
            bool IfTitle2 = Convert.ToBoolean(Settings.Default["enableTitleLine2"]);
            string Title2 = Settings.Default["pageTitleLine2"].ToString();
            string Title2Font = Settings.Default["title2Font"].ToString();
            string Title2Size = Settings.Default["title2Size"].ToString();
            string PanelInfoFont = Settings.Default["panelInfoFont"].ToString();
            string PanelInfoSize = Settings.Default["panelInfoSize"].ToString();
            string PresentersFont = Settings.Default["presentersFont"].ToString();
            string PresentersSize = Settings.Default["presentersSize"].ToString();
            string VideoInfoFont = Settings.Default["videoInfoFont"].ToString();
            string VideoInfoSize = Settings.Default["VideoInfoSize"].ToString();
            string Upcoming1Font = Settings.Default["upcoming1Font"].ToString();
            string Upcoming1Size = Settings.Default["upcoming1Size"].ToString();
            string Upcoming2Font = Settings.Default["upcoming2Font"].ToString();
            string Upcoming2Size = Settings.Default["upcoming2Size"].ToString();
            string SpanStart = "<span style=\"Font-Family: ";
            string FontSize = "; font-size: ";
            string SpanClose = "px\">";
            string br = "<br>";
            string br2 = "<br><br>";
            string SpanEnd = "</span>";

            string Title1Span = SpanStart + Title1Font + FontSize + Title1Size + SpanClose;
            string PageTitle = Title1Span + Title1 + SpanEnd;

            if (IfTitle2 == true)
            {
                string Title2Span = SpanStart + Title2Font + FontSize + Title2Size + SpanClose;
                PageTitle = PageTitle + br + Title2Span + Title2 + SpanEnd;
            }


            PanelInfo = SpanStart + PanelInfoFont + FontSize + PanelInfoSize + SpanClose + PanelInfo + SpanEnd;

            if (IfPresenters == true) { Presenters = br + SpanStart + PresentersFont + FontSize + PresentersSize + SpanClose + "Presented By" + br + Presenters + SpanEnd; }
            else { Presenters = ""; }

            if (IfPlaying == true) { Playing = br2 + SpanStart + VideoInfoFont + FontSize + VideoInfoSize + SpanClose + Playing + SpanEnd + br + SpanStart + VideoInfoFont + FontSize + VideoInfoSize + SpanClose + "[No Video Information]" + SpanEnd; }
            else { Playing = ""; }

            string Upcoming = "";
            if (IfUpcoming == true)
            {
                if (Upcoming2.Length > 0)
                {
                    if (Upcoming1.Length > 0) { Upcoming = br2 + SpanStart + Upcoming1Font + FontSize + Upcoming1Size + SpanClose + Upcoming1 + SpanEnd + br + SpanStart + Upcoming2Font + FontSize + Upcoming2Size + SpanClose + Upcoming2 + SpanEnd; }
                    else { Upcoming = br2 + SpanStart + Upcoming2Font + FontSize + Upcoming2Size + SpanClose + Upcoming2 + SpanEnd; }
                }
                else { Upcoming = br2 + SpanStart + Upcoming1Font + FontSize + Upcoming1Size + SpanClose + Upcoming1 + SpanEnd; }
            }
            else { Upcoming = ""; }

            string[] PanelInfoTXT = { PageTitle, "<div class=\"middle\"><p>", PanelInfo, Presenters + Playing + Upcoming, "</p></div>" };
            string filePath = Settings.Default["siteLocation"].ToString();
            System.IO.File.WriteAllLines(filePath + @"\panelinfo.htm", PanelInfoTXT);
        }
        public void CreateDisplayPage(List<string> Logos)
        {
            CreateCSS();
            List<string> displayPage = new List<string>(new string[] { "<!DOCTYPE html>", "<head>", "<title>Panel Information</title>", "" });

            displayPage.AddRange(new string[] { "", "<link rel=\"stylesheet\" type=\"text/css\" href=\"amvtt.css\">", "<script type=\"text/javascript\" src=\"javascript/jquery-2.2.3.js\" ></script >", "</head><body>", "<div id=\"PanelInfo\" class=\"main\"></div>", "<script type=\"text/javascript\" src=\"javascript/panelinfo.js\"></script>" });
            if (Logos.Count > 0)
            {
                displayPage.Add("<p class=\"bottom\">");
                for (int i = 0; i < Logos.Count; i++)
                {
                    displayPage.Add("<img src=\"logos/" + Logos[i] + "\" height=80>");
                }
                displayPage.Add("</p>");
            }
            displayPage.AddRange(new string[] { "</body></html>" });
            string filePath = Settings.Default["siteLocation"].ToString();
            File.WriteAllLines(filePath + @"\display.html", displayPage);
        }

        public void CreateCSS()
        {
            string fontDir = Settings.Default["siteLocation"].ToString() + @"\fonts";
            string customBGcolor = Settings.Default["cssBGcolor"].ToString();
            string customTextColor = Settings.Default["cssTextColor"].ToString();
            string customTextSize = Settings.Default["cssTextSize"].ToString();
            FindThings FindThings = new FindThings();
            List<string> fontList = FindThings.FindImportedFonts(fontDir);
            List<string> fontNames = FindThings.FindFontNames(fontList);

            List<string> css = new List<string>();
            string type = "";

            for (int i = 0; i < fontList.Count; i++)
            {
                css.Add("@font-face {");
                css.Add("font-family: " + fontNames[i] + ";");
                css.Add("src:");
                css.Add("url(" + "fonts/" + fontList[i] + ")");
                type = fontList[i].Substring(fontList[i].Length - 3, 3);
                if (type == "otf") { css.Add("format('" + "opentype" + "')"); }
                if (type == "ttf") { css.Add("format('" + "truetype" + "')"); }
                css.Add("}");
            }
            css.AddRange(new string[] { "", "html, body {", "color: " + customTextColor + ";", "font-size: " + customTextSize + "px;", "font-family: helvetica;", "background-color: " + customBGcolor + ";", "text-align: center;", "}", "", "div.main {", "margin: 0 auto;", "height: 98%;", "width: 98%;", "position: absolute;", "display: table;", "}", "", "div.middle {", "height: 70%;", "width: 100%;", "position: relative;", "display: table;", "}", "", "div.middle p {", "display: table-cell;", "vertical-align: middle;", "text-align: center;", "}", "", "p.bottom{", "width: 99%;", "text-align: center;", "position: absolute;", "bottom: 20px;", "}", "a {", "color: " + customTextColor + ";", "}", "a.link {", "color: " + customTextColor + ";", "}", "a.visited {", "color: " + customTextColor + ";", "}" });

            string filePath = Settings.Default["siteLocation"].ToString();
            File.WriteAllLines(filePath + @"\amvtt.css", css);
        }
    }
    class AddThings
    {

        public void SavePNG(Image img, string imgfolder, string filename)
        {
            byte[] byteArray = new byte[0];
            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                stream.Close();

                File.WriteAllBytes(imgfolder + filename, stream.ToArray());
            }
        }

        public void AddFiles(string filter, string location)
        {
            Microsoft.Win32.OpenFileDialog FindFontsDialog = new Microsoft.Win32.OpenFileDialog();

            FindFontsDialog.InitialDirectory = @"c:\";
            FindFontsDialog.Filter = filter;
            FindFontsDialog.FilterIndex = 1;
            FindFontsDialog.Multiselect = true;
            FindFontsDialog.RestoreDirectory = true;


            if (Directory.Exists(location))
            {
                if (FindFontsDialog.ShowDialog() == true)
                {


                    string[] files = FindFontsDialog.FileNames;
                    string filename;
                    string destfile;
                    foreach (string font in files)
                    {
                        filename = Path.GetFileName(font);
                        destfile = Path.Combine(location, filename);
                        if (font != destfile && File.Exists(destfile) != true)
                        {
                            File.Copy(font, destfile);
                        }

                    }
                }
            }
            else
            {

                MessageBox.Show("The fonts folder has been moved or deleted!", "Error", MessageBoxButtons.OK);
            }
        }

    }

    class FindThings
    {
        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int RemoveFontResourceEx(string lpszFilename, int fl, IntPtr pdv);

        public List<string> FindFontNames(List<string> fontList)
        {
            List<string> fontNames = new List<string>(fontList);
            string fontfile = "";
            for (int i = 0; i < fontNames.Count; i++)
            {
                PrivateFontCollection fontname = new PrivateFontCollection();
                fontfile = Settings.Default["siteLocation"].ToString() + @"\fonts\" + fontNames[i];
                fontname.AddFontFile(fontfile);
                fontNames[i] = fontname.Families[0].Name;
                fontname.Dispose();
                RemoveFontResourceEx(fontfile, 16, IntPtr.Zero);
            }

            return fontNames;
        }

        public List<string> FindImportedFonts(string fontDir)
        {
            List <string> fontList = new List<string>(Directory.EnumerateFiles(fontDir, "*.ttf"));
            for (int i = 0; i < fontList.Count; i++)
            {
                fontList[i] = Path.GetFileName(fontList[i]);
            }
            return fontList;
        }

        public List<string> FindImportedLogos(string logoDir)
        {
            List <string> logoList = new List<string>(Directory.EnumerateFiles(logoDir, "*.png"));
            logoList.AddRange(Directory.EnumerateFiles(logoDir, "*.jpg"));
            logoList.AddRange(Directory.EnumerateFiles(logoDir, "*.jpeg"));
            logoList.AddRange(Directory.EnumerateFiles(logoDir, "*.gif"));
            for (int i = 0;  i < logoList.Count; i++)
            {
                logoList[i] = Path.GetFileName(logoList[i]);
            }
            return logoList;
        }
    }
    class MPCHC
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SendMessage(IntPtr hwnd, int wMsg, int wParam, uint lParam);

        public bool ControlMPC(int command)
        {
            // Command List
            // 1  = Start MPC
            // 2  = Restart MPC
            // 3+ = wm_command

            Process[] RunningProcesses = Process.GetProcesses();
            bool mpcrunning = false;
            Parallel.ForEach(RunningProcesses, process =>
            {
                string ProcessName = process.ProcessName;

                ProcessName = ProcessName.ToLower();
                if (ProcessName.CompareTo("mpc-hc64") == 0 || ProcessName.CompareTo("mpc-hc") == 0)
                {

                    if (command > 2)
                    {
                        SendMessage(process.MainWindowHandle, 0x111, command, 0);
                    }
                    if (command == 2)
                    {
                        DialogResult dialogResult = MessageBox.Show(ProcessName.ToUpper() + " needs to be restarted for changes to apply, would you like to do that now?", "Restart MPC-HC", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dialogResult == DialogResult.Yes)
                        {
                            string processpath = process.MainModule.FileName;
                            process.Kill();
                            process.WaitForExit();
                            Process.Start(processpath);

                        }
                    }
                    mpcrunning = true;
                }
            });
            if (mpcrunning == false)
            {
                if (command == 0 || command > 2)
                {
                    DialogResult dialogResult = MessageBox.Show("MPC-HC is not running, would you like to start it now?", "Start MPC-HC", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialogResult == DialogResult.Yes)
                    {
                        command = 1;
                    }
                }
                if (command == 1)
                {
                    Process.Start(Settings.Default["mpcAppLocation"].ToString());
                    mpcrunning = true;
                }
            }
            return mpcrunning;
        }

        public bool DetectWebServer()
        {

            var mpcRegSettings = Registry.CurrentUser.OpenSubKey(@"Software\MPC-HC\MPC-HC\Settings", true);
            bool mpcWebRunning = false;
            if (mpcRegSettings != null)
            {

                if (mpcRegSettings.GetValue("EnableWebServer") != null)
                {
                    if (mpcRegSettings.GetValue("EnableWebServer").ToString() != "1")
                    {
                        DialogResult dialogResult = MessageBox.Show("MPC-HC web server is not enabled, would you like to do that now?", "Start MPC-HC Web Server", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dialogResult == DialogResult.Yes)
                        {
                            StartWebServer();
                            mpcWebRunning = true;
                        }

                    }
                    else
                    {
                        mpcWebRunning = true;
                    }
                }

            }
            return mpcWebRunning;
        }

        public void StartWebServer()
        {
            var mpcRegSettings = Registry.CurrentUser.OpenSubKey(@"Software\MPC-HC\MPC-HC\Settings", true);
            mpcRegSettings.SetValue("EnableWebServer", "1", RegistryValueKind.DWord);
            ControlMPC(2);
        }

        public void ChangeMPCSettings(string SettingsLocation, string StartFull, string WebPort, string PlaylistLocation, string PlaylistSize)
        {


            var mpcRegSettings = Registry.CurrentUser.OpenSubKey(SettingsLocation, true);
            if (mpcRegSettings == null)
            {
                Registry.CurrentUser.CreateSubKey(SettingsLocation);
                mpcRegSettings = Registry.CurrentUser.OpenSubKey(SettingsLocation, true);
            }
            mpcRegSettings.SetValue("LaunchFullScreen", StartFull, RegistryValueKind.DWord);
            mpcRegSettings.SetValue("FullScreenMonitor", @"\\.\DISPLAY" + Settings.Default["defaultPlayMonitor"].ToString(), RegistryValueKind.String);
            mpcRegSettings.SetValue("HideFullscreenControls", "1", RegistryValueKind.DWord);
            mpcRegSettings.SetValue("HideFullscreenControlsPolicy", "0", RegistryValueKind.DWord);
            mpcRegSettings.SetValue("LogoID2", "206", RegistryValueKind.DWord);
            mpcRegSettings.SetValue("ShowOSD", "0", RegistryValueKind.DWord);

            if (Convert.ToBoolean(Settings.Default["mpcEnableCustom"]) == true)
            {
                mpcRegSettings.SetValue("WebRoot", Settings.Default["siteLocation"], RegistryValueKind.String);
            }
            else
            {
                mpcRegSettings.SetValue("WebRoot", "*", RegistryValueKind.String);
            }

            mpcRegSettings.SetValue("WebServerPort", WebPort, RegistryValueKind.DWord);

            var mpcRegPlaylist = Registry.CurrentUser.OpenSubKey(PlaylistSize, true);
            if (mpcRegPlaylist == null)
            {
                Registry.CurrentUser.CreateSubKey(PlaylistSize);
                mpcRegPlaylist = Registry.CurrentUser.OpenSubKey(PlaylistSize, true);
            }
            mpcRegPlaylist.SetValue("sizeFloatCX", "300", RegistryValueKind.DWord);
            mpcRegPlaylist.SetValue("sizeFloatCY", "400", RegistryValueKind.DWord);

            mpcRegPlaylist = Registry.CurrentUser.OpenSubKey(PlaylistLocation, true);
            mpcRegPlaylist.SetValue("DockPosX", "200", RegistryValueKind.DWord);
            mpcRegPlaylist.SetValue("DockPosY", "200", RegistryValueKind.DWord);
            mpcRegPlaylist.SetValue("DockState", "59423", RegistryValueKind.DWord);
        }
    }
}
