namespace ProductBox.Models.Media
{
    public class Image
    {
        public int Id { get; set; }
        public string Folder { get; set; }
        public string Src { get; set; }
        public string Ext { get; set; }

        public List<Product> Products { get; set; } = new();

        public Image(string folder, string src, string ext)
        {
            Folder = folder;
            Src = src;
            Ext = ext;
        }
    }
}
