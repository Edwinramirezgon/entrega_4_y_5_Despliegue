using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TradingJournal.API.Data;
using TradingJournal.API.Helpers;
using TradingJournal.Shared.DTOs;
using TradingJournal.Shared.Entities;

namespace TradingJournal.API.Controllers
{



    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("/api/Pruebacis")]
    public class PruebaciControllers : ControllerBase
    {

        private readonly DataContext _context;

        //Constructor
        public PruebaciControllers(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync([FromQuery] PaginationDTO pagination)
        {

            var queryable = _context.Pruebacis
             .AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }
            return Ok(await queryable
            .OrderBy(x => x.Id)
            .Paginate(pagination)
            .ToListAsync());
        }

        [HttpGet("totalPages")]
        public async Task<ActionResult> GetPages([FromQuery] PaginationDTO pagination)

        {
            var queryable = _context.Pruebacis.AsQueryable();
            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }
            double count = await queryable.CountAsync();
            double totalPages = Math.Ceiling(count / pagination.RecordsNumber);
            return Ok(totalPages);
        }


        //Method Create
        [HttpPost]
        public async Task<ActionResult> PostAsync(Pruebaci pruebaci)
        {
            _context.Add(pruebaci);
            await _context.SaveChangesAsync();
            return Ok(pruebaci);
        }

        //Method Get by ID (Read)
        [HttpGet("{id}")]
        public async Task<ActionResult> GetAsync(string id)
        {
            var pruebaci = await _context.Pruebacis.FirstOrDefaultAsync
                (x => x.Id.Equals(id));

            if (pruebaci == null)
            {
                return NotFound();
            }
            return Ok(pruebaci);
        }

        //Method Update
        [HttpPut]
        public async Task<ActionResult> PutAsync(Pruebaci pruebaci)
        {
            _context.Update(pruebaci);
            await _context.SaveChangesAsync();
            return Ok(pruebaci);
        }

        //Metod Delete
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(string id)
        {
            var pruebaci = await _context.Pruebacis.FirstOrDefaultAsync
                  (x => x.Id.Equals(id));

            if (pruebaci == null)
            {
                return NotFound();
            }
            _context.Remove(pruebaci);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        [AllowAnonymous]
        [HttpGet("combo")]
        public async Task<ActionResult> GetCombo()
        {
            return Ok(await _context.Pruebacis.ToListAsync());
        }
    }
}
