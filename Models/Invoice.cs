using CSHelper.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace projectman.Models
{

    #region Invoice
    [Table("invoice")]
    public class Invoice : UsesID
    {
        [Display(Name = "公司名稱")]
        [ForeignKey("company_id")]
        public virtual Company company { get; set; }

        [Display(Name = "公司ID")]
        public long? company_id { get; set; }

        [Display(Name = "發票號碼")]
        public string number { get; set; }

        [Display(Name = "開立日期")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "yyyy-MM-dd, HH:mm:ss.FFF")]
        public DateTime issue_date { get; set; }

        [Display(Name = "總金額")]
        [Column(TypeName = "money")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C0}", ApplyFormatInEditMode = false)]
        public decimal total_amount { get; set; }
        public virtual List<InvoiceItem> items { get; set; }

        [Display(Name = "⽇期")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "yyyy-MM-dd, HH:mm:ss.FFF")]
        public DateTime created { get; set; } = DateTime.Now;
    }

    [Table("invoice_item")]
    public class InvoiceItem : UsesID
    {
        [Display(Name = "ID")]
        [ForeignKey("invoice_id")]
        public virtual Invoice invoice { get; set; }

        [Display(Name = "ID")]
        public long? invoice_id { get; set; }

        [Display(Name = "收款項目")]
        [ForeignKey("incoming_payment_id")]
        public virtual ProjectIncomingPayment incoming_payment { get; set; }

        [Display(Name = "收款項目ID")]
        public long? incoming_payment_id { get; set; }

        [Display(Name = "小計")]
        [Column(TypeName = "money")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C0}", ApplyFormatInEditMode = false)]
        public decimal amount { get; set; }
    }
    #endregion
}
