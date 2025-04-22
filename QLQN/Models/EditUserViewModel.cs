using System;
using System.ComponentModel.DataAnnotations;

namespace QLQN.Models
{
    public class EditUserViewModel
    {
        [Required]
        public int Id { get; set; } // Id của User

        [Display(Name = "Họ tên")]
        public string FullName { get; set; }

        [Display(Name = "Số điện thoại")]
        public string Phone { get; set; }

       

    }
}
