using System;
using System.Collections.Generic;
namespace Trades.Models
{
    public class Users
    {
        public int UsersId {get;set;}
        public string FirstName {get;set;}
        public string LastName {get;set;}
        public string Username {get;set;}
        public string Password {get;set;}
        public double Wallet {get;set;}
        public DateTime CreatedAt {get;set;}
        public DateTime UpdatedAt {get;set;}
        public List<Bids> Bids  {get;set;}

        public Users(){
           Bids= new List<Bids>();
        }
    }
}