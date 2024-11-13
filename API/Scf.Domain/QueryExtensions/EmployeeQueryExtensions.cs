using Scf.Domain.TenantModels;
using MongoDB.Bson;

namespace Scf.Domain
{
    public static class EmployeeQueryExtensions
    {
        /// <summary>
        /// Çalışanları departmanlara göre filtreler
        /// </summary>
        /// <param name="groupIds">Departman kimlik numaraları</param>
        /// <returns></returns>
        public static IQueryable<Employee> FilterByGroups(this IQueryable<Employee> employees, params ObjectId[] groupIds)
        {
            return employees.Where(x => x.GroupIds.Any(ed => groupIds.Contains(ed)));
        }

        public static IQueryable<Employee> FilterByEmployeeManagers(this IQueryable<Employee> employees, params ObjectId[] managerIds)
        {
#pragma warning disable CS8629 // Nullable value type may be null.
            return employees.Where(x => managerIds.Contains(x.ManagerId.Value));
#pragma warning restore CS8629 // Nullable value type may be null.
        }

        public static IQueryable<Employee> FilterByPositions(this IQueryable<Employee> employees, params ObjectId[] positionIds)
        {
#pragma warning disable CS8629 // Nullable value type may be null.
            return employees.Where(x => positionIds.Contains(x.PositionId.Value));
#pragma warning restore CS8629 // Nullable value type may be null.
        }

        public static IQueryable<Employee> FilterByTitles(this IQueryable<Employee> employees, params ObjectId[] titleIds)
        {
#pragma warning disable CS8629 // Nullable value type may be null.
            return employees.Where(x => titleIds.Contains(x.TitleId.Value));
#pragma warning restore CS8629 // Nullable value type may be null.
        }
    }
}
