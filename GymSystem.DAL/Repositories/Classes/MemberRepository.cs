using GymSystem.DAL.Contexts;
using GymSystem.DAL.Entities;
using GymSystem.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymSystem.DAL.Repositories.Classes
{
    public class MemberRepository : GenericRepository<Member> , IMemberRepository
    {
        private readonly GymDbContext dbcontext;

        public MemberRepository(GymDbContext _dbcontext) : base(_dbcontext) 
        {
            dbcontext = _dbcontext;
        }

    }
}
