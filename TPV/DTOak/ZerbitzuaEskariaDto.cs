namespace TPV.DTOak
{
    public class PlateraEskariaDto
    {
        public int PlateraId { get; set; }
        public int Kantitatea { get; set; }
        public virtual bool Zerbitzatuta { get; set; }
    }

    public class ZerbitzuaEskariaDto
    {
        public int LangileId { get; set; }
        public int MahaiaId { get; set; }
        public int ErreserbaId { get; set; }
        public List<PlateraEskariaDto> Platerak { get; set; } = new();
        public string? Egoera { get; set; }
        public virtual bool Zerbitzatuta { get; set; }
    }
}
