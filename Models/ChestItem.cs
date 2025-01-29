namespace GrpcService1.Models;

public class ChestItem
{
    public int Id { get; private set; }
    public int ChestId { get; private set; }
    public int ItemId { get; set; }
    public decimal DropChance { get; set; }
    public Chest? Chest { get; private set; }
    public Item? Item { get; private set; }

    private ChestItem() { } // For EF Core

    public ChestItem(int itemId, decimal dropChance)
    {
        ItemId = itemId;
        DropChance = dropChance;
    }

    public ChestItem(int chestId, int itemId, decimal dropChance, Chest? chest = null, Item? item = null)
    {
        ChestId = chestId;
        ItemId = itemId;
        DropChance = dropChance;
        Chest = chest;
        Item = item;
    }
}