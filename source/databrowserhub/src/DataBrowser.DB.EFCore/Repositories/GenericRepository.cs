using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataBrowser.Domain.Interfaces.Repositories;
using DataBrowser.Domain.Specifications.Query;
using DataBrowser.Interfaces;
using DataBrowser.Interfaces.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataBrowser.Entities.SQLite
{
    public class GenericRepository<T> : IRepository<T> where T : class, IAggregateRoot
    {
        protected readonly DatabaseContext _dbContext;

        public GenericRepository(DatabaseContext dbContext, IRequestContext requestContext)
        {
            _dbContext = dbContext;
            _dbContext.requestContext = requestContext;
        }

        public IUnitOfWork UnitOfWork => _dbContext;

        public virtual async Task<T> GetByIdAsync(int id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        public async Task<IReadOnlyList<T>> ListAllAsync()
        {
            return await _dbContext.Set<T>().ToListAsync();
        }

        public async Task<IReadOnlyList<T>> FindAsync(IQuerySpecification<T> spec)
        {
            return await ApplySpecification(spec).ToListAsync();
        }

        public async Task<int> CountAsync(IQuerySpecification<T> spec)
        {
            return await ApplySpecification(spec).CountAsync();
        }

        //public async Task SaveChangeAsync()
        //{
        //    await _dbContext.SaveChangesAsync();
        //}

        public T Add(T entity)
        {
            _dbContext.Set<T>().Add(entity);

            return entity;
        }

        public void Update(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
        }

        public virtual void Delete(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
        }

        private IQueryable<T> ApplySpecification(IQuerySpecification<T> spec)
        {
            return SpecificationEvaluator<T>.GetQuery(_dbContext.Set<T>().AsQueryable(), spec);
        }
    }
}