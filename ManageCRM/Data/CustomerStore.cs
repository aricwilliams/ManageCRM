using ManageCRM.Models.DTO;

namespace ManageCRM.Data
{
    public static class CustomerStore
    {
        public static List<CustomerDTO> CustomerList = new List<CustomerDTO>
            {
                new CustomerDTO{Id=1, Name="Toni Boi",Address="123 doug st",Notes="gave us beer"},
                new CustomerDTO{Id=2,Name="Kala Brat",Address="232 doug st",Notes="customer brought cake last time"}
            };
    }
}
