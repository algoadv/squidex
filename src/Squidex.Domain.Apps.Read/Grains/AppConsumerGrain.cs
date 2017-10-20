// ==========================================================================
//  AppConsumerGrain.cs
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex Group
//  All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orleans;
using Squidex.Infrastructure;
using Squidex.Infrastructure.CQRS.Events;
using Squidex.Infrastructure.Log;
using Squidex.Infrastructure.Tasks;

namespace Squidex.Domain.Apps.Read.Grains
{
    public sealed class AppConsumerGrain : Grain<AppConsumerState>, IAppConsumerGrain
    {
        private readonly ISemanticLog log;
        private readonly IEventStore eventStore;
        private readonly IEnumerable<IEventConsumer> eventConsumers;
        private readonly EventDataFormatter formatter;
        private readonly RetryWindow retryWindow = new RetryWindow(TimeSpan.FromMinutes(5), 5);
        private SubscriptionWrapper subscription;

        private sealed class SubscriptionWrapper : IEventSubscriber, IDisposable
        {
            private readonly IEventSubscription subscription;
            private readonly IAppConsumerGrain grain;

            public Guid Id { get; } = Guid.NewGuid();

            public SubscriptionWrapper(IEventStore eventStore, IAppConsumerGrain grain, string position = null)
            {
                this.grain = grain;

                subscription = eventStore.CreateSubscription(this, null, position);
            }

            public void Dispose()
            {
                subscription.StopAsync().Forget();
            }

            public Task OnEventAsync(IEventSubscription subscription, StoredEvent @event)
            {
                return grain.OnEventAsync(Id, @event);
            }

            public Task OnErrorAsync(IEventSubscription subscription, Exception exception)
            {
                return grain.OnErrorAsync(Id, exception.ToString());
            }
        }

        public AppConsumerGrain(
            ISemanticLog log,
            IEventStore eventStore,
            IEnumerable<IEventConsumer> eventConsumers,
            EventDataFormatter formatter)
        {
            this.log = log;

            this.eventStore = eventStore;
            this.eventConsumers = eventConsumers;

            this.formatter = formatter;
        }

        public override async Task OnActivateAsync()
        {
            if (State.IsRunning)
            {
                await StartAsync();
            }
        }

        public Task StartAsync()
        {
            return DoAsync(() =>
            {
                SubscribeThisAsync(null);

                State.IsRunning = true;

                return TaskHelper.Done;
            });
        }

        public Task RestartAsync()
        {
            return DoAsync(async () =>
            {
                UnsubscribeThisAsync();

                await ClearAsync();

                SubscribeThisAsync(null);

                State.IsRunning = true;
            });
        }

        public async Task OnEventAsync(Guid subscriptionId, StoredEvent storedEvent)
        {
            if (subscriptionId == subscription?.Id)
            {
                await DoAsync(async () =>
                {
                    await DispatchConsumerAsync(ParseEvent(storedEvent));

                    State.Position = storedEvent.EventPosition;
                });
            }
        }

        public async Task OnErrorAsync(Guid subscriptionId, string error)
        {
            if (subscriptionId == subscription?.Id)
            {
                await DoAsync(() =>
                {
                    if (retryWindow.CanRetryAfterFailure())
                    {
                        Task.Delay(ReconnectWaitMs).ContinueWith(t => dispatcher.SendAsync(new Reconnect { StateId = newStateId })).Forget();
                    }
                    UnsubscribeThisAsync();

                    State.Error = error;
                    State.IsRunning = false;

                    return TaskHelper.Done;
                });
            }
        }

        private async Task DoAsync(Func<Task> action)
        {
            try
            {
                await action();
            }
            catch (Exception ex)
            {
                State.Error = ex.ToString();
                State.IsRunning = false;
            }
            finally
            {
                await WriteStateAsync();
            }
        }

        private void SubscribeThisAsync(string position)
        {
            if (subscription == null)
            {
                subscription = new SubscriptionWrapper(eventStore, this.AsReference<IAppConsumerGrain>(), State.Position);
            }
        }

        private void UnsubscribeThisAsync()
        {
            if (subscription != null)
            {
                subscription.Dispose();
                subscription = null;
            }
        }

        private async Task ClearAsync()
        {
            var actionId = Guid.NewGuid().ToString();

            log.LogInformation(w => w
                .WriteProperty("action", "EventConsumerReset")
                .WriteProperty("actionId", actionId)
                .WriteProperty("state", "Started"));

            using (log.MeasureTrace(w => w
                .WriteProperty("action", "EventConsumerReset")
                .WriteProperty("actionId", actionId)
                .WriteProperty("state", "Completed")))
            {
                await Task.WhenAll(eventConsumers.Select(c => c.ClearAsync()));
            }
        }

        private async Task DispatchConsumerAsync(Envelope<IEvent> @event)
        {
            var eventId = @event.Headers.EventId().ToString();
            var eventType = @event.Payload.GetType().Name;

            log.LogInformation(w => w
                .WriteProperty("action", "HandleEvent")
                .WriteProperty("actionId", eventId)
                .WriteProperty("state", "Started")
                .WriteProperty("eventId", eventId)
                .WriteProperty("eventType", eventType));

            using (log.MeasureTrace(w => w
                .WriteProperty("action", "HandleEvent")
                .WriteProperty("actionId", eventId)
                .WriteProperty("state", "Completed")
                .WriteProperty("eventId", eventId)
                .WriteProperty("eventType", eventType)))
            {
                await Task.WhenAll(eventConsumers.Select(c => c.On(@event)));
            }
        }

        private Envelope<IEvent> ParseEvent(StoredEvent message)
        {
            var @event = formatter.Parse(message.Data);

            @event.SetEventPosition(message.EventPosition);
            @event.SetEventStreamNumber(message.EventStreamNumber);

            return @event;
        }
    }
}
