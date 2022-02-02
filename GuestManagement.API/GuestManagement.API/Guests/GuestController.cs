using AutoMapper;
using GuestManagement.Application.Guests;
using GuestManagement.Application.Guests.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GuestManagement.API.Guests
{
    [Route("api/[controller]")]
    [ApiController]
    public class GuestController : ControllerBase
    {
        private readonly IMediator mediator;
        private readonly IMapper mapper;

        public GuestController(
            IMediator mediator,
            IMapper mapper)
        {
            this.mediator = mediator;
            this.mapper = mapper;
        }
        
        
        // GET: api/<GuestController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<GuestController>/5
        [HttpGet("{email}")]
        public string Get(string email)
        {
            return email;
        }

        // POST api/<GuestController>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] RegisterGuestDto registerGuestDto)
        {
            await mediator.Send(mapper.Map<CreateGuestCommand>(registerGuestDto));

            return Created($"/api/Guest/{registerGuestDto.Email}", null);
        }

        [HttpPost("VisitRegistration")]
        public async Task<ActionResult> RegisterForVisit([FromBody] VisitRegistrationDto visitRegistrationDto)
        {          
            await mediator.Send(mapper.Map<VisitRegistrationCommand>(visitRegistrationDto));

            return Ok();
        }
 
       

        // PUT api/<GuestController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<GuestController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
