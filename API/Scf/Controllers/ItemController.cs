//using Scf.Domain;
//using Scf.Domain.Services;
//using Scf.Domain.TenantModels;
//using Scf.Models.Filters;
//using Scf.Models.Forms;
//using Scf.Models.Summaries;

//namespace Scf.Controllers
//{
//    public class ItemController : EntityWithTenantControllerBase<ItemService, Item, ItemForm, ItemFilter, ItemSummary>
//    {
//        protected ItemController(DomainContext domainContext, ISessionService sessionService, ItemService entityService)
//            : base(domainContext, sessionService, entityService)
//        {

//        }

//        protected override ItemForm CreateForm(Item entity)
//        {
//            return entity.Type switch
//            {
//                nameof(ItemAsStock) => new ItemAsStockForm((ItemAsStock)entity),
//                nameof(ItemAsService) => new ItemAsServiceForm((ItemAsService)entity),
//                _ => throw new NotImplementedException($"`{entity.Type}` item tipi için form tanımlanmadı.")
//            };
//        }

//        protected override ItemSummary CreateSummary(Item entity)
//        {
//            return new ItemSummary(entity);
//        }
//    }
//}
