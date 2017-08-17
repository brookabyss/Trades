using System;
using System.ComponentModel.DataAnnotations;
namespace Trades.Common
{
    public class DateRangeCustomAttribute : RangeAttribute
    {
        public DateRangeCustomAttribute (string maximumValue): base(typeof(DateTime),DateTime.Now.ToString(),maximumValue){

        }
    }

}