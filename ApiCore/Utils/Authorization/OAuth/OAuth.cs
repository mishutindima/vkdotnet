using System;
using System.Collections.Generic;
using System.Text;
using ApiCore.Utils.Authorization.Exceptions;

namespace ApiCore.Utils.Authorization
{
    public class OAuth: IOAuthProvider
    {
        public SessionInfo Authorize(int appId, string scope, string display, bool relogin)
        {
            OAuthWnd wnd = new OAuthWnd(appId, scope, display, relogin);
            wnd.ShowDialog();
            if (wnd.Authenticated)
                return wnd.SessionData;
            else
                throw new AuthorizationFailedException("Authorization failed!");
        }
    }
}
