using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using EyeXFramework.Wpf;
using Gaze.Data;

namespace Gaze
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    /// 
    
    public partial class App : Application
    {
        public WpfEyeXHost eyeXHost;
        public UserData userData;

        public App()
        {
            eyeXHost = new WpfEyeXHost();
            eyeXHost.Start();
            userData = new UserData();
        }

    protected override void OnStartup(StartupEventArgs e)
      {
            base.OnStartup(e);
            //check settings for language
            //get language files from web service
      }

    protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            eyeXHost.Dispose();
        }

    }
}
