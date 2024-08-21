using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace WpfGram.Utils
{
    public sealed class EventArgsExtension : MarkupExtension
    {
        /// <summary>
        /// Gets or sets the path to the binding source property.
        /// </summary>
        public PropertyPath? Path { get; set; }

        /// <summary>
        /// Gets or sets the converter to use.
        /// </summary>
        public IValueConverter? Converter { get; set; }

        /// <summary>
        /// Gets or sets the parameter to pass to the <see cref="Converter"/>.
        /// </summary>
        public object? ConverterParameter { get; set; }

        /// <summary>
        /// Gets or sets the converter target type to pass to the <see cref="Converter"/>. Default is '<see cref="object"/>'.
        /// </summary>
        public Type ConverterTargetType { get; set; } = typeof(object);

        /// <summary>
        /// Gets or sets the culture in which to evaluate the converter.
        /// </summary>
        [TypeConverter(typeof(CultureInfoIetfLanguageTagConverter))]
        public CultureInfo? ConverterCulture { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventArgsExtension"/> class.
        /// </summary>
        public EventArgsExtension() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventArgsExtension"/> class using the specified path.
        /// </summary>
        public EventArgsExtension(string path)
        {
            Path = new PropertyPath(path);
        }

        /// <inheritdoc/>
        public override object ProvideValue(IServiceProvider serviceProvider) => this;

        internal object? GetArgumentValue(EventArgs eventArgs, XmlLanguage? language)
        {
            if (Path == null)
                return eventArgs;

            object? value = PropertyPathHelper.Evaluate(Path, eventArgs);

            if (Converter != null)
                value = Converter.Convert(value, ConverterTargetType, ConverterParameter, ConverterCulture ?? language?.GetSpecificCulture() ?? CultureInfo.CurrentUICulture);

            return value;
        }
        internal static class PropertyPathHelper
        {
            private static readonly object s_fallbackValue = new object();

            public static object? Evaluate(PropertyPath path, object source)
            { 
                var target = new DependencyTarget();
                var binding = new Binding() { Path = path, Source = source, Mode = BindingMode.OneTime, FallbackValue = s_fallbackValue };
                BindingOperations.SetBinding(target, DependencyTarget.ValueProperty, binding);

                if (target.Value == s_fallbackValue)
                    throw new ArgumentException($"Could not resolve property path '{path.Path}' on source object type '{source.GetType()}'.");

                return target.Value;
            }

            private class DependencyTarget : DependencyObject
            {
                public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(DependencyTarget));

                public object? Value
                {
                    get => GetValue(ValueProperty);
                    set => SetValue(ValueProperty, value);
                }
            }
        }
    }
    public sealed class EventSenderExtension : MarkupExtension
    {
        /// <inheritdoc/>
        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
