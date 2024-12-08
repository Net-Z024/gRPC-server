namespace GrpcService1.Models;

public class UserProduct
{
    public int Id { get; private set; }
    public int UserId { get; private set; }
    public int ProductId { get; private set; }
    public User User { get; private set; }
    public Product Product { get; private set; }

    private UserProduct() { } // For EF Core

    public UserProduct(int userId, int productId)
    {
        UserId = userId;
        ProductId = productId;
    }
}