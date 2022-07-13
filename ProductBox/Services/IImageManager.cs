using ProductBox.Models.Media;

namespace ProductBox.Services
{
    public interface IImageManager
    {
        /// <summary>
        /// Uploading images
        /// </summary>
        /// <param name="files">Files for uploading</param>
        /// <param name="folder">Folder for images</param>
        /// <returns>Genererated images from files</returns>
        Task<List<Image>> UploadImages(IList<IFormFile> files, string folder);

        /// <summary>
        /// Delete images from folder
        /// </summary>
        /// <param name="images">Images to delete</param>
        /// <param name="folder">Folder with images</param>
        /// <returns></returns>
        Task<List<Image>> DeleteImages(IList<int> imageIds, string folder);
    }
}
