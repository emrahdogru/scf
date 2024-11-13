using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Scf.Domain.Services
{
    public interface ILoggerService
    {
        Task<FLog> Critical(string description, object? extra = null, ModelStateDictionary? ms = null, Exception? ex = null);
        Task<FLog> Debug(string description, object? extra = null, ModelStateDictionary? ms = null);
        Task<FLog> Error(string description, object? extra = null, ModelStateDictionary? ms = null, Exception? ex = null);
        Task<FLog> Information(string description, object? extra = null, ModelStateDictionary? ms = null);
        Task<FLog> Trace(string description, object? extra = null, ModelStateDictionary? ms = null);
        Task<FLog> Warning(string description, object? extra = null, ModelStateDictionary? ms = null, Exception? ex = null);
        IQueryable<FLog> GetAll();
    }
}