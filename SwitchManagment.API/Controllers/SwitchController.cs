using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SwitchManagment.API.Db;
using SwitchManagment.API.Db.Entities;
using SwitchManagment.API.Db.Entities.ACL.AccessMasks;
using SwitchManagment.API.Extensions;
using SwitchManagment.API.Models.Dto.Switch.Port;
using SwitchManagment.API.Models.Dto.Switch.Request;
using SwitchManagment.API.Models.Dto.Switch.Request.Get;
using SwitchManagment.API.Models.Dto.Switch.Response;
using SwitchManagment.API.Models.Dto.Switch.Response.Admin;
using SwitchManagment.API.Models.Dto.Switch.Response.Get;
using SwitchManagment.API.Models.Dto.Switch.Response.Port;
using SwitchManagment.API.SwitchService.Data;
using SwitchManagment.API.SwitchService.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SwitchManagment.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [ApiController]
    public class SwitchController : ControllerBase
    {
        private readonly ILogger<SwitchTestController> _logger;
        private readonly IMapper _mapper;
        private readonly ApplicationContext _context;
        private readonly ISwitchService _switchService;
        private readonly IDataProtector _dataProtector;


        public SwitchController(ILogger<SwitchTestController> logger, IMapper mapper, ApplicationContext context, ISwitchService switchService, IDataProtectionProvider dataProtectorProvider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _switchService = switchService ?? throw new ArgumentNullException(nameof(switchService));
            _dataProtector = dataProtectorProvider.CreateProtector("SwitchController") ?? throw new ArgumentNullException(nameof(dataProtectorProvider));
        }

        // GET: api/Switch
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SwitchResponse>>> GetSwitches() =>
            Ok(_mapper.Map<IEnumerable<SwitchResponse>>(await _context.Switches.ToListAsync()));


        private async Task<(GetResponse getResponse, IEnumerable<SwitchEntity>)> GetSwitchesBase(GetRequest switchGet)
        {
            GetResponse getResponse = _mapper.Map<GetResponse>(switchGet);

            var filter = _context.Switches.Like(switchGet.Filters);

            int count = await filter.CountAsync();

            getResponse.PageNav.CountElements = count;
            getResponse.PageNav.PageNum = getResponse.PageNav.PageNum > getResponse.PageNav.PageCount ? getResponse.PageNav.PageCount : getResponse.PageNav.PageNum;

            var result = await filter
                .OrderBy(getResponse.Sort.Field, getResponse.Sort.IsAscending)
                .Skip((getResponse.PageNav.PageNum - 1) * getResponse.PageNav.PageSize)
                .Take(getResponse.PageNav.PageSize)
                .ToListAsync();

            return (getResponse, result);
        }


        [HttpGet("getswitches")]
        public async Task<ActionResult<SwitchGetResponse>> GetSwitches1([FromQuery] GetRequest switchGet)
        {
            (GetResponse getResponse, IEnumerable<SwitchEntity> switches) = await GetSwitchesBase(switchGet);

            return Ok(new SwitchGetResponse { SwitchGetInfo = getResponse, Switches = _mapper.Map<IEnumerable<SwitchResponse>>(switches) });
        }


        [HttpGet("admin/getswitches")]
        public async Task<ActionResult<AdminSwitchGetResponse>> AdminGetSwitches([FromQuery] GetRequest switchGet)
        {
            (GetResponse getResponse, IEnumerable<SwitchEntity> switches) = await GetSwitchesBase(switchGet);

            return Ok(new AdminSwitchGetResponse { SwitchGetInfo = getResponse, Switches = _mapper.Map<IEnumerable<AdminSwitchResponse>>(switches) });
        }


        /*
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SwitchWithPortsResponse>> GetSwitch([Range(1, int.MaxValue)][FromRoute] int id)
        {
            SwitchEntity @switch = await _context.Switches.FindAsync(id) is SwitchEntity switchEntity ? Ok(_mapper.Map<SwitchResponse>(switchEntity)) :
                Problem(detail: "Switch with this 'id' not exist.", statusCode: StatusCodes.Status404NotFound);

        }
        */

        private ConnectConfig GetConnectConfig(SwitchEntity switchEntity) =>
            new ConnectConfig
            {
                IpOrName = switchEntity.IpOrName,
                Login = switchEntity.Login,
                Password = _dataProtector.Unprotect(switchEntity.EncryptedPassword),
                SuperPassword = _dataProtector.Unprotect(switchEntity.EncryptedSuperPassword)
            };

        private SwitchWithPortsResponse GetSwitchWithPortsResponse(SwitchEntity switchEntity, SwitchInfo switchInfo)
        {
            SwitchWithPortsResponse switchWithPortsResponse = _mapper.Map<SwitchWithPortsResponse>(switchEntity);

            switchWithPortsResponse.Ports = _mapper.Map<IEnumerable<PortResponse>>(switchInfo.Ports);
            switchWithPortsResponse.VlansInfo = _mapper.Map<IEnumerable<VlanResponse>>(switchInfo.Vlans);

            return switchWithPortsResponse;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SwitchWithPortsResponse>> GetSwitch([Range(1, int.MaxValue)][FromRoute]int id)
        {
            if(await _context.Switches.FindAsync(id) is SwitchEntity switchEntity)
            {
                var switchInfo = await _switchService.GetSwitchInfo(GetConnectConfig(switchEntity));

                return GetSwitchWithPortsResponse(switchEntity, switchInfo);
            }

            return Problem(detail: "Switch with this 'id' not exist.", statusCode: StatusCodes.Status404NotFound);
        }

        private string GetHash(string str) =>
            Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(str)));

        private AdminSwitchWithPortsResponse AdminGetSwitchWithPortsResponse(SwitchEntity switchEntity, SwitchInfo switchInfo)
        {
            AdminSwitchWithPortsResponse switchWithPortsResponse = _mapper.Map<AdminSwitchWithPortsResponse>(switchEntity);
            switchWithPortsResponse.HashPassword = GetHash(_dataProtector.Unprotect(switchEntity.EncryptedPassword));
            switchWithPortsResponse.HashSuperPassword = GetHash(_dataProtector.Unprotect(switchEntity.EncryptedSuperPassword));

            switchWithPortsResponse.Ports = _mapper.Map<IEnumerable<PortResponse>>(switchInfo.Ports);
            switchWithPortsResponse.VlansInfo = _mapper.Map<IEnumerable<VlanResponse>>(switchInfo.Vlans);

            return switchWithPortsResponse;
        }

        [HttpGet("admin/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AdminSwitchWithPortsResponse>> AdminGetSwitch([Range(1, int.MaxValue)][FromRoute] int id)
        {
            if (await _context.Switches.FindAsync(id) is SwitchEntity switchEntity)
            {
                var switchInfo = await _switchService.GetSwitchInfo(GetConnectConfig(switchEntity));

                return AdminGetSwitchWithPortsResponse(switchEntity, null);
            }

            return Problem(detail: "Switch with this 'id' not exist.", statusCode: StatusCodes.Status404NotFound);
        }



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

        [HttpPost("admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<int>> AdminAddSwitch([FromBody]AdminSwitchCreateRequest switchCreateRequest)
        {
            try
            {
                SwitchEntity switchEntity = _mapper.Map<SwitchEntity>(switchCreateRequest);
                switchEntity.EncryptedPassword = _dataProtector.Protect(switchCreateRequest.Password);
                switchEntity.EncryptedSuperPassword = _dataProtector.Protect(switchCreateRequest.SuperPassword);


                var addResult = (await _context.Switches.AddAsync(switchEntity)).Entity;
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetSwitch", new { id = addResult.Id }, addResult.Id);
            }
            catch(DbUpdateException e) when(e.InnerException is SqliteException innerException && innerException.Message == "SQLite Error 19: 'UNIQUE constraint failed: Switches.IpOrName'.")
            {
                //throw new HttpRequestException("Switch with this 'IpOrName' already exist.", e, System.Net.HttpStatusCode.Conflict);
                return Problem(detail: "Switch with this 'IpOrName' already exist.", statusCode: StatusCodes.Status409Conflict);
            }
        }


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

        private async Task<SwitchEntity> GetSwitchWithIntfAndACL(int id) =>
            await _context.Switches
                .Include(sw => sw.Interfaces)
                .ThenInclude(i => i.ACLVlans)
                .FirstOrDefaultAsync(sw => sw.Id == id);

        private PortConfigAccess GetPortConfigAccess(SwitchEntity switchEntity, InterfaceEntity interfaceEntity, ConfigurePortAccessRequest portSetting)
        {
            PortConfigAccess portConfigAccess = _mapper.Map<PortConfigAccess>(switchEntity);
            portConfigAccess.Password = _dataProtector.Unprotect(switchEntity.EncryptedPassword);
            portConfigAccess.SuperPassword = _dataProtector.Unprotect(switchEntity.EncryptedSuperPassword);
            
            _mapper.Map(interfaceEntity, portConfigAccess);
            _mapper.Map(portSetting, portConfigAccess);

            return portConfigAccess;
        }

        [HttpPut("{id}/port/{portId}/access")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> ConfigurePortAccess([Range(1, int.MaxValue)][FromRoute] int id, [Range(1, int.MaxValue)][FromRoute] int portId, ConfigurePortAccessRequest portSetting)
        {
            string[] groupsSid = User.Claims
                .Where(claim => claim.Type == ClaimTypes.GroupSid)
                .Select(claim => claim.Value)
                .ToArray();

            var ACEVlanOnInterfaces = await _context.ACEVlanOnInterfaces
                .Include(aceVlOnIf => aceVlOnIf.Interface)
                .ThenInclude(@if => @if.Switch)
                .FirstOrDefaultAsync(ace => ace.Vlan == portSetting.Vlan && ace.IdOnSwitch == portId && ace.SwitchId == id && ace.AccessMask.HasFlag(AccessMaskVlanOnInterface.WriteAccess) && groupsSid.Contains(ace.GroupSID));

            if(ACEVlanOnInterfaces is not null)
            {
                await _switchService.ConfigurePort(GetPortConfigAccess(ACEVlanOnInterfaces.Interface.Switch, ACEVlanOnInterfaces.Interface, portSetting));

                return NoContent();
            }

            return Problem(detail: "No access on write for this vlan.", statusCode: StatusCodes.Status403Forbidden);

            /*
            SwitchEntity switchEntity = await GetSwitchWithIntfAndACL(id);

            //if (await _context.Switches.FindAsync(id) is SwitchEntity switchEntity)
            if (switchEntity is not null)
            {
                InterfaceEntity interfaceEntity = switchEntity.Interfaces.FirstOrDefault(i => i.IdOnSwitch == portId);

                if(interfaceEntity is not null)
                {
                    ACEVlanOnInterfaceEntity aceVlanOnInterfaceEntity = interfaceEntity.ACLVlans.FirstOrDefault(aclVl => 
                        aclVl.Vlan == portSetting.Vlan && aclVl.AccessMask.HasFlag(AccessMaskVlanOnInterface.WriteAccess) && User.HasClaim(ClaimTypes.GroupSid, aclVl.GroupSID));

                    if(aceVlanOnInterfaceEntity is not null)
                    {
                        await _switchService.ConfigurePort(GetPortConfigAccess(switchEntity, interfaceEntity, portSetting));

                        return NoContent();
                    }

                    return Problem(detail: "No access on write for this vlan.", statusCode: StatusCodes.Status403Forbidden);
                }

                return Problem(detail: "Interface with this 'portId' not exist.", statusCode: StatusCodes.Status404NotFound);
            }

            return Problem(detail: "Switch with this 'id' not exist.", statusCode: StatusCodes.Status404NotFound);
            */

            /*
                        try
                        {
                            await _switchService.ConfigurePort(portConfigAccess);
                        }
                        catch (SwitchServiceException e) when (e.ErrorType == SwitchServiceErrorType.WrongInterface)
                        {
                            return Problem(detail: "Interface with this name not exist.", statusCode: StatusCodes.Status404NotFound);
                        }
                        */
        }




        [HttpPut("{id}/port/{portId}/trunk")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        public Task<ActionResult> ConfigurePortTrunk([Range(1, int.MaxValue)][FromRoute] int id, [Range(1, int.MaxValue)][FromRoute] int portId, ConfigurePortTrunkRequest portSetting)
        {

            throw new Exception("Test Exception");
        }

        private async Task<bool> SwitchEntityExists(int id) =>
            await _context.Switches.AnyAsync(e => e.Id == id);
    }
}
