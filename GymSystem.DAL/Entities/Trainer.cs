using GymSystem.DAL.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Collections.Specialized.BitVector32;

namespace GymSystem.DAL.Entities
{
    public class Trainer : GymUser
    {
        public Specialty Specialty { get; set; }    

        public ICollection<Session> Sessions { get; set; } = new HashSet<Session>();
    }
}
