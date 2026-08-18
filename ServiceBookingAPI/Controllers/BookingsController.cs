using Microsoft.AspNetCore.Mvc;
using ServiceBooking.Application.Bookings.Commands.CreateBooking;

namespace ServiceBooking.WebUI.Controllers;

public class BookingsController : ApiControllerBase
{
    [HttpPost]
    public async Task<ActionResult<string>> Create(CreateBookingCommand command)
    {
        // We simply send the command to MediatR.
        // The ValidationBehavior will automatically check the data.
        // The CreateBookingCommandHandler will handle the logic.
        var id = await Mediator.Send(command);

        // Return 201 Created with the new ID
        return CreatedAtAction(nameof(Create), new { id }, id);
    }

    // Example of a Query (Assuming you've built a GetBookingQuery)
    /*
    [HttpGet("{id}")]
    public async Task<ActionResult<BookingDto>> Get(string id)
    {
        return await Mediator.Send(new GetBookingQuery(id));
    }
    */
}