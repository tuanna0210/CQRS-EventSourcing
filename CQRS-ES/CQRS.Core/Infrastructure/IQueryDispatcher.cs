using CQRS.Core.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQRS.Core.Infrastructure
{
    public interface IQueryDispatcher<TEntity>
    {
        void RegisterHandler<TQuery>(Func<TQuery, List<TEntity>> handler) where TQuery: BaseQuery;
        Task<List<TEntity>> SendAsync(BaseQuery query);
    }
}
