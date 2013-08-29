using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;

namespace Infrastructure.Translations
{
    public class TranslationService : ITranslationService
    {
        private readonly IEnumerable<Translation> _translations;
        private readonly CultureInfo _culture;

        public TranslationService(IEnumerable<Translation> translations, CultureInfo culture)
        {
            if (translations == null)
            {
                throw new NullReferenceException("Translations are missing");
            }

            _translations = translations;
            _culture = culture;
        }

        public dynamic Translate
        {
            get
            {
                return new DynamicTranslatableObject(
                    TranslationLookupFunction
                );
            }
        }

        private Func<string, CultureInfo, string> TranslationLookupFunction
        {
            get
            {
                return (code, culture) =>
                {
                    culture = culture ?? _culture;
                    
                    var result = _translations.FirstOrDefault(x => x.Code.Equals(code, StringComparison.InvariantCultureIgnoreCase) && x.Culture.Equals(culture)) ??
                                     _translations.FirstOrDefault(x => x.Code.Equals(code, StringComparison.InvariantCultureIgnoreCase) && x.Culture.Equals(_culture));

                    return result == null ? code : result.Text;
                };
            }
        }
        
        private class DynamicTranslatableObject : DynamicObject
        {
            private readonly Func<string, CultureInfo, string> _translationLookupFunction;

            public DynamicTranslatableObject(Func<string, CultureInfo, string> translationLookupFunction)
            {
                _translationLookupFunction = translationLookupFunction;
            }

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                result = _translationLookupFunction(binder.Name, null);

                return true;
            }

            public override bool TryInvoke(InvokeBinder binder, object[] args, out object result)
            {
                result = string.Empty;

                if (args.Count() == 1)
                {
                    var code = args[0].ToString();

                    result = _translationLookupFunction(code, null);
                }
                else if (args.Count() == 2)
                {
                    result = _translationLookupFunction(args[0].ToString(), (CultureInfo)args[1]);
                }

                return true;
            }
        }
    }
}