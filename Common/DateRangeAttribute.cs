using System;
using System.ComponentModel.DataAnnotations;
namespace Trades.Common
{
    public class DateRangeAttribute : RangeAttribute
    {
        public DateRangeAttribute (string maximumValue): base(typeof(DateTime),DateTime.Now.ToString(),maximumValue){

        }
    }

}