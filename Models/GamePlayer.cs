using System.ComponentModel.DataAnnotations;

namespace GrpcService1.Models
{
    public class GamePlayer
    {
        [Key]
        public int Id { get; set; }
        public int gameId { get; set; }
        public int userId { get; set; }
        public bool isReady { get; set; }
        public int? spinResult { get; set; }
        public Item spinResultItem { get; set; }
        public User user { get; set; }
        public Game game { get; set; }

    }
}
