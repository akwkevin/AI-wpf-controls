using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;
using Microsoft.Win32;

namespace AIStudio.Wpf.Controls
{
    /// <summary>
    /// WebView.xaml 的交互逻辑
    /// </summary>
    public partial class WebBrowser : UserControl
    {
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
          "Source", typeof(string), typeof(WebBrowser), new PropertyMetadata(default(string), SourcePropertyChangedCallback));

        public string Source
        {
            get
            {
                return (string)this.GetValue(SourceProperty);
            }
            set
            {
                this.SetValue(SourceProperty, value);
            }
        }

        public static readonly DependencyProperty IsBrowserOpenProperty = DependencyProperty.Register(
             "IsBrowserOpen", typeof(bool), typeof(WebBrowser), new PropertyMetadata(false, SourcePropertyChangedCallback));

        public bool IsBrowserOpen
        {
            get
            {
                return (bool)this.GetValue(IsBrowserOpenProperty);
            }
            set
            {
                this.SetValue(IsBrowserOpenProperty, value);
            }
        }

        private static void SourcePropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var uploadFile = (WebBrowser)dependencyObject;
            if (uploadFile != null)
            {
                if (uploadFile.IsLoaded)
                {
                    uploadFile.Navigate();
                }
            }
        }

        public WebBrowser()
        {
            InitializeComponent();

            int ieVersion = GetBrowserVersion();
            if (IfWindowsSupport())
            {
                SetWebBrowserFeatures(ieVersion < 11 ? ieVersion : 11);
            }
            else
            {
                //如果不支持IE8  则修改为当前系统的IE版本
                SetWebBrowserFeatures(ieVersion < 7 ? 7 : ieVersion);
            }

            webBrowser.Navigated += WebBrowser_Navigated;
            this.Loaded += WebBrowser_Loaded;
        }

        private void WebBrowser_Navigated(object sender, NavigationEventArgs e)
        {
            SuppressScriptErrors(sender as System.Windows.Controls.WebBrowser, true);
        }

        private void WebBrowser_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= WebBrowser_Loaded;
            Navigate();
        }

        private void Navigate()
        {
            if (IsBrowserOpen)
            {
                //webBrowser.Document
                //System.Diagnostics.Process.Start(Source);
                System.Diagnostics.Process.Start("explorer.exe", Source);
                link.NavigateUri = new Uri(Source);
                run.Text = Source;
                txtHit.Visibility = Visibility.Visible;
                webBrowser.Visibility = Visibility.Collapsed;
            }
            else
            {
                txtHit.Visibility = Visibility.Collapsed;
                webBrowser.Visibility = Visibility.Visible;
                webBrowser.Navigate(Source);
            }
        }

        public void SuppressScriptErrors(System.Windows.Controls.WebBrowser wb, bool Hide)
        {
            FieldInfo fiComWebBrowser = typeof(System.Windows.Controls.WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fiComWebBrowser == null) return;

            object objComWebBrowser = fiComWebBrowser.GetValue(wb);
            if (objComWebBrowser == null) return;

            objComWebBrowser.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, objComWebBrowser, new object[] { Hide });
        }

        #region WebBrowser设置
        /// <summary>
        /// 查询系统环境是否支持IE8以上版本
        /// </summary>
        public static bool IfWindowsSupport()
        {
            bool isWin7 = Environment.OSVersion.Version.Major > 6;
            bool isSever2008R2 = Environment.OSVersion.Version.Major == 6
                && Environment.OSVersion.Version.Minor >= 1;

            if (!isWin7 && !isSever2008R2)
            {
                return false;
            }
            else return true;
        }

        static void SetWebBrowserFeatures(int ieVersion)
        {
            // don't change the registry if running in-proc inside Visual Studio
            if (LicenseManager.UsageMode != LicenseUsageMode.Runtime)
                return;
            //获取程序及名称
            var appName = System.IO.Path.GetFileName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            //得到浏览器的模式的值
            UInt32 ieMode = GeoEmulationModee(ieVersion);
            var featureControlRegKey = @"HKEY_CURRENT_USER\Software\Microsoft\Internet Explorer\Main\FeatureControl\";
            //设置浏览器对应用程序（appName）以什么模式（ieMode）运行
            Registry.SetValue(featureControlRegKey + "FEATURE_BROWSER_EMULATION", appName, ieMode, RegistryValueKind.DWord);
            // enable the features which are "On" for the full Internet Explorer browser
            Registry.SetValue(featureControlRegKey + "FEATURE_ENABLE_CLIPCHILDREN_OPTIMIZATION", appName, 1, RegistryValueKind.DWord);
        }

        static int GetBrowserVersion()
        {
            int browserVersion = 0;
            using (var ieKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Internet Explorer",
              RegistryKeyPermissionCheck.ReadSubTree,
              System.Security.AccessControl.RegistryRights.QueryValues))
            {
                var version = ieKey.GetValue("svcVersion");
                if (null == version)
                {
                    version = ieKey.GetValue("Version");
                    if (null == version)
                        throw new ApplicationException("Microsoft Internet Explorer is required!");
                }
                int.TryParse(version.ToString().Split('.')[0], out browserVersion);
            }

            if (browserVersion < 7)
            {
                throw new ApplicationException("Not Support");
            }
            return browserVersion;
        }

        static UInt32 GeoEmulationModee(int browserVersion)
        {
            UInt32 mode = 11000; // Internet Explorer 11. Webpages containing standards-based !DOCTYPE directives are displayed in IE11 Standards mode. 
            switch (browserVersion)
            {
                case 7:
                    mode = 7000; // Webpages containing standards-based !DOCTYPE directives are displayed in IE7 Standards mode. 
                    break;
                case 8:
                    mode = 8000; // Webpages containing standards-based !DOCTYPE directives are displayed in IE8 mode. 
                    break;
                case 9:
                    mode = 9000; // Internet Explorer 9. Webpages containing standards-based !DOCTYPE directives are displayed in IE9 mode.                    
                    break;
                case 10:
                    mode = 10000; // Internet Explorer 10.
                    break;
                case 11:
                    mode = 11000; // Internet Explorer 11
                    break;
            }
            return mode;
        }
        #endregion

        private void link_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink link = sender as Hyperlink;
            Process.Start("explorer.exe", link.NavigateUri.AbsoluteUri);
        }
    }
}
