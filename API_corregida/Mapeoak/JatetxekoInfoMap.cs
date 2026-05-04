using FluentNHibernate.Mapping;
using JatetxeaApi.Modeloak;

namespace JatetxeaApi.Mapeoak
{
    public class JatetxekoInfoMap : ClassMap<JatetxekoInfo>
    {
        public JatetxekoInfoMap()
        {
            Table("jatetxeko_info");

            Id(x => x.Id)
                .Column("id")
                .GeneratedBy.Identity();

            Map(x => x.Izena)
                .Column("izena")
                .Not.Nullable();

            Map(x => x.KaxaTotal)
                .Column("kaxa_total")
                .Not.Nullable();

            Map(x => x.Helbidea)
                .Column("helbidea")
                .Not.Nullable();

            Map(x => x.TelefonoZenbakia)
                .Column("telefono_zenbakia")
                .Not.Nullable();
        }
    }
}
