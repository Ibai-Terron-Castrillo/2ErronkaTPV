namespace JatetxeaApi.Modeloak
{
    public class Rolak
    {
        public virtual int Id { get; set; }
        public virtual string Izena { get; set; }

        public Rolak() { }

        public Rolak(string izena)
        {
            Izena = izena;
        }
    }
}
