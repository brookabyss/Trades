using System;
using System.Collections.Generic;
namespace Trades.Models
{
    public class Auctions
    {
        public int AuctionsId {get;set;}
        public string ProductName {get;set;}
        public string Description {get;set;}
        public double StartingBid {get;set;}
        public DateTime EndDate {get;set;}
        public DateTime CreatedAt {get;set;}
        public DateTime UpdatedAt {get;set;}
        public List<Bids> Bids  {get;set;}
        public int UsersId {get;set;}
        public Users Users {get;set;}

        public Auctions(){
           Bids= new List<Bids>();
        }
    }
}