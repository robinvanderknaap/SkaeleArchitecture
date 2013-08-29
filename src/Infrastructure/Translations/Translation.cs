using System;
using System.Globalization;
using Infrastructure.DomainBase;

namespace Infrastructure.Translations
{
    public class Translation
    {
        private readonly string _code;
        private readonly string _text;
        private readonly CultureInfo _culture;

        protected Translation(){}

        public Translation
        (
            string code,
            string  text,
            CultureInfo culture
        )
        {
            _code = code.Required();
            _text = text.Required();
            _culture = culture.Required();
        }

        public virtual Guid Id { get; protected set; }

        public virtual CultureInfo Culture
        {
            get { return _culture; }
        }

        public virtual string Code
        {
            get { return _code; }
        }

        public virtual string Text
        {
            get { return _text; }
        }
    }
}
