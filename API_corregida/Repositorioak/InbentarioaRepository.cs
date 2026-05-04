using JatetxeaApi.DTOak;
using JatetxeaApi.Modeloak;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using NHSession = NHibernate.ISession;
using NHSessionFactory = NHibernate.ISessionFactory;

namespace JatetxeaApi.Repositorioak
{
    public class InbentarioaRepository
    {
        private readonly NHSession _session;

        public InbentarioaRepository(NHSessionFactory sessionFactory)
        {
            _session = sessionFactory.GetCurrentSession();
        }

        public virtual void Add(Inbentarioa item)
        {
            using var tx = _session.BeginTransaction();
            _session.Save(item);
            tx.Commit();
        }

        public virtual Inbentarioa? Get(int id)
        {
            return _session.Query<Inbentarioa>()
                .SingleOrDefault(x => x.Id == id);
        }

        public virtual Inbentarioa? Get(string izena)
        {
            return _session.Query<Inbentarioa>()
                .SingleOrDefault(x => x.Izena == izena);
        }

        public virtual IList<Inbentarioa> GetAll()
        {
            return _session.Query<Inbentarioa>().ToList();
        }

        public virtual void Update(Inbentarioa item)
        {
            using var tx = _session.BeginTransaction();
            _session.Update(item);
            tx.Commit();
        }

        public virtual void Delete(Inbentarioa item)
        {
            using var tx = _session.BeginTransaction();
            _session.Delete(item);
            tx.Commit();
        }

        public EragiketaEmaitzaDto AldatuKantitatea(int id, int aldaketa)
        {
            using var session = NHibernateHelper.SessionFactory.OpenSession();
            using var tx = session.BeginTransaction();

            try
            {
                var e = session.Get<Inbentarioa>(id);
                if (e == null)
                {
                    return new EragiketaEmaitzaDto
                    {
                        Ondo = false,
                        Mezua = "Ez da aurkitu",
                        Id = id
                    };
                }

                var berria = e.Kantitatea + aldaketa;
                if (berria < 0)
                {
                    return new EragiketaEmaitzaDto
                    {
                        Ondo = false,
                        Mezua = "Ezin da kantitatea 0 baino txikiagoa izan",
                        Id = id,
                        KantitateBerria = e.Kantitatea
                    };
                }

                e.Kantitatea = berria;
                e.AzkenEguneratzea = DateTime.Now;

                session.Update(e);
                tx.Commit();

                return new EragiketaEmaitzaDto
                {
                    Ondo = true,
                    Mezua = "Kantitatea eguneratuta",
                    Id = id,
                    KantitateBerria = berria
                };
            }
            catch (Exception ex)
            {
                if (tx.IsActive) tx.Rollback();

                return new EragiketaEmaitzaDto
                {
                    Ondo = false,
                    Mezua = $"Errorea kantitatea aldatzean: {ex.Message}",
                    Id = id
                };
            }
        }
    }
}