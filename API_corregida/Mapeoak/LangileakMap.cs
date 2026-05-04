using FluentNHibernate.Mapping;
using JatetxeaApi.Modeloak;

namespace JatetxeaApi.Mapeoak
{
    public class LangileakMap : ClassMap<Langileak>
    {
        public LangileakMap()
        {
            Table("langileak");

            Id(x => x.Id)
                .Column("id")
                .GeneratedBy.Identity();

            Map(x => x.Izena)
                .Column("izena")
                .Not.Nullable();

            Map(x => x.Erabiltzailea)
                .Column("erabiltzailea")
                .Not.Nullable();

            Map(x => x.Pasahitza)
                .Column("pasahitza")
                .Not.Nullable();

            Map(x => x.Aktibo)
                .Column("aktibo")
                .Not.Nullable();

            Map(x => x.ErregistroData)
                .Column("erregistro_data")
                .Nullable();

            Map(x => x.RolaId)
                .Column("rola_id")
                .Nullable();

            Map(x => x.TxatBaimena)
                .Column("txat_baimena")
                .Not.Nullable()
                .Default("0");
        }
    }
}
