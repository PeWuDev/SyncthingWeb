using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Syncthing.Integration.Exceptions;

namespace SyncthingWeb.Exceptions
{
    public class GlobalExceptionFilter : IExceptionFilter, IDisposable
    {
        public void OnException(ExceptionContext context)
        {
            context.ExceptionHandled = true;
            var exc = context.Exception;
            if (exc is SyncthingConnectionException)
            {
                context.Result = new RedirectToActionResult("Syncthing", "Error", new { area ="" , message = exc.Message});
            }
            else
            {
                context.Result = new RedirectToActionResult("Index", "Error", new { area = "" });
            }

        }

        public void Dispose()
        {
        }
    }
}
