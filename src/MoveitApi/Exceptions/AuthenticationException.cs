namespace MoveitApi.Exceptions
{
    public class AuthenticationException : MoveitException
    {
        public AuthenticationException() : base("Authentication Failed")
        {
        }
    }
}
