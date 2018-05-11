namespace BizyBoard.Data.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Models.DbEntities;

    public interface IRepository<T> where T : class, IEntityBase, new()
    {
        IEnumerable<T> AllIncluding(params Expression<Func<T, object>>[] includeProperties);
        IEnumerable<T> GetAll();
        int Count();
        T GetSingle(int id);
        T GetSingle(Expression<Func<T, bool>> predicate);
        T GetSingle(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);
        IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate);
        void Add(T entity, AppUser user);
        void Update(T entity, AppUser user);
        void Delete(T entity, AppUser user);
        void DeleteWhere(Expression<Func<T, bool>> predicate, AppUser user);
        void Commit();
    }
}