using FluentNHibernate.Mapping;
using JatetxeaApi.Modeloak;

namespace JatetxeaApi.Mapeoak
{
    public class RolakMap : ClassMap<Rolak>
    {
        public RolakMap()
        {
            Table("rolak");

            Id(x => x.Id)
                .Column("id")
                .GeneratedBy.Identity();

            Map(x => x.Izena)
                .Column("izena")
                .Not.Nullable();
        }
    }
}
