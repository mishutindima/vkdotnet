﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace ApiCore.Utils.Authorization
{
    public partial class OAuthWnd : Form
    {
        public int AppId;
        public string Scope;
        public string Display;
        public bool Revoke;

        public OAuthSessionInfo SessionData;
        public bool Authenticated = false;

        public OAuthWnd(int appId, string scope, string display, bool revoke)
        {
            this.AppId = appId;
            this.Scope = scope;
            this.Display = display;
            Revoke = revoke;
            InitializeComponent();
        }

        private void LoginWnd_Shown(object sender, EventArgs e)
        {
            string urlTemplate = "http://api.vkontakte.ru/oauth/authorize?client_id={0}&scope={1}&redirect_uri={2}&display={3}&response_type=token&revoke={4}";
            this.LoginBrowser.Navigate(string.Format(urlTemplate, new object[]
                {
                    this.AppId,
                    this.Scope, 
                    "http://vkontakte.ru/api/login_success.html", 
                    this.Display, 
                    Convert.ToByte(Revoke)
                }));
        }

        private void LoginBrowser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            if (e.Url.ToString().Contains("#"))
            {
                Regex r = new Regex(@"\#(.*)");
                string[] json = r.Match(e.Url.ToString()).Value.Replace("#", "").Split('&');
                Hashtable h = new Hashtable();
                foreach (string str in json)
                {
                    string[] kv = str.Split('=');
                    h[kv[0]] = kv[1];
                }

                this.SessionData = new OAuthSessionInfo();
                this.SessionData.AppId = this.AppId;
                this.SessionData.Scope = this.Scope;
                this.SessionData.Token = (string)h["access_token"];
                this.SessionData.Expire = Convert.ToInt32(h["expires_in"]);
                this.SessionData.UserId = Convert.ToInt32(h["user_id"]);

                this.Authenticated = true;
                this.Close();
            }

        }
    }
}
