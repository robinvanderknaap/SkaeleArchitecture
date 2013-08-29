using System.Dynamic;
using System.Globalization;
using Infrastructure.ApplicationSettings;
using Infrastructure.Translations;
using RazorMailMessage.TemplateBase;

namespace Infrastructure.TemplateMailMessages
{
    public class ViewBaseClass<TModel> : DefaultTemplateBase<TModel>
    {
        private readonly ITranslationService _translationService;
        private CultureInfo _culture;

        public ViewBaseClass(ITranslationService translationService, IApplicationSettings applicationSettings)
        {
            _translationService = translationService;
            ApplicationSettings = applicationSettings;
        }

        public IApplicationSettings ApplicationSettings { get; private set; }

        public CultureInfo Culture
        {
            get { return _culture ?? ApplicationSettings.DefaultCulture; }
            set { _culture = value; }
        }

        public dynamic T
        {
            get { return new DynamicTranslationService(_translationService, Culture); }
        }

        private class DynamicTranslationService : DynamicObject
        {
            private readonly ITranslationService _translationService;
            private readonly CultureInfo _culture;

            public DynamicTranslationService(ITranslationService translationService, CultureInfo culture)
            {
                _translationService = translationService;
                _culture = culture;
            }

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                result = _translationService.Translate(binder.Name, _culture);

                return true;
            }
        }
    }
}
