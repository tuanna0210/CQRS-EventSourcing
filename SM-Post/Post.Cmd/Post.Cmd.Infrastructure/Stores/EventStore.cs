﻿using CQRS.Core.Domain;
using CQRS.Core.Events;
using CQRS.Core.Exceptions;
using CQRS.Core.Infrastructure;
using CQRS.Core.Producers;
using Post.Cmd.Domain.Aggregates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Post.Cmd.Infrastructure.Stores
{
    public class EventStore : IEventStore
    {
        private readonly IEventStoreRepository _eventStoreRepository;
        private readonly IEventProducer _eventProducer;
        public EventStore(IEventStoreRepository eventStoreRepository,
            IEventProducer eventProducer)
        {
            _eventStoreRepository = eventStoreRepository;
            _eventProducer = eventProducer;
        }

        public async Task<List<Guid>> GetAggregateIdsAsync()
        {
            var eventStream = await _eventStoreRepository.FindAllAsync();
            if(eventStream == null || !eventStream.Any())
            {
                throw new ArgumentNullException(nameof(eventStream), "Could not retrieve event stream from event store!");
            }
            return eventStream.Select(x => x.AggregateIdentifier).Distinct().ToList();
        }

        public async Task<List<BaseEvent>> GetEventsAsync(Guid aggregateId)
        {
            var eventStream = await _eventStoreRepository.FindByAggregateId(aggregateId);
            if (eventStream == null || !eventStream.Any()) {
                throw new AggregateNotFoundException("Incorrect post ID provided");
            }
            return eventStream.OrderBy(x => x.Version).Select(x=> x.EventData).ToList();
        }

        public async Task SaveEventsAsync(Guid aggregateId, IEnumerable<BaseEvent> events, int expectedVersion)
        {
            var eventStream = await _eventStoreRepository.FindByAggregateId(aggregateId);

            //optimistic concurence check
            if(expectedVersion != -1 && eventStream[^1].Version != expectedVersion)
            {
                throw new ConcurrencyException();
            }
            var version = expectedVersion;
            foreach(var @event in events)
            {
                version++;
                @event.Version = version;
                var eventType = @event.GetType().Name;
                var eventModel = new EventModel()
                {
                    Timestamp = DateTime.Now,
                    AggregateIdentifier = aggregateId,
                    AggregateType = nameof(PostAggregate), //T-TEMP, event store cho aggregate khac thi phai cung cap gia tri khac
                    Version = version,
                    EventType = eventType,
                    EventData = @event
                };
                await _eventStoreRepository.SaveAsync(eventModel);


                var topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC");//T-TEMP, co the cho vao config
                await _eventProducer.ProduceAsync(topic, @event);
            }
        }
    }
}
