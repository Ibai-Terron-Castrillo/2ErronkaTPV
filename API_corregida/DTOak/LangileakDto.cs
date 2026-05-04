using System;

namespace JatetxeaApi.DTOak
{
    public class LangileakDto
    {
        public int Id { get; set; }
        public string Izena { get; set; }
        public string Erabiltzailea { get; set; }
        public string Pasahitza { get; set; }
        public string Aktibo { get; set; }
        public DateTime? ErregistroData { get; set; }
        public int? RolaId { get; set; }
        public bool TxatBaimena { get; set; }
    }

    public class LangileakSortuDto
    {
        public string Izena { get; set; }
        public string Erabiltzailea { get; set; }
        public string Pasahitza { get; set; }
        public string Aktibo { get; set; } = "Bai";
        public DateTime? ErregistroData { get; set; }
        public int? RolaId { get; set; }
        public bool TxatBaimena { get; set; } = false;
    }

    public class LoginRequest
    {
        public string Erabiltzailea { get; set; }
        public string Pasahitza { get; set; }
    }
}
