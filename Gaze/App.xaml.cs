using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using EyeXFramework.Wpf;

namespace Gaze
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    /// 
    
    public partial class App : Application
    {
        public WpfEyeXHost eyeXHost;

        public App()
        {
            eyeXHost = new WpfEyeXHost();
            eyeXHost.Start();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            eyeXHost.Dispose();
        }

    }
}
