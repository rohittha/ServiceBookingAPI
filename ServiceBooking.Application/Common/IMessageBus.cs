using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBooking.Application.Common.Interfaces;

public interface IMessageBus
{
    Task PublishAsync<T>(T message, string queueOrTopicName, CancellationToken cancellationToken = default);
}
