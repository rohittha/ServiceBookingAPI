using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBooking.Application.Common.Models;

public record BookingCreatedIntegrationEvent(
    string BookingId,
    string BusinessId,
    string ServiceId,
    string? EmployeeId,
    DateTime BookingDateTime,
    string CustomerEmail);