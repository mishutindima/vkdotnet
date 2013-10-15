using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiCore.Authorization
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
}
