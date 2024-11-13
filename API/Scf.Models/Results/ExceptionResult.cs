using FluentValidation;
using FluentValidation.Results;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Models.Results
{
    public class ExceptionResult
    {
        protected ExceptionResult(ValidationException exception) {
            this.Message = exception.Message;
            this.Type = exception.GetType().Name;

            this.Errors = exception.Errors.Select(x => new Error(x)).ToArray();
        }

        protected ExceptionResult(UserException exception)
        {
            this.Message = exception.Message;
            this.Type = exception.GetType().Name;
        }

        protected ExceptionResult(UserAuthorizationException exception)
        {
            this.Message = exception.Message;
            this.Type = exception.GetType().Name;
        }

        protected ExceptionResult(TenantMismatchException exception)
        {
            this.Message = exception.Message;
            this.Type = exception.GetType().Name;
        }

        protected ExceptionResult(InvalidTokenException exception)
        {
            this.Message = exception.Message;
            this.Type = exception.GetType().Name;
        }

        protected ExceptionResult(CannotDeleteException exception)
        {
            this.Message = exception.Message;
            this.Type = exception.GetType().Name;
        }

        protected ExceptionResult(EntityNotFountException exception)
        {
            this.Message = exception.Message;
            this.Type = exception.GetType().Name;
            this.StatusCode = 404;
        }

        protected ExceptionResult(Exception exception)
        {
            this.Message = "Internal Server Error";
            this.Type = "Exception";
            this.StatusCode = 500;
        }

        [JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public int StatusCode { get; } = 400;

        public string Message { get; }
        public string Type { get; }

        public IEnumerable<Error> Errors { get; } = Array.Empty<Error>();

        public class Error
        {
            public Error(ValidationFailure error)
            {
                this.Code = error.ErrorCode;
                this.Message = error.ErrorMessage;
                this.PropertyName = error.PropertyName;
                this.Severity = error.Severity;
            }

            public string Code { get; }
            public string Message { get; }
            public string PropertyName { get;}
            public Severity Severity { get; }
        }

        protected static IEnumerable<ExceptionResult> CreateAggregate(AggregateException ex)
        {
            foreach (var e in ex.InnerExceptions)
            {
                if (e != null)
                {
                    var results = Create(e);
                    if (results != null)
                        foreach (var r in results)
                            yield return r;
                }
            }
        }

        public static IEnumerable<ExceptionResult>? Create(Exception ex)
        {

            return ex.GetType().Name switch
            {
                nameof(ValidationException) => new ExceptionResult[] { new ExceptionResult((ValidationException)ex) },
                nameof(EntityNotFountException) => new ExceptionResult[] { new ExceptionResult((EntityNotFountException)ex) },
                nameof(CannotDeleteException) => new ExceptionResult[] { new ExceptionResult((CannotDeleteException)ex) },
                nameof(InvalidTokenException) => new ExceptionResult[] { new ExceptionResult((InvalidTokenException)ex) },
                nameof(TenantMismatchException) => new ExceptionResult[] { new ExceptionResult((TenantMismatchException)ex) },
                nameof(UserAuthorizationException) => new ExceptionResult[] { new ExceptionResult((UserAuthorizationException)ex) },
                nameof(UserException) => new ExceptionResult[] { new ExceptionResult((UserException)ex) },
                nameof(AggregateException) => CreateAggregate((AggregateException)ex).ToArray(),
                _ => null
                //_ => new ExceptionResult[] { new ExceptionResult((Exception)ex) }
            };
        }
    }
}
