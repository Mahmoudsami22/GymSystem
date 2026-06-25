using System;
using System.Collections.Generic;
using System.Text;

namespace GymSystem.DAL.Entities
{
    public class Plan : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int Duration { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }

        public ICollection<MemberShip> MemberShips { get; set; } = new HashSet<MemberShip>();
    }
}
