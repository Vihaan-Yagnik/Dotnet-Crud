using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Admin3.Models
{
    public class UserModel
    {
        public int UserID { get; set; }
        [Required]
        [StringLength(100,ErrorMessage ="Please Enter A valid name")]
        public string UserName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [Phone]
        public string MobileNo { get; set; }
        [Required]
        public string Address { get; set; }
        public bool IsActive { get; set; }
    }
}
