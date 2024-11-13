namespace Scf.Domain
{
    internal static class Checkers
    {
        /// <summary>
        /// Her iki entity de aynı tenant'a ait olduğunu doğrular.
        /// </summary>
        /// <param name="entity1">Birinci entity</param>
        /// <param name="entity2">İkinci entity</param>
        /// <exception cref="TenantMismatchException"></exception>
        public static void CheckTenant(ITenantEntity? entity1, ITenantEntity? entity2)
        {
            if (entity1 != null && entity2 != null && entity1.TenantId != entity2.TenantId)
                throw new TenantMismatchException($"[{entity1.GetType().Name}:{entity1.Id}] ve [{entity2.GetType().Name}:{entity2.Id}] aynı hesaba ait değil.");
        }

        /// <summary>
        /// Entity'nin belirtilen tenant'a ait olduğunu doğrular
        /// </summary>
        /// <param name="tenant">Tenant</param>
        /// <param name="entity">Entity</param>
        /// <exception cref="TenantMismatchException"></exception>
        public static void CheckTenant(Tenant? tenant, ITenantEntity? entity)
        {
            if (tenant != null && entity != null && tenant.Id != entity.TenantId)
                throw new TenantMismatchException($"[{entity.GetType().Name}:{entity.Id}], {tenant.Id} aynı hesabına ait değil.");
        }

        /// <summary>
        /// Kullanıcının bu tenant'ta yetkili olduğunu doğrular
        /// </summary>
        /// <param name="tenant">Tenant</param>
        /// <param name="user">Kullanıcı</param>
        /// <exception cref="TenantMismatchException"></exception>
        public static void CheckTenant(ITenant? tenant, User? user)
        {
            if (tenant != null && user != null && !user.TenantIds.Contains(tenant.Id))
                throw new TenantMismatchException($"{user.Id} kullanıcısı, {tenant.Id} hesabında yetkili değil.");
        }
    }
}
