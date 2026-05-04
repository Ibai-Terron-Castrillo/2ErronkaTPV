using FluentNHibernate.Mapping;
using JatetxeaApi.Modeloak;

namespace JatetxeaApi.Mapeoak
{
    public class ZerbituzakMap : ClassMap<Zerbitzuak>
    {
        public ZerbituzakMap()
        {
            Table("zerbitzuak");

            Id(x => x.Id)
                .Column("id")
                .GeneratedBy.Identity();

            Map(x => x.LangileId)
                .Column("langile_id")
                .Not.Nullable();

            Map(x => x.MahaiaId)
                .Column("mahaia_id")
                .Not.Nullable();

            Map(x => x.ErreserbaId)
                .Column("erreserba_id")
                .Nullable();

            Map(x => x.EskaeraData)
                .Column("eskaera_data")
                .Nullable();

            Map(x => x.Egoera)
                .Column("egoera")
                .Not.Nullable();

            Map(x => x.Guztira)
                .Column("guztira")
                .Nullable();
        }
    }
}
