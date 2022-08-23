﻿using System.Linq.Expressions;
using Common.DatabaseModels.Models.Plan;
using Microsoft.Extensions.DependencyInjection;
using Noots.DatabaseContext.GenericRepositories;

namespace Noots.DatabaseContext.Repositories.Billing
{
    public class BillingPlanCacheRepository
    {
        private readonly IServiceScopeFactory scopeFactory;
        
        private Dictionary<BillingPlanTypeENUM, BillingPlan> _cache = null;
        
        public BillingPlanCacheRepository(IServiceScopeFactory scopeFactory)
        {
            this.scopeFactory = scopeFactory;
        }

        private async Task InitAsync()
        {
            using var scope = scopeFactory.CreateScope();   
            var context = scope.ServiceProvider.GetRequiredService<NootsDBContext>();
            var repo = new Repository<BillingPlan, BillingPlanTypeENUM>(context);
            if (_cache == null)
            {
                var ents = await repo.GetAllAsync();
                _cache = ents.ToDictionary(x => x.Id);
            }
        }
        
        public async Task<List<BillingPlan>> GetAllCacheAsync()
        {
            await InitAsync();
            return _cache.Values.ToList();
        }

        public async Task<BillingPlan> FirstOrDefaultCacheAsync(BillingPlanTypeENUM id)
        {
            await InitAsync();
            return _cache[id];
        }
    }
}
