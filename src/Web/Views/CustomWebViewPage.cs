using System.Web.Mvc;
using Infrastructure.ApplicationSettings;
using Infrastructure.Translations;

namespace Web.Views
{
    /// <summary>
    /// Custom web view page
    /// Adds CultureService, LocalizationService and Localize to the views
    /// </summary>
    /// <typeparam name="TModel">dynamic</typeparam>
    public abstract class CustomWebViewPage<TModel> : WebViewPage<TModel>
    {
        private ITranslationService _translationService;

        public IApplicationSettings Settings
        {
            get { return Resolve<IApplicationSettings>(); }
        }

        public dynamic T
        {
            get { return TranslationService.Translate; }
        }

        public ITranslationService TranslationService
        {
            get { return _translationService ?? (_translationService = Resolve<ITranslationService>()); }
        }

        // Shortcut to resolver
        private T Resolve<T>()
        {
            return DependencyResolver.Current.GetService<T>();
        }
    }
}