namespace JatetxeaApi.Modeloak
{
    public class PlaterenOsagaiak
    {
        public virtual int PlateraId { get; set; }
        public virtual int InbentarioaId { get; set; }
        public virtual decimal Kantitatea { get; set; }

        public PlaterenOsagaiak() { }

        public PlaterenOsagaiak(int platareaId, decimal kantitatea) 
        {
            PlateraId = platareaId;
            Kantitatea = kantitatea;
        }

        public PlaterenOsagaiak(int platareaId, int inbentarioaId, decimal kantitatea)
        {
            PlateraId = platareaId;
            InbentarioaId = inbentarioaId;
            Kantitatea = kantitatea;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var other = (PlaterenOsagaiak)obj;
            return PlateraId == other.PlateraId && InbentarioaId == other.InbentarioaId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(PlateraId, InbentarioaId);
        }
    }
}
