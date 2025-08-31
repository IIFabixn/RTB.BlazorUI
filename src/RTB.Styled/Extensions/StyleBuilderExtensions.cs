using RTB.Blazor.Styled.Core;
using RTB.Blazor.Styled.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.Blazor.Styled.Extensions
{
    public static class StyleBuilderExtensions
    {
        /// <summary>
        /// Casts to concrete StyleBuilder or throws.
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static StyleBuilder AsConcrete(this IStyleBuilder b)
            => b as StyleBuilder ?? throw new InvalidOperationException("Requires StyleBuilder");

        // Selectors
        public static IStyleBuilder AppendSelector(this IStyleBuilder builder, string selector,
        params (string Key, string Value)[] declarations)
        {
            var mod = builder.AsConcrete().GetOrAddModule<SelectorModule>();
            foreach (var (k, v) in declarations)
                mod.Append(selector, k, v);
            return builder;
        }

        // Media
        public static IStyleBuilder AppendMedia(this IStyleBuilder builder, string mediaQuery, string innerCssBlock)
        {
            builder.AsConcrete().GetOrAddModule<MediaModule>().Append(mediaQuery, innerCssBlock);
            return builder;
        }

        // Animations
        public static IStyleBuilder AppendAnimation(this IStyleBuilder builder, string name)
        {
            builder.AsConcrete().GetOrAddModule<AnimationModule>().Ensure(name);
            return builder;
        }

        public static IStyleBuilder AppendKeyFrame(this IStyleBuilder builder, string name, string offset,
            params (string Key, string Value)[] declarations)
        {
            builder.AsConcrete()
                   .GetOrAddModule<AnimationModule>()
                   .AppendFrame(name, offset, declarations);
            return builder;
        }
    }
}
