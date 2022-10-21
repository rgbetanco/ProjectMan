using CSHelper.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;

namespace repairman.Models
{
    public abstract class BaseUser : UsesID
    {
        [Display(Name = "姓名")]
        [Required]
        [MinLength(1)]
        public string name { get; set; }

        [Display(Name = "可使用")]
        [Required]
        public bool enabled { get; set; }

        [Display(Name = "建檔時間")]
        [BindNever]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "yyyy-MM-dd, HH:mm:ss.FFF")]
        public DateTime date_created { get; set; }

        [Display(Name = "使用者")]
        [Required]
        [MinLength(3)]
        public string username { get; set; }

        [Display(Name = "密碼")]
        [BindNever]
        public string pass { get; set; }        // hashed password

        [Display(Name = "備註")]
        public string desc { get; set; }

        [NotMapped]
        [BindProperty(Name = "pass", SupportsGet = false)]
        public string UnencryptedPassword
        {
            set
            {
                SetPassword(value);
            }

            get
            {
                return pass;
            }
        }

        private void SetPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {   // disable use of password if no password is set
                pass = null;
                return;
            }

            byte[] salt = RandomNumberGenerator.GetBytes(16);

            var hash = KeyDerivation.Pbkdf2(
                    password: password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 10000,
                    numBytesRequested: 256 / 8
            );

            byte[] hashBytes = new byte[salt.Length + hash.Length];
            Array.Copy(salt, 0, hashBytes, 0, salt.Length);
            Array.Copy(hash, 0, hashBytes, salt.Length, hash.Length);

            pass = Convert.ToBase64String(hashBytes);
        }

        public bool VerifyPassword(string password)
        {
            if (pass == null)
                return false;   // if password is disabled, can't 

            /* Extract the bytes */
            byte[] hashBytes = Convert.FromBase64String(pass);

            /* Get the salt */
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);

            /* Compute the hash on the password the user entered */
            var hash = KeyDerivation.Pbkdf2(
                    password: password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 10000,
                    numBytesRequested: 256 / 8
            );

            /* Compare the results */
            for (int i = 0; i < hash.Length; i++)
                if (hashBytes[i + 16] != hash[i])
                    return false;

            return true;
        }
    }

}
