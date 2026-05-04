namespace JatetxeaApi.Modeloak
{
    public class Kategoria
    {
        public virtual int Id { get; set; }  
        public virtual string Izena { get; set; }

        public Kategoria() { }

        public Kategoria(string izena)
        {
            Izena = izena;
        }
    }
}
