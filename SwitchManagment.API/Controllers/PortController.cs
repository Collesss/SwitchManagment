using AutoMapper;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SwitchManagment.API.Db;
using SwitchManagment.API.Db.Entities;
using SwitchManagment.API.Db.Entities.ACL.AccessMasks;
using SwitchManagment.API.Models.Dto.Switch.Port;
using SwitchManagment.API.SwitchService.Data;
using SwitchManagment.API.SwitchService.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SwitchManagment.API.Controllers
{
    [Route("api/switch")]
    [ApiController]
    public class PortController : ControllerBase
    {
        private readonly ILogger<PortController> _logger;
        private readonly IMapper _mapper;
        private readonly ApplicationContext _context;
        private readonly ISwitchService _switchService;
        private readonly IDataProtector _dataProtector;

        public PortController(ILogger<PortController> logger, IMapper mapper, ApplicationContext context, ISwitchService switchService, IDataProtectionProvider dataProtectorProvider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _switchService = switchService ?? throw new ArgumentNullException(nameof(switchService));
            _dataProtector = dataProtectorProvider?.CreateProtector("SwitchController") ?? throw new ArgumentNullException(nameof(dataProtectorProvider));
        }

        [HttpPut("{id}/port/access")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> ConfigurePortAccess([Range(1, int.MaxValue)][FromRoute] int id, ConfigurePortAccessRequest portSetting)
        {
            string[] groupsSid = GetUserGroupSIDs();

            var ACEVlanOnInterfaces = await _context.ACLVlanOnInterfaces
                .Include(aceVlOnIf => aceVlOnIf.Switch)
                .FirstOrDefaultAsync(ace => ace.Vlan == portSetting.Vlan && ace.InterfaceName == portSetting.InterfaceName && ace.SwitchId == id && ace.AccessMask.HasFlag(AccessMaskVlanOnInterface.WriteAccess) && groupsSid.Contains(ace.GroupSID));

            if (ACEVlanOnInterfaces is not null)
            {
                await _switchService.ConfigurePort(GetPortConfigAccess(ACEVlanOnInterfaces.Switch, , portSetting));

                return NoContent();
            }

            return Problem(detail: "No write access for this VLAN.", statusCode: StatusCodes.Status403Forbidden);
        }


        [HttpPut("{id}/port/trunk")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        public Task<ActionResult> ConfigurePortTrunk([Range(1, int.MaxValue)][FromRoute] int id, [Range(1, int.MaxValue)][FromRoute] int portId, ConfigurePortTrunkRequest portSetting)
        {

            throw new Exception("Test Exception");
        }


        private PortConfigAccess GetPortConfigAccess(SwitchEntity switchEntity, ConfigurePortAccessRequest portSetting)
        {
            PortConfigAccess portConfigAccess = _mapper.Map<PortConfigAccess>(switchEntity);
            portConfigAccess.Password = _dataProtector.Unprotect(switchEntity.EncryptedPassword);
            portConfigAccess.SuperPassword = _dataProtector.Unprotect(switchEntity.EncryptedSuperPassword);

            _mapper.Map(portSetting, portConfigAccess);

            return portConfigAccess;
        }

        private string[] GetUserGroupSIDs() =>
            User.Claims
                .Where(claim => claim.Type == ClaimTypes.GroupSid)
                .Select(claim => claim.Value)
                .ToArray();
    }
}


/*
private async Task<SwitchEntity> GetSwitchWithIntfAndACL(int id) =>
            await _context.Switches
                .Include(sw => sw.Interfaces)
                .ThenInclude(i => i.ACLVlans)
                .FirstOrDefaultAsync(sw => sw.Id == id);


             
*/

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