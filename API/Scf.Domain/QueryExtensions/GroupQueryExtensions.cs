using Scf.Domain.TenantModels;
using MongoDB.Bson;

namespace Scf.Domain
{

    public static class GroupQueryExtensions
    {
        /// <summary>
        /// Grupları yöneticilerine göre filtreler
        /// </summary>
        /// <param name="managerIds">Yönetici çalışan kimlik numaraları</param>
        /// <returns></returns>
        public static IQueryable<Group> FilterByManagers(this IQueryable<Group> query, params ObjectId[] managerIds)
        {
#pragma warning disable CS8629 // Nullable value type may be null.
            return query.Where(x => managerIds.Contains(x.ManagerId.Value));
#pragma warning restore CS8629 // Nullable value type may be null.
        }


        public static IQueryable<Group> FilterByParents(this IQueryable<Group> query, params ObjectId[] parentIds)
        {
#pragma warning disable CS8629 // Nullable value type may be null.
            return query.Where(x => parentIds.Contains(x.ParentId.Value));
#pragma warning restore CS8629 // Nullable value type may be null.
        }
    }
}
