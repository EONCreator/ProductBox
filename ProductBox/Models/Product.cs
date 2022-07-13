using ProductBox.Models.Media;

namespace ProductBox.Models
{
    public class Product
    {
        public int Id { get; private set; }
        public DateTime DateCreated { get; private set; }

        public List<Image> Images { get; private set; } = new();

        public string Name { get; private set; }
        public string Description { get; private set; }
        public float Price { get; private set; }
        public int Quantity { get; private set; }

        public Product()
        {
            DateCreated = DateTime.Now;
        }

        public void SetName(string name) => Name = name;
        public void SetDescription(string description) => Description = description;
        public void SetPrice(float price) => Price = price;
        public void SetQuantity(int quantity) => Quantity = quantity;

        public void AddImages(List<Image> images) => Images.AddRange(images);
        public void RemoveImages(List<Image> images) => images.ForEach(i => Images.Remove(i));
    }
}
