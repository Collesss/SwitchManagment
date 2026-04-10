using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SwitchManagment.API.Db;
using SwitchManagment.API.Db.Entities;
using SwitchManagment.API.Models.Dto.Interface.Request;
using SwitchManagment.API.Models.Dto.Interface.Response;
using SwitchManagment.API.SwitchService.Interfaces;
using System.ComponentModel.DataAnnotations;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SwitchManagment.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [ApiController]
    public class InterfaceController : ControllerBase
    {
        private readonly ILogger<InterfaceController> _logger;
        private readonly IMapper _mapper;
        private readonly ApplicationContext _context;

        public InterfaceController(ILogger<InterfaceController> logger, IMapper mapper, ApplicationContext context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }



        // GET: api/<InterfaceController>
        [HttpGet]
        public IEnumerable<string> GetInterface()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<InterfaceController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<InterfaceResponse>> Get([Range(1, int.MaxValue)][FromRoute] int id)
        {
            if (await _context.Interfaces.FindAsync(id) is InterfaceEntity interfaceEntity)
                return Ok(_mapper.Map<InterfaceResponse>(interfaceEntity));

            return Problem(detail: "Interface with this 'id' not exist.", statusCode: StatusCodes.Status404NotFound);
        }

        // POST api/<InterfaceController>
        [HttpPost]
        public async Task<ActionResult<int>> Post([FromBody] AdminInterfaceCreateRequest interfaceCreate)
        {
            try
            {
                var result = (await _context.Interfaces.AddAsync(_mapper.Map<InterfaceEntity>(interfaceCreate))).Entity;
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetInterface", new { id = result.Id }, result.Id);
            }
            //catch (DbUpdateException e) when (e.InnerException is SqliteException innerException && innerException.Message == "SQLite Error 19: 'UNIQUE constraint failed: Switches.IpOrName'.")
            catch (DbUpdateException e)
            {
                return Problem(detail: "Switch with this 'IpOrName' already exist.", statusCode: StatusCodes.Status409Conflict);
            }
        }


        /*
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }
        */

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                _context.Interfaces.Remove(new InterfaceEntity { Id = id });
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Problem(detail: "Switch with this 'id' not exist.", statusCode: StatusCodes.Status404NotFound);
            }
        }
    }
}
