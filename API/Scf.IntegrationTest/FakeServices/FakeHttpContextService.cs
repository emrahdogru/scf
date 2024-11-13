using Scf.Domain.Services;
using Scf.IntegrationTest.Helpers;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.IntegrationTest.FakeServices
{
    public class FakeHttpContextService : IHttpContextService
    {
        public string? Token { get; set; }

        public ObjectId? TenantId { get; set; } = TenantHelper.AbsoluteTenantId;

        public IHeaderDictionary? Headers { get; set; }

        public string? RemoteIpAddress { get; set; } = "integrationtest";

        public string? GetEncodedPathAndQuery()
        {
            return "integrationtest";
        }
    }
}
