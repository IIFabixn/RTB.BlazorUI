using RTB.Blazor.Styled;
using System;

namespace RTB.Blazor.UI.Styles;

public interface IStyle
{
    StyleBuilder ToStyle();
}
