using CSHelper.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace repairman.Models
{
    public class Dept : UsesID
    {

        [Display(Name = "名稱")]
        [Required]
        public string name { get; set; }

        [Display(Name = "說明")]
        public string desc { get; set; }

        public virtual List<ServiceRequest> requests { get; set; }

    }
}
