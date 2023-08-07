using ApiServices.DataDB.Repository;

namespace Services.DataDB.Repository
{
    public interface IStoreDbRepository
    {       
        //Обощённый метод редактирования одного экземпляра
        public void Update<TEntity>(TEntity editEntity) where TEntity : class;     
        //Добавляем Обьект
        public void Add<TSource>(TSource newEntity) where TSource : class;
        //Создать обьект асинхронно
        public Task AddAsync<TSource>(TSource newEntity) where TSource : class;
        //Удаление по ID
        public void Delete<TSource>(Guid id) where TSource : class;

        public void DeleteByEntity<TSource>(TSource entity) where TSource : class;

        //Metyod: Создаёт прямой запрос в базу данных
        public TEntity DirectRequestToDatabase<TEntity>(Func<AppDbContext, TEntity> Get);       
        public Task<TEntity> DirectRequestToDatabaseAsync<TEntity>(Func<AppDbContext, TEntity> Get);
    }
}
