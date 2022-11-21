using CSHelper.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace projectman.Models
{

    public enum ProductCategory : int
    {
        [Display(Name = "未定")]
        Unknown = 0,

        [Display(Name = "軟體授權")]
        Software = 1,

        [Display(Name = "硬體")]
        Hardware = 2,

        [Display(Name = "服務合約")]
        Service = 3,

        [Display(Name = "消耗品")]
        Accessory = 4,
    }

    [Table("product")]
    public class Product : UsesID
    {
        [Display(Name = "商品類別")]
        public ProductCategory category { get; set; }

        [Display(Name = "品牌")]
        [ForeignKey("brand_id")]
        public virtual ProductBrand brand { get; set; }

        [Display(Name = "品牌")]
        public long brand_id { get; set; }

        [Display(Name = "型號")]
        public string model_name { get; set; }

        [Display(Name = "品名")]
        public string name { get; set; }

        [Display(Name = "說明")]
        public string desc { get; set; }

        [Display(Name = "商品編號")]
        public string code { get; set; }
    }

    [Table("product_brand")]
    public class ProductBrand : UsesID
    {
        [Display(Name = "商牌")]
        public string name { get; set; }
    }

}
