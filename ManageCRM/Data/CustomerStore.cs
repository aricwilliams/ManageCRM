using ManageCRM.Models.DTO;

namespace ManageCRM.Data
{
    public static class CustomerStore
    {
        public static List<CustomerDTO> CustomerList = new List<CustomerDTO>
            {
                new CustomerDTO{Id=1, Name="Toni Boi"},
                new CustomerDTO{Id=2,Name="Kala Brat"}
            };
    }
}
