namespace JatetxeaApi.DTOak
{
    public class ZerbitzatutaPatchDto
    {
        public bool Zerbitzatuta { get; set; }
    }

    public class KantitateaAldatuDto
    {
        public int Aldaketa { get; set; }
    }

    public class EragiketaEmaitzaDto
    {
        public bool Ondo { get; set; }
        public string Mezua { get; set; } = "";
        public int? Id { get; set; }
        public int? KantitateBerria { get; set; }
    }

    public class PlateraDisponibilitateaDto
    {
        public int Id { get; set; }
        public string Izena { get; set; } = "";
        public int? KategoriaId { get; set; }
        public string? KategoriaIzena { get; set; }
        public string Erabilgarri { get; set; } = "Bai";
        public int PrestatuDaitezkeenUnitateak { get; set; }
    }
}