using FluentNHibernate.Mapping;
using JatetxeaApi.Modeloak;

namespace JatetxeaApi.Mapeoak
{
    public class KategoriaMap : ClassMap<Kategoria>
    {
        public KategoriaMap()
        {
            Table("kategoriak");

            Id(x => x.Id)
                .Column("id")
                .GeneratedBy.Identity();

            Map(x => x.Izena)
                .Column("izena")
                .Not.Nullable();
        }
    }
}
