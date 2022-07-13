using ProductBox.Models.Media;

namespace ProductBox.Models.ViewModels
{
    public class AddProductViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public float? Price { get; set; }
        public int? Quantity { get; set; }
    }

    public class EditProductViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public float? Price { get; set; }
        public int? Quantity { get; set; }

        public List<int> DeletedImageIds { get; set; } = new();
    }
}
