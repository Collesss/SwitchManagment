using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
namespace SwitchManagment.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CredentialController : ControllerBase
    {
        private readonly ILogger<CredentialController> _logger;
        private readonly IDataProtector _dataProtector;

        public CredentialController(ILogger<CredentialController> logger, IDataProtector dataProtector)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dataProtector = dataProtector ?? throw new ArgumentNullException(nameof(dataProtector));
        }

        // GET: api/<CredentialController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<CredentialController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<CredentialController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<CredentialController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<CredentialController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
