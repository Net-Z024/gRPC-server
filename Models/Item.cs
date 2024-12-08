namespace GrpcService1.Models;

public class Item
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public decimal Value { get; private set; }
    public ICollection<UserItem> UserItems { get; private set; }
    public ICollection<ChestItem> ChestItems { get; private set; }

    private Item() { } // For EF Core

    public Item(string name, decimal value)
    {
        Name = name;
        Value = value;
        UserItems = new List<UserItem>();
        ChestItems = new List<ChestItem>();
    }
}