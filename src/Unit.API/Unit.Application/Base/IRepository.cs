using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Unit.Application.Base
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> AsQueryable();
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null);
        Task<T?> GetByIdAsync(int id);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
