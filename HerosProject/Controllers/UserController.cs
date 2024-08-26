using HerosProject.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HerosProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DataContext _dataContext;

        public UserController(DataContext datacontext)
        {
            _dataContext = datacontext;
        }
        [Authorize]
        [HttpGet]
        [Route("users")]
        public async Task<IActionResult> getUser()
        {
            var userData = await _dataContext.users.ToListAsync();
            if (userData == null) { 
            return NotFound("Record not found");
            }

            return Ok(userData);
        }

        [HttpPost]
        [Route("users")]
        public async Task<IActionResult> crateUser([FromQuery] User user)
        {
             var result = await _dataContext.users.AddAsync(user);
             await _dataContext.SaveChangesAsync();
            return Ok(user);
        }

        [HttpGet]
        [Route("users/{id}")]
        public async Task<IActionResult> GetUserByID(int id)
        {

            var result = await _dataContext.users.FindAsync(id);
            if (result == null) {
                return BadRequest("Record not found");
            }

            return Ok(result);
        }

        [HttpPut]
        [Route("users/{id}")]
        public async Task<IActionResult> UpdateUase([FromForm] User user, int id)
        {
            var result = await _dataContext.users.FindAsync(id);
            if (result == null)
            {
                return NotFound("User recod not found");
            }
            else
            {
                result.Email = user.Email;
                result.LastName = user.LastName;
                result.FristName = user.FristName;
                result.Uname = user.Uname;
                await _dataContext.SaveChangesAsync();
            }
            return Ok(result);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                // Find the user by ID
                var user = await _dataContext.users.FindAsync(id);

                if (user == null)
                {
                    return NotFound("User not found."); // Use consistent lowercase for clarity
                }

                // Efficiently remove the user (assuming a single user entity)
                _dataContext.users.Remove(user);

                // Save changes to the database
                await _dataContext.SaveChangesAsync();

                return Ok();
            }
            catch (DbUpdateException ex)
            {
                // Handle database update exceptions (e.g., constraint violations)
                return BadRequest(new { message = "An error occurred while deleting the user. Please ensure there are no dependent entities or referential integrity issues.", exception = ex.ToString() }); // Log exception details for debugging (remove in production)
            }
            catch (Exception ex) // Catch other unexpected exceptions
            {
                // Log the exception and return a generic error message
                // (consider using a dedicated logging framework)
                Console.WriteLine($"An unexpected error occurred: {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the user.");
            }
        }




    }
}
