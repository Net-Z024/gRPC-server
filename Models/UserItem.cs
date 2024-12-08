namespace GrpcService1.Models;

public class UserItem
{
    public int Id { get; private set; }
    public int UserId { get; private set; }
    public int ItemId { get; private set; }
    public User User { get; private set; }
    public Item Item { get; private set; }

    private UserItem() { } // For EF Core

    public UserItem(int userId, int ItemId)
    {
        UserId = userId;
        ItemId = ItemId;
    }
}