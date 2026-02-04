using ECommerce.Core.interfaces;
using ECommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Repositories
{
   public class GenericRepositry<T> : IGenericRepositry<T> where T : class
    {
        protected readonly AppDbContext _appDbContext;

        public GenericRepositry(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<IReadOnlyCollection<T>> GetAllAsync()
        {
            return await _appDbContext.Set<T>().AsNoTracking().ToListAsync();
        }

        public async Task<IReadOnlyCollection<T>> GetAllAsync(params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T>? query = _appDbContext.Set<T>().AsNoTracking();

            foreach (var item in includes) 
            {
            query = query.Include(item);    
            }

            return await query.ToListAsync(); 
            
        }

        public async Task<T?> GetByIdAsync(int id)
        {
           return await _appDbContext.Set<T>().FindAsync(id); 
        }

        public Task<T?> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes) // .....  
        {
            IQueryable<T>? query = _appDbContext.Set<T>();

            foreach(var item in includes) 
            {
                query = query.Include(item);
            }   
            var entity = query.FirstOrDefaultAsync(predicate: x => EF.Property<int>(x,"Id") == id);
            return entity; 

        }
        public async Task AddAsync(T entity)
        {
            await _appDbContext.Set<T>().AddAsync(entity);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            _appDbContext.Entry(entity).State = EntityState.Modified;
             await _appDbContext.SaveChangesAsync();  
        }

        public  async Task DeleteAsync(int id) // ..... 
        {
            T? entity = await _appDbContext.Set<T>().FindAsync(id);
            _appDbContext.Set<T>().Remove(entity!);  //    
            await _appDbContext.SaveChangesAsync();

        }

      
    }
}
