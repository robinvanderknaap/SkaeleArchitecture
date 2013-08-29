using Domain.Users;
using FluentNHibernate.Mapping;

namespace Data.Mappings
{
    public class UserMap : ClassMap<User>
    {
        public UserMap()
        {
            Table("Users");
            Id(user => user.Id);
            Map(user => user.Email)
                .Not.Nullable()
                .Length(255)
                .Unique()
                .Access.CamelCaseField(Prefix.Underscore);
            Map(user => user.DisplayName)
                .Not.Nullable()
                .Length(100)
                .Access.CamelCaseField(Prefix.Underscore);
            Map(user => user.Culture)
                .Not.Nullable()
                .Access.CamelCaseField(Prefix.Underscore);
            Map(user => user.IsActive)
                .Not.Nullable();
            HasMany(x => x.Roles)
               .Table("User_Roles")
               .Element("Role")
               .AsSet()
               .Access.CamelCaseField(Prefix.Underscore)
               .Not.LazyLoad();

            Map(x => x.Modified)
                .Not.Nullable();
            Map(x => x.Created)
                .Not.Nullable();
        }
    }
}
