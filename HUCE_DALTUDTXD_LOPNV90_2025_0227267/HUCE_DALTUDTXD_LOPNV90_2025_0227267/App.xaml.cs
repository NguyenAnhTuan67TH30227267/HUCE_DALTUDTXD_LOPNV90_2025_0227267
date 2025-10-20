using HUCE_DALTUDTXD_LOPNV90_2025_0227267.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace HUCE_DALTUDTXD_LOPNV90_2025_0227267
{
    public partial class App : Application
    {
        protected void ApplicationStart(object sender, StartupEventArgs e)
        {
            var loginView = new LoginView();
            loginView.Show();
        }
    }
}

