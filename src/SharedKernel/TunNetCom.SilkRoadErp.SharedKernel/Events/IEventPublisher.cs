using System;
using System.Collections.Generic;
using System.Text;

namespace TunNetCom.SilkRoadErp.SharedKernel.Events;

/// <summary>
/// Interface for publishing events
/// </summary>
public interface IEventPublisher
{
    /// <summary>
    /// Publishes an event to all registered handlers
    /// </summary>
    /// <typeparam name="TEvent">The type of event to publish</typeparam>
    /// <param name="event">The event instance</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken) where TEvent : IEvent;
}