using Microsoft.AspNetCore.Mvc;
using ServiceBooking.Application.Bookings.Commands.CreateBooking;
using ServiceBooking.Application.BusinessProfiles.Queries.GetBusinessProfileById;

namespace ServiceBooking.WebUI.Controllers;

public class BusinessProfileController : ApiControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<GetBusinessProfileByIdDto>> Get(string id)
    {
        var result = await Mediator.Send(new GetBusinessProfileByIdQuery(id));

        if (result == null)
        {
            return NotFound();
        }
        return Ok(result);
    }
}
