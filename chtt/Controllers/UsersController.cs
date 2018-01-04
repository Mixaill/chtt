using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using chtt.Models;
using chtt.Models.UsersViewModels;

namespace chtt.Controllers
{
    [Produces("application/json")]
    [Route("api/Users")]
    [Authorize]
    public class UsersController : Controller
    {
        private readonly chttContext _context;
        private readonly UserManager<User> _userManager;

        public UsersController(chttContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/Users
        /// <summary>
        /// Get list of all users
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 401)]
        public async Task<IActionResult> GetUsers()
        {
            if (User == null)
            {
                return Unauthorized();
            }

            return Ok(await _context.User.Select(x => new GetViewModel(x)).ToListAsync());
        }
        
        // GET: api/Users/<Username>
        /// <summary>
        /// Get data about particular user
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [HttpGet("{username}")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 401)]
        [ProducesResponseType(typeof(void), 404)]
        public async Task<IActionResult> GetUser([FromRoute] string username)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.User.SingleOrDefaultAsync(m => m.UserName == username);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(new GetViewModel(user));
        }

        // PUT: api/Users/username
        /// <summary>
        /// Updates current user last online time
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [HttpPut("{username}")]
        [ProducesResponseType(typeof(void), 204)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 401)]
        [ProducesResponseType(typeof(void), 403)]
        public async Task<IActionResult> PutUser([FromRoute] string username)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currentUser = await _userManager.FindByNameAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (currentUser.NormalizedUserName != username.ToUpper())
            {
                return Forbid();
            }

            var user = await _context.User.Where(x => x.NormalizedUserName == username.ToUpper()).SingleOrDefaultAsync();
            if (user == null)
            {
                return NotFound();
            }

            user.LastOnline = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}