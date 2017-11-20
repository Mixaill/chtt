using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using chtt.Models;

namespace chtt.Controllers
{
    [Produces("application/json")]
    [Route("api/Conversations")]
    public class ConversationsController : Controller
    {
        private readonly chttContext _context;

        public ConversationsController(chttContext context)
        {
            _context = context;
        }

        // GET: api/Conversations
        [HttpGet]
        public IEnumerable<Conversation> GetRoom()
        {
            return _context.Room;
        }

        // GET: api/Conversations/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetConversation([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var conversation = await _context.Room.SingleOrDefaultAsync(m => m.ConversationId == id);

            if (conversation == null)
            {
                return NotFound();
            }

            return Ok(conversation);
        }

        // PUT: api/Conversations/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutConversation([FromRoute] int id, [FromBody] Conversation conversation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != conversation.ConversationId)
            {
                return BadRequest();
            }

            _context.Entry(conversation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ConversationExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Conversations
        [HttpPost]
        public async Task<IActionResult> PostConversation([FromBody] Conversation conversation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Room.Add(conversation);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetConversation", new { id = conversation.ConversationId }, conversation);
        }

        // DELETE: api/Conversations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteConversation([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var conversation = await _context.Room.SingleOrDefaultAsync(m => m.ConversationId == id);
            if (conversation == null)
            {
                return NotFound();
            }

            _context.Room.Remove(conversation);
            await _context.SaveChangesAsync();

            return Ok(conversation);
        }

        private bool ConversationExists(int id)
        {
            return _context.Room.Any(e => e.ConversationId == id);
        }
    }
}