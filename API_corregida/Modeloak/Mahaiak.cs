namespace JatetxeaApi.Modeloak
{
    public class Mahaiak
    {
        public virtual int Id { get; set; }
        public virtual int MahaiaZbk { get; set; }
        public virtual int Edukiera { get; set; }
        public virtual string? Egoera { get; set; } = "Libre";

        public Mahaiak() { }

        public Mahaiak(int mahaiaZbk, int edukiera, string? egoera = "Libre")
        {
            MahaiaZbk = mahaiaZbk;
            Edukiera = edukiera;
            Egoera = egoera;
        }
    }
}
