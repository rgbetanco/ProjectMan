using CSHelper.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace repairman.Repositories
{
    public enum ErrorCode
    {
        [Display(Name = "成功")]
        SUCCESS = 0,

        [Display(Name = "找不到帳號")]
        NOT_FOUND_USER = 1,

        [Display(Name = "帳號未使用")]
        USER_DISABLED = 2,

        [Display(Name = "密碼錯誤")]
        USER_PASSWORD_MISMATCH = 3,



    }
    public class ErrorCodeException : Exception
    {
        public ErrorCode errorCode { get; internal set; }

        public ErrorCodeException(ErrorCode code)
        {
            this.errorCode = code;
        }

        public string GetDisplayName()
        {
            return errorCode.GetDisplayName();
        }
    }

}
