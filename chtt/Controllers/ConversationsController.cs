using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using chtt.Models;
using chtt.Models.ConversationsViewModels;

namespace chtt.Controllers
{
    [Produces("application/json")]
    [Route("api/Conversations")]
    [Authorize]
    public class ConversationsController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly chttContext _context;

        public ConversationsController(chttContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/Conversations
        /// <summary>
        /// Gets list of all conversations
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 401)]
        public async Task<IActionResult> GetConversations()
        {
            if (User == null)
            {
                return Unauthorized();
            }
            var currentUser = await _userManager.FindByNameAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var conversations = new List<GetViewModel>();

            var query = _context.Conversation.Include(x => x.Author).Include("ConversationUsers.User");
            await query.LoadAsync();

            foreach (var conversation in query.Where(x => x.Users.Any(y => y.Id == currentUser.Id)))
            {
                conversations.Add(new GetViewModel(conversation));
            }

            return Ok(conversations);
        }

        // GET: api/Conversations/5
        /// <summary>
        /// Get particular conversation details
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 401)]
        [ProducesResponseType(typeof(void), 403)]
        [ProducesResponseType(typeof(void), 404)]
        public async Task<IActionResult> GetConversation([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var conversation = await _context.Conversation.Include("ConversationUsers.User").SingleOrDefaultAsync(m => m.ConversationId == id);

            if (conversation == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.FindByNameAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (!conversation.Users.Contains(currentUser))
            {
                return Forbid();
            }

            return Ok(new GetViewModel(conversation));
        }

        // PUT: api/Conversations/5
        /// <summary>
        /// Update particular conversation info
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(void), 204)]
        [ProducesResponseType(typeof(void), 401)]
        [ProducesResponseType(typeof(void), 403)]
        [ProducesResponseType(typeof(void), 404)]
        public async Task<IActionResult> PutConversation([FromRoute] int id, [FromBody] UpdateViewModel conversationViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != conversationViewModel.ConversationId)
            {
                return BadRequest();
            }

            if (!_context.Conversation.Any(e => e.ConversationId == id))
            {
                return NotFound();
            }

            var currentUser = await _userManager.FindByNameAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var conversation = await _context.Conversation.Include("ConversationUsers.User").Where(x => x.ConversationId == id).SingleOrDefaultAsync();
            if (conversation==null)
            {
                return NotFound();
            }

            if (conversation.Users.All(x => x.Id != currentUser.Id))
            {
                return Forbid();
            }

            conversation.Description = conversationViewModel.Description;
            conversation.Name = conversationViewModel.Name;

            //add new users
            foreach (var username in conversationViewModel.Users)
            {
                if (conversation.Users.All(x => x.UserName != username))
                {
                    var user = await _context.User.Where(x => x.UserName == username).SingleOrDefaultAsync();
                    if (user != null)
                        conversation.Users.Add(user);
                }
            }

            //remove users
            if (conversation.Author.UserName == currentUser.UserName)
            {
                foreach (var user in conversation.Users)
                {
                    if (!conversationViewModel.Users.Contains(user.UserName))
                    {
                        conversation.Users.Remove(user);
                    }
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persist contact us.");
                return BadRequest(ModelState);
            }
            return NoContent();
        }

        // POST: api/Conversations
        /// <summary>
        /// Creates new conversation
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 401)]
        public async Task<IActionResult> PostConversation([FromBody] CreateConversationViewModel createConversationViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currentUser = await _userManager.FindByNameAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var conversation = new Conversation
            {
                Name = createConversationViewModel.Name,
                Description = createConversationViewModel.Description,
                Author = currentUser,
            }; 

            _context.Conversation.Add(conversation);

            _context.ConversationUser.Add(new ConversationUser { Conversation = conversation, User = currentUser });
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetConversation", new { id = conversation.ConversationId }, new GetViewModel(conversation));
        }

        // DELETE: api/Conversations/5
        /// <summary>
        /// Deletes conversation
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 401)]
        [ProducesResponseType(typeof(void), 403)]
        [ProducesResponseType(typeof(void), 404)]
        public async Task<IActionResult> DeleteConversation([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var conversation = await _context.Conversation.SingleOrDefaultAsync(m => m.ConversationId == id);
            if (conversation == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.FindByNameAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (conversation.Author.Id != currentUser.Id)
            {
                return Forbid();
            }

            _context.Conversation.Remove(conversation);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        /// <summary>
        /// Get list of Ids of all messages
        /// </summary>
        // GET: api/Conversation/5/Messages
        [HttpGet("{id}/Messages")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 401)]
        [ProducesResponseType(typeof(void), 403)]
        [ProducesResponseType(typeof(void), 404)]
        public async Task<IActionResult> GetConversationMessages([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            var currentUser = await _userManager.FindByNameAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var conversation = await _context.Conversation.SingleOrDefaultAsync(c => c.ConversationId == id);
            if (conversation == null)
            {
                return NotFound();
            }

            if (!conversation.Users.Contains(currentUser))
            {
                return Forbid();
            }

            var messagesIds = await _context.Message.Where(y => y.Conversation == conversation).Select(x => x.MessageId).ToListAsync();

            return Ok(messagesIds);
        }

    }
}
