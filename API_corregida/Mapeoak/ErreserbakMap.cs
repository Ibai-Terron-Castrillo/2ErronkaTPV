using FluentNHibernate.Mapping;
using JatetxeaApi.Modeloak;

namespace JatetxeaApi.Mapeoak
{
    public class ErreserbakMap : ClassMap<Erreserbak>
    {
        public ErreserbakMap()
        {
            Table("erreserbak");

            Id(x => x.Id)
                .Column("id")
                .GeneratedBy.Identity();

            Map(x => x.MahaiaId)
                .Column("mahaia_id")
                .Not.Nullable();

            Map(x => x.Izena)
                .Column("izena")
                .Not.Nullable();

            Map(x => x.Telefonoa)
                .Column("telefonoa")
                .Not.Nullable();

            Map(x => x.ErreserbaData)
                .Column("erreserba_data")
                .Nullable();

            Map(x => x.PertsonaKop)
                .Column("pertsona_kop")
                .Nullable();

            Map(x => x.Egoera)
                .Column("egoera")
                .Not.Nullable();

            Map(x => x.Oharrak)
                .Column("oharrak")
                .Nullable();
        }
    }
}
