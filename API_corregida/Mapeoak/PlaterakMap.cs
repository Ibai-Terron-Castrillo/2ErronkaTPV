using FluentNHibernate.Mapping;
using JatetxeaApi.Modeloak;

namespace JatetxeaApi.Mapeoak
{
    public class PlaterakMap : ClassMap<Platerak>
    {
        public PlaterakMap()
        {
            Table("platerak");

            Id(x => x.Id)
                .Column("id")
                .GeneratedBy.Identity();

            Map(x => x.Izena)
                .Column("izena")
                .Not.Nullable();

            Map(x => x.Deskribapena)
                .Column("deskribapena")
                .Nullable();

            Map(x => x.Prezioa)
                .Column("prezioa")
                .Not.Nullable();

            Map(x => x.KategoriaId)
                .Column("kategoria_id")
                .Nullable();

            Map(x => x.Erabilgarri)
                .Column("erabilgarri")
                .Not.Nullable();

            Map(x => x.SortzeData)
                .Column("sortze_data")
                .Nullable();

            Map(x => x.Irudia)
                .Column("irudia")
                .Nullable();
        }
    }
}
