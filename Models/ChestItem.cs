namespace GrpcService1.Models;

public class ChestItem
{
    public int Id { get; private set; }
    public int ChestId { get; private set; }
    public int ItemId { get; private set; }
    public decimal DropChance { get; private set; }
    public Chest Chest { get; private set; }
    public Item Item { get; private set; }

    private ChestItem() { } // For EF Core

    public ChestItem(int chestId, int ItemId, decimal dropChance)
    {
        ChestId = chestId;
        ItemId = ItemId;
        DropChance = dropChance;
    }
}