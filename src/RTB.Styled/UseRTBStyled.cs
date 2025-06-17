using Microsoft.Extensions.DependencyInjection;
using RTB.Styled.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.Styled
{
    public static class RTBStyledExtension
    {
        public static IServiceCollection UseRTBStyled(this IServiceCollection collection)
        {
            return collection.AddScoped<IStyleRegistry, StyleRegistry>();
        }
    }
}
