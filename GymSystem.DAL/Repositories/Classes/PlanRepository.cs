using GymSystem.DAL.Contexts;
using GymSystem.DAL.Entities;
using GymSystem.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymSystem.DAL.Repositories.Classes
{
    public class PlanRepository : IPlanRepository
    {
        private readonly GymDbContext dbcontext;
        public PlanRepository(GymDbContext _dbcontext)
        {
            dbcontext = _dbcontext;
        }

        public async Task<IEnumerable<Plan>> GetAll(bool isTracked, CancellationToken ct = default)
        {
            var Plans = isTracked ? dbcontext.Plans : dbcontext.Plans.AsNoTracking();
            return await Plans.ToListAsync(ct);
        }

        public async Task<Plan?> GetById(int id, CancellationToken ct = default)
        {
            var plan = await dbcontext.Plans.FirstOrDefaultAsync(p => p.Id == id , ct);
            return plan;
        }
        public void Add(Plan plan)
        {
            dbcontext.Add(plan);
        }

        public void Update(Plan plan)
        {
            dbcontext.Update(plan);
        }

        public void Delete(int id)
        {
            var plan = dbcontext.Plans.FirstOrDefault(p => p.Id == id);
            if (plan != null)
            {
                dbcontext.Plans.Remove(plan);
            }
        }
        public async Task<int> completeAsync()
        {
            return await dbcontext.SaveChangesAsync();
        }

        
    }
}
