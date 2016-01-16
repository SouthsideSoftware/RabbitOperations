using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using RabbitOperations.Collector.Configuration;
using RabbitOperations.Collector.Models;
using RabbitOperations.Domain.Configuration;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace RabbitOperations.Collector.Api.v1
{
    [Route("api/[controller]", Name = "defaultRabbitServer")]
    [Route("api/v1/[controller]", Name = "version1RabbitServer")]
    public class RabbitServerController : Controller
    {
        // GET: api/values
        [HttpGet]
        public IEnumerable<RabbitServer> Get()
        {
           return new List<RabbitServer>
           {
               new RabbitServer
               {
                   Name = "Server1",
                   Url = "http://foo"
               },
               new RabbitServer
               {
                   Name = "Server1",
                   Url = "http://foo"
               }
           };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
