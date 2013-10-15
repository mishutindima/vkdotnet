using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using ApiCore.Utils.Authorization.Exceptions;

namespace ApiCore.Utils.Authorization
{
    public class OAuthUries
    {
        public OAuthUries(Uri authUri, Uri tokenUri)
        {
            AuthUri = authUri;
            TokenUri = tokenUri;
        }

        public Uri AuthUri { get; private set; }
        public Uri TokenUri { get; private set; }
    }

    public class OAuth
    {
        private const string AuthorizeUri = "http://api.vkontakte.ru/oauth/authorize?client_id={0}&scope={1}&redirect_uri={2}&display=page&response_type=token&revoke={3}";
        private const string TokenUri = "https://oauth.vk.com/blank.html";

        private int AppId;
        private string Scope;

        public OAuth(int appId, string scope)
        {
            AppId = appId;
            Scope = scope;
        }

        public OAuthUries GetAuthUries(bool relogin)
        {
            return
                new OAuthUries(
                    new Uri(string.Format(AuthorizeUri, new object[] {AppId, Scope, TokenUri, Convert.ToByte(relogin)})),
                    new Uri(TokenUri));
        }

        public OAuthSessionInfo Authorize(string tokenResponse)
        {
            Regex r = new Regex(@"\#(.*)");
            string[] json = r.Match(tokenResponse).Value.Replace("#", "").Split('&');
            Hashtable h = new Hashtable();
            foreach (string str in json)
            {
                string[] kv = str.Split('=');
                h[kv[0]] = kv[1];
            }

            return new OAuthSessionInfo
                {
                    AppId = this.AppId,
                    Scope = this.Scope,
                    Token = (string) h["access_token"],
                    Expire = Convert.ToInt32(h["expires_in"]),
                    UserId = Convert.ToInt32(h["user_id"])
                };
        }
    }
}
