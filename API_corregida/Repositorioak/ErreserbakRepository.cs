using JatetxeaApi.Modeloak;

using NHibernate;

using System.Collections.Generic;

using System.Linq;

using NHSession = NHibernate.ISession;

using NHSessionFactory = NHibernate.ISessionFactory;
 
namespace JatetxeaApi.Repositorioak

{

    public class ErreserbakRepository

    {

        private readonly NHSession _session;
 
        public ErreserbakRepository(NHSessionFactory sessionFactory)

        {

            _session = sessionFactory.GetCurrentSession();

        }
 
        public virtual void Add(Erreserbak item)

        {

            using var tx = _session.BeginTransaction();

            _session.Save(item);

            tx.Commit();

        }
 
        public virtual Erreserbak? Get(int id)

        {

            return _session.Query<Erreserbak>().SingleOrDefault(x => x.Id == id);

        }
 
        public virtual IList<Erreserbak> GetAll()

        {

            return _session.Query<Erreserbak>().ToList();

        }
 
        public virtual void Update(Erreserbak item)

        {

            using var tx = _session.BeginTransaction();

            _session.Update(item);

            tx.Commit();

        }
 
        public virtual void Delete(Erreserbak item)

        {

            using var tx = _session.BeginTransaction();

            _session.Delete(item);

            tx.Commit();

        }
 
        public virtual IList<Erreserbak> GetGaur()

        {

            var gaur = DateTime.Today;

            var bihar = gaur.AddDays(1);
 
            return _session.Query<Erreserbak>()

                .Where(x => x.ErreserbaData.HasValue &&

                            x.ErreserbaData.Value >= gaur &&

                            x.ErreserbaData.Value < bihar)

                .OrderBy(x => x.ErreserbaData)

                .ToList();

        }
 
        public virtual IList<Erreserbak> GetEtorkizunak()

        {

            var orain = DateTime.Now;
 
            return _session.Query<Erreserbak>()

                .Where(x => x.ErreserbaData.HasValue &&

                            x.ErreserbaData.Value >= orain)

                .OrderBy(x => x.ErreserbaData)

                .ToList();

        }
 
        public virtual IList<Erreserbak> Bilatu(DateTime? data, TimeSpan? ordua)

        {

            var orain = DateTime.Now;

            var query = _session.Query<Erreserbak>()

                .Where(x => x.ErreserbaData.HasValue);
 
            if (data == null && ordua == null)

            {

                return query

                    .Where(x => x.ErreserbaData!.Value >= orain)

                    .OrderBy(x => x.ErreserbaData)

                    .ToList();

            }
 
            if (data != null && ordua == null)

            {

                var hasiera = data.Value.Date;

                var amaiera = hasiera.AddDays(1);
 
                return query

                    .Where(x => x.ErreserbaData!.Value >= hasiera &&

                                x.ErreserbaData!.Value < amaiera &&

                                x.ErreserbaData!.Value >= orain)

                    .OrderBy(x => x.ErreserbaData)

                    .ToList();

            }
 
            if (data == null && ordua != null)

            {

                var gaur = orain.Date;

                var helburua = gaur.Add(ordua.Value);
 
                return query

                    .Where(x => x.ErreserbaData!.Value == helburua)

                    .OrderBy(x => x.ErreserbaData)

                    .ToList();

            }
 
            var dataOrdua = data!.Value.Date.Add(ordua!.Value);
 
            return query

                .Where(x => x.ErreserbaData!.Value == dataOrdua)

                .OrderBy(x => x.ErreserbaData)

                .ToList();

        }

    }

}
 