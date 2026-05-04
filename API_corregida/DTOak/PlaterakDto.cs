namespace JatetxeaApi.DTOak
{
    public class PlaterakSortuDto
    {
        public string Izena { get; set; }
        public string? Deskribapena { get; set; }
        public decimal Prezioa { get; set; }
        public int? KategoriaId { get; set; }
        public string Erabilgarri { get; set; } = "Bai";
        public string? Irudia { get; set; }
    }

    public class PlaterakPatchDto
    {
        public string? Izena { get; set; }
        public string? Deskribapena { get; set; }
        public decimal? Prezioa { get; set; }
        public int? KategoriaId { get; set; }
        public string? Erabilgarri { get; set; }
        public string? Irudia { get; set; }
    }

    public class PlaterakDto
    {
        public int Id { get; set; }
        public string Izena { get; set; }
        public string? Deskribapena { get; set; }
        public decimal Prezioa { get; set; }
        public int? KategoriaId { get; set; }
        public string Erabilgarri { get; set; }
        public DateTime? SortzeData { get; set; }
        public string? Irudia { get; set; }
    }
}
