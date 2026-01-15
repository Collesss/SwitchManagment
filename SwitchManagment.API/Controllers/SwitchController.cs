using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SwitchManagment.API.Models.Dto.Switch;
using SwitchManagment.API.Repository.Entities;
using SwitchManagment.API.Repository.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SwitchManagment.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SwitchController : ControllerBase
    {
        private readonly ILogger<SwitchController> _logger;
        private readonly IMapper _mapper;
        private readonly ISwitchRepository _switchRepository;

        public SwitchController(ILogger<SwitchController> logger, IMapper mapper, ISwitchRepository switchRepository) 
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _switchRepository = switchRepository ?? throw new ArgumentNullException(nameof(switchRepository));
        }


        // GET: api/<SwitchController>
        [HttpGet]
        public async Task<IEnumerable<SwitchResponse>> Get() =>
            _mapper.Map<IEnumerable<SwitchResponse>>(await _switchRepository.GetAll());

        // GET api/<SwitchController>/5
        [HttpGet("{id}")]
        public async Task<SwitchResponse> Get(int id) =>
            _mapper.Map<SwitchResponse>(await _switchRepository.GetById(id));
        
        [HttpPost]
        public async Task<int> Post([FromBody] SwitchCreateRequest switchCreate) =>
            await _switchRepository.Add(_mapper.Map<SwitchEntity>(switchCreate));

        /*
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }
        */

        [HttpDelete("{id}")]
        public async Task Delete(int id) =>
            await _switchRepository.DeleteById(id);
    }
}
