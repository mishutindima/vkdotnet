using System;

namespace ApiCore.Authorization
{
    public class AuthorizationFailedException: Exception
    {
        public AuthorizationFailedException(string message): base(message)
        {
        }
    }
}
