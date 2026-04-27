using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPV.MODELOAK
{    public class Erreserbak
    {
        public virtual int Id { get; set; }
        public virtual int MahaiaId { get; set; }
        public virtual string Izena { get; set; }
        public virtual int Telefonoa { get; set; }
        public virtual DateTime? ErreserbaData { get; set; }
        public virtual int? PertsonaKop { get; set; }
        public virtual string Egoera { get; set; } = "Itxaropean";
        public virtual string? Oharrak { get; set; }
    }
}
