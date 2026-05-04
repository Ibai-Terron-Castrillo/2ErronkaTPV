namespace GastuakApi.DTOak
{
    public class ZerbitzuEgoeraPatchDto
    {
        public string Egoera { get; set; } = "";
    }

    public class ZerbitzuLaburpenaDto
    {
        public int ZerbitzuaId { get; set; }
        public int ZerbitzuXehetasunaId { get; set; }
        public int PlateraId { get; set; }
        public string PlateraIzena { get; set; } = "";
        public int? KategoriaId { get; set; }
        public string? KategoriaIzena { get; set; }
        public int Kantitatea { get; set; }
        public decimal PrezioUnitarioa { get; set; }
        public bool Zerbitzatuta { get; set; }
    }
}