using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Net.Http.Headers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Scf.Domain
{

    [BsonIgnoreExtraElements]
    public class FLog
    {
        [BsonId(IdGenerator = typeof(MongoDB.Bson.Serialization.IdGenerators.ObjectIdGenerator))]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; private set; } = ObjectId.GenerateNewId();


        internal FLog(string description, BsonDocument? extra = null, LogLevel level = LogLevel.Trace, Exception? ex = null, ModelStateDictionary? ms = null, HttpContext? httpContext = null, ITenant? tenant = null, IUser? user = null, System.Reflection.MethodBase? method = null)
        {
            this.Description = description;

            if (ex != null)
                this.Exception = new ExceptionData(ex);

            if (ms != null)
                this.ModelState = new ModelStateDictionatyData(ms);

            this.Date = DateTime.UtcNow;
            this.Level = level;
            this.Extra = extra;

            this.TenantId = tenant?.Id;
            this.UserId = user?.Id;

            if (method != null)
            {
                this.ClassName = method.DeclaringType?.FullName;
                this.MethodName = method.Name;
            }

            if (httpContext != null)
            {
                var httpContextDoc = new BsonDocument
                {
                    ["RawUrl"] = httpContext.Request.GetEncodedPathAndQuery(),
                    ["HttpMethod"] = httpContext.Request.Method
                };

                try
                {
                    httpContextDoc["UrlReferrer"] = httpContext.Request.Headers[HeaderNames.Referer].ToString();
                }
                catch
                {
                    httpContextDoc["UrlReferrer"] = "";
                }

                httpContextDoc["UserAgent"] = httpContext.Request.Headers[HeaderNames.UserAgent].ToString(); ;
                httpContextDoc["RemoteIpAddress"] = string.Join(":", httpContext.Connection.RemoteIpAddress?.ToString(), httpContext.Connection.RemotePort.ToString());
                this.HttpContext = httpContextDoc;
            }
        }

        [BsonConstructor]
        protected FLog()
        { }

        [BsonElement]
        public string Signature
        {
            get
            {
                return Utility.Tools.Md5($"{this.ClassName}{this.MethodName}{this.Description}");
            }
        }

        [BsonElement]
        public LogLevel Level { get; protected set; }

        [BsonElement]
        public DateTime Date { get; protected set; }

        [BsonElement]
        public string Description { get; protected set; } = "";

        [BsonIgnoreIfNull]
        [BsonElement]
        public ExceptionData? Exception { get; protected set; }

        [BsonIgnoreIfNull]
        [BsonElement]
        public ModelStateDictionatyData? ModelState { get; protected set; }

        [BsonElement]
        public BsonDocument? Extra { get; protected set; }

        public BsonDocument? HttpContext { get; protected set; }

        [BsonElement]
        public string? ClassName { get; protected set; }

        [BsonElement]
        public string? MethodName { get; protected set; }

        public ObjectId? TenantId { get; protected set; }

        [BsonElement]
        public ObjectId? UserId { get; protected set; }
    }

    public class ModelStateDictionatyData
    {
        [BsonConstructor]
        private ModelStateDictionatyData()
        { }

        public ModelStateDictionatyData(ModelStateDictionary? modelStateDictionary)
        {
            if (modelStateDictionary != null)
            {
                this.IsValid = modelStateDictionary.IsValid;
                this.ModelState = modelStateDictionary.Select(x => new ModelStateData(x.Key, x.Value));
            }
        }

        [BsonElement]
        public bool IsValid { get; private set; }

        [BsonElement]
        public IEnumerable<ModelStateData>? ModelState { get; private set; }
        public class ModelStateData
        {
            public ModelStateData(string key, ModelStateEntry? modelState)
            {
                this.Key = key;

                if (modelState != null)
                {
                    this.ModelError = modelState.Errors.Select(e => new ModelErrorData(e));
                    this.Value = modelState?.AttemptedValue;
                }
            }

            [BsonElement]
            public string Key { get; private set; }

            [BsonElement]
            public string? Value { get; private set; }

            [BsonElement]
            public IEnumerable<ModelErrorData>? ModelError { get; private set; }
        }

        public class ModelErrorData
        {
            public ModelErrorData(ModelError modelError)
            {
                if (modelError == null)
                    return;

                this.ErrorMessage = modelError.ErrorMessage;

                if (modelError.Exception != null)
                    this.Exception = new ExceptionData(modelError.Exception);
            }

            [BsonElement]
            public string ErrorMessage { get; private set; } = "";

            [BsonElement]
            public ExceptionData? Exception { get; private set; }
        }
    }

    public class ExceptionData
    {
        [BsonConstructor]
        private ExceptionData()
        {

        }

        public ExceptionData(Exception ex)
        {
            if (ex == null)
                return;

            Message = ex.Message;
            StackTrace = ex.StackTrace;

            if (ex.InnerException != null)
            {
                InnerException = new ExceptionData(ex.InnerException);
            }
        }

        [BsonElement]
        public string Message { get; private set; } = "";


        [BsonElement]
        public string? StackTrace { get; private set; }

        [BsonIgnoreIfNull]
        [BsonElement]
        public ExceptionData? InnerException { get; private set; }
    }

    //
    // Summary:
    //     Defines logging severity levels.
    public enum LogLevel
    {
        //
        // Summary:
        //     Logs that contain the most detailed messages. These messages may contain sensitive
        //     application data. These messages are disabled by default and should never be
        //     enabled in a production environment.
        Trace = 0,
        //
        // Summary:
        //     Logs that are used for interactive investigation during development. These logs
        //     should primarily contain information useful for debugging and have no long-term
        //     value.
        Debug = 1,
        //
        // Summary:
        //     Logs that track the general flow of the application. These logs should have long-term
        //     value.
        Information = 2,
        //
        // Summary:
        //     Logs that highlight an abnormal or unexpected event in the application flow,
        //     but do not otherwise cause the application execution to stop.
        Warning = 3,
        //
        // Summary:
        //     Logs that highlight when the current flow of execution is stopped due to a failure.
        //     These should indicate a failure in the current activity, not an application-wide
        //     failure.
        Error = 4,
        //
        // Summary:
        //     Logs that describe an unrecoverable application or system crash, or a catastrophic
        //     failure that requires immediate attention.
        Critical = 5,
    }
}
