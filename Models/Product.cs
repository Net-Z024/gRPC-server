namespace GrpcService1.Models;

public class Product
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public decimal Value { get; private set; }
    public ICollection<UserProduct> UserProducts { get; private set; }
    public ICollection<ChestProduct> ChestProducts { get; private set; }

    private Product() { } // For EF Core

    public Product(string name, decimal value)
    {
        Name = name;
        Value = value;
        UserProducts = new List<UserProduct>();
        ChestProducts = new List<ChestProduct>();
    }
}