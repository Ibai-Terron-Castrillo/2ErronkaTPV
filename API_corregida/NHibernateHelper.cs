using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using JatetxeaApi.Mapeoak;
using NHibernate;
using NHibernate.Tool.hbm2ddl;

namespace JatetxeaApi
{
    public class NHibernateHelper
    {
        private static ISessionFactory _sessionFactory;

        public static ISessionFactory SessionFactory =>
            _sessionFactory ??= CreateSessionFactory();

        private static ISessionFactory CreateSessionFactory()
        {
            var config = Fluently.Configure()
                .Database(MySQLConfiguration.Standard
                    .ConnectionString("Server=localhost;Port=3306;Database=jatetxea;Uid=root;Pwd=abc123ABC;"))
                .Mappings(m =>
                {
                    m.FluentMappings.AddFromAssemblyOf<InbentarioaMap>();
                    m.FluentMappings.AddFromAssemblyOf<PlaterenOsagaiakMap>();
                    m.FluentMappings.AddFromAssemblyOf<PlaterakMap>();
                    m.FluentMappings.AddFromAssemblyOf<ZerbitzuXehetasunakMap>();
                    m.FluentMappings.AddFromAssemblyOf<KategoriaMap>();
                    m.FluentMappings.AddFromAssemblyOf<ZerbituzakMap>();
                    m.FluentMappings.AddFromAssemblyOf<LangileakMap>();
                    m.FluentMappings.AddFromAssemblyOf<RolakMap>();
                    m.FluentMappings.AddFromAssemblyOf<MahaiakMap>();
                    m.FluentMappings.AddFromAssemblyOf<ErreserbakMap>();
                    m.FluentMappings.AddFromAssemblyOf<JatetxekoInfoMap>();
                })
                .ExposeConfiguration(cfg =>
                {
                    cfg.SetProperty("current_session_context_class", "async_local");
                })
                .BuildConfiguration();

            return config.BuildSessionFactory();
        }

        public static void dbEguneratu(NHibernate.Cfg.Configuration config)
        {
            var schemaUpdate = new SchemaUpdate(config);
            schemaUpdate.Execute(false, true);
        }

        public static void dbBirSortu(NHibernate.Cfg.Configuration config)
        {
            var schemaExport = new SchemaExport(config);
            schemaExport.Drop(true, true);
            schemaExport.Create(true, true);
        }
    }
}
