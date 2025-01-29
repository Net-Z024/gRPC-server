namespace GrpcService1.Models;

public class Chest
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public ICollection<ChestItem> PossibleItems { get; set; } = new List<ChestItem>();

    public Chest() { }

    public Chest(string name, decimal price)
    {
        Name = name;
        Price = price;
        PossibleItems = new List<ChestItem>();
    }

    public Chest(int? id, string name, decimal price, IEnumerable<ChestItem>? possibleItems = null)
    {
        if(id != null) Id = id.Value;
        Name = name;
        Price = price;
        PossibleItems = possibleItems?.ToList() ?? new List<ChestItem>();
    }

    public void AddPossibleItem(int itemId, decimal dropChance)
    {
        if (dropChance < 0 || dropChance > 1)
            throw new ArgumentException("Drop chance must be between 0 and 1");

        var chestItem = new ChestItem(Id, itemId, dropChance);
        PossibleItems.Add(chestItem);
    }

    public void Update(string name, decimal price, IEnumerable<ChestItem> possibleItems)
    {
        Name = name;
        Price = price;
        PossibleItems.Clear();
        foreach (var item in possibleItems)
        {
            PossibleItems.Add(item);
        }
    }

    public void ClearPossibleItems()
    {
        PossibleItems.Clear();
    }
}