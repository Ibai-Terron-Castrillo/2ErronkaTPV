using JatetxeaApi.Modeloak;
using NHibernate;
using System.Collections.Generic;
using System.Linq;
using NHSession = NHibernate.ISession;
using NHSessionFactory = NHibernate.ISessionFactory;

namespace JatetxeaApi.Repositorioak
{
    public class JatetxekoInfoRepository
    {
        private readonly NHSession _session;

        public JatetxekoInfoRepository(NHSessionFactory sessionFactory)
        {
            _session = sessionFactory.GetCurrentSession();
        }

        public virtual void Add(JatetxekoInfo item)
        {
            using var tx = _session.BeginTransaction();
            _session.Save(item);
            tx.Commit();
        }

        public virtual JatetxekoInfo? Get(int id)
        {
            return _session.Query<JatetxekoInfo>().SingleOrDefault(x => x.Id == id);
        }

        public virtual JatetxekoInfo? Get(string izena)
        {
            return _session.Query<JatetxekoInfo>().SingleOrDefault(x => x.Izena == izena);
        }

        public virtual IList<JatetxekoInfo> GetAll()
        {
            return _session.Query<JatetxekoInfo>().ToList();
        }

        public virtual void Update(JatetxekoInfo item)
        {
            using var tx = _session.BeginTransaction();
            _session.Update(item);
            tx.Commit();
        }

        public virtual void Delete(JatetxekoInfo item)
        {
            using var tx = _session.BeginTransaction();
            _session.Delete(item);
            tx.Commit();
        }
    }
}