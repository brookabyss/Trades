
using Microsoft.EntityFrameworkCore;
 
namespace Trades.Models
{
    public class TradeContext : DbContext
    {
        // base() calls the parent class' constructor passing the "options" parameter along
        public TradeContext(DbContextOptions<TradeContext> options) : base(options) { }
        public DbSet<Users> Users {get;set;}
        public DbSet<Auctions> Auctions {get;set;}
        public DbSet<Bids> Bids {get;set;}
    }
}