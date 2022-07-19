using Microsoft.EntityFrameworkCore;
using ProductBox.Models;
using ProductBox.Models.Media;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Image = ProductBox.Models.Media.Image;

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

        public void ResizeImage(string imagePath, string saveTo, Size size)
        {
            using (SixLabors.ImageSharp.Image image = SixLabors.ImageSharp.Image.Load(imagePath))
            {
                image.Mutate(x => x
                     .Resize(new ResizeOptions
                     {
                         PadColor = Color.White,
                         Mode = ResizeMode.Pad,
                         Position = AnchorPositionMode.Center,
                         Size = new Size(1024 / size.Width, 1024 / size.Height)
                     }));
                image.Save(saveTo);
            }
        }

        public async Task<List<Image>> UploadImages(IList<IFormFile> files, string folder)
        {
            var images = new List<Image>();
            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    string path = Path.Combine(_hostingEnvironment.WebRootPath, "images", folder, formFile.FileName);

                    using (var stream = System.IO.File.Create(path))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                    images.Add(new Image(folder, Path.GetFileNameWithoutExtension(path), Path.GetExtension(path)));
                }
            }

            return images;
        }

        public async Task<List<Image>> UploadImages(IList<IFormFile> files, string folder, List<ImageSize> sizes)
        {
            var images = await UploadImages(files, folder);
            foreach (var image in images)
            {
                string fileName = $"{image.Src}{image.Ext}";
                string folderPath = Path.Combine(_hostingEnvironment.WebRootPath, "images", folder);

                string imagePath = Path.Combine(folderPath, fileName);
                foreach (var size in sizes)
                {
                    string savedImageName = $"{image.Src}_{size.Name}{image.Ext}";
                    string savedImagePath = Path.Combine(folderPath, savedImageName);
                    ResizeImage(imagePath, savedImagePath, size.Size);
                }
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
