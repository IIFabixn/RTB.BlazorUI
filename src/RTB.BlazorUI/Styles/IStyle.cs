using RTB.Blazor.Styled;
using RTB.Blazor.Styled.Core;
using System;

namespace RTB.Blazor.Styles;

public interface IStyle
{
    IStyleBuilder ToStyle();
}
