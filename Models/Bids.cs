using System;
namespace Trades.Models
{
    public class Bids
    {
        public int BidsId {get;set;}
        public double Amount {get;set;}
        
        public int UsersId {get;set;}
        public Users Users {get;set;}

        public int AuctionsId {get;set;}
        public Auctions Auctions {get;set;}


        public DateTime CreatedAt {get;set;}
        public DateTime UpdatedAt {get;set;}
    }
}