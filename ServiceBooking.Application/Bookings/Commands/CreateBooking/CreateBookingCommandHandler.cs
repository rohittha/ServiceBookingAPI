using MediatR;
using ServiceBooking.Application.Common.Interfaces;
using ServiceBooking.Domain.Entities;
using ServiceBooking.Domain.ValueObjects;

namespace ServiceBooking.Application.Bookings.Commands.CreateBooking;

public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, string>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IBusinessProfileRepository _businessRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateBookingCommandHandler(
        IBookingRepository bookingRepository,
        IBusinessProfileRepository businessRepository,
        IUnitOfWork unitOfWork)
    {
        _bookingRepository = bookingRepository;
        _businessRepository = businessRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<string> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        // 1. Validate that the business exists
        var business = await _businessRepository.GetByIdAsync(request.BusinessId, cancellationToken);
        if (business == null)
            throw new Exception("Business not found.");

        // 2. Convert raw strings to Domain Value Objects
        var customerEmail = Email.Create(request.CustomerEmail);

        // 3. Create the Domain Entity 
        // Logic inside the Booking constructor will validate dates and required fields
        var booking = new Booking(
            request.BusinessId,
            request.ServiceId,
            request.EmployeeId,
            request.BookingDateTime,
            customerEmail);

        // 4. Persist to Repository
        await _bookingRepository.AddAsync(booking, cancellationToken);

        // 5. Save Changes via Unit of Work
        // This is where the DB transaction actually happens
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return booking.Id;
    }
}