using JatetxeaApi.Modeloak;
using NHibernate;
using System.Collections.Generic;
using System.Linq;
using NHSession = NHibernate.ISession;
using NHSessionFactory = NHibernate.ISessionFactory;

namespace JatetxeaApi.Repositorioak
{
    public class MahaiakRepository
    {
        private readonly NHSession _session;

        public MahaiakRepository(NHSessionFactory sessionFactory)
        {
            _session = sessionFactory.GetCurrentSession();
        }

        public MahaiakRepository()
        {
        }  

        public virtual void Add(Mahaiak item)
        {
            using var tx = _session.BeginTransaction();
            _session.Save(item);
            tx.Commit();
        }

        public virtual Mahaiak? Get(int mahaiazbk)
        {
            return _session.Query<Mahaiak>().SingleOrDefault(x => x.MahaiaZbk == mahaiazbk);
        }

        public virtual IList<Mahaiak> GetAll()
        {
            return _session.Query<Mahaiak>().ToList();
        }

        public virtual void Update(Mahaiak item)
        {
            using var tx = _session.BeginTransaction();
            _session.Update(item);
            tx.Commit();
        }

        public virtual void Delete(Mahaiak item)
        {
            using var tx = _session.BeginTransaction();
            _session.Delete(item);
            tx.Commit();
        }
        public virtual IList<Mahaiak> GetByIds(IEnumerable<int> ids)
        {
            var idList = ids.Distinct().ToList();
            if (!idList.Any()) return new List<Mahaiak>();

            return _session.Query<Mahaiak>()
                .Where(x => idList.Contains(x.Id))
                .ToList();
        }

    }
}
