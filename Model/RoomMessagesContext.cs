using Microsoft.EntityFrameworkCore;
using TestApi.Messages;

namespace TestApi.Model
{
    public class RoomMessagesContext(DbContextOptions<RoomMessagesContext> options) : DbContext(options)
    {
        public DbSet<RoomMessage> RoomMessages { get; set; }
    }
}
