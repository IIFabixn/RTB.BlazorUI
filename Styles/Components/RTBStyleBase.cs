using Microsoft.AspNetCore.Components;
using RTB.BlazorUI.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.BlazorUI.Styles.Components
{
    public abstract class RTBStyleBase : ComponentBase
    {
        [CascadingParameter] protected StyleBuilder StyleBuilder { get; set; } = default!;

        [Parameter] public bool Condition { get; set; } = true;

        protected virtual bool ConditionInternal => Condition;

        private bool _hasAppliedStyles = false;
        private int _lastParameterHash = 0;

        protected override void OnParametersSet()
        {
            var currentHash = GetHashCode();
            if (_hasAppliedStyles && currentHash == _lastParameterHash)
                return;

            _lastParameterHash = currentHash;
            _hasAppliedStyles = true;

            base.OnParametersSet();
        }
    }
}
