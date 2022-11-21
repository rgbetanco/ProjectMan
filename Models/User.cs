using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using CSHelper.Models;

namespace projectman.Models
{

    [Flags]
    public enum UserPermission : int
    {
        [Display(Name = "資料查詢")]
        Search = 0x1,

        [Display(Name = "資料新增")]
        Add = 0x02,

        [Display(Name = "資料編輯")]
        Edit = 0x04,

        [Display(Name = "紀錄查詢")]
        ReadLog = 0x08,

        [Display(Name = "帳號管理")]
        ManageUser = 0x10,

        [Display(Name = "用戶管理")]
        ManageMember = 0x20,

        [Display(Name = "全部")]
        All = Search | Add | Edit | ReadLog | ManageUser | ManageMember
    }

    public enum PermGroupType : int
    {
        [Display(Name = "登入")]
        Login = 1,

        [Display(Name = "帳號管理")]
        ManageUser = 2,

        [Display(Name = "系統管理")]
        ManageSetting = 3,

        [Display(Name = "用戶管理")]
        ManageMember = 4
    }

    [Index(nameof(username), IsUnique = true)]
    [Table("user")]
    public class User : BaseUser
    {
        [Display(Name = "使用權限")]
        [Required]
        public UserPermission perm { get; set; }

        [Display(Name = "Windows使用者職別碼")]
        public string win_user_sid { get; set; }


        [Display(Name = "密碼錯誤數")]
        public int bad_password_count { get; set; }

        [Display(Name = "群組")]
        public virtual List<UserGroup> groups { get; set; }
        [Display(Name = "業務")]
        public bool isSalePerson { get; set; } = false;
    }

    [Table("group")]
    public class Group : UsesID
    {
        [Display(Name = "名稱")]
        [Required]
        [MinLength(1)]
        public string name { get; set; }

        [Display(Name = "建檔時間")]
        [BindNever]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "yyyy-MM-dd, HH:mm:ss.FFF")]
        public DateTime date_created { get; set; }

        [Display(Name = "備註")]
        public string desc { get; set; }
    }

    [Table("user_group")]
    public class UserGroup : UsesID
    {

        [ForeignKey("user_id")]
        public virtual User user { get; set; }
        
        public long user_id { get; set; }

        [ForeignKey("group_id")]
        public virtual Group group { get; set; }

        public long group_id { get; set; }
    }

    [Table("perm_group")]
    public class PermGroup : UsesID
    {
        // either group_id or win_group_id must be defined
        public PermGroupType type { get; set; }

        [ForeignKey("group_id")]
        public virtual Group group { get; set; }

        public long? group_id { get; set; }

        [Display(Name = "Windows群組職別碼")]
        public string win_group_sid { get; set; }
    }

}

