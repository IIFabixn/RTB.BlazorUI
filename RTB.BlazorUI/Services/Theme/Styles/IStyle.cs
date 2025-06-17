using RTB.Styled;
using System;

namespace RTB.BlazorUI.Services.Theme.Styles;

public interface IStyle
{
    StyleBuilder ToStyle();
}
