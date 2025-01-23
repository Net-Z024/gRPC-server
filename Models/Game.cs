using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.ComponentModel.DataAnnotations;

namespace GrpcService1.Models
{
    public class Game

    {
        [Key]
        public int Id { get;  set; }  
        public int hostId { get;  set; }
        public User host { get; set; }
        public int caseId { get; set; }
        public Chest chest { get;  set; }
        public int maxPlayers { get;  set; }
        public bool isStarted { get; set; }
        public DateTime createdAt { get; set; }

        public ICollection<GamePlayer> GamePlayers{ get; set; }

    }

}
