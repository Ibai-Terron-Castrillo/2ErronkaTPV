using System;

namespace JatetxeaApi.Modeloak
{
    public class Zerbitzuak
    {
        public virtual int Id { get; set; }
        public virtual int LangileId { get; set; }
        public virtual int MahaiaId { get; set; }
        public virtual int? ErreserbaId { get; set; }
        public virtual DateTime? EskaeraData { get; set; }
        public virtual string Egoera { get; set; } = "Itxaropean";
        public virtual decimal? Guztira { get; set; }

        public Zerbitzuak() { }

        public Zerbitzuak(int langileId, int mahaiaId, int? erreserbaid, DateTime? eskaeraData = null, string egoera = "Itxaropean", decimal? guztira = null)
        {
            LangileId = langileId;
            MahaiaId = mahaiaId;
            ErreserbaId = erreserbaid;
            EskaeraData = eskaeraData ?? DateTime.Now;
            Egoera = egoera;
            Guztira = guztira;
        }
    }
}
