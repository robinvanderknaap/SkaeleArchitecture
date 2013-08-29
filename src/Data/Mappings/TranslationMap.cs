using FluentNHibernate.Mapping;
using Infrastructure.Translations;

namespace Data.Mappings
{
    public class TranslationMap : ClassMap<Translation>
    {
        public TranslationMap()
        {
            Table("Translations");

            Id(x => x.Id);

            Map(x => x.Culture)
                .Access.CamelCaseField(Prefix.Underscore)
                .Not.Nullable();
            Map(x => x.Code)
                .Access.CamelCaseField(Prefix.Underscore)
                .Not.Nullable();
            Map(x => x.Text)
                .Length(9999)
                .Access.CamelCaseField(Prefix.Underscore)
                .Not.Nullable();
        }
    }
}
