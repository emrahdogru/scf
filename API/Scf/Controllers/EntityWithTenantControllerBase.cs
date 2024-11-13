using Scf.Domain;
using Scf.Domain.Services;
using Scf.Models;
using Scf.Models.Forms;
using Scf.Models.Results;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MongoDB.Bson;
using System.Text;
using Newtonsoft.Json;

namespace Scf.Controllers
{

    [ApiController]
    public abstract class EntityWithTenantControllerBase<TService, TEntity, TForm, TFilter, TSummary> : ControllerBase
        where TService : class, ITenantEntityService<TEntity>
        where TEntity : class, ITenantEntity
        where TForm : class, ITenantEntityForm<TEntity>, new()
        where TFilter : class, IFilter<TEntity>, new()
        where TSummary: class
    {
        protected readonly DomainContext domainContext;
        protected readonly ISessionService sessionService;
        protected readonly TService entityService;

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        private static JsonSerializerSettings jsonSerializerSettings = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

        private static void InitializeJsonSerializerSettings()
        {
            if (jsonSerializerSettings == null)
            {
                jsonSerializerSettings = new JsonSerializerSettings();
                jsonSerializerSettings.Converters.Add(new Scf.Utility.JsonConverters.ObjectIdJsonConverter());
                jsonSerializerSettings.Converters.Add(new Scf.Utility.JsonConverters.ObjectIdListJsonConverter());
                jsonSerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            }
        }

        protected EntityWithTenantControllerBase(DomainContext domainContext, ISessionService sessionService, TService entityService)
        {
            if (sessionService.Tenant == null)
                throw new TenantMismatchException("Tenant belirtilmedi.");

            InitializeJsonSerializerSettings();

            domainContext.IsAutoTrackChangesEnabled = false;

            this.Tenant = sessionService.Tenant;

            this.domainContext = domainContext;
            this.sessionService = sessionService;
            this.entityService = entityService;
        }

        private async Task<string> GetRequestBodyAsString()
        {
            string? json;

            using (var ms = new MemoryStream())
            {
                await Request.BodyReader.CopyToAsync(ms);
                json = Encoding.UTF8.GetString(ms.ToArray());
            }

            return json ?? string.Empty;
        }

        protected async Task<TEntity> GetEntity(ObjectId id)
        {
            TEntity? entity;

            if (id == ObjectId.Empty)
                entity = entityService.Create(Tenant);
            else
                entity = await entityService.Find(id, Tenant);

            if (entity == null)
                throw new EntityNotFountException(typeof(TEntity).Name, id);

            return entity;
        }

        protected async Task<TEntity> GetOrCreateEntity(ObjectId id)
        {
            TEntity? entity = await entityService.Find(id, Tenant);

            if (entity == null)
            {
                if (id != ObjectId.Empty && !entityService.IsFreeObjectId(id))
                    throw new TenantMismatchException("Bu kayıt üzerinde işlem yetkiniz yok.");

                entity = entityService.Create(Tenant);
                entity.Id = id;
            }

            return entity;
        }

        /// <summary>
        /// İşlem yapılmakta olan hesap.
        /// </summary>
        protected virtual ITenant Tenant { get; private set; }

        /// <summary>
        ///  Mevcut bir kaydı düzenlemek için form oluşturur
        /// </summary>
        /// <param name="entity">Düzenlenecek entity</param>
        /// <returns></returns>
        protected abstract TForm CreateForm(TEntity entity);

        /// <summary>
        /// Boş form oluşturur
        /// </summary>
        /// <returns></returns>
        protected abstract TForm CreateForm();
        protected abstract TSummary CreateSummary(TEntity entity);
        

        /// <summary>
        /// Form için Validator nesnesini oluşturur ve döner
        /// </summary>
        /// <returns></returns>
        protected AbstractValidator<TForm>? CreateFormValidator(TForm form)
        {
            var validatorType = form.GetType().Assembly.GetType(typeof(TForm).FullName + "+Validator");

            if (validatorType == null)
                return null;
            else
                return Activator.CreateInstance(validatorType, domainContext) as AbstractValidator<TForm>;
        }

        [HttpGet("{id}")]
        public async Task<TForm> GetAsync(ObjectId id)
        {
            TForm form;

            if(id != ObjectId.Empty)
            {
                var entity = await GetEntity(id);
                form = CreateForm(entity);
            }
            else
            {
                form = new TForm();


                await TryUpdateModelAsync<TForm>(form, "", new QueryStringValueProvider(BindingSource.Query, Request.Query, null));
            }

            return form;
        }

        [HttpPut]
        public async Task<TForm> PutAsync()
        {
            var form = CreateForm();

            string json = await GetRequestBodyAsString();

            JsonConvert.PopulateObject(json, form, jsonSerializerSettings);


            var validator = this.CreateFormValidator(form);
            
            if(validator != null)
                await validator.ValidateAndThrowAsync(form);

            var entity = await GetOrCreateEntity(form.Id);
            await form.Bind(domainContext, sessionService.User, entity);

            await entityService.Save(entity);

            return CreateForm(entity);
        }

        [HttpPost("validate")]
        public async Task ValidateAsync([FromBody]TForm form)
        {
            var validator = this.CreateFormValidator(form);

            if(validator != null)
                await validator.ValidateAndThrowAsync(form);
        }

        [HttpDelete("{id}")]
        public async Task DeleteAsync(ObjectId id)
        {
            User user = sessionService.User;
            var entity = await GetEntity(id);

            entity.Remove(user);
            await entityService.Save(entity);
        }

        [HttpPost("list")]
        public FilterResult<TSummary> List([FromBody]TFilter filter)
        {
            var query = entityService.GetAll(Tenant);
            query = filter.Apply(query, out int totalRecordCount);

            var data = query.ToArray().Select(x => CreateSummary(x)).ToArray();

            var result = new FilterResult<TSummary>(filter)
            {
                Result = data,
                TotalRecordCount = totalRecordCount
            };

            return result;
        }
    }
}
