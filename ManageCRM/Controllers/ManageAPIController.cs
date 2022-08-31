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
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<CustomerDTO>> GetCustomers()
        {
            return Ok(CustomerStore.CustomerList);
        }


        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<CustomerDTO> CreateCustomer([FromBody] CustomerDTO customerDTO)
        {
            if (customerDTO == null)
            {
                return BadRequest(customerDTO);
            }
            if (customerDTO.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            customerDTO.Id = CustomerStore.CustomerList.OrderByDescending(u => u.Id).FirstOrDefault().Id + 1;
            CustomerStore.CustomerList.Add(customerDTO);

            return Ok(customerDTO);
        }
    }
}

//link operation, where on the list we want the 1st or default where id == id we passed in param
//return type is action rsult 200 for sucess request