namespace TPV
{
    public static class SaioGlobala
    {
        public static int LangileId { get; private set; }
        public static int RolaId { get; private set; }
        public static string ErabiltzaileIzena { get; private set; } = "";
        public static string? Tokena { get; private set; }
        public static bool TxatBaimenduta { get; private set; }

        public static void Ezarri(int langileId, int rolaId, string erabiltzaileIzena, string? tokena = null, bool? txatBaimenduta = null)
        {
            LangileId = langileId;
            RolaId = rolaId;
            ErabiltzaileIzena = erabiltzaileIzena ?? "";
            Tokena = tokena;
            TxatBaimenduta = txatBaimenduta ?? false;

            if (!TxatBaimenduta)
            {
                try { TxatBezeroa.Instantzia.Deskonektatu(); } catch { }
            }
        }

        public static void Garbitu()
        {
            try { TxatBezeroa.Instantzia.Deskonektatu(); } catch { }
            LangileId = 0;
            RolaId = 0;
            ErabiltzaileIzena = "";
            Tokena = null;
            TxatBaimenduta = false;
        }
    }
}
