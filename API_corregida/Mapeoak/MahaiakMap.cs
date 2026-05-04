using FluentNHibernate.Mapping;
using JatetxeaApi.Modeloak;

namespace JatetxeaApi.Mapeoak
{
    public class MahaiakMap : ClassMap<Mahaiak>
    {
        public MahaiakMap()
        {
            Table("mahaiak");

            Id(x => x.Id)
                .Column("id")
                .GeneratedBy.Identity();

            Map(x => x.MahaiaZbk)
                .Column("mahaia_zbk")
                .Not.Nullable();

            Map(x => x.Edukiera)
                .Column("edukiera")
                .Not.Nullable();

            Map(x => x.Egoera)
                .Column("egoera")
                .Nullable();
        }
    }
}
