namespace TPV.MODELOAK
{
    public class Platerak
    {
        public virtual int Id { get; set; }
        public virtual string Izena { get; set; }
        public virtual string? Deskribapena { get; set; }
        public virtual decimal Prezioa { get; set; }
        public virtual int? KategoriaId { get; set; }
        public virtual string Erabilgarri { get; set; }
        public virtual DateTime? SortzeData { get; set; }
        public virtual string? Irudia { get; set; }
    }
}
