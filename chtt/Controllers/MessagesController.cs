using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

using chtt.Models;
using chtt.Models.MessagesViewModels;

namespace chtt.Controllers
{
    [Produces("application/json")]
    [Route("api/Messages")]
    [Authorize]
    public class MessagesController : Controller
    {
        private readonly chttContext _context;
        private readonly UserManager<User> _userManager;

        public MessagesController(chttContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Get info about particular message
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/Messages/5
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 401)]
        [ProducesResponseType(typeof(void), 403)]
        [ProducesResponseType(typeof(void), 404)]
        public async Task<IActionResult> GetMessage([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var message = await _context.Message.SingleOrDefaultAsync(m => m.MessageId == id);
            if (message == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.FindByNameAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var conversation = await _context.Conversation.SingleOrDefaultAsync(c => c == message.Conversation);
            if (!conversation.Users.Contains(currentUser))
            {
                return Forbid();
            }

            return Ok(new GetViewModel(message));
        }

        // POST: api/Messages
        /// <summary>
        /// Creates message
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(void), 201)]
        [ProducesResponseType(typeof(void), 401)]
        [ProducesResponseType(typeof(void), 403)]
        [ProducesResponseType(typeof(void), 404)]
        public async Task<IActionResult> PostMessage([FromBody] CreateMessageViewModel messageModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currentUser = await _userManager.FindByNameAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var conversation = await _context.Conversation.SingleOrDefaultAsync(c => c.ConversationId == messageModel.ConversationId);
            if (conversation == null)
            {
                return NotFound();
            }

            if (!conversation.Users.Contains(currentUser))
            {
                return Forbid();
            }

            var message = new Message
            {
                Conversation = conversation,
                Content = messageModel.Content,
                Author = currentUser,
            };

            _context.Message.Add(message);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMessage", new { id = message.MessageId }, new GetViewModel(message));
        }

        /// <summary>
        /// Delete message
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE: api/Messages/5
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(void), 204)]
        [ProducesResponseType(typeof(void), 401)]
        [ProducesResponseType(typeof(void), 403)]
        [ProducesResponseType(typeof(void), 404)]
        public async Task<IActionResult> DeleteMessage([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var message = await _context.Message.SingleOrDefaultAsync(m => m.MessageId == id);
            if (message == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.FindByNameAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var conversation = await _context.Conversation.SingleOrDefaultAsync(c => c == message.Conversation);
            if (!conversation.Users.Contains(currentUser))
            {
                return Forbid();
            }

            _context.Message.Remove(message);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
