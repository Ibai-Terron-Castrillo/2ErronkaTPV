namespace JatetxeaApi.DTOak
{
    public class MahaiakDto
    {
        public int Id { get; set; }
        public int MahaiaZbk { get; set; }
        public int Edukiera { get; set; }
        public string? Egoera { get; set; }
    }

    public class MahaiakSortuDto
    {
        public int MahaiaZbk { get; set; }
        public int Edukiera { get; set; }
        public string? Egoera { get; set; } = "Libre";
    }
}
