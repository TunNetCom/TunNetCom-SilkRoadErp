using System;
using System.Collections.Generic;
using System.Text;

namespace TunNetCom.SilkRoadErp.SharedKernel.Events;

/// <summary>
/// Marker interface for all event handlers in the application
/// </summary>
public interface IEventHandler;

/// <summary>
/// Base interface for typed event handlers
/// </summary>
/// <typeparam name="TEvent">Type of event handled by this handler</typeparam>
public interface IEventHandler<in TEvent> : IEventHandler where TEvent : IEvent
{
    Task HandleAsync(TEvent @event, CancellationToken cancellationToken);
}

