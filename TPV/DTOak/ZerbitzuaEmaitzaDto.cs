namespace TPV.DTOak
{
    public class ZerbitzuErroreaDto
    {
        public int PlateraId { get; set; }
        public string PlateraIzena { get; set; } = "";
        public virtual bool Zerbitzatuta { get; set; }
    }

    public class ZerbitzuaEmaitzaDto
    {
        public bool Ondo { get; set; }
        public int? ZerbitzuaId { get; set; }
        public List<ZerbitzuErroreaDto> Erroreak { get; set; } = new();
        public virtual bool Zerbitzatuta { get; set; }
    }
}
