using System;
using System.ComponentModel.DataAnnotations;
namespace Trades
{
    public class DateRangeAttribute : ValidationAttribute
{
    public DateTime Minimum { get; set; }
   

    public DateRangeAttribute()
    {
        this.Minimum = DateTime.Now;
        
    }

    public override bool IsValid(object value)
    {
        DateTime dateValue = (DateTime)value ;
        
        return dateValue >this.Minimum;
    }
}
}