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
    public class PlaterakRepository
    {
        private readonly NHSession _session;

        public PlaterakRepository(NHSessionFactory sessionFactory)
        {
            _session = sessionFactory.GetCurrentSession();
        }

        public PlaterakRepository()
        {
        }

        public virtual void Add(Platerak item)
        {
            using var tx = _session.BeginTransaction();
            _session.Save(item);
            tx.Commit();
        }

        public virtual Platerak? Get(int id)
        {
            return _session.Query<Platerak>().SingleOrDefault(x => x.Id == id);
        }

        public virtual Platerak? Get(String izena)
        {
            return _session.Query<Platerak>().SingleOrDefault(x => x.Izena == izena);
        }

        public virtual IList<Platerak> GetAll()
        {
            return _session.Query<Platerak>().ToList();
        }

        public virtual void Update(Platerak item)
        {
            using var tx = _session.BeginTransaction();
            _session.Update(item);
            tx.Commit();
        }

        public virtual void Delete(Platerak item)
        {
            using var tx = _session.BeginTransaction();
            _session.Delete(item);
            tx.Commit();
        }

        public IList<PlateraDisponibilitateaDto> GetDisponibilitatea()
        {
            using var session = NHibernateHelper.SessionFactory.OpenSession();

            var platerak = session.Query<Platerak>().ToList();
            var kategoriak = session.Query<Kategoria>().ToList();
            var errezetak = session.Query<PlaterenOsagaiak>().ToList();
            var inbentarioa = session.Query<Inbentarioa>().ToList();

            var kategoriaMap = kategoriak.ToDictionary(k => k.Id, k => k.Izena);
            var stockMap = inbentarioa.ToDictionary(i => i.Id, i => i.Kantitatea);

            var emaitza = new List<PlateraDisponibilitateaDto>();

            foreach (var platera in platerak)
            {
                var platerarenErrezeta = errezetak
                    .Where(x => x.PlateraId == platera.Id)
                    .ToList();

                int prestatuDaitezkeenUnitateak;

                if (!string.Equals(platera.Erabilgarri, "Bai", StringComparison.OrdinalIgnoreCase))
                {
                    prestatuDaitezkeenUnitateak = 0;
                }
                else if (!platerarenErrezeta.Any())
                {
                    prestatuDaitezkeenUnitateak = 0;
                }
                else
                {
                    var maximoak = new List<int>();

                    foreach (var osagaia in platerarenErrezeta)
                    {
                        if (!stockMap.TryGetValue(osagaia.InbentarioaId, out var stock))
                        {
                            maximoak.Add(0);
                            continue;
                        }

                        var beharrezkoa = (int)Math.Round(osagaia.Kantitatea, MidpointRounding.AwayFromZero);

                        if (beharrezkoa <= 0)
                        {
                            maximoak.Add(int.MaxValue);
                        }
                        else
                        {
                            maximoak.Add(stock / beharrezkoa);
                        }
                    }

                    prestatuDaitezkeenUnitateak = maximoak.Any() ? maximoak.Min() : 0;
                    if (prestatuDaitezkeenUnitateak < 0) prestatuDaitezkeenUnitateak = 0;
                }

                emaitza.Add(new PlateraDisponibilitateaDto
                {
                    Id = platera.Id,
                    Izena = platera.Izena,
                    KategoriaId = platera.KategoriaId,
                    KategoriaIzena = platera.KategoriaId.HasValue && kategoriaMap.ContainsKey(platera.KategoriaId.Value)
                        ? kategoriaMap[platera.KategoriaId.Value]
                        : null,
                    Erabilgarri = platera.Erabilgarri,
                    PrestatuDaitezkeenUnitateak = prestatuDaitezkeenUnitateak
                });
            }

            return emaitza
                .OrderBy(x => x.KategoriaIzena ?? "")
                .ThenBy(x => x.Izena)
                .ToList();
        }
    }
}
