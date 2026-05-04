using FluentNHibernate.Mapping;
using JatetxeaApi.Modeloak;

namespace JatetxeaApi.Mapeoak
{
    public class InbentarioaMap : ClassMap<Inbentarioa>
    {
        public InbentarioaMap()
        {
            Table("inbentarioa");

            Id(x => x.Id)
                .Column("id")
                .GeneratedBy.Identity();

            Map(x => x.Izena)
                .Column("izena")
                .Not.Nullable();

            Map(x => x.Deskribapena)
                .Column("deskribapena")
                .Nullable();

            Map(x => x.Kantitatea)
                .Column("kantitatea")
                .Not.Nullable();

            Map(x => x.NeurriaUnitatea)
                .Column("neurria_unitatea")
                .Nullable();

            Map(x => x.StockMinimoa)
                .Column("stock_minimoa")
                .Not.Nullable();

            Map(x => x.AzkenEguneratzea)
                .Column("azken_eguneratzea")
                .Nullable();
        }
    }
}
