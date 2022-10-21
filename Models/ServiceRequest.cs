using CSHelper.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace repairman.Models
{

    public enum ServiceRequestStatus : int
    {
        [Display(Name = "待處裡")]
        Pending = 0,

        [Display(Name = "處理中")]
        Processing = 1,

        [Display(Name = "無法處理")]
        Unfixable = 2,

        [Display(Name = "等零件")]
        AwaitingParts = 3,

        [Display(Name = "完成")]
        Completed = 4
    }

    public class ServiceRequestCat : UsesID
    {
        [Display(Name = "服務類別")]
        [Required]
        public string name { get; set; }

        [Display(Name = "說明")]
        public string desc { get; set; }

        public virtual List<ServiceRequestSubCat> subcats { get; set; }
    }

    public class ServiceRequestSubCat : UsesID
    {
        [Display(Name = "服務類別")]
        public long cat_id { get; set; }


        [ForeignKey("cat_id")]
        public virtual ServiceRequestCat cat { get; set; }


        [Display(Name = "富類別")]
        [Required]
        public string name { get; set; }

        [Display(Name = "說明")]
        public string desc { get; set; }

        public virtual List<ServiceRequest> requests { get; set; }
    }

    public class ServiceRequest : UsesID
    {


        [Display(Name = "服務類別")]
        public long sub_cat_id { get; set; }


        [ForeignKey("sub_cat_id")]
        public virtual ServiceRequestSubCat sub_cat { get; set; }

        [Display(Name = "狀態")]
        public ServiceRequestStatus status { get; set; }

        [Display(Name = "補充說明")]
        public string desc { get; set; }


        [Display(Name = "部門")]
        public long dept_id { get; set; }

        [ForeignKey("dept_id")]
        public virtual Dept dept { get; set; }


        [Display(Name = "姓名")]
        [Required]
        [MinLength(1)]
        public string name { get; set; }

        [Display(Name = "E-mail")]
        [Required]
        [MinLength(1)]
        public string email { get; set; }

        [Display(Name = "連絡電話")]
        [Required]
        [MinLength(1)]
        public string phone { get; set; }


        [Display(Name = "建檔時間")]
        [DataType(DataType.DateTime)]
        public DateTime date { get; set; }

        [Display(Name = "最後變動時間")]
        [DataType(DataType.DateTime)]
        public DateTime modify_date { get; set; }


        [ForeignKey("member_id")]
        public virtual Member member { get; set; }

        [Display(Name = "登記者")]
        public long? member_id { get; set; }

        [Display(Name = "圖片")]
        public virtual List<ServiceRequestPic> pics { get; set; }


        [Display(Name = "附件")]
        public virtual List<ServiceRequestFile> files { get; set; }

        [Display(Name = "回復")]
        public virtual List<ServiceRequestReply> replies { get; set; }
    }

    public abstract class UsesServiceRequestID : UsesID
    {
        [ForeignKey("request_id")]
        public virtual ServiceRequest request { get; set; }

        [Display(Name = "服務案件")]
        public long request_id { get; set; }
    }


    public class ServiceRequestPic : PicFileModel
    {
        [ForeignKey("request_id")]
        public virtual ServiceRequest request { get; set; }

        [Display(Name = "服務案件")]
        public long request_id { get; set; }
    }

    public class ServiceRequestFile : FileModel
    {
        [ForeignKey("request_id")]
        public virtual ServiceRequest request { get; set; }

        [Display(Name = "服務案件")]
        public long request_id { get; set; }
    }

    public class ServiceRequestReply : UsesServiceRequestID
    {
        [Display(Name = "處裡方式")]
        public string title { get; set; }

        [Display(Name = "補充說明")]
        public string desc { get; set; }

        [Display(Name = "回覆時間")]
        [DataType(DataType.DateTime)]
        public DateTime date { get; set; }

        [Display(Name = "最後修改時間")]
        [DataType(DataType.DateTime)]
        public DateTime modify_date { get; set; }

        [Display(Name = "狀態")]
        public ServiceRequestStatus status { get; set; }

        [ForeignKey("user_id")]
        [Display(Name = "回覆人員")]
        public virtual User user { get; set; }

        [Display(Name = "回覆人員")]
        public long user_id { get; set; }

        [Display(Name = "圖片")]
        public virtual List<ServiceRequestReplyPic> pics { get; set; }

        [Display(Name = "附件")]
        public virtual List<ServiceRequestReplyFile> files { get; set; }
    }

    public class ServiceRequestReplyFile : FileModel
    {
        [ForeignKey("reply_id")]
        [Display(Name = "文章回覆")]
        public virtual ServiceRequestReply reply { get; set; }

        [Display(Name = "文章回覆")]
        public long reply_id { get; set; }
    }

    public class ServiceRequestReplyPic : PicFileModel
    {
        [ForeignKey("reply_id")]
        [Display(Name = "服務案件回覆")]
        public virtual ServiceRequestReply reply { get; set; }

        [Display(Name = "服務案件回覆ID")]
        public long reply_id { get; set; }
    }
}