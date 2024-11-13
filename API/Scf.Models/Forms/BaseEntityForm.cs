using Scf.Domain;
using FluentValidation;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Models.Forms
{
    public abstract class BaseEntityForm<TEntity> : IEntityForm<TEntity> where TEntity : class, IEntity
    {
        public BaseEntityForm() { }

        public BaseEntityForm(TEntity entity)
        {
            this.Id = entity.Id;
        }

        public ObjectId Id { get; set; } = ObjectId.Empty;

        public virtual Task Bind(DomainContext context, User user, TEntity entity)
        {
            if (entity.Deleted?.IsDeleted == true)
                throw new UserException(((DomainContext)entity.Context).LanguageService.Get(x => x.YouCannotModifyADeletedRecord));

            entity.Id = Id;
            return Task.CompletedTask;
        }

        Task IEntityForm<TEntity>.Bind(IEntityContext context, IUser user, TEntity entity)
        {
            return Bind((DomainContext)context, (User)user, entity);
        }
    }

    public abstract class BaseTenantEntityForm<TEntity> : ITenantEntityForm<TEntity> where TEntity : class, ITenantEntity
    {
        public BaseTenantEntityForm() { }

        public BaseTenantEntityForm(TEntity entity)
        {
            this.Id = entity?.Id ?? ObjectId.Empty;
        }

        public ObjectId Id { get; set; } = ObjectId.Empty;

        public virtual Task Bind(DomainContext context, User user, TEntity entity)
        {
            if (entity.Deleted?.IsDeleted == true)
                throw new UserException(((DomainContext)entity.Context).LanguageService.Get(x => x.YouCannotModifyADeletedRecord));

            entity.Id = Id;
            return Task.CompletedTask;
        }

        Task ITenantEntityForm<TEntity>.Bind(IEntityContext context, IUser user, TEntity entity)
        {
            return Bind((DomainContext)context, (User)user, entity);
        }
    }
}
