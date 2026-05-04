using JatetxeaApi.Modeloak;
using NHibernate;
using System.Collections.Generic;
using System.Linq;
using NHSession = NHibernate.ISession;
using NHSessionFactory = NHibernate.ISessionFactory;

namespace JatetxeaApi.Repositorioak
{
    public class KategoriaRepository
    {
        private readonly NHSession _session;

        public KategoriaRepository(NHSessionFactory sessionFactory)
        {
            _session = sessionFactory.GetCurrentSession();
        }

        public virtual void Add(Kategoria item)
        {
            using var tx = _session.BeginTransaction();
            _session.Save(item);
            tx.Commit();
        }

        public virtual Kategoria? Get(int id)
        {
            return _session.Query<Kategoria>().SingleOrDefault(x => x.Id == id);
        }

        public virtual Kategoria? Get(string izena)
        {
            return _session.Query<Kategoria>().SingleOrDefault(x => x.Izena == izena);
        }

        public virtual IList<Kategoria> GetAll()
        {
            return _session.Query<Kategoria>().ToList();
        }

        public virtual void Update(Kategoria item)
        {
            using var tx = _session.BeginTransaction();
            _session.Update(item);
            tx.Commit();
        }

        public virtual void Delete(Kategoria item)
        {
            using var tx = _session.BeginTransaction();
            _session.Delete(item);
            tx.Commit();
        }
    }
}