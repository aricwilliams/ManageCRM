using Microsoft.AspNetCore.Mvc;
using ManageCRM.Models;
using ManageCRM.Models.DTO;
using ManageCRM.Data;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;

namespace ManageCRM.Controllers
{
    //this route parameter is here so if controller name is changed, still works
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerAPIController : ControllerBase
    {
        //used logger to get details in log file, see log file
        private readonly ILogger<CustomerAPIController> _logger;
        private readonly ApplicationDbContext _db; 

        public CustomerAPIController(ILogger<CustomerAPIController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;   
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        //IEnumerable bc i am returning list of customers
        public ActionResult<IEnumerable<CustomerDTO>> GetCustomers()
        {
            _logger.LogInformation("Gett all customers");
            return Ok(_db.Customers.ToList());
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
            var customer = _db.Customers.FirstOrDefault(u => u.Id == id);
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
            if(_db.Customers.FirstOrDefault(u => u.Name.ToLower() == customerDTO.Name.ToLower()) != null)
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
            //manual conversion
            //Customers expects Customers, cannot implicitly convert customerDTO into Customer
            //mapping the properties
            Customer model = new()
            {
                Id = customerDTO.Id,
                Name = customerDTO.Name,
                Email = customerDTO.Email,
                Phone = customerDTO.Phone,
                Address = customerDTO.Address,
                Notes = customerDTO.Notes

            };
            _db.Customers.Add(model);
            _db.SaveChanges();  

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
            var customer = _db.Customers.FirstOrDefault(u => u.Id == id);
            if (customer == null)
            {
                return NotFound();
            }
            _db.Customers.Remove(customer);
            _db.SaveChanges();
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
            //before we retreve the customer from the db, then update each one. EF Core will figure wich record to
            //update 
            Customer model = new()
            {
                Id = customerDTO.Id,
                Name = customerDTO.Name,
                Email = customerDTO.Email,
                Phone = customerDTO.Phone,
                Address = customerDTO.Address,
                Notes = customerDTO.Notes

            };
            _db.Customers.Update(model);
            _db.SaveChanges();

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
            //when we retrive a record EF is tracking this.. we dont want it to here, so AsNoTracking
            //the customer object below, we are not making changes to that object, we are not saving
            var customer = _db.Customers.AsNoTracking().FirstOrDefault(u => u.Id == id);
            //we only getting part of what needs to be updated 
            //convert customer to CustomerDTO
            CustomerDTO customerDTO = new()
            {
                Id = customer.Id,
                Name = customer.Name,
                Email = customer.Email,
                Phone = customer.Phone,
                Address = customer.Address,
                Notes = customer.Notes

            };

            if (customer == null)
            {
                return BadRequest();
            }
            patchDTO.ApplyTo(customerDTO, ModelState);
            //Any changes we have in PatchDTO that is of type CustomerDTO, that will be applied 
            //to customerDTO , the local variable customerDTO
            //next we update the record 
            //convert customerDTO back to customer

            Customer model = new Customer()
            {
                Id = customerDTO.Id,
                Name = customerDTO.Name,
                Email = customerDTO.Email,
                Phone = customerDTO.Phone,
                Address = customerDTO.Address,
                Notes = customerDTO.Notes

            };
            _db.Customers.Update(model);
            _db.SaveChanges();

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