using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chats.Domain.Exceptions;
using Chats.Domain.SeedWork;
using Chats.Domain.Specification;
using Chats.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Chats.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T>
        where T : class, IAggregateRoot
    {
        private readonly ChatsContext _context;

        public GenericRepository(ChatsContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IUnitOfWork UnitOfWork => _context;

        public T Create(T entity)
            => _context.Set<T>().Add(entity).Entity;

        public async Task<T> GetAsync(string id) 
            => await _context.Set<T>().FindAsync(id);

        public async Task<(ICollection<T>, int)> GetAsync(ISpecification<T> specification)
        {
            var result = await _context.Set<T>()
                .AsNoTracking()
                .ApplyCriteria(specification)
                .ApplyOrderBy(specification)
                .ApplyPagination(specification)
                .ApplyIncludes(specification)
                .ToListAsync();

            int total = result.Count;
            if (specification.IsPaginationEnabled)
            {
                total = await _context.Set<T>()
                    .AsNoTracking()
                    .ApplyCriteria(specification)
                    .ApplyOrderBy(specification)
                    .ApplyIncludes(specification)
                    .CountAsync();
            }

            return (result, total);
        }

        public async Task DeleteAsync(string id)
        {
            var entity = await _context.Set<T>().FindAsync(id)
                       ?? throw new ChatDomainException($"Could not delete the {nameof(T)}", new[] { $"{nameof(T)} not found" });

            _context.Set<T>().Remove(entity);
        }

        public T Update(T entity)
            => _context.Set<T>().Update(entity).Entity;
    }
}
