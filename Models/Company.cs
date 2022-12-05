using CSHelper.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace projectman.Models
{
    #region Company
    [Table("company")]
    public class Company : UsesID
    {
        //NAME
        [Display(Name = "名稱")]
        public string name { get; set; }
        //CREDIT
        [Display(Name = "信⽤")]
        [ForeignKey("credit_rating_code")]
        public virtual CreditRating credit_rating { get; set; }

        [Display(Name = "信⽤代碼")]
        public string credit_rating_code { get; set; }
        //REMARKS
        [Display(Name = "備註")]
        public string remarks { get; set; }
        //NATIONAL ID
        [Display(Name = "統⼀編號")]
        public string vatid { get; set; }
        //WEBSITE
        [Display(Name = "網址")]
        public string website { get; set; }
        //PHONE NUMBER
        [Display(Name = "電話")]
        public virtual List<CompanyPhone> phones { get; set; }
        //ADDRESS
        [Display(Name = "地址")]
        public virtual List<CompanyAddress> addresses { get; set; }
        //EMAIL
        [Display(Name = "電⼦信箱")]
        public virtual List<CompanyEmail> emails { get; set; }
    }

    [Table("credit_rating")]
    public class CreditRating
    {
        [DisplayName("代碼")]
        [Required]
        [Key]
        public string code { get; set; }

        [DisplayName("名稱")]
        public string name { get; set; }

        [DisplayName("說明")]
        public string desc { get; set; }
    }

    public enum PhoneType : short
    {
        [Display(Name = "住家")]
        Home = 1,

        [Display(Name = "⼿機")]
        Mobile = 2,

        [Display(Name = "辦公")]
        Office = 3,
    }

    public abstract class UsesCompanyID : UsesID
    {
        [Display(Name = "單位")]
        [ForeignKey("company_id")]
        public virtual Company company { get; set; }

        [Display(Name = "單位ID")]
        public long company_id { get; set; }
    }

    [Table("company_phone")]
    public class CompanyPhone : UsesCompanyID
    {
        [Required]
        [Display(Name = "電話")]
        [DataType(DataType.PhoneNumber)]
        [Phone]
        //NUMBER
        public string number { get; set; }

        [Required]
        [Display(Name = "電話類別")]
        public PhoneType type { get; set; }
    }

    [Table("company_address")]
    public class CompanyAddress : UsesCompanyID
    {
        [Required]
        [Display(Name = "地址")]
        public string addr { get; set; }

        [Required]
        [Display(Name = "地址類別")]
        public ContactAddressType type { get; set; }
    }

    [Table("company_email")]
    public class CompanyEmail : UsesCompanyID
    {
        [Required]
        [Display(Name = "電⼦信箱")]
        [EmailAddress]
        public string email { get; set; }

        [Required]
        [Display(Name = "電⼦類別")]
        public ContactAddressType type { get; set; }
    }
    #endregion

    // "our company" can have multiple registered companies.. each
    // project may be bid under a different company
    [Table("internal_company")]
    public class InternalCompany : UsesID
    {
        //NAME
        [Display(Name = "名稱")]
        public string name { get; set; }

        //NATIONAL ID
        [Display(Name = "統⼀編號")]
        public string vatid { get; set; }

        //REMARKS
        [Display(Name = "備註")]
        public string remarks { get; set; }
    }
}
