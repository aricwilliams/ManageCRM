using Microsoft.AspNetCore.Mvc;
using ManageCRM.Models;
using ManageCRM.Models.DTO;
using ManageCRM.Data;
using Microsoft.AspNetCore.JsonPatch;

namespace ManageCRM.Controllers
{
    //this route parameter is here so if controller name is changed, still works
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerAPIController : ControllerBase
    {
        //used logger to get details in log file, see log file
        private readonly ILogger<CustomerAPIController> _logger;

        public CustomerAPIController(ILogger<CustomerAPIController> logger)
        {
            _logger = logger;
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        //IEnumerable bc i am returning list of customers
        public ActionResult<IEnumerable<CustomerDTO>> GetCustomers()
        {
            _logger.LogInformation("Gett all customers");
            return Ok(CustomerStore.CustomerList);
        }


        [HttpGet("{id:int}", Name = "GetCustomer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<CustomerDTO> GetCustomer(int id)
        {
            if(id == 0)
            {
                _logger.LogError(" Get customer Error with Id" + id);
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
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<CustomerDTO> CreateCustomer([FromBody] CustomerDTO customerDTO)
        {
            if(CustomerStore.CustomerList.FirstOrDefault(u => u.Name.ToLower() == customerDTO.Name.ToLower()) != null)
            {
                ModelState.AddModelError("CustomErrorMessage", "Customer already exists!");
                return BadRequest(ModelState);
            }
            if (customerDTO == null)
            {
                return BadRequest(customerDTO);
            }
            if (customerDTO.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            //this next line is temporary store of data objects, this err line will remain until
            //i hook up the database.
            //this was done bc it's easier to get the API details done 1st
            customerDTO.Id = CustomerStore.CustomerList.OrderByDescending(u => u.Id).FirstOrDefault().Id + 1;
            CustomerStore.CustomerList.Add(customerDTO);

            return CreatedAtRoute("GetCustomer", new {id = customerDTO.Id },customerDTO);
        }
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("{id:int}", Name = "DeleteCustomer")]
        public IActionResult DeleteCustomer(int id)
        {
            if (id == 0)
            {
                return BadRequest();    
            }
            var customer = CustomerStore.CustomerList.FirstOrDefault(u => u.Id == id);
            if (customer == null)
            {
                return NotFound();
            }
            CustomerStore.CustomerList.Remove(customer);
            return NoContent(); 
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPut("{id:int}", Name = "UpdateCustomer")]
        public IActionResult UpdateCustomer(int id, [FromBody]CustomerDTO customerDTO)
        {
            if (customerDTO == null || id !=customerDTO.Id)
            {
                return BadRequest();
            }
            var customer = CustomerStore.CustomerList.FirstOrDefault(u => u.Id == id);
            customer.Name = customerDTO.Name;
            customer.Address = customerDTO.Address;
            customer.Notes = customerDTO.Notes;

            return NoContent();
        }

        [HttpPatch("{id:int}", Name = "UpdatePartialCustomer")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdatePartialCustomer(int id, JsonPatchDocument<CustomerDTO> patchDTO)
        {
            if (patchDTO == null || id == 0)
            {
                return BadRequest();
            }
            var customer = CustomerStore.CustomerList.FirstOrDefault(u => u.Id == id);

            if (customer == null)
            {
                return BadRequest();
            }
            patchDTO.ApplyTo(customer, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return NoContent();
        }
    }
}

//link operation, where on the list we want the 1st or default where id == id we passed in param
//return type is action rsult 200 for sucess request

//now give the url where the resoucre is create when the user creates the object, "give me the id"