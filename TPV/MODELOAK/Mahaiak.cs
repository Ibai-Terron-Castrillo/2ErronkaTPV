using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPV.MODELOAK
{
    public class Mahaiak
    {
        public virtual int Id { get; set; }
        public virtual int MahaiaZbk { get; set; }
        public virtual int Edukiera { get; set; }
        public virtual string? Egoera { get; set; } = "Libre";
    }
}
