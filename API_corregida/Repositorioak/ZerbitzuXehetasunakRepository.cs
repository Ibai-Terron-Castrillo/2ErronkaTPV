using JatetxeaApi.Modeloak;
using NHibernate;
using System.Collections.Generic;
using System.Linq;
using NHSession = NHibernate.ISession;
using NHSessionFactory = NHibernate.ISessionFactory;
using JatetxeaApi.DTOak;
using NHibernate.Linq;

namespace JatetxeaApi.Repositorioak
{
    public class ZerbitzuXehetasunakRepository
    {
        private readonly NHSession _session;

        public ZerbitzuXehetasunakRepository(NHSessionFactory sessionFactory)
        {
            _session = sessionFactory.GetCurrentSession();
        }

        public ZerbitzuXehetasunakRepository()
        {
        }


        public virtual void Add(ZerbitzuXehetasunak item)
        {
            using var tx = _session.BeginTransaction();
            _session.Save(item);
            tx.Commit();
        }

        public virtual ZerbitzuXehetasunak? Get(int id)
        {
            return _session.Query<ZerbitzuXehetasunak>().SingleOrDefault(x => x.Id == id);
        }

        public virtual IList<ZerbitzuXehetasunak> GetAll()
        {
            return _session.Query<ZerbitzuXehetasunak>().ToList();
        }

        public virtual void Update(ZerbitzuXehetasunak item)
        {
            using var tx = _session.BeginTransaction();
            _session.Update(item);
            tx.Commit();
        }

        public virtual void Delete(ZerbitzuXehetasunak item)
        {
            using var tx = _session.BeginTransaction();
            _session.Delete(item);
            tx.Commit();
        }

        public IList<ZerbitzuXehetasunak> GetByZerbitzuaId(int zerbitzuaId)
        {
            using var session = NHibernateHelper.SessionFactory.OpenSession();
            return session.Query<ZerbitzuXehetasunak>()
                .Where(x => x.ZerbitzuaId == zerbitzuaId)
                .OrderBy(x => x.Id)
                .ToList();
        }

        public EragiketaEmaitzaDto AldatuZerbitzatutaEtaStock(int id, bool zerbitzatutaBerria)
        {
            using var session = NHibernateHelper.SessionFactory.OpenSession();
            using var tx = session.BeginTransaction();

            try
            {
                var xehetasuna = session.Get<ZerbitzuXehetasunak>(id);
                if (xehetasuna == null)
                {
                    return new EragiketaEmaitzaDto
                    {
                        Ondo = false,
                        Mezua = "Ez da zerbitzu xehetasuna aurkitu",
                        Id = id
                    };
                }

                if (xehetasuna.Zerbitzatuta == zerbitzatutaBerria)
                {
                    return new EragiketaEmaitzaDto
                    {
                        Ondo = true,
                        Mezua = "Ez dago aldaketarik",
                        Id = id
                    };
                }

                var errezeta = session.Query<PlaterenOsagaiak>()
                    .Where(x => x.PlateraId == xehetasuna.PlateraId)
                    .ToList();

                foreach (var osagaia in errezeta)
                {
                    var inbentarioa = session.Get<Inbentarioa>(osagaia.InbentarioaId);
                    if (inbentarioa == null)
                    {
                        tx.Rollback();
                        return new EragiketaEmaitzaDto
                        {
                            Ondo = false,
                            Mezua = $"Ez da inbentarioko osagaia aurkitu (id={osagaia.InbentarioaId})",
                            Id = id
                        };
                    }

                    var oinarrizkoAldaketa = (int)Math.Round(osagaia.Kantitatea * xehetasuna.Kantitatea, MidpointRounding.AwayFromZero);
                    var aldaketa = zerbitzatutaBerria ? -oinarrizkoAldaketa : oinarrizkoAldaketa;

                    var kantitateBerria = inbentarioa.Kantitatea + aldaketa;
                    if (kantitateBerria < 0)
                    {
                        tx.Rollback();
                        return new EragiketaEmaitzaDto
                        {
                            Ondo = false,
                            Mezua = $"Ez dago stock nahikorik osagai honentzat: {inbentarioa.Izena}",
                            Id = id
                        };
                    }

                    inbentarioa.Kantitatea = kantitateBerria;
                    inbentarioa.AzkenEguneratzea = DateTime.Now;
                    session.Update(inbentarioa);
                }

                xehetasuna.Zerbitzatuta = zerbitzatutaBerria;
                session.Update(xehetasuna);

                tx.Commit();

                return new EragiketaEmaitzaDto
                {
                    Ondo = true,
                    Mezua = zerbitzatutaBerria ? "Platera eginda eta stock eguneratuta" : "Platera atzera eta stock leheneratuta",
                    Id = id
                };
            }
            catch (Exception ex)
            {
                if (tx.IsActive) tx.Rollback();

                return new EragiketaEmaitzaDto
                {
                    Ondo = false,
                    Mezua = $"Errorea zerbitzatuta aldatzean: {ex.Message}",
                    Id = id
                };
            }
        }
    }
}
