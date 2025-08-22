using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;   // الموديل Message
using PROtofile.Date.DTOS;
using PROtofile.Date.Models;

using static PROtofile.Date.AppDbcontext;          // MessageCreateDto

namespace PROtofile.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly AppDbContext _db;
        public MessagesController(AppDbContext db) => _db = db;

        // GET: api/messages?q=word&page=1&pageSize=20
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Message>>> GetAll(
            [FromQuery] string? q,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0 || pageSize > 200) pageSize = 20;

            var query = _db.Messages.AsNoTracking()
                        .OrderByDescending(x => x.CreatedAt)
                        .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.ToLower();
                query = query.Where(x =>
                    x.FullName.ToLower().Contains(term) ||
                    x.Email.ToLower().Contains(term) ||
                    x.MessageBody.ToLower().Contains(term) ||
                    (x.LocationText ?? "").ToLower().Contains(term));
            }

            var total = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            // ممكن ترجع كائن به بيانات الصفحة لو حابب
            Response.Headers["X-Total-Count"] = total.ToString();
            return Ok(items);
        }

        // GET: api/messages/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Message>> GetOne(int id)
        {
            var m = await _db.Messages.FindAsync(id);
            return m is null ? NotFound() : Ok(m);
        }

        // POST: api/messages  (الموقع ييجي من الفرونت في LocationText)
        [HttpPost]
        public async Task<ActionResult<Message>> Create([FromBody] MessageCreateDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var entity = new Message
            {
                FullName = dto.FullName,
                Email = dto.Email,
                MessageBody = dto.MessageBody,
                LocationText = dto.LocationText, // 👈 ده اللي يظهر في الداشبورد
                // City/Country هنسيبهم null لو مش محتاجينهم
            };

            _db.Messages.Add(entity);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOne), new { id = entity.Id }, entity);
        }

        // PUT: api/messages/5  (تعديل كامل بسيط)
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] MessageCreateDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var entity = await _db.Messages.FindAsync(id);
            if (entity is null) return NotFound();

            entity.FullName = dto.FullName;
            entity.Email = dto.Email;
            entity.MessageBody = dto.MessageBody;
            entity.LocationText = dto.LocationText;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/messages/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _db.Messages.FindAsync(id);
            if (entity is null) return NotFound();

            _db.Messages.Remove(entity);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}