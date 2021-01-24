using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Sayar.TimeZoneConverter.Models
{
    public class ConversionViewModel
    {
        public string Time { get; set; }
        [Required]
        public string MyDate { get; set; }
        public string ConvertTo { get; set; }
    }

}
