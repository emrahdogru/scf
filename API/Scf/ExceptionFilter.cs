using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using Scf.Models.Results;

namespace Scf
{

    public class ExceptionFilter : IActionFilter, IOrderedFilter
    {
        public int Order => 0; // int.MaxValue - 10;

        public void OnActionExecuting(ActionExecutingContext context) { }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            var exception = context.Exception;

            if (exception is not null)
            {
                var exceptionResult = ExceptionResult.Create(exception);

                if (exceptionResult != null && exceptionResult.Any())
                {
                    context.Result = new ObjectResult(exceptionResult)
                    {
                        StatusCode = exceptionResult.Max(x => x.StatusCode),
                    };

                    context.ExceptionHandled = true;
                }
            }
        }
    }
}
