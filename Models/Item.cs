namespace GrpcService1.Models;

public class Item
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public decimal Value { get; private set; }
    public string ImageUrl { get; private set; }
    public ICollection<UserItem> UserItems { get; private set; }
    public ICollection<ChestItem> ChestItems { get; private set; }

    private Item() { } // For EF Core

    public Item(string name, decimal value, string imageUrl)
    {
        Name = name;
        Value = value;
        ImageUrl = imageUrl;
        UserItems = new List<UserItem>();
        ChestItems = new List<ChestItem>();
    }

    public void Update(string name, decimal value, string imageUrl)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name cannot be empty or null", nameof(name));
        }

        if (value < 0)
        {
            throw new ArgumentException("Value must be a positive number", nameof(value));
        }

        if (string.IsNullOrWhiteSpace(imageUrl))
        {
            throw new ArgumentException("Image URL cannot be empty or null", nameof(imageUrl));
        }

        Name = name;
        Value = value;
        ImageUrl = imageUrl;
    }
}