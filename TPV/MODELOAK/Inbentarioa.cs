namespace TPV.MODELOAK
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
    }
}
