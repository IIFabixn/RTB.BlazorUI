using System;
using RTB.BlazorUI.Helper;

namespace RTB.BlazorUI.Styles;

public interface IStyle
{
    StyleBuilder ToStyle(StyleBuilder builder);
}
