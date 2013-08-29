using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Cache;
using Infrastructure.Translations;
using NHibernate;
using NHibernate.Linq;

namespace Data
{
    public class TranslationRepository : ITranslationRepository
    {
        private readonly ISession _session;
        private readonly IKeyValueCache _keyValueCache;
        private const string CacheKey = "db.Translations";

        public TranslationRepository(
            ISession session,
            IKeyValueCache keyValueCache
        )
        {
            _session = session;
            _keyValueCache = keyValueCache;
        }

        public void Save(Translation translation)
        {
            _session.Save(translation);
            _keyValueCache.Remove(CacheKey);
        }

        public Translation Get(Guid id)
        {
            return _session.Load<Translation>(id);
        }

        public IEnumerable<Translation> GetAll()
        {
            var translations = _keyValueCache.Get(CacheKey);

            if (translations == null)
            {
                _keyValueCache.Add(CacheKey, _session.Query<Translation>().ToList());
            }

            return _keyValueCache.Get(CacheKey);
        }
    }
}
