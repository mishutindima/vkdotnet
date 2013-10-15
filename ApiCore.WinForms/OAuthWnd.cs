using System;
using System.Collections;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using ApiCore.Utils.Authorization;

namespace ApiCore.WinForms
{
    public partial class OAuthWnd : Form
    {
        private OAuthUries Uries;
        public string TokenResponse { get; private set; }

        public OAuthWnd(OAuthUries uries)
        {
            Uries = uries;
            InitializeComponent();
        }

        private void LoginWnd_Shown(object sender, EventArgs e)
        {
            LoginBrowser.Navigate(Uries.AuthUri);
        }

        private void LoginBrowser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            if (e.Url.ToString().Contains(Uries.TokenUri.ToString()))
            {
                TokenResponse = e.Url.ToString();
                Close();
            }
        }
    }
}
