using CSHelper.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace projectman.Models
{
    public enum ContactAddressType : short
    {
        [Display(Name = "住宅")]
        Home = 1,

        [Display(Name = "工作")]
        Work = 2,

        [Display(Name = "戶籍")]
        Household = 3,

        [Display(Name = "聯絡")]
        Contact = 4,

        [Display(Name = "其他")]
        Other = 999
    }

    [Table("contact")]
    public class Contact : UsesID
    {
        //NAME
        [Display(Name = "姓名")]
        public string name { get; set; }
        //DEPARTMENT
        [Display(Name = "部⾨")]
        public string department { get; set; }
        //REMARKS
        [Display(Name = "備註")]
        public string remarks { get; set; }
        //PHONE NUMBER
        [Display(Name = "電話")]
        public virtual List<ContactPhone> phones { get; set; }
        //ADDRESS
        [Display(Name = "地址")]
        public virtual List<ContactAddress> addresses { get; set; }
        //EMAIL
        [Display(Name = "電⼦信箱")]
        public virtual List<ContactEmail> emails { get; set; }
        //MEMBERS
        public virtual List<ContactCompany> companies { get; set; }
    }

    public abstract class UsesContactID : UsesID {
        [Display(Name = "人員")]
        [ForeignKey("persona_id")]
        public virtual Contact contact { get; set; }

        [Display(Name = "人員ID")]
        public long contact_id { get; set; }
    }

    [Table("contact_phone")]
    public class ContactPhone : UsesContactID
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

        [Display(Name = "預設聯繫電話")]
        public bool is_default { get; set; } = false;
    }

    [Table("contact_address")]
    public class ContactAddress : UsesContactID
    {

        [Required]
        [Display(Name = "地址")]
        public string addr { get; set; }

        [Required]
        [Display(Name = "地址類別")]
        public ContactAddressType type { get; set; }
    }

    [Table("contact_email")]
    public class ContactEmail : UsesContactID
    {

        [Required]
        [Display(Name = "電⼦信箱")]
        [EmailAddress]
        public string email { get; set; }

        [Required]
        [Display(Name = "電⼦類別")]
        public ContactAddressType type { get; set; }
    }


    [Table("contact_company")]
    public class ContactCompany : UsesContactID
    {
        [Display(Name = "單位")]
        [ForeignKey("company_id")]
        public virtual Company company { get; set; }

        [Display(Name = "單位")]
        public long company_id { get; set; }


        [Display(Name = "職稱")]
        public string job_title { get; set; }
    }

    public class CompanyContactsViewModel
    {
        [Display(Name = "ID")]
        public long ID { get; set; }

        [Display(Name = "名稱")]
        public string name { get; set; }
        [Display(Name = "職稱")]
        public string job_title { get; set; }
        [Display(Name = "電話")]
        public string phone { get; set; }
    }

    public class CompanyInPersonasViewModel
    {
        public string company_name { get; set; }
        public long company_id { get; set; }
        public string job_title { get; set; }
    }

}
