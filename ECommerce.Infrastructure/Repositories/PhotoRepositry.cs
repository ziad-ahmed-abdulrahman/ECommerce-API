using ECommerce.Core.Entites.Product;
using ECommerce.Core.interfaces;
using ECommerce.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Repositories
{
    public class PhotoRepositry : GenericRepositry<Photo>, IPhotoRepositry
    {
        public PhotoRepositry(AppDbContext appDbContext) : base(appDbContext)
        {
        }

        public async Task<bool> AddPhotosAsync(IReadOnlyCollection<Photo> photos)
        {

           await _appDbContext.AddRangeAsync(photos);
            var saved = await _appDbContext.SaveChangesAsync();
            return saved > 0;


        }
        public async Task<bool> RemovePhotosAsync(IReadOnlyCollection<Photo> photos)
        {
            if (photos == null || photos.Count == 0)
                return false;
            _appDbContext.RemoveRange(photos);
            var saved = await _appDbContext.SaveChangesAsync();
            return saved > 0;


        }
    }
}
