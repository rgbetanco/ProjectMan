using CSHelper.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace repairman.Models
{
    #region Project
    [Table("project")]
    public class ProjectModel : UsesID
    {
        [Display(Name = "名稱")]
        [Required]
        public string name { get; set; }

        [Display(Name = "編號")]
        public string number { get; set; }

        [Display(Name = "品項")]
        public ServiceTypeEnum service_type { get; set; }

        [Display(Name = "狀態")]
        public ProjectStatusEnum status { get; set; }

        [Display(Name = "開始⽇期")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "yyyy-MM-dd, HH:mm:ss.FFF")]
        public DateTime starting_datetime { get; set; }

        [Display(Name = "結束⽇期")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "yyyy-MM-dd, HH:mm:ss.FFF")]
        public DateTime ending_datetime { get; set; }

        [Display(Name = "業務")]
        [ForeignKey("user_id")]
        public virtual User user { get; set; }

        [Display(Name = "業務")]  //THIS SHOULD CONNECT TO ADMIN TABLE
        public long? user_id { get; set; }

        [Display(Name = "重要性")]
        public ImportanceEnum importance_id { get; set; }

        [Display(Name = "單位")]
        [ForeignKey("company_id")]
        public virtual CompanyModel company { get; set; }

        [Display(Name = "單位")]
        public long? company_id { get; set; }

        [Display(Name = "聯絡⼈")]
        [ForeignKey("persona_id")]
        public virtual PersonaModel persona { get; set; }

        [Display(Name = "聯絡⼈")]
        public long? persona_id { get; set; }

        [Display(Name = "地點")]
        public string contact_address { get; set; }

        [Display(Name = "聯繫電話")]
        public string contact_phone { get; set; }

        [Display(Name = "備註")]
        public string remarks { get; set; }

        [Display(Name = "產品清單")]
        public IList<ProductModel> product_list { get; set; }

        [Display(Name = "請款期程")]
        public IList<IncomingPaymentModel> incoming_payment { get; set; }

        [Display(Name = "付款期程")]
        public IList<OutgoingPaymentModel> outgoing_payment { get; set; }

        public long? connected_project_id { get; set; }
    }

    public enum ServiceTypeEnum {
        [Display(Name = "所有")]
        Undefined = 0,
        ServiceA = 1,
        ServiceB = 2
    }

    public enum ProjectStatusEnum : int
    {
        [Display(Name = "所有")]
        Undefined = 0,

        [Display(Name = "待處裡")]
        Pending = 1,

        [Display(Name = "處理中")]
        Processing = 2,

        [Display(Name = "無法處理")]
        Unfixable = 3,

        [Display(Name = "等零件")]
        AwaitingParts = 4,

        [Display(Name = "完成")]
        Completed = 5
    }

    [Table("sales_person")]
    public class SalesPersonModel : UsesID
    {
        public string name { get; set; }
    }

    public enum ImportanceEnum : int
    {
        ImportanceA = 0,
        ImportanceB = 1
    }
    [Table("contact")]
    public class ContactModel : UsesID
    {
        [Display(Name = "聯絡⼈")]
        public string name { get; set; }
        [Display(Name = "地點")]
        public string address { get; set; }
        [Display(Name = "聯繫電話")]
        public string phone { get; set; }
    }

    public abstract class ProductCategoryFK : UsesID
    {
        [Display(Name = "ID")]
        [ForeignKey("project_id")]
        public virtual ProjectModel project { get; set; }

        [Display(Name = "ID")]
        public long? project_id { get; set; }

        [Display(Name = "商牌 ")]
        [ForeignKey("product_brand_id")]
        public virtual ProductBrandModel brand { get; set; }
        [Display(Name = "商牌 ")]
        public long? product_brand_id { get; set; }

        [Display(Name = "型號")]
        [ForeignKey("product_model_id")]
        public virtual ProductModelModel model { get; set; }
        [Display(Name = "型號")]
        public long? product_model_id { get; set; }
    }
    
    [Table("product_list")]
    public class ProductModel : ProductCategoryFK
    {
        [Display(Name = "類別")]
        public ProductCategoryEnum category { get; set; }

        [Display(Name = "序號 / 啟動⾦鑰")]
        public string serial_number { get; set; }
    }

    [Table("incoming_payment")]
    public class IncomingPaymentModel : UsesID
    {
        [Display(Name = "ID")]
        [ForeignKey("project_id")]
        public virtual ProjectModel project { get; set; }

        [Display(Name = "ID")]
        public long? project_id { get; set; }

        [Display(Name = "⽇期")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "yyyy-MM-dd")]
        public DateTime issueDate { get; set; }

        [Display(Name = "項⽬")]
        public string item { get; set; }

        [Display(Name = "⾦額")]
        public double amount { get; set; }

        [Display(Name = "發票")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "yyyy-MM-dd")]
        public string invoice { get; set; }
    }
    public enum ProductCategoryEnum : int
    {
        CategoryA = 0,
        CategoryB = 1
    }
    [Table("product_brand")]
    public class ProductBrandModel : UsesID
    {
        [Display(Name = "商牌")]
        public ProductCategoryEnum category { get; set; }

        [Display(Name = "商牌")]
        public string brand_name { get; set; }
    }

    [Table("product_model")]
    public class ProductModelModel : UsesID
    {
        [Display(Name = "類別")]
        public ProductCategoryEnum category { get; set; }

        [Display(Name = "商牌")]
        public string model_name { get; set; }
    }
    [Table("outgoing_payment")]
    public class OutgoingPaymentModel : UsesID
    {
        [Display(Name = "ID")]
        [ForeignKey("project_id")]
        public virtual ProjectModel project { get; set; }

        [Display(Name = "ID")]
        public long? project_id { get; set; }

        [Display(Name = "⽇期")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "yyyy-MM-dd")]
        public DateTime issueDate { get; set; }

        [Display(Name = "單位")]
        [ForeignKey("company_id")]
        public virtual CompanyModel company { get; set; }

        [Display(Name = "單位")]
        public long? company_id { get; set; }

        [Display(Name = "⾦額")]
        public double amount { get; set; }
    }
    #endregion
    #region Company
    [Table("company")]
    public class CompanyModel : UsesID
    {
        //NAME
        [Display(Name = "名稱")]
        public string name { get; set; }
        //CREDIT
        [Display(Name = "信⽤")]
        [ForeignKey("credit_id")]
        public virtual CreditModel credit { get; set; }
        [Display(Name = "信⽤")]
        public long? credit_id { get; set; }
        //REMARKS
        [Display(Name = "備註")]
        public string remarks { get; set; }
        //NATIONAL ID
        [Display(Name = "統⼀編號")]
        public string nationalID { get; set; }
        //WEBSITE
        [Display(Name = "網址")]
        public string website { get; set; }
        //PHONE NUMBER
        [Display  (Name = "電話")]
        public IList<CompanyPhoneModel> phone { get; set; }
        //ADDRESS
        [Display (Name = "地址")]
        public IList<CompanyAddressModel> address { get; set; }
        //EMAIL
        [Display(Name = "電⼦信箱")]
        public IList<CompanyEmailModel> email { get; set; }
    }

    [Table("company_persona")]
    public class CompanyPersonaModel : UsesID
    {
        [Display(Name = "ID")]
        [ForeignKey("persona_id")]
        public virtual PersonaModel persona { get; set; }
        public long persona_id { get; set; }
    }

    [Table("credit")]
    public class CreditModel : UsesID
    {
        [Display(Name = "名稱")]
        public string name { get; set; }
    }

    public enum PhoneType : short
    {
        [Display(Name = "⼿機")]
        Home = 1,

        [Display(Name = "電話")]
        Work = 2,
    }

    public enum SourceType
    {
        company = 1,
        persona = 2,
    }

    public enum ContactType : short
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

    [Table("company_phone")]
    public class CompanyPhoneModel : UsesID
    {
        [Display(Name = "單位管")]
        [ForeignKey("company_id")]
        public virtual CompanyModel company { get; set; }

        [Display(Name = "單位管")]
        public long company_id { get; set; }

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
    public class CompanyAddressModel : UsesID
    {
        [Display(Name = "單位管")]
        [ForeignKey("company_id")]
        public virtual CompanyModel company { get; set; }

        [Display(Name = "單位管")]
        public long company_id { get; set; }

        [Required]
        [Display(Name = "地址")]
        public string addr { get; set; }

        [Required]
        [Display(Name = "地址類別")]
        public ContactType type { get; set; }
    }

    [Table("company_email")]
    public class CompanyEmailModel : UsesID
    {
        [Display(Name = "單位管")]
        [ForeignKey("company_id")]
        public virtual CompanyModel company { get; set; }

        [Display(Name = "單位管")]
        public long company_id { get; set; }

        [Required]
        [Display(Name = "電⼦信箱")]
        [EmailAddress]
        public string email { get; set; }

        [Required]
        [Display(Name = "電⼦類別")]
        public ContactType type { get; set; }
    }
    #endregion
    #region Persona
    [Table("persona_phone")]
    public class PersonaPhoneModel : UsesID
    {
        [Display(Name = "單位管")]
        [ForeignKey("persona_id")]
        public virtual PersonaModel persona { get; set; }

        [Display(Name = "單位管")]
        public long persona_id { get; set; }

        [Required]
        [Display(Name = "電話")]
        [DataType(DataType.PhoneNumber)]
        [Phone]
        //NUMBER
        public string number { get; set; }

        [Required]
        [Display(Name = "電話類別")]
        public PhoneType type { get; set; }
        [Display(Name = "聯繫電話")]
        public bool default_number { get; set; } = false;
    }

    [Table("persona_address")]
    public class PersonaAddressModel : UsesID
    {
        [Display(Name = "單位管")]
        [ForeignKey("persona_id")]
        public virtual PersonaModel persona { get; set; }

        [Display(Name = "單位管")]
        public long persona_id { get; set; }

        [Required]
        [Display(Name = "地址")]
        public string addr { get; set; }

        [Required]
        [Display(Name = "地址類別")]
        public ContactType type { get; set; }
    }

    [Table("persona_email")]
    public class PersonaEmailModel : UsesID
    {
        [Display(Name = "單位管")]
        [ForeignKey("persona_id")]
        public virtual PersonaModel persona { get; set; }

        [Display(Name = "單位管")]
        public long persona_id { get; set; }

        [Required]
        [Display(Name = "電⼦信箱")]
        [EmailAddress]
        public string email { get; set; }

        [Required]
        [Display(Name = "電⼦類別")]
        public ContactType type { get; set; }
    }

    [Table("persona")]
    public class PersonaModel : UsesID
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
        public IList<PersonaPhoneModel> phone { get; set; }
        //ADDRESS
        [Display(Name = "地址")]
        public IList<PersonaAddressModel> address { get; set; }
        //EMAIL
        [Display(Name = "電⼦信箱")]
        public IList<PersonaEmailModel> email { get; set; }
        //MEMBERS
        public IList<PersonaCompanyModel> personas_company { get; set; }
    }

    [Table("persona_company")]
    public class PersonaCompanyModel : UsesID
    {
        [Display(Name = "單位管")]
        [ForeignKey("company_id")]
        public virtual CompanyModel company { get; set; }

        [Display(Name = "單位管")]
        public long company_id { get; set; }

        [Display(Name = "單位管")]
        [ForeignKey("persona_id")]
        public virtual PersonaModel persona { get; set; }

        [Display(Name = "單位管")]
        public long persona_id { get; set; }

        [Display(Name = "職稱")]
        public string job_title { get; set; }
    }

    public class PersonaInCompanyViewModel
    {
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
    #endregion
    
}
