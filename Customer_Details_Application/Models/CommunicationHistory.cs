using System;
using System.ComponentModel.DataAnnotations;
namespace Customer_Details_Application.Models
{
 
        public class CommunicationHistory
        {
            public int CommunicationId { get; set; }
            [Required(ErrorMessage = "Customer ID is required")]
            public int CustomerId { get; set; }
            [Required(ErrorMessage = "Communication Date is required")]
            public DateTime CommunicationDate { get; set; }
            [Required(ErrorMessage = "Communication Type is required")]
            [StringLength(50, ErrorMessage = "Communication Type must not exceed 50 characters")]
            public string CommunicationType { get; set; }
            [Required(ErrorMessage = "Communication Details is required")]
            public string CommunicationDetails { get; set; }
        }
    
}
