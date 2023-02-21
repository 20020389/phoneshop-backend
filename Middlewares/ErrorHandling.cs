using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using PhoneShop.Lib;

namespace PhoneShop.Middleware;

public class HttpExceptionAttribute : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        if (context.Exception is HttpException)
        {
            var exception = (HttpException)context.Exception;
            context.Result = new ObjectResult(new { message = exception.message })
            {
                StatusCode = ((int)exception.status)
            };
        }
    }
}
