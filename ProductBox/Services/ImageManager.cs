using Microsoft.EntityFrameworkCore;
using ProductBox.Models;
using ProductBox.Models.Media;

namespace ProductBox.Services
{
    public class ImageManager : IImageManager
    {
        private AppDbContext _context;
        private IWebHostEnvironment _hostingEnvironment;

        public ImageManager(AppDbContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<List<Image>> UploadImages(IList<IFormFile> files, string folder)
        {
            var images = new List<Image>();
            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    string path = Path.Combine(_hostingEnvironment.WebRootPath, folder, formFile.FileName);

                    using (var stream = System.IO.File.Create(path))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
                images.Add(new Image { Folder = folder, Src = formFile.FileName.Split('.')[0], Ext = formFile.FileName.Split('.')[1] });
            }

            return images;
        }

        public async Task<List<Image>> DeleteImages(IList<int> imageIds, string folder)
        {
            var images = await _context.Images.Where(i => imageIds.Contains(i.Id)).ToListAsync();

            foreach (var image in images)
            {
                string imageName = $"{image.Src}.{image.Ext}";
                string path = Path.Combine(_hostingEnvironment.WebRootPath, folder, imageName);

                if (File.Exists(path))
                    File.Delete(path);
            }

            return images;
        }
    }
}
