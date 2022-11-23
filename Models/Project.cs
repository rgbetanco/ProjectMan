﻿using CSHelper.Models;
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

        [Display(Name = "開發案")]
        DevelopmentContract = 1,

        [Display(Name = "採購案")]
        ProductProcurement = 2,

        [Display(Name = "服務案")]
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

        [Display(Name = "發票")]
        public string invoice { get; set; }
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