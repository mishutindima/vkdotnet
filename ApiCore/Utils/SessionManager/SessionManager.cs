using System;
using System.Collections.Generic;
using ApiCore.Utils.Authorization;
using ApiCore.Utils.Authorization.Exceptions;

namespace ApiCore.Utils.SessionManager
{
    public delegate void SessionManagerLogHandler(object sender, string msg);
    public class SessionManager
    {
        private List<SessionInfo> sessions;

        public int AppId;
        public int Permissions = 0;
        public string Scope = null;

        public SessionManager(int appId, int perms)
        {
            this.AppId = appId;
            this.Permissions = perms;
        }

        public SessionManager(int apiId, string scope)
        {
            this.AppId = apiId;
            this.Scope = scope;
        }

        public event SessionManagerLogHandler Log;
        public event EventHandler<OAuthEventArgs> OAuthRequired;
        /// <summary>
        /// Logging event function
        /// </summary>
        /// <param name="msg">Message to log</param>
        protected virtual void OnLog(string msg)
        {
            if (Log != null)
                Log(this, msg);
        }

        protected virtual void OnOAuthRequired(OAuthEventArgs eventArgs)
        {
            if (OAuthRequired != null)
                OAuthRequired(this, eventArgs);
        }

        public SessionInfo GetOAuthSession(bool relogin)
        {
            this.OnLog("Creating OAuth login wnd...");
            var auth = new OAuth(AppId, Scope);
            
            try
            {
                var eventArgs = new OAuthEventArgs(auth.GetAuthUries(relogin));
                OnOAuthRequired(eventArgs);
                return auth.Authorize(eventArgs.TokenResponse);
            }
            catch(AuthorizationFailedException e)
            {
                this.OnLog("Authorization failed: "+e.Message);
                return null;
            }
            catch(Exception e)
            {
                this.OnLog("Authorization failed by unknown reason!");
                return null;
            }
            return null;
        }
    }

    public class OAuthEventArgs : EventArgs
    {
        public OAuthEventArgs(OAuthUries uries)
        {
            Uries = uries;
        }

        public OAuthUries Uries { get; private set; }

        public string TokenResponse { get; set; }
    }
}
