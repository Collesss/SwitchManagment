using AutoMapper;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SwitchManagment.API.Db;
using SwitchManagment.API.Db.Entities;
using SwitchManagment.API.Db.Entities.ACL.AccessMasks;
using SwitchManagment.API.Models.Dto.ACL.AccessMask;
using SwitchManagment.API.Models.Dto.Port;
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
                await _switchService.ConfigurePort(GetPortConfigAccess(ACEVlanOnInterfaces.Switch, portSetting));

                return NoContent();
            }

            return Problem(detail: "No write access for this VLAN.", statusCode: StatusCodes.Status403Forbidden);
        }


        [HttpPut("{id}/port/trunk")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> ConfigurePortTrunk([Range(1, int.MaxValue)][FromRoute] int id, [Range(1, int.MaxValue)][FromRoute] int portId, ConfigurePortTrunkRequest portSetting)
        {
            string[] groupsSid = GetUserGroupSIDs();

            var ACEVlanOnInterfaces = await _context.ACLVlanOnInterfaces
                .Include(aceVlOnIf => aceVlOnIf.Switch)
                .Where(ace => portSetting.Vlans.Contains(ace.Vlan) && ace.InterfaceName == portSetting.InterfaceName && ace.SwitchId == id && ace.AccessMask.HasFlag(AccessMaskVlanOnInterface.WriteTrunk) && groupsSid.Contains(ace.GroupSID))
                .GroupBy(ace => ace.Vlan)
                .Select(groupAce => groupAce.First())
                .ToListAsync();


            if (portSetting.Vlans.All(vlan => ACEVlanOnInterfaces.Any(ace => ace.Vlan == vlan)))
            {
                await _switchService.ConfigurePort(GetPortConfigTrunk(ACEVlanOnInterfaces.First().Switch, portSetting));

                return NoContent();
            }

            return Problem(detail: "There is no write access to at least one VLAN in the list.", statusCode: StatusCodes.Status403Forbidden);

        }


        private PortConfigAccess GetPortConfigAccess(SwitchEntity switchEntity, ConfigurePortAccessRequest portSetting)
        {
            PortConfigAccess portConfigAccess = _mapper.Map<PortConfigAccess>(switchEntity);
            portConfigAccess.Password = _dataProtector.Unprotect(switchEntity.EncryptedPassword);
            portConfigAccess.SuperPassword = _dataProtector.Unprotect(switchEntity.EncryptedSuperPassword);

            _mapper.Map(portSetting, portConfigAccess);

            return portConfigAccess;
        }

        private PortConfigTrunk GetPortConfigTrunk(SwitchEntity switchEntity, ConfigurePortTrunkRequest portSetting)
        {
            PortConfigTrunk portConfigTrunk = _mapper.Map<PortConfigTrunk>(switchEntity);
            portConfigTrunk.Password = _dataProtector.Unprotect(switchEntity.EncryptedPassword);
            portConfigTrunk.SuperPassword = _dataProtector.Unprotect(switchEntity.EncryptedSuperPassword);

            _mapper.Map(portSetting, portConfigTrunk);

            return portConfigTrunk;
        }

        /*
        private PortConfigAccess GetPortConfig(SwitchEntity switchEntity, ConfigurePortAccessRequest portSetting) =>
            GetPortConfig<PortConfigAccess, ConfigurePortAccessRequest>(switchEntity, portSetting);

        private PortConfigTrunk GetPortConfig(SwitchEntity switchEntity, ConfigurePortTrunkRequest portSetting) =>
            GetPortConfig<PortConfigTrunk, ConfigurePortTrunkRequest>(switchEntity, portSetting);

        private T GetPortConfig<T, V>(SwitchEntity switchEntity, V portSetting) where T : PortConfig where V : class
        {
            T portConfig = _mapper.Map<T>(switchEntity);
            portConfig.Password = _dataProtector.Unprotect(switchEntity.EncryptedPassword);
            portConfig.SuperPassword = _dataProtector.Unprotect(switchEntity.EncryptedSuperPassword);

            _mapper.Map(portSetting, portConfig);

            return portConfig;
        }
        */

        private string[] GetUserGroupSIDs() =>
            User.Claims
                .Where(claim => claim.Type == ClaimTypes.GroupSid)
                .Select(claim => claim.Value)
                .ToArray();
    }
}