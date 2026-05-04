namespace JatetxeaApi.Modeloak
{
    public class JatetxekoInfo
    {
        public virtual int Id { get; set; }
        public virtual string Izena { get; set; }
        public virtual decimal KaxaTotal { get; set; }
        public virtual string Helbidea { get; set; }
        public virtual int TelefonoZenbakia { get; set; }

        public JatetxekoInfo() { }

        public JatetxekoInfo(string izena, decimal kaxaTotal, string helbidea, int telefonoZenbakia)
        {
            Izena = izena;
            KaxaTotal = kaxaTotal;
            Helbidea = helbidea;
            TelefonoZenbakia = telefonoZenbakia;
        }
    }
}
