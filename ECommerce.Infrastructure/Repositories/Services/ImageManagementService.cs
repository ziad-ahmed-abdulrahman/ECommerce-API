using ECommerce.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Repositories.Services
{
    public class ImageManagementService : IImageManagementService
    {
        private readonly IFileProvider _fileProvider;
        public ImageManagementService(IFileProvider fileProvider)
        {
            _fileProvider = fileProvider;
        }

        public async Task<List<string>> AddImageAsync(IFormFileCollection files, string src)
        {
            var saveImageSrc = new List<string>();

            var ImageDirectory = Path.Combine("wwwroot", "Images", src);

            if (Directory.Exists(ImageDirectory) is not true)
            {
                Directory.CreateDirectory(ImageDirectory);
            }

            foreach (IFormFile file in files)
            {
                if (file.Length > 0)
                {
                    var ImageName = file.FileName;

                    var ImageSrc = $"/Images/{src}/{ImageName}";

                    var root = Path.Combine(ImageDirectory, ImageName); // // for saving in server

                    using (FileStream stream = new FileStream(path: root, FileMode.Create))
                    {
                        await file.CopyToAsync(stream); 
                    }

                    saveImageSrc.Add(item: ImageSrc); // for frontend 
                }
            }

            return saveImageSrc;
        }

        public Task DeleteImageAsync(string src)
        {
            var info = _fileProvider.GetFileInfo(src);

            string? root = info.PhysicalPath;

            if (root != null)
                File.Delete(root);

           return Task.CompletedTask;

        }
    }
}
