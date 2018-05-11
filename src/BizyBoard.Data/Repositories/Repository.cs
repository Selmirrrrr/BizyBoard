namespace BizyBoard.Data.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Context;
    using Microsoft.EntityFrameworkCore;
    using Models.DbEntities;

    public class Repository<T> : IRepository<T> where T : class, IEntityBase, new()
    {
        private readonly AppDbContext _context;
        private readonly AppUser _user;

        public Repository(AppDbContext context, AppUser user)
        {
            _context = context;
            _user = user;
        }

        public virtual IEnumerable<T> GetAll() => _context.Set<T>().AsEnumerable();

        public virtual int Count() => _context.Set<T>().Count();

        public virtual IEnumerable<T> AllIncluding(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>();

            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

            return query.AsEnumerable();
        }

        public T GetSingle(int id) => _context.Set<T>().FirstOrDefault(x => x.Id == id);

        public T GetSingle(Expression<Func<T, bool>> predicate) => _context.Set<T>().FirstOrDefault(predicate);

        public T GetSingle(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>();

            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

            return query.Where(predicate).FirstOrDefault();
        }

        public virtual IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate) => _context.Set<T>().Where(predicate);

        public virtual void Add(T entity, AppUser user)
        {
            _context.Entry(entity);
            _context.Set<T>().Add(entity);
        }

        public virtual void Update(T entity, AppUser user)
        {
            var dbEntityEntry = _context.Entry(entity);
            dbEntityEntry.State = EntityState.Modified;
        }

        public virtual void Delete(T entity, AppUser user)
        {
            var dbEntityEntry = _context.Entry(entity);
            dbEntityEntry.State = EntityState.Deleted;
        }

        public virtual void DeleteWhere(Expression<Func<T, bool>> predicate, AppUser user)
        {
            IEnumerable<T> entities = _context.Set<T>().Where(predicate);

            foreach (var entity in entities) _context.Entry<T>(entity).State = EntityState.Deleted;
        }

        public virtual void Commit()
        {
            _context.SaveChanges();
        }
    }
}