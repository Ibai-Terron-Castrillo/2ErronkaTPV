using JatetxeaApi.Modeloak;
using NHibernate;
using System.Collections.Generic;
using System.Linq;
using NHSession = NHibernate.ISession;
using NHSessionFactory = NHibernate.ISessionFactory;

namespace JatetxeaApi.Repositorioak
{
    public class LangileakRepository
    {
        private readonly NHSession _session;

        public LangileakRepository(NHSessionFactory sessionFactory)
        {
            _session = sessionFactory.GetCurrentSession();
        }

        public LangileakRepository()
        {
        }

        public virtual void Add(Langileak item)
        {
            using var tx = _session.BeginTransaction();
            _session.Save(item);
            tx.Commit();
        }

        public virtual Langileak? Get(int id)
        {
            return _session.Query<Langileak>().SingleOrDefault(x => x.Id == id);
        }

        public virtual Langileak? Get(string izena)
        {
            return _session.Query<Langileak>().SingleOrDefault(x => x.Izena == izena);
        }

        public virtual IList<Langileak> GetAll()
        {
            return _session.Query<Langileak>().ToList();
        }

        public virtual void Update(Langileak item)
        {
            using var tx = _session.BeginTransaction();
            _session.Update(item);
            tx.Commit();
        }

        public virtual void Delete(Langileak item)
        {
            using var tx = _session.BeginTransaction();
            _session.Delete(item);
            tx.Commit();
        }
    }
}
