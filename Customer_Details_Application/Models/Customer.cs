using System.ComponentModel.DataAnnotations;

namespace Customer_Details_Application.Models
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }
        [Required]
        public string? FirstName { get; set; }
        [Required]
        public string? LastName { get; set; }
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? PhoneNumber { get; set; }

    }
}
//using Microsoft.AspNetCore.Mvc;
//using System.ComponentModel.DataAnnotations;
//namespace Customer_Details_Application.Models
//{
//    public class Customer
//    {
//        [Key]
//        public int CustomerId { get; set; }

//        [Required(ErrorMessage = "First Name is required.")]
//        public string FirstName { get; set; }

//        [Required(ErrorMessage = "Last Name is required.")]
//        public string LastName { get; set; }

//        [Required(ErrorMessage = "Email is required.")]
//        public string Email { get; set; }

//        [Required(ErrorMessage = "Phone Number is required.")]
//         public string PhoneNumber { get; set; }
//    }
//}