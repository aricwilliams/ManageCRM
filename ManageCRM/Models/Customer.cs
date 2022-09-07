using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManageCRM.Models
{
    public class Customer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Email { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int Phone { get; set; }
        public string Address { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }   
//In the future I will break this down more, will need to bring in diffrent parts of customer
//I want customer to have jobs,pay rates,foreman...each customer will be a weekly/biweekly Job

    }
}
