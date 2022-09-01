using System.ComponentModel.DataAnnotations;

namespace ManageCRM.Models.DTO
{
    public class CustomerDTO
    {
        public int Id { get; set; }
        public string Email { get; set; }
        [Required]
        [MaxLength(225)]
        public string Name { get; set; }
        [Required]
        public int Phone { get; set; }
        public string Address { get; set; }
        public string Notes { get; set; }
    }
}

//data annotation we apply to model class, this will serve as validation
