using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace repairman.Models
{
    [Table("member")]
    public class Member : BaseUser
    {
        [Display(Name = "密碼錯誤數")]
        public int bad_password_count { get; set; }


        [Display(Name = "E-mail")]
        [EmailAddress]
        public string email { get; set; }

        [Display(Name = "電話")]
        [Phone]
        public string phone { get; set; }


        [Display(Name = "部門")]
        [ForeignKey("dept_id")]
        public virtual Dept dept { get; set; }

        [Display(Name = "部門")]
        public long? dept_id { get; set; }
    }

}
