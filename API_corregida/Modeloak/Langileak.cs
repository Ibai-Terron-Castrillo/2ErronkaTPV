using System;

namespace JatetxeaApi.Modeloak
{
    public class Langileak
    {
        public virtual int Id { get; set; }
        public virtual string Izena { get; set; }
        public virtual string Erabiltzailea { get; set; }
        public virtual string Pasahitza { get; set; }
        public virtual string Aktibo { get; set; } = "Bai";
        public virtual DateTime? ErregistroData { get; set; }
        public virtual int? RolaId { get; set; }
        public virtual bool TxatBaimena { get; set; } = false;

        public Langileak() { }

        public Langileak(string izena, string erabiltzailea, string pasahitza, string aktibo = "Bai", DateTime? erregistroData = null, int? rolaId = null, bool txatBaimena = false)
        {
            Izena = izena;
            Erabiltzailea = erabiltzailea;
            Pasahitza = pasahitza;
            Aktibo = aktibo;
            ErregistroData = erregistroData ?? DateTime.Now;
            RolaId = rolaId;
            TxatBaimena = txatBaimena;
        }
    }
}
