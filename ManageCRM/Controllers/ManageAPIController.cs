using Microsoft.AspNetCore.Mvc;
using ManageCRM.Models;
using ManageCRM.Models.DTO;
using ManageCRM.Data;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

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
        private readonly IMapper _mapper;

        public CustomerAPIController(ILogger<CustomerAPIController> logger, ApplicationDbContext db, IMapper mapper)
        {
            _logger = logger;
            _db = db;   
            _mapper = mapper;   
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        //IEnumerable bc i am returning list of customers
        public async Task<ActionResult<IEnumerable<CustomerDTO>>> GetCustomers()
        {
            _logger.LogInformation("Gett all customers");
            IEnumerable<Customer> customerList = await _db.Customers.ToListAsync();
            //use mapper functon, then pass CustomerDTO as destination then the onject
            return Ok(_mapper.Map<List<CustomerDTO>>(customerList));
        }


        [HttpGet("{id:int}", Name = "GetCustomer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CustomerDTO>> GetCustomer(int id)
        {
            if(id == 0)
            {
                _logger.LogError(" Get customer Error with Id" + id);
                return BadRequest();
            }
            var customer = await _db.Customers.FirstOrDefaultAsync(u => u.Id == id);
            if (customer == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<CustomerDTO>(customer));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task< ActionResult<CustomerDTO>> CreateCustomer([FromBody] CustomerCreateDTO createDTO)
        {
            if( await _db.Customers.FirstOrDefaultAsync(u => u.Name.ToLower() == createDTO.Name.ToLower()) != null)
            {
                ModelState.AddModelError("CustomErrorMessage", "Customer already exists!");
                return BadRequest(ModelState);
            }
            if (createDTO == null)
            {
                return BadRequest(createDTO);
            }
           
            //this next one line will replace lines below, output is Customer, input is createDTO
            Customer model = _mapper.Map<Customer>(createDTO);
            //manual conversion
            //Customers expects Customers, cannot implicitly convert customerDTO into Customer
            //mapping the properties
            //Customer model = new()
            //{
            //    Name = createDTO.Name,
            //    Email = createDTO.Email,
            //    Phone = createDTO.Phone,
            //    Address = createDTO.Address,
            //    Notes = createDTO.Notes

            //};
           await _db.Customers.AddAsync(model);
           await _db.SaveChangesAsync();  
            //EF will keep track of the customer id inside model
            //now we return the model
            return CreatedAtRoute("GetCustomer", new {id = model.Id },model);
        }
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("{id:int}", Name = "DeleteCustomer")]
        public async Task< IActionResult> DeleteCustomer(int id)
        {
            if (id == 0)
            {
                return BadRequest();    
            }
            var customer = await _db.Customers.FirstOrDefaultAsync(u => u.Id == id);
            if (customer == null)
            {
                return NotFound();
            }
            _db.Customers.Remove(customer);
            await _db.SaveChangesAsync();
            return NoContent(); 
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPut("{id:int}", Name = "UpdateCustomer")]
        public async Task<IActionResult> UpdateCustomer(int id, [FromBody]CustomerUpdateDTO updateDTO)
        {
            if (updateDTO == null || id != updateDTO.Id)
            {
                return BadRequest();
            }

            Customer model = _mapper.Map<Customer>(updateDTO);

            //before we retreve the customer from the db, then update each one. EF Core will figure wich record to
            //update 
            //Customer model = new()
            //{
            //    Id = updateDTO.Id,
            //    Name = updateDTO.Name,
            //    Email = updateDTO.Email,
            //    Phone = updateDTO.Phone,
            //    Address = updateDTO.Address,
            //    Notes = updateDTO.Notes

            //};
            _db.Customers.Update(model);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id:int}", Name = "UpdatePartialCustomer")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdatePartialCustomer(int id, JsonPatchDocument<CustomerUpdateDTO> patchDTO)
        {
            if (patchDTO == null || id == 0)
            {
                return BadRequest();
            }
            //when we retrive a record EF is tracking this.. we dont want it to here, so AsNoTracking
            //the customer object below, we are not making changes to that object, we are not saving
            var customer = await _db.Customers.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
            //we only getting part of what needs to be updated 
            //convert customer to CustomerDTO
            //now we update to CustomerUpdateDTO, apply patch, update to Customer 
            //CustomerUpdateDTO customerDTO = new()
            //{
            //    Id = customer.Id,
            //    Name = customer.Name,
            //    Email = customer.Email,
            //    Phone = customer.Phone,
            //    Address = customer.Address,
            //    Notes = customer.Notes

            //};

            CustomerUpdateDTO customerDTO = _mapper.Map<CustomerUpdateDTO>(customer);

            if (customer == null)
            {
                return BadRequest();
            }
            patchDTO.ApplyTo(customerDTO, ModelState);
            //Any changes we have in PatchDTO that is of type CustomerDTO, that will be applied 
            //to customerDTO , the local variable customerDTO
            //next we update the record 
            //convert customerDTO back to customer

            //Customer model = new Customer()
            //{
            //    Id = customerDTO.Id,
            //    Name = customerDTO.Name,
            //    Email = customerDTO.Email,
            //    Phone = customerDTO.Phone,
            //    Address = customerDTO.Address,
            //    Notes = customerDTO.Notes

            //};

            Customer model = _mapper.Map<Customer>(customerDTO);


            _db.Customers.Update(model);
            await _db.SaveChangesAsync();

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