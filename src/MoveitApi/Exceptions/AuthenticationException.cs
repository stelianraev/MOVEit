using Movit.API.Exceptions;

namespace MoveitApi.Exceptions
{
    public class AuthenticationException : MoveitException
    {
        public AuthenticationException() : base("Authentication Failed")
        {
        }
    }
}
