namespace JatetxeaApi.DTOak
{
    public class InbentarioaSortuDto
    {
        public string Izena { get; set; }
        public string? Deskribapena { get; set; }
        public int Kantitatea { get; set; }
        public string? NeurriaUnitatea { get; set; }
        public int StockMinimoa { get; set; }
    }

    public class InbentarioaPatchDto
    {
        public string? Izena { get; set; }
        public string? Deskribapena { get; set; }
        public int? Kantitatea { get; set; }
        public string? NeurriaUnitatea { get; set; }
        public int? StockMinimoa { get; set; }
    }

    public class InbentarioaDto
    {
        public int Id { get; set; }
        public string Izena { get; set; }
        public string? Deskribapena { get; set; }
        public int Kantitatea { get; set; }
        public string? NeurriaUnitatea { get; set; }
        public int StockMinimoa { get; set; }
        public DateTime? AzkenEguneratzea { get; set; }
    }
}
