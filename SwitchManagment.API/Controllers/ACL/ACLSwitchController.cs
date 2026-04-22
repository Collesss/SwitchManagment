using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SwitchManagment.API.Db;
using SwitchManagment.API.Db.Entities.ACL.ACEs;
using SwitchManagment.API.Models.Dto.ACL.Switch;
using System.ComponentModel.DataAnnotations;

namespace SwitchManagment.API.Controllers.ACL
{
    [Route("api/acl/switch")]
    [ApiController]
    public class ACLSwitchController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger<ACLSwitchController> _logger;
        private readonly ApplicationContext _context;

        public ACLSwitchController(ILogger<ACLSwitchController> logger, IMapper mapper, ApplicationContext context) 
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<ACESwitchResponse>> GetACE([Range(1, int.MaxValue)][FromRoute] int id) =>
            (await _context.ACLSwitches.FindAsync(id)) is ACESwitchEntity ace ? 
            Ok(_mapper.Map<ACESwitchResponse>(ace)) : 
            Problem(detail: "ACE with this 'id' not exist.", statusCode: StatusCodes.Status404NotFound);


        [HttpPost]
        public async Task<ActionResult<int>> AddACE(ACESwitchAddRequest aceSwitchAdd)
        {
            try
            {
                var addResult = (await _context.AddAsync(_mapper.Map<ACESwitchEntity>(aceSwitchAdd))).Entity;

                await _context.SaveChangesAsync();

                return CreatedAtAction("GetACESwitch", new { id = addResult.Id }, addResult.Id);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteACE([Range(1, int.MaxValue)][FromRoute] int id)
        {
            try
            {
                _context.ACLSwitches.Remove(new ACESwitchEntity { Id = id });
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Problem(detail: "ACE with this 'id' not exist.", statusCode: StatusCodes.Status404NotFound);
            }
        }
    }
}
