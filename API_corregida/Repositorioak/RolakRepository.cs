using JatetxeaApi.Modeloak;
using NHibernate;
using System.Collections.Generic;
using System.Linq;
using NHSession = NHibernate.ISession;
using NHSessionFactory = NHibernate.ISessionFactory;

namespace JatetxeaApi.Repositorioak
{
    public class RolakRepository
    {
        private readonly NHSession _session;

        public RolakRepository(NHSessionFactory sessionFactory)
        {
            _session = sessionFactory.GetCurrentSession();
        }

        public RolakRepository()
        {
        }

        public virtual void Add(Rolak item)
        {
            using var tx = _session.BeginTransaction();
            _session.Save(item);
            tx.Commit();
        }

        public virtual Rolak? Get(int id)
        {
            return _session.Query<Rolak>().SingleOrDefault(x => x.Id == id);
        }

        public virtual Rolak? Get(string izena)
        {
            return _session.Query<Rolak>().SingleOrDefault(r => r.Izena == izena);
        }


        public virtual IList<Rolak> GetAll()
        {
            return _session.Query<Rolak>().ToList();
        }

        public virtual void Update(Rolak item)
        {
            using var tx = _session.BeginTransaction();
            _session.Update(item);
            tx.Commit();
        }

        public virtual void Delete(Rolak item)
        {
            using var tx = _session.BeginTransaction();
            _session.Delete(item);
            tx.Commit();
        }
    }
}
