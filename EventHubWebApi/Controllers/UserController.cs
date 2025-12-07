using EventHubLibrary.Models;
using EventHubLibrary.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EventHubWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserRepository _userRepo;
        private readonly ILogger<UsersController> _logger;

        public UsersController(UserRepository userRepo, ILogger<UsersController> logger)
        {
            _userRepo = userRepo;
            _logger = logger;
        }

        // GET: api/users
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userRepo.GetAllAsync();
                _logger.LogInformation("Fetched all users, Count={Count}", users.Count());
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all users");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/users/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                var user = await _userRepo.GetByIdAsync(id);
                if (user == null)
                {
                    _logger.LogWarning("User not found: UserId={UserId}", id);
                    return NotFound();
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user: UserId={UserId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: api/users
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            try
            {
                await _userRepo.AddAsync(user);
                _logger.LogInformation("User created: UserId={UserId}", user.UserId);
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT: api/users/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User updatedUser)
        {
            try
            {
                var user = await _userRepo.GetByIdAsync(id);
                if (user == null)
                {
                    _logger.LogWarning("User not found for update: UserId={UserId}", id);
                    return NotFound();
                }

                // Update fields
                user.UserName = updatedUser.UserName;
                user.Email = updatedUser.Email;
                user.PhoneNumber = updatedUser.PhoneNumber;

                await _userRepo.UpdateAsync(user);
                _logger.LogInformation("User updated: UserId={UserId}", id);

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user: UserId={UserId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE: api/users/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var user = await _userRepo.GetByIdAsync(id);
                if (user == null)
                {
                    _logger.LogWarning("User not found for deletion: UserId={UserId}", id);
                    return NotFound();
                }

                await _userRepo.DeleteAsync(id);
                _logger.LogInformation("User deleted: UserId={UserId}", id);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user: UserId={UserId}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
