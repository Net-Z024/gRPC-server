namespace GrpcService1.Models;

public class Chest
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public decimal Price { get; private set; }
    public ICollection<ChestItem> PossibleItems { get; private set; }

    private Chest() { } // For EF Core

    public Chest(string name, decimal price)
    {
        Name = name;
        Price = price;
        PossibleItems = new List<ChestItem>();
    }

    public void AddPossibleItem(Item Item, decimal dropChance)
    {
        if (dropChance < 0 || dropChance > 1)
            throw new ArgumentException("Drop chance must be between 0 and 1");

        var chestItem = new ChestItem(this.Id, Item.Id, dropChance);
        PossibleItems.Add(chestItem);
    }
}