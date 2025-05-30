﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.BlazorUI.Helper
{
    /// <summary>
    /// This class contains static properties representing common CSS values.
    /// </summary>
    public static class CssHelper
    {
        public static class Overflow
        {
            public static string Auto => "auto";
            public static string Hidden => "hidden";
            public static string Scroll => "scroll";
        }

        public static class Display
        {
            public static string Block => "block";
            public static string Flex => "flex";
            public static string Grid => "grid";
            public static string InlineBlock => "inline-block";
            public static string InlineFlex => "inline-flex";
            public static string InlineGrid => "inline-grid";
            public static string None => "none";
        }

        public enum Alignment
        {
            Start,
            Center,
            End,
            Stretch,
            SpaceBetween,
        }

        public static class FlexDirection
        {
            public static string Row => "row";
            public static string Column => "column";
            public static string RowReverse => "row-reverse";
            public static string ColumnReverse => "column-reverse";
        }

        public static class JustifyContent
        {
            public static string Center => "center";
            public static string Start => "flex-start";
            public static string End => "flex-end";
            public static string SpaceBetween => "space-between";
            public static string SpaceAround => "space-around";
            public static string SpaceEvenly => "space-evenly";
        }

        public static class AlignItems
        {
            public static string Center => "center";
            public static string Start => "flex-start";
            public static string End => "flex-end";
            public static string Stretch => "stretch";
            public static string Baseline => "baseline";
        }

        public static class TextAlign
        {
            public static string Left => "left";
            public static string Center => "center";
            public static string Right => "right";
            public static string Justify => "justify";
        }

        public static class ScrollSnapAlign
        {
            public static string Start => "start";
            public static string Center => "center";
            public static string End => "end";
        }
    }
}
