using CQRS.Core.Domain;
using CQRS.Core.Handlers;
using CQRS.Core.Infrastructure;
using CQRS.Core.Producers;
using Post.Cmd.Domain.Aggregates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Post.Cmd.Infrastructure.Handlers
{
    public class EventSourcingHandler : IEventSourcingHandler<PostAggregate>
    {
        private readonly IEventStore _eventStore;
        private readonly IEventProducer _eventProducer;
        public EventSourcingHandler(IEventStore eventStore, IEventProducer eventProducer)
        {
            _eventStore = eventStore;
            _eventProducer = eventProducer;
        }
        public async Task<PostAggregate> GetByIdAsync(Guid id)
        {
            var aggregate = new PostAggregate();
            var events = await _eventStore.GetEventsAsync(id);

            if (events == null || !events.Any()) return aggregate;
            aggregate.ReplayEvents(events);
            aggregate.Version = events.Select(x => x.Version).Max();
            return aggregate;
        }

        public async Task RepublishEventsAsync()
        {
            string topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC");
            var aggregateIds = await _eventStore.GetAggregateIdsAsync();
            if (aggregateIds == null || !aggregateIds.Any()) return;
            foreach ( var aggregateId in aggregateIds)
            {
                var aggregate = await GetByIdAsync(aggregateId);
                if(aggregate == null || !aggregate.Active) continue;

                var events = await _eventStore.GetEventsAsync(aggregateId);
                foreach(var @event in events)
                {
                    await _eventProducer.ProduceAsync(topic, @event);
                }    
            }
        }

        public async Task SaveAsync(AggregateRoot aggregate)
        {
            await _eventStore.SaveEventsAsync(aggregate.Id, aggregate.GetUncommittedChanges(), aggregate.Version);
            aggregate.MarkChangesAsCommited();
        }
    }
}
