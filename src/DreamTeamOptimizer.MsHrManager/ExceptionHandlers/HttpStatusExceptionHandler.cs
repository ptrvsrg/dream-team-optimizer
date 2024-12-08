using System;
using System.Net;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using DreamTeamOptimizer.Core.Exceptions;
using DreamTeamOptimizer.Core.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace DreamTeamOptimizer.MsHrManager.ExceptionHandlers;

public class HttpStatusExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext context,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not HttpStatusException httpStatusException)
        {
            return false;
        }

        context.Response.ContentType = MediaTypeNames.Application.Json;
        context.Response.StatusCode = (int)httpStatusException.StatusCode;

        var response = new Error(context.Response.StatusCode, httpStatusException.Message, DateTime.Now,
            context.Request.Path);

        await context.Response.WriteAsJsonAsync(response, cancellationToken);

        return true;
    }
}