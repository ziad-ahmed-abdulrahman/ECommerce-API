using ECommerce.Core.Entites.Product;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.interfaces
{
    public interface IPhotoRepositry : IGenericRepositry<Photo>
    {
      public Task<bool> AddPhotosAsync(IReadOnlyCollection<Photo> photos);
      public Task<bool> RemovePhotosAsync(IReadOnlyCollection<Photo> photos); 

    }
}
