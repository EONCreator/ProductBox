using ProductBox.Models.Media;
using SixLabors.ImageSharp;
using Image = ProductBox.Models.Media.Image;

namespace ProductBox.Services
{
    public interface IImageManager
    {
        /// <summary>
        /// Resize uploaded image with specific width and height
        /// </summary>
        /// <param name="imagePath">Path of uploaded image</param>
        /// <param name="saveTo">Path to save image</param>
        /// <param name="size">New image size</param>
        void ResizeImage(string imagePath, string saveTo, Size size);
        /// <summary>
        /// Uploading images
        /// </summary>
        /// <param name="files">Files for uploading</param>
        /// <param name="folder">Folder for images</param>
        /// <returns>Genererated images from files</returns>
        Task<List<Image>> UploadImages(IList<IFormFile> files, string folder);
        /// <summary>
        /// Uploading images
        /// </summary>
        /// <param name="files">Files for uploading</param>
        /// <param name="folder">Folder for images</param>
        /// <param name="sizes">Size array to resize image</param>
        /// <returns>Genererated images from files</returns>
        Task<List<Image>> UploadImages(IList<IFormFile> files, string folder, List<ImageSize> sizes);
        /// <summary>
        /// Delete images from folder
        /// </summary>
        /// <param name="images">Images to delete</param>
        /// <param name="folder">Folder with images</param>
        /// <returns></returns>
        Task<List<Image>> DeleteImages(IList<int> imageIds, string folder);
    }

    /// <summary>
    /// Specific size for image with name
    /// </summary>
    public struct ImageSize
    {
        public string Name { get; set; }
        public Size Size { get; set; }
    }
}
