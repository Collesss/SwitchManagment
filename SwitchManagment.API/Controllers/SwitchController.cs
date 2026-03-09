using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SwitchManagment.API.Db;
using SwitchManagment.API.Db.Entities;
using SwitchManagment.API.Extensions;
using SwitchManagment.API.Models.Dto.Switch;
using SwitchManagment.API.Models.Dto.Switch.Port;
using SwitchManagment.API.Models.Dto.Switch.Request;
using SwitchManagment.API.Models.Dto.Switch.Request.Get;
using SwitchManagment.API.Models.Dto.Switch.Response;
using SwitchManagment.API.Models.Dto.Switch.Response.Admin;
using SwitchManagment.API.Models.Dto.Switch.Response.Get;
using SwitchManagment.API.SwitchService.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace SwitchManagment.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SwitchController : ControllerBase
    {
        private readonly ILogger<SwitchTestController> _logger;
        private readonly IMapper _mapper;
        private readonly ApplicationContext _context;
        //private readonly ISwitchService _switchService;

        public SwitchController(ILogger<SwitchTestController> logger, IMapper mapper, ApplicationContext context, ISwitchService switchService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            //_switchService = switchService ?? throw new ArgumentNullException(nameof(switchService));
        }

        // GET: api/Switch
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SwitchResponse>>> GetSwitches() =>
            Ok(_mapper.Map<IEnumerable<SwitchResponse>>(await _context.Switches.ToListAsync()));


        [HttpGet("getswitches")]
        public async Task<ActionResult<SwitchGetResponse>> GetSwitches1([FromQuery] GetRequest switchGet)
        {
            //Error, this shit cant be translate suka blat, i dont know, try rewrite like my method OrderBy.
            //var filter = _context.Switches.Where(@switch => switchGet.Filters.All(filter => EF.Functions.Like(filter.Key, filter.Value)));

            GetResponse getResponse = _mapper.Map<GetResponse>(switchGet);

            var filter = _context.Switches.Like(switchGet.Filters);

            //using var transaction = await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted)

            int count = await filter.CountAsync();

            getResponse.PageNav.CountElements = count;
            getResponse.PageNav.PageNum = getResponse.PageNav.PageNum > getResponse.PageNav.PageCount ? getResponse.PageNav.PageCount : getResponse.PageNav.PageNum;

            var result = filter
                .OrderBy(getResponse.Sort.Field, getResponse.Sort.IsAscending)
                .Skip((getResponse.PageNav.PageNum - 1) * getResponse.PageNav.PageSize)
                .Take(getResponse.PageNav.PageSize);

            return Ok(new SwitchGetResponse { SwitchGetInfo = getResponse, Switches = _mapper.Map<IEnumerable<SwitchResponse>>(await result.ToListAsync())});
        }


        [HttpGet("admin/getswitches")]
        public async Task<ActionResult<AdminSwitchGetResponse>> AdminGetSwitches([FromQuery] GetRequest switchGet)
        {
            //Error, this shit cant be translate suka blat, i dont know, try rewrite like my method OrderBy.
            //var filter = _context.Switches.Where(@switch => switchGet.Filters.All(filter => EF.Functions.Like(filter.Key, filter.Value)));

            GetResponse getResponse = _mapper.Map<GetResponse>(switchGet);

            var filter = _context.Switches.Like(switchGet.Filters);

            //using var transaction = await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted)

            int count = await filter.CountAsync();

            getResponse.PageNav.CountElements = count;
            getResponse.PageNav.PageNum = getResponse.PageNav.PageNum > getResponse.PageNav.PageCount ? getResponse.PageNav.PageCount : getResponse.PageNav.PageNum;

            var result = filter
                .OrderBy(getResponse.Sort.Field, getResponse.Sort.IsAscending)
                .Skip((getResponse.PageNav.PageNum - 1) * getResponse.PageNav.PageSize)
                .Take(getResponse.PageNav.PageSize);

            return Ok(new AdminSwitchGetResponse { SwitchGetInfo = getResponse, Switches = _mapper.Map<IEnumerable<AdminSwitchResponse>>(await result.ToListAsync()) });
        }


        // GET: api/Switch/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SwitchResponse>> GetSwitch([Range(1, int.MaxValue)][FromRoute]int id) =>
            await _context.Switches.FindAsync(id) is SwitchEntity switchEntity ? Ok(_mapper.Map<SwitchResponse>(switchEntity)) : 
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
        [HttpPost("admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<int>> AdminAddSwitch([FromBody]AdminSwitchCreateRequest switchEntity)
        {
            try
            {
                var addResult = (await _context.Switches.AddAsync(_mapper.Map<SwitchEntity>(switchEntity))).Entity;
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetSwitch", new { id = addResult.Id }, addResult.Id);
            }
            catch(DbUpdateException e) when(e.InnerException is SqliteException innerException && innerException.Message == "SQLite Error 19: 'UNIQUE constraint failed: Switches.IpOrName'.")
            {
                //throw new HttpRequestException("Switch with this 'IpOrName' already exist.", e, System.Net.HttpStatusCode.Conflict);
                return Problem(detail: "Switch with this 'IpOrName' already exist.", statusCode: StatusCodes.Status409Conflict);
            }
        }

        // DELETE: api/Switch/5
        [HttpDelete("admin/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AdminDeleteSwitch([Range(1, int.MaxValue)][FromRoute]int id)
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


        [HttpPut("{id}/port/access")]
        public Task<ActionResult> ConfigurePortAccess([Range(1, int.MaxValue)][FromRoute] int id, ConfigurePortAccessRequest portSetting)
        {

            throw new Exception("Test Exception");
        }


        [HttpPut("{id}/port/trunk")]
        public Task<ActionResult> ConfigurePortTrunk([Range(1, int.MaxValue)][FromRoute] int id, ConfigurePortTrunkRequest portSetting)
        {

            throw new Exception("Test Exception");
        }

        private async Task<bool> SwitchEntityExists(int id) =>
            await _context.Switches.AnyAsync(e => e.Id == id);
    }
}
