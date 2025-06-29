﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.Blazor.Theme.Services;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class ThemeAttribute : Attribute
{
    public bool IsDefault { get; set; } = false;
}
