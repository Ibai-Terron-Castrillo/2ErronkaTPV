using FluentNHibernate.Mapping;
using JatetxeaApi.Modeloak;

namespace JatetxeaApi.Mapeoak
{
    public class PlaterenOsagaiakMap : ClassMap<PlaterenOsagaiak>
    {
        public PlaterenOsagaiakMap()
        {
            Table("plateren_osagaiak");

            CompositeId()
                .KeyProperty(x => x.PlateraId, "platera_id")
                .KeyProperty(x => x.InbentarioaId, "inbentarioa_id");

            Map(x => x.Kantitatea)
                .Column("kantitatea")
                .Not.Nullable();
        }
    }
}
