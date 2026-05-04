using FluentNHibernate.Mapping;
using JatetxeaApi.Modeloak;

namespace JatetxeaApi.Mapeoak
{
    public class ZerbitzuXehetasunakMap : ClassMap<ZerbitzuXehetasunak>
    {
        public ZerbitzuXehetasunakMap()
        {
            Table("zerbitzu_xehetasunak");

            Id(x => x.Id)
                .Column("id")
                .GeneratedBy.Identity();

            Map(x => x.ZerbitzuaId)
                .Column("zerbitzua_id")
                .Not.Nullable();

            Map(x => x.PlateraId)
                .Column("platera_id")
                .Not.Nullable();

            Map(x => x.Kantitatea)
                .Column("kantitatea")
                .Not.Nullable();

            Map(x => x.PrezioUnitarioa)
                .Column("prezio_unitarioa")
                .Not.Nullable();

            Map(x => x.Zerbitzatuta)
                .Column("zerbitzatuta")
                .Not.Nullable();

        }
    }
}
