using Scf.Domain;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Scf.Database
{
    public interface IBaseEntitySet
    {
        EntityContext Context { get; }

        string CollectionName { get; }
        /// <summary>
        /// EntityTable içindeki değişen kayıtları tespit eder ve toplu olarak veritabanına kaydeder.
        /// </summary>
        /// <param name="sessionHandle"></param>
        /// <returns></returns>
        Task SaveChangesAsync(IClientSessionHandle sessionHandle);

        /// <summary>
        /// Collection üzerindeki Indexleri vs. oluşturur
        /// </summary>
        /// <returns></returns>
        Task InitializeCollection();

        /// <summary>
        /// Collection içindeki tüm kayıtları yeniden kaydeder. Bu işlem modeldeki bazı değişikliklerin
        /// geçmiş kayıtlara yansıması için gerekli olabilir.
        /// </summary>
        Task ResaveAllAsync(Action<IEntity?, Exception> onError, bool ignoreVersionCheck = false);

        /// <summary>
        /// Güncel doküman versiyonu. Entity modelinde veri yapısını etkileyen değişikliklerde
        /// versiyon numarası artar. Geçmiş kayıtlarda eski versiyonda kalan kayıtları bu alan
        /// üzerinden yakalayarak, yeniden kaydederek güncelliyoruz.
        /// </summary>
        int CurrentVersion { get; }
    }

    public interface IBaseEntitySet<T>: IBaseEntitySet
    {
        IQueryable<T> AsQueryable();

        IMongoCollection<T> GetMongoCollection();
        bool IsEntityChanged(T entity);

        /// <summary>
        /// Entity yeni mi? Veritabanında bu Id ile oluşturulmuş bir kayıt yok ise yenidir.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        bool IsNew(T entity);

        /// <summary>
        /// Sadece verilen Entity'i veritabanına kaydeder. 
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="sessionHandle">Transaction var ise geçilmelidir.</param>
        /// <returns></returns>
        Task SaveAsync(T entity, IClientSessionHandle? sessionHandle = null);

        /// <summary>
        /// Entity'i entityTable'a ekler. Eklerken hash değerini hesaplar
        /// </summary>
        /// <param name="entity">Entity</param>
        void SetToEntityTable(T entity);

        /// <summary>
        /// Yeni entity'i entityTable'a ekler. Hash değeri null olarak eklenir.
        /// </summary>
        /// <param name="entity">Entity</param>
        void AddToEntityTable(T entity);

        /// <summary>
        /// Entity'nin entityTable'da olup olmadığını kontrol eder.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>Entity</returns>
        bool IsEntityTableContains(T entity);

        /// <summary>
        /// EntityTable içinde verilen kuşullara göre arama yapar
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IEnumerable<T?> FindInEntityTable(Func<T?, bool> predicate);

        /// <summary>
        /// Veritabanı indexleri
        /// </summary>
        EntitySetIndex<T>[] Indexes { get; set; }
    }

    public interface IEntitySet<T> : IBaseEntitySet<T>
    {
        /// <summary>
        /// Yeni bir entity oluşturur ve context'e ekler
        /// </summary>
        /// <returns></returns>
        T Create();

        /// <summary>
        /// Oluşturulmuş bir entity'i Context'e ekler. Veritabanında bir işlem yapmaz.
        /// </summary>
        /// <param name="entity"></param>
        /// <exception cref="InvalidOperationException">Eğer entity zaten context içinde ise bu hatayı fırlatır</exception>
        void Attach(T entity);

        /// <summary>
        /// Id numarasından veritabanındaki Entity'i bulur ve döner
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<T?> FindAsync(ObjectId id);

        /// <summary>
        /// Kaydı bulamazsa hata fırlatır
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tenant"></param>
        /// <exception cref="EntityNotFountException"></exception>
        /// <returns></returns>
        Task<T> FindOrThrowAsync(ObjectId id);

        /// <summary>
        /// Id numarasından veritabanındaki Entity'leri bulur ve döner
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<T[]> FindManyAsync(params ObjectId[] ids);

        /// <summary>
        /// En az bir kaydı bulamazsa hata fırlatır
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tenant">Entity'nin bağlı olduğu tenant</param>
        /// <exception cref="EntityNotFountException"></exception>
        /// <returns></returns>
        Task<T[]> FindManyOrThrowAsync(params ObjectId[] ids);

        /// <summary>
        /// Tüm kayıtları döner
        /// </summary>
        /// <param name="includeDeleted">Silinen kayıtlar da dahil mi?</param>
        /// <returns></returns>
        IQueryable<T> GetAll(bool includeDeleted = false);
    }

    public interface IEntitySetWithTenant<T> : IBaseEntitySet<T>
    {
        T Create(ITenant tenant);
        Task<T?> FindAsync(ObjectId id, ITenant tenant);

        /// <summary>
        /// Id numarasından veritabanındaki Entity'i bulur ve döner. Kaydı bulamazsa hata fırlatır
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tenant">Entity'nin bağlı olduğu tenant</param>
        /// <exception cref="EntityNotFountException"></exception>
        /// <returns></returns>
        Task<T> FindOrThrowAsync(ObjectId id, ITenant tenant);
        Task<T[]> FindManyAsync(ITenant tenant, params ObjectId[] ids);

        /// <summary>
        /// En az bir kaydı bulamazsa hata fırlatır
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tenant">Entity'nin bağlı olduğu tenant</param>
        /// <exception cref="EntityNotFountException"></exception>
        /// <returns></returns>
        Task<T[]> FindManyOrThrowAsync(ITenant tenant, params ObjectId[] ids);
        IQueryable<T> GetAll(ITenant tenant, bool includeDeleted = false);
        IQueryable<T> GetAllWitoutContext(ITenant tenant, bool includeDeleted = false);
    }
}