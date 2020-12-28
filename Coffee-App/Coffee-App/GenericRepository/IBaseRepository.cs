using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Coffee_App.GenericRepository
{
    public interface IBaseRepository<T> where T :class
    {
        IEnumerable<T> GetAll();
        T Get(object id);
        void Add(T t);
        void Delete(object key);
        void Update(T items);
        int SaveChanges();
        Task<int> SaveChangesAsync();

    }
}
