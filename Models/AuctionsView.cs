using System.ComponentModel.DataAnnotations;
using System;
namespace Trades.Models
{
    public class AuctionsView
    {
        [Required]
        [MinLength(3, ErrorMessage="Product name has to be 3 characters in length at least")]
        public string ProductName {get;set;}
        [Required]
        [MinLength(10, ErrorMessage="Description has to be 10 characters in length at least")]
        public string Description {get;set;}
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a bid greater than 0.")]
        public int StartingBid {get;set;}
        
       [Required]
        public DateTime EndDate {get;set;}
        
    }
}