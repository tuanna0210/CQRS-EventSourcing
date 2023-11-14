using CQRS.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQRS.Core.Domain
{
    public abstract class AggregateRoot
    {
        protected Guid _id;
        private readonly List<BaseEvent> _changes = new();
        public Guid Id { get { return _id; } }
        public int Version { get; set; } = -1;

        public IEnumerable<BaseEvent> GetUncommittedChanges()
        {
            return _changes;
        }

        public void MarkChangesAsCommited()
        {
            _changes.Clear();
        }

        private void ApplyChange(BaseEvent @event, bool isNew) //IsNew: is new event, true in RaiseEvent method, because the event is true when it is raised, not when load from event store
        {
            var method = this.GetType().GetMethod("Apply", new Type[] { @event.GetType() });

            if(method == null)
            {
                throw new ArgumentNullException(nameof(method), $"The Apply method was not found in the aggregate for {nameof(@event)}");
            }
            method.Invoke(this, new object[] { @event });
            if(isNew)
            {
                _changes.Add( @event );
            }
        }
        protected void RaiseEvent(BaseEvent @event)
        {
            ApplyChange(@event, true);
        }
        /// <summary>
        /// Replay events loaded from event store
        /// </summary>
        /// <param name="events"></param>
        public void ReplayEvents(IEnumerable<BaseEvent> events)
        {
            foreach(BaseEvent @event in events)
            {
                ApplyChange(@event, false);//IsNew = false because those events are loaded from event store, which are old event
            }
        }
    }
}
