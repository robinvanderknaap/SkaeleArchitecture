using System;
using System.Collections.Generic;

namespace Infrastructure.Translations
{
    public interface ITranslationRepository
    {
        void Save(Translation translation);
        Translation Get(Guid id);
        IEnumerable<Translation> GetAll();
    }
}
