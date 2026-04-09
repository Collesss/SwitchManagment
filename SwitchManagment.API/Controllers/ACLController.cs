using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SwitchManagment.API.Db;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SwitchManagment.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ACLController : ControllerBase
    {
        private readonly ILogger<ACLController> _logger;
        private readonly IMapper _mapper;
        private readonly ApplicationContext _context;

        public ACLController(ILogger<ACLController> logger, IMapper mapper, ApplicationContext context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }


        // GET: api/<ACLController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<ACLController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ACLController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ACLController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ACLController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
