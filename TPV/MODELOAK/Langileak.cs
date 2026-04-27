public class Langileak
{
    public virtual int Id { get; set; }
    public virtual string Izena { get; set; }
    public virtual string Erabiltzailea { get; set; }
    public virtual string Pasahitza { get; set; }
    public virtual string Aktibo { get; set; } = "Bai";
    public virtual DateTime? ErregistroData { get; set; }
    public virtual int? RolaId { get; set; }
}
