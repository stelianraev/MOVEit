namespace Movit.API.Exceptions
{
    public class MoveitException : Exception
    {
        public MoveitException()
        {
        }

        public MoveitException(string message)
            : base(message)
        {
        }

        public MoveitException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
