namespace GrpcService1.Models;

public class ChestProduct
{
    public int Id { get; private set; }
    public int ChestId { get; private set; }
    public int ProductId { get; private set; }
    public decimal DropChance { get; private set; }
    public Chest Chest { get; private set; }
    public Product Product { get; private set; }

    private ChestProduct() { } // For EF Core

    public ChestProduct(int chestId, int productId, decimal dropChance)
    {
        ChestId = chestId;
        ProductId = productId;
        DropChance = dropChance;
    }
}