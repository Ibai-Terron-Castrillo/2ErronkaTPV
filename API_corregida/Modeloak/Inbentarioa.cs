namespace JatetxeaApi.Modeloak
{
    public class Inbentarioa
    {
        public virtual int Id { get; set; }
        public virtual string Izena { get; set; }
        public virtual string? Deskribapena { get; set; }
        public virtual int Kantitatea { get; set; }
        public virtual string? NeurriaUnitatea { get; set; }
        public virtual int StockMinimoa { get; set; }
        public virtual DateTime? AzkenEguneratzea { get; set; }

        public Inbentarioa()
        {
        }

        public Inbentarioa(string izena, int kantitatea, int stockMinimoa)
        {
            Izena = izena;
            Kantitatea = kantitatea;
            StockMinimoa = stockMinimoa;
            AzkenEguneratzea = DateTime.Now;
        }
    }
}
