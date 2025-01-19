using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GrpcService1.Models
{
    public class User
    {
        // EF Core will automatically set this to auto-increment
        [Key]
        public int Id { get; private set; }  // Auto-incremented integer primary key
        public string IdentityId { get; set; }
        public decimal Balance { get; private set; }
        public ICollection<UserItem> UserItems { get; private set; } = new List<UserItem>();
        public virtual IdentityUser IdentityUser { get; private set; }

        // For EF Core
        private User() { }

        public User(string identityUserId)
        {
            IdentityId = identityUserId;
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
}
