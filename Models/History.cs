using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace repairman.Models
{
    public class History
    {
        public enum ActionType : short
        {
            [Display(Name = "瀏覽")]
            View = 1,

            [Display(Name = "搜尋")]
            Search = 2,

            [Display(Name = "新增")]
            Add = 3,

            [Display(Name = "修改")]
            Update = 4,

            [Display(Name = "刪除")]
            Delete = 5,

            [Display(Name = "匯入")]
            Import = 6,

            [Display(Name = "登入")]
            Login = 7
        }
    }
}
