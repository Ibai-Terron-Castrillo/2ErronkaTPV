using System;

namespace JatetxeaApi.DTOak
{
    public class ErreserbakDto
    {
        public int Id { get; set; }
        public int MahaiaId { get; set; }
        public string Izena { get; set; }
        public int Telefonoa { get; set; }
        public DateTime? ErreserbaData { get; set; }
        public int? PertsonaKop { get; set; }
        public string Egoera { get; set; }
        public string? Oharrak { get; set; }
    }

    public class ErreserbakSortuDto
    {
        public int MahaiaId { get; set; }
        public string Izena { get; set; }
        public int Telefonoa { get; set; }
        public DateTime? ErreserbaData { get; set; }
        public int? PertsonaKop { get; set; }
        public string Egoera { get; set; } = "Itxaropean";
        public string? Oharrak { get; set; }
    }
}
