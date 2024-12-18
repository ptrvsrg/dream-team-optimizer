using System.Net;

namespace DreamTeamOptimizer.Core.Exceptions;

public class HttpStatusException : Exception
{
    public HttpStatusCode StatusCode { get; }

    public HttpStatusException() : this(HttpStatusCode.InternalServerError, "")
    {
    }

    public HttpStatusException(string message) : this(HttpStatusCode.InternalServerError, message)
    {
    }

    public HttpStatusException(HttpStatusCode statusCode) : this(statusCode, "")
    {
    }

    public HttpStatusException(HttpStatusCode statusCode, string message) : base(message)
    {
        StatusCode = statusCode;
    }
}