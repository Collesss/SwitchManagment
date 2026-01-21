using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SwitchManagment.API.Db;
using SwitchManagment.API.Db.Entities;
using SwitchManagment.API.Extensions;
using SwitchManagment.API.Models.Dto.Switch;
using SwitchManagment.API.Models.Dto.Switch.Request;
using SwitchManagment.API.Models.Dto.Switch.Response;
using System.ComponentModel.DataAnnotations;

namespace SwitchManagment.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SwitchController : ControllerBase
    {
        private readonly ILogger<SwitchTestController> _logger;
        private readonly IMapper _mapper;
        private readonly ApplicationContext _context;

        public SwitchController(ILogger<SwitchTestController> logger, IMapper mapper, ApplicationContext context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // GET: api/Switch
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SwitchAnnotationResponse>>> GetSwitches() =>
            Ok(_mapper.Map<IEnumerable<SwitchAnnotationResponse>>(await _context.Switches.ToListAsync()));


        [HttpGet("getswitches")]
        public async Task<ActionResult<SwitchGetAnnotationResponse>> GetSwitches1([FromQuery] SwitchGet switchGet)
        {
            //Error, this shit cant be translate suka blat, i dont know, try rewrite like my method OrderBy.
            //var filter = _context.Switches.Where(@switch => switchGet.Filters.All(filter => EF.Functions.Like(filter.Key, filter.Value)));

            var filter = _context.Switches.Like(switchGet.Filters);

            int count = await filter.CountAsync();

            switchGet.PageNav.CountElements = count;

            switchGet.PageNav.PageNum = switchGet.PageNav.PageNum > switchGet.PageNav.PageCount ? switchGet.PageNav.PageCount : switchGet.PageNav.PageNum;

            var result = filter
                .OrderBy(switchGet.Sort.Field, switchGet.Sort.IsAscending)
                .Skip((switchGet.PageNav.PageNum - 1) * switchGet.PageNav.PageSize)
                .Take(switchGet.PageNav.PageSize);

            return Ok(new SwitchGetAnnotationResponse { SwitchGetInfo = switchGet, Switches = _mapper.Map<IEnumerable<SwitchAnnotationResponse>>(await result.ToListAsync())});
        }
        

        // GET: api/Switch/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SwitchAnnotationResponse>> GetSwitch([Range(1, int.MaxValue)][FromRoute]int id) =>
            await _context.Switches.FindAsync(id) is SwitchEntity switchEntity ? Ok(_mapper.Map<SwitchAnnotationResponse>(switchEntity)) : 
            Problem(detail: "Switch with this 'id' not exist.", statusCode: StatusCodes.Status404NotFound);


        // PUT: api/Switch/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /*
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSwitchEntity(int id, SwitchEntity switchEntity)
        {
            if (id != switchEntity.Id)
            {
                return BadRequest();
            }

            _context.Entry(switchEntity).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SwitchEntityExists(id))
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
        */

        // POST: api/Switch
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<int>> PostSwitch([FromBody]SwitchCreateRequest switchEntity)
        {
            try
            {
                var addResult = (await _context.Switches.AddAsync(_mapper.Map<SwitchEntity>(switchEntity))).Entity;
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetSwitch", new { id = addResult.Id }, addResult.Id);
            }
            catch(DbUpdateException e) when(e.InnerException is SqliteException innerException && innerException.Message == "SQLite Error 19: 'UNIQUE constraint failed: Switches.IpOrName'.")
            {
                return Problem(detail: "Switch with this 'IpOrName' already exist.", statusCode: StatusCodes.Status409Conflict);
            }
        }

        // DELETE: api/Switch/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteSwitch([Range(1, int.MaxValue)][FromRoute]int id)
        {
            try
            {
                _context.Switches.Remove(new SwitchEntity { Id = id });
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Problem(detail: "Switch with this 'id' not exist.", statusCode: StatusCodes.Status404NotFound);
            }
        }

        private bool SwitchEntityExists(int id)
        {
            return _context.Switches.Any(e => e.Id == id);
        }
    }
}
