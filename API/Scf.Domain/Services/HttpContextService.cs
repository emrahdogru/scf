using Scf.Utility.ExtensionMethods;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Domain.Services
{
    public class HttpContextService : IHttpContextService
    {
        readonly HttpContext? httpContext;

        public HttpContextService(IHttpContextAccessor accessor)
        {
            httpContext = accessor.HttpContext;
        }

        public string? Token => httpContext?.Request.GetToken();

        public ObjectId? TenantId => httpContext?.Request.GetTenantId();

        public IHeaderDictionary? Headers => httpContext?.Request.Headers;

        public string? GetEncodedPathAndQuery()
        {
            return httpContext?.Request.GetEncodedPathAndQuery();
        }

        public string? Method
        {
            get
            {
                return httpContext?.Request.Method;
            }
        }

        public string? RemoteIpAddress => httpContext?.Connection?.RemoteIpAddress?.ToString();
    }
}
