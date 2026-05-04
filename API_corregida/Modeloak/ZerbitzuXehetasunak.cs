namespace JatetxeaApi.Modeloak
{
    public class ZerbitzuXehetasunak
    {
        public virtual int Id { get; set; }
        public virtual int ZerbitzuaId { get; set; }
        public virtual int PlateraId { get; set; }
        public virtual int Kantitatea { get; set; }
        public virtual decimal PrezioUnitarioa { get; set; }
        public virtual bool Zerbitzatuta { get; set; }

        public ZerbitzuXehetasunak() { }

        public ZerbitzuXehetasunak(int zerbitzuaId, int plateraId, int kantitatea, decimal prezioUnitarioa, bool zerbitzatuta)
        {
            ZerbitzuaId = zerbitzuaId;
            PlateraId = plateraId;
            Kantitatea = kantitatea;
            PrezioUnitarioa = prezioUnitarioa;
            Zerbitzatuta = zerbitzatuta;
        }
    }
}
