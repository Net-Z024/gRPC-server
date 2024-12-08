namespace GrpcService1.Models;

public class User
{
    public int Id { get; private set; }
    public string IdentityId { get; private set; }
    public decimal Balance { get; private set; }
    public ICollection<UserItem> UserItems { get; private set; }

    private User() { } // For EF Core

    public User(string identityId)
    {
        IdentityId = identityId;
        Balance = 0;
        UserItems = new List<UserItem>();
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

    public void AddItem(Item Item)
    {
        var userItem = new UserItem(this.Id, Item.Id);
        UserItems.Add(userItem);
    }
}