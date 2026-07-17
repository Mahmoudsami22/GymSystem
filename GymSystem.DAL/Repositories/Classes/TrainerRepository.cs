using GymSystem.DAL.Contexts;
using GymSystem.DAL.Entities;
using GymSystem.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymSystem.DAL.Repositories.Classes
{
    public class TrainerRepository : GenericRepository<Trainer>, ITrainerRepository
    {
        private readonly GymDbContext dbcontext;

        public TrainerRepository(GymDbContext _dbcontext) : base(_dbcontext)
        {
            dbcontext = _dbcontext;
        }
    }
}
