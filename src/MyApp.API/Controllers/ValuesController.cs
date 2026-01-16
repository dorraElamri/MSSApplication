using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace MyApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ValuesController : ControllerBase
    {
        // GET: api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return $"value {id}";
        }

        // POST api/values
        [HttpPost]
        public IActionResult Post([FromBody] string value)
        {
            return Ok($"You posted: {value}");
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] string value)
        {
            return Ok($"You updated {id} to {value}");
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            return Ok($"Deleted {id}");
        }
    }
}
