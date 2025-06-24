using RTB.Styled;
using System;

namespace RTB.BlazorUI.Styles;

public interface IStyle
{
    StyleBuilder ToStyle();
}
