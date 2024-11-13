using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Diagnostics;

namespace Scf.Domain.Services
{
    public class LoggerService : ILoggerService
    {
        readonly ISessionService sessionService;
        readonly IHttpContextAccessor HttpContextAccessor;
        readonly IMongoDbService mongoDbService;

        public LoggerService(ISessionService sessionService, IHttpContextAccessor httpContextAccessor, IMongoDbService mongoDbService)
        {
            this.sessionService = sessionService;
            this.HttpContextAccessor = httpContextAccessor;
            this.mongoDbService = mongoDbService;
        }

        private static BsonDocument ObjectToBsonDocument(object? extra)
        {
            BsonDocument? bsonDoc = null;
            if (extra != null)
                bsonDoc = extra.ToBsonDocument();

            if (bsonDoc == null)
                bsonDoc = new BsonDocument();

            return bsonDoc;
        }

        protected IMongoCollection<FLog> GetCollection()
        {
            return mongoDbService.Database.GetCollection<FLog>("Log");
        }

        protected async Task<FLog> Add(string description, object? extra = null, LogLevel level = LogLevel.Trace, System.Reflection.MethodBase? method = null, Exception? ex = null, ModelStateDictionary? ms = null)
        {
            var bsonDoc = ObjectToBsonDocument(extra);

            var log = new FLog(description, bsonDoc, level, ex, ms, HttpContextAccessor.HttpContext, sessionService.Tenant, sessionService.User, method);
            await GetCollection().InsertOneAsync(log);
            return log;
        }

        public async Task<FLog> Trace(string description, object? extra = null, ModelStateDictionary? ms = null)
        {
            StackTrace stackTrace = new();
            StackFrame frame = stackTrace.GetFrames()[1];
            var method = frame.GetMethod();

            return await Add(description, extra, LogLevel.Trace, method, ms: ms);
        }

        public async Task<FLog> Debug(string description, object? extra = null, ModelStateDictionary? ms = null)
        {
            StackTrace stackTrace = new();
            StackFrame frame = stackTrace.GetFrames()[1];
            var method = frame.GetMethod();

            return await Add(description, extra, LogLevel.Debug, method, ms: ms);
        }

        public async Task<FLog> Information(string description, object? extra = null, ModelStateDictionary? ms = null)
        {
            StackTrace stackTrace = new();
            StackFrame frame = stackTrace.GetFrames()[1];
            var method = frame.GetMethod();

            return await Add(description, extra, LogLevel.Information, method, ms: ms);
        }

        public async Task<FLog> Warning(string description, object? extra = null, ModelStateDictionary? ms = null, Exception? ex = null)
        {
            StackTrace stackTrace = new();
            StackFrame frame = stackTrace.GetFrames()[1];
            var method = frame.GetMethod();

            return await Add(description, extra, LogLevel.Warning, method, ex, ms);
        }

        public async Task<FLog> Error(string description, object? extra = null, ModelStateDictionary? ms = null, Exception? ex = null)
        {
            StackTrace stackTrace = new();
            StackFrame frame = stackTrace.GetFrames()[1];
            var method = frame.GetMethod();

            return await Add(description, extra, LogLevel.Error, method, ex, ms);
        }

        public async Task<FLog> Critical(string description, object? extra = null, ModelStateDictionary? ms = null, Exception? ex = null)
        {
            StackTrace stackTrace = new();
            StackFrame frame = stackTrace.GetFrames()[1];
            var method = frame.GetMethod();

            return await Add(description, extra, LogLevel.Critical, method, ex, ms);
        }

        public IQueryable<FLog> GetAll()
        {
            return GetCollection().AsQueryable();
        }
    }
}
