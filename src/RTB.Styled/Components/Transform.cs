using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.Blazor.Styled.Components
{
    public class Transform : RTBStyleBase
    {
        public enum Functions { Matrix, Matrix3D, Perspective, Rotate, Rotate3D, RotateX, RotateY, RotateZ, Translate, Translate3D, TranslateX, TranslateY, TranslateZ, Scale, Scale3D, ScaleX, ScaleY, ScaleZ, Skew, SkewW, SkewY }

        [Parameter] public Functions Function { get; set; } = Functions.Matrix;
        [Parameter] public string[]? Values { get; set; }

        protected override StyleBuilder BuildStyle(StyleBuilder builder)
        {
            return builder.Transform(Function, Values);
        }
    }

    public static class TransformExtensions
    {
        public static StyleBuilder Transform(this StyleBuilder builder, Transform.Functions function, params string[]? values)
        {
            if (values == null || values.Length == 0)
            {
                return builder.Append("transform", function.ToString().ToLowerInvariant());
            }
            return builder.Append("transform", $"{function.ToString().ToLowerInvariant()}({string.Join(", ", values)})");
        }
    }
}
