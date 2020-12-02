using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Coffee_App.GenericRepository
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        public readonly DbContext _dbContext;
        public readonly DbSet<T> _dbSet;

        public BaseRepository(DbContext dbContext) // 
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<T>();
        }


        public void Add(T t)
        {
            _dbSet.Add(t);
        }

        public void Delete(object key)
        {
            T existing = _dbSet.Find(key);
            if (existing != null)
            {
                _dbSet.Remove(existing);
            }
        }

        public T Get(object id)
        {
            return _dbSet.Find(id);
        }


        public IEnumerable<T> GetAll()
        {
            return _dbSet.AsEnumerable();
        }

        public int SaveChanges()
        {
            return _dbContext.SaveChanges();
        }

        public Task<int> SaveChangesAsync()
        {
            return _dbContext.SaveChangesAsync();
        }

        public void Update(object key, T items)
        {
            // _dbSet.Update(items);
            T exist = _dbSet.Find(key);
            if (exist != null)
            {
                _dbContext.Entry(exist).CurrentValues.SetValues(items);
            }
        }
    }
}
