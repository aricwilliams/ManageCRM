using AutoMapper;
using ManageCRM.Models;
using ManageCRM.Models.DTO;

namespace ManageCRM
{
    public class MappingConfig : Profile
    {
        //create constructor 
        public MappingConfig()
        {
            //what will be the source and destination
            CreateMap<Customer, CustomerDTO>();
            CreateMap<CustomerDTO, Customer>();
            CreateMap<Customer, CustomerCreateDTO>().ReverseMap();
            CreateMap<Customer, CustomerUpdateDTO>().ReverseMap();

        }
    }
}
