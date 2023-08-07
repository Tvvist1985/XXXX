using ApiServices.DataDB.Repository;

namespace Services.DataDB.Repository
{
    public class SQLStoreDbRepository : IStoreDbRepository
    {
        private readonly AppDbContext db;

        public SQLStoreDbRepository(AppDbContext db) { this.db = db; }
              
        //Обощённый метод редактирования одного экземпляра
        public void Update<TEntity>(TEntity editEntity) where TEntity : class
        {
            db.Update(editEntity);
            db.SaveChanges();
        }
               
        //Добавляем Обьект
        public void Add<TSource>(TSource newEntity) where TSource : class
        {
            db.Add(newEntity);
            db.SaveChanges();
        }
        //Добавляет асинхронно
        public async Task AddAsync<TSource>(TSource newEntity) where TSource : class
        {
            await db.AddAsync(newEntity);
            db.SaveChangesAsync();
        }

        //Удаление по ID
        public async void Delete<TSource>(Guid id) where TSource : class
        {
            var pageToDelete = db.Find<TSource>(id);

            if (pageToDelete != null)
            {
                db.Remove(pageToDelete);
                db.SaveChanges();
            }

        }
        //Удаление по обьекту
        public void DeleteByEntity<TSource>(TSource entity) where TSource : class
        {
            if (entity != null)
            {
                db.Remove(entity);
                db.SaveChanges();
            }
        }

        //Method: Для создания любого запроса в базу данных
        public TEntity DirectRequestToDatabase<TEntity>(Func<AppDbContext, TEntity> Get) => Get(db);
       
        //Method: Для создания любого запроса в базу данных
        public async Task<TEntity> DirectRequestToDatabaseAsync<TEntity>(Func<AppDbContext, TEntity> Get) => await Task.Run(() => Get(db));
    }
}
