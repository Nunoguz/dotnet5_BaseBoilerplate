using Nunoguz_Boilerplate.Domain.Repositories;
using Nunoguz_Boilerplate.Shared;
using Nunoguz_Boilerplate.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Nunoguz_Boilerplate.Persistence.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        // Should implement -UnitOfWork- to work
        protected readonly DatabaseContext _context;
        protected readonly ILogger<dynamic> _logger;

        public GenericRepository(DatabaseContext context, ILogger<dynamic> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task AddAsync(T entity)
        {
            if (entity != null)
            {
                try
                {
                    await _context.Set<T>().AddAsync(entity);
                    await SaveAllChangesAsync();
                }
                catch (Exception exception)
                {
                    _logger.LogError($"Sth went wrong when adding new entity to db -> {exception.Message}");
                    throw new ApiException(new Error { Message = "An error occured while adding new record", StackTrace = exception.StackTrace });
                }

            }
            else
            {
                _logger.LogError($"Couldn't add to DB cause given entity is null ");
                throw new ApiException(new Error { Message = "Couldn't add to DB cause given entity is null" });
            }
        }

        public async void AddRangeAsync(IEnumerable<T> entities)
        {
            if (entities != null)
            {
                try
                {
                    await _context.Set<T>().AddRangeAsync(entities);
                    await SaveAllChangesAsync();
                }
                catch (Exception exception)
                {
                    _logger.LogError($"Sth went wrong when adding new entity to db -> {exception.Message}");
                    throw new ApiException(new Error { Message = "An error occured while adding new record", StackTrace = exception.StackTrace });
                }

            }
            else
            {
                _logger.LogError($"Couldn't add to DB cause given entity is null ");
                throw new ApiException(new Error { Message = "Couldn't add to DB, please chech the given" });
            }
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> expression)
        {
            try
            {
                var found = (IEnumerable<T>)await _context.Set<T>().FindAsync(expression);
                return found;
            }
            catch (Exception exception)
            {
                _logger.LogError($"Not found -> {exception.Message}");
                throw new ApiException(new Error { Message = "Not found", StackTrace = exception.StackTrace });
            }
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<T>().FindAsync(id);
            }
            catch (Exception exception)
            {
                _logger.LogError($"Given id = {id} did not match any record -> {exception.Message}");
                throw new ApiException(new Error { Message = "There is no matching record with given", StackTrace = exception.StackTrace });
            }
        }

        public void Remove(T entity)
        {
            try
            {
                _context.Set<T>().Remove(entity);
                SaveAllChangesAsync();
            }
            catch (Exception exception)
            {
                _logger.LogError($"Verilen entity = {entity} silinemedi -> {exception.Message}");
                throw new ApiException(new Error { Message = "An error occured while deleting", StackTrace = exception.StackTrace });
            }
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            try
            {
                _context.Set<T>().RemoveRange(entities);
                SaveAllChangesAsync();
            }
            catch (Exception exception)
            {
                _logger.LogError($"Given entities = {entities} could not delete -> {exception.Message}");
                throw new ApiException(new Error { Message = "An error occured while deleting", StackTrace = exception.StackTrace });
            }
        }

        public async Task SaveAllChangesAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception exception)
            {
                _logger.LogError($"An error occured while updating changes -> {exception.Message} - {exception.InnerException} -{exception.StackTrace}");
                throw new ApiException(new Error { Message = "An error occured while updating changes.", StackTrace = exception.StackTrace });
            }
        }

        public async void Update(T entity)
        {
            try
            {
                _context.Set<T>().Update(entity);
                await SaveAllChangesAsync();
            }
            catch (Exception exception)
            {
                _logger.LogError($"An error occured while updating changes -> {exception.Message} - {exception.InnerException} -{exception.StackTrace}");
                throw new ApiException(new Error { Message = "An error occured while updating changes", StackTrace = exception.StackTrace });
            }
        }

        void IGenericRepository<T>.AddAsync(T entity)
        {
            throw new NotImplementedException();
        }

        bool IGenericRepository<T>.Update(T entity)
        {
            throw new NotImplementedException();
        }
    }
}
