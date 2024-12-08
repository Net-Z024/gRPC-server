namespace GrpcService1.Models;

public class User
{
    public int Id { get; private set; }
    public string IdentityId { get; private set; }
    public decimal Balance { get; private set; }
    public ICollection<UserProduct> UserProducts { get; private set; }

    private User() { } // For EF Core

    public User(string identityId)
    {
        IdentityId = identityId;
        Balance = 0;
        UserProducts = new List<UserProduct>();
    }

    public void AddBalance(decimal amount)
    {
        if (amount < 0) throw new ArgumentException("Amount cannot be negative");
        Balance += amount;
    }

    public bool TrySpendBalance(decimal amount)
    {
        if (amount < 0) throw new ArgumentException("Amount cannot be negative");
        if (Balance < amount) return false;
        
        Balance -= amount;
        return true;
    }

    public void AddProduct(Product product)
    {
        var userProduct = new UserProduct(this.Id, product.Id);
        UserProducts.Add(userProduct);
    }
}