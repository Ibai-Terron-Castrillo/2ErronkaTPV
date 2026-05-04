using System;

namespace JatetxeaApi.Modeloak
{
    public class Erreserbak
    {
        public virtual int Id { get; set; }
        public virtual int MahaiaId { get; set; }
        public virtual string Izena { get; set; }
        public virtual int Telefonoa { get; set; }
        public virtual DateTime? ErreserbaData { get; set; }
        public virtual int? PertsonaKop { get; set; }
        public virtual string Egoera { get; set; } = "Itxaropean";
        public virtual string? Oharrak { get; set; }

        public Erreserbak() { }

        public Erreserbak(int mahaiaId, string izena, int telefonoa, DateTime? erreserbaData = null, int? pertsonaKop = null, string egoera = "Itxaropean", string? oharrak = null)
        {
            MahaiaId = mahaiaId;
            Izena = izena;
            Telefonoa = telefonoa;
            ErreserbaData = erreserbaData ?? DateTime.Now;
            PertsonaKop = pertsonaKop;
            Egoera = egoera;
            Oharrak = oharrak;
        }
    }
}
