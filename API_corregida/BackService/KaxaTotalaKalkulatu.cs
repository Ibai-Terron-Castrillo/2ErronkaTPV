using JatetxeaApi.Modeloak;
using Microsoft.Extensions.Hosting;
using NHibernate;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


//Jatetxeko kaxa totalaren kalkulua egiten duen background service-a.
namespace JatetxeaApi.BackService
{
    public class KaxaTotalaKalkulatu : BackgroundService
    {
        private readonly ISessionFactory _sessionFactory;

        public KaxaTotalaKalkulatu(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await KaxaEguneratu();

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                await KaxaEguneratu();
            }
        }

        private async Task KaxaEguneratu()
        {
            using var session = _sessionFactory.OpenSession();
            using var tx = session.BeginTransaction();

            try
            {
                var zerbitzuak = session.Query<Zerbitzuak>()
                    .Where(z => z.Guztira != null && z.Egoera == "Ordainduta")
                    .Select(z => z.Guztira)
                    .ToList();

                decimal guztira = zerbitzuak.Any()
                    ? zerbitzuak.Sum(z => z.Value)
                    : 0;

                var jatetxea = session.Query<JatetxekoInfo>().FirstOrDefault();
                if (jatetxea == null) return;

                jatetxea.KaxaTotal = guztira;
                session.Update(jatetxea);

                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }
    }
}
