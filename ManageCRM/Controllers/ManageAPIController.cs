using Microsoft.AspNetCore.Mvc;
using ManageCRM.Models;
using ManageCRM.Models.DTO;
using ManageCRM.Data;

namespace ManageCRM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManageAPIController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<CustomerDTO>> GetCustomers()
        {
            return Ok(CustomerStore.CustomerList);
        }


        [HttpGet("id")]
        public ActionResult<CustomerDTO> GetCustomer(int id)
        {
            if(id == 0)
            {
                return BadRequest();
            }
            var customer = CustomerStore.CustomerList.FirstOrDefault(u => u.Id == id);
            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }
    }
}

//link operation, where on the list we want the 1st or default where id == id we passed in param
//return type is action rsult 200 for sucess request