using System;

namespace TPV.MODELOAK
{
    public class Zerbitzuak
    {
        public virtual int Id { get; set; }
        public virtual int LangileId { get; set; }
        public virtual int MahaiaId { get; set; }
        public virtual int ErreserbaId { get; set; }
        public virtual DateTime? EskaeraData { get; set; }
        public virtual string Egoera { get; set; } = "Itxaropean";
        public virtual decimal? Guztira { get; set; }
    }
}
