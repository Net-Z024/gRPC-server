namespace GrpcService1.Models;

public class User
{
    public int Id { get; private set; }
    public string IdentityId { get; private set; }
    public decimal Balance { get; private set; }
    public ICollection<UserItem> UserItems { get; private set; } = new List<UserItem>();

    private User() { } // For EF Core

    public User(int userId)
    {
        Id = userId;
        IdentityId = Guid.NewGuid().ToString();
        Balance = 0;
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

    public void AddItem(Item item)
    {
        var userItem = new UserItem(user: this, item: item);
        UserItems.Add(userItem);
    }
}