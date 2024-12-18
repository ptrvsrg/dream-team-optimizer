using System.Net;
using System.Net.Mime;
using DreamTeamOptimizer.Core.Models;
using Microsoft.AspNetCore.Diagnostics;

namespace DreamTeamOptimizer.MsHrDirector.ExceptionHandlers;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext context,
        Exception exception,
        CancellationToken cancellationToken)
    {
        context.Response.ContentType = MediaTypeNames.Application.Json;
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var response = new Error(context.Response.StatusCode, exception.Message, DateTime.Now, context.Request.Path);

        await context.Response.WriteAsJsonAsync(response, cancellationToken);

        return true;
    }
}