using CSHelper.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace projectman.Models
{

    public enum ProjectType
    {
        [Display(Name = "未定")]
        Undefined = 0,

        [Display(Name = "專案")]
        DevelopmentContract = 1,

        [Display(Name = "租賃")]
        LeaseContract = 2,

        [Display(Name = "維護")]
        ServiceContract = 3
    }

    public enum ProjectStatus : int
    {
        [Display(Name = "未定")]
        Undefined = 0,

        [Display(Name = "準備標案")]
        Bidding = 1,

        [Display(Name = "議價中")]
        Negotiating = 2,

        [Display(Name = "執行中")]
        Executing = 3,

        [Display(Name = "等待驗收")]
        AwaitingAcceptance = 4,

        [Display(Name = "保固中")]
        WarrantyServicePeriod = 5,

        [Display(Name = "放棄投標")]
        GiveupBid = 10,

        [Display(Name = "投標失敗")]
        FailedBid = 11,

        [Display(Name = "無法結案")]
        FailedExecution = 12,

        [Display(Name = "結案")]
        Completed = 100,
    }

    [Table("project_importance")]
    public class ProjectImportance
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

    public class ProjectSubtype : UsesID
    {

        [Display(Name = "品項")]
        public ProjectType type { get; set; }

        [Display(Name = "名稱")]
        public string name { get; set; }
    }

    #region Project
    [Table("project")]
    public class Project : UsesID
    {
        [Display(Name = "名稱")]
        [Required]
        public string name { get; set; }

        [Display(Name = "編號")]
        public string number { get; set; }

        [Display(Name = "品項")]
        public ProjectType type { get; set; }

        [Display(Name = "狀態")]
        public ProjectStatus status { get; set; }

        [Display(Name = "開始⽇期")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "yyyy-MM-dd, HH:mm:ss.FFF")]
        public DateTime starting_datetime { get; set; }

        [Display(Name = "結束⽇期")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "yyyy-MM-dd, HH:mm:ss.FFF")]
        public DateTime ending_datetime { get; set; }


        [Display(Name = "承辦公司")]
        [ForeignKey("internal_company_id")]
        public virtual InternalCompany internal_company { get; set; }

        [Display(Name = "承辦公司")]
        public long internal_company_id { get; set; }


        [Display(Name = "負責部門")]
        [ForeignKey("group_id")]
        public virtual Group group { get; set; }

        [Display(Name = "負責部門")]
        public long group_id { get; set; }




        [Display(Name = "業務")]
        [ForeignKey("user_id")]
        public virtual User user { get; set; }

        [Display(Name = "業務")]  //THIS SHOULD CONNECT TO ADMIN TABLE
        public long? user_id { get; set; }

        [Display(Name = "重要性")]
        [ForeignKey("importance_id")]
        public virtual ProjectImportance importance { get; set; }

        [Display(Name = "重要性編號")]
        public string importance_id { get; set; }

        [Display(Name = "單位")]
        [ForeignKey("company_id")]
        public virtual Company company { get; set; }

        [Display(Name = "單位")]
        public long? company_id { get; set; }

        [Display(Name = "聯絡⼈")]
        [ForeignKey("contact_id")]
        public virtual Contact contact { get; set; }

        [Display(Name = "聯絡⼈")]
        public long? contact_id { get; set; }

        [Display(Name = "地點")]
        public string contact_address { get; set; }

        [Display(Name = "聯繫電話")]
        public string contact_phone { get; set; }

        [Display(Name = "備註")]
        public string remarks { get; set; }

        [Display(Name = "產品清單")]
        public virtual List<ProjectProduct> products { get; set; }

        [Display(Name = "請款期程")]
        public virtual List<ProjectIncomingPayment> incoming_payments { get; set; }

        [Display(Name = "付款期程")]
        public virtual List<ProjectOutgoingPayment> outgoing_payments { get; set; }

        [Display(Name = "細項")]
        public virtual List<ProjectSubtypeEntry> subtypes { get; set; }

        [Display(Name = "截止日期表")]
        public virtual List<ProjectTimelineEntry> timelines { get; set; }

        public long? connected_project_id { get; set; }
    }

    public abstract class UsesProjectID : UsesID
    {
        [Display(Name = "ID")]
        [ForeignKey("project_id")]
        public virtual Project project { get; set; }

        [Display(Name = "ID")]
        public long project_id { get; set; }
    }


    [Table("project_product")]
    public class ProjectProduct : UsesProjectID
    {
        [Display(Name = "商品")]
        [ForeignKey("product_id")]
        public virtual Product product { get; set; }

        [Display(Name = "商品ID")]
        public long product_id { get; set; }

        [Display(Name = "序號 / 啟動⾦鑰")]
        public string serial_number { get; set; }
    }

    [Table("project_subtype_entry")]
    public class ProjectSubtypeEntry : UsesProjectID
    {
        [Display(Name = "細項")]
        public ProjectSubtype subtype { get; set; }

        [Display(Name = "細項ID")]
        public long subtype_id { get;set; }
    }

    [Table("project_timeline_entry")]
    public class ProjectTimelineEntry : UsesProjectID
    {
        [Display(Name = "到期⽇期")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "yyyy-MM-dd")]
        public DateTime due_date { get; set; } = DateTime.UtcNow;

        [Display(Name = "說明")]
        public string desc { get; set; }

        // set to UtcNow, when order slip number is set for the first time
        [Display(Name = "完成日期")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "yyyy-MM-dd")]
        public DateTime? complete_date { get; set; }

    }

    [Table("project_incoming_payment")]
    public class ProjectIncomingPayment : UsesProjectID
    {
        [Display(Name = "應收款⽇期")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "yyyy-MM-dd")]
        public DateTime due_date { get; set; } = DateTime.UtcNow;

        [Display(Name = "項⽬")]
        public string item { get; set; }

        [Display(Name = "⾦額")]
        [Column(TypeName = "money")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C0}", ApplyFormatInEditMode = false)]
        public decimal amount { get; set; }

        private string _orderslip_number;
        private string _invoice_number;

        [Display(Name = "銷貨單號")]
        public string orderslip_number
        {
            get => _orderslip_number; 
            
            set
            {
                _orderslip_number= value;
                if(!string.IsNullOrWhiteSpace(value) && orderslip_date == null )
                    orderslip_date = DateTime.UtcNow;
            }
        }

        // set to UtcNow, when order slip number is set for the first time
        [Display(Name = "銷貨單建檔日期")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "yyyy-MM-dd, HH:mm:ss.FFF")]
        public DateTime? orderslip_date { get; set; }

        [Display(Name = "發票")]
        public string invoice_number { get => _invoice_number; set
            {
                _invoice_number= value;
                if( !string.IsNullOrWhiteSpace(value) && invoice_date==null )
                    invoice_date = DateTime.UtcNow;
            }
        }

        // set to UtcNow, when invoice number is set for the first time
        [Display(Name = "發票建檔日期")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "yyyy-MM-dd, HH:mm:ss.FFF")]
        public DateTime? invoice_date { get; set; }
    }

    [Table("project_outgoing_payment")]
    public class ProjectOutgoingPayment : UsesProjectID
    {
        [Display(Name = "應付款⽇期")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "yyyy-MM-dd")]
        public DateTime due_date { get; set; }

        [Display(Name = "單位")]
        [ForeignKey("company_id")]
        public virtual Company company { get; set; }

        [Display(Name = "單位")]
        public long? company_id { get; set; }

        [Display(Name = "⾦額")]
        [Column(TypeName = "money")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C0}", ApplyFormatInEditMode = false)]
        public decimal amount { get; set; }
    }
    #endregion

}
