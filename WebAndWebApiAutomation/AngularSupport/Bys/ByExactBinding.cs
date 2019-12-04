using WebAndWebApiAutomation.AngularSupport.Bys.Base;
using WebAndWebApiAutomation.AngularSupport.Scripts;

namespace WebAndWebApiAutomation.AngularSupport.Bys
{
    internal class ByExactBinding : AngularBaseBy
    {
        /// <summary>
        /// Creates a new instance of <see cref="ByExactBinding"/>.
        /// </summary>
        /// <param name="binding">The exact binding, e.g. '{{cat.name}}'.</param>
        public ByExactBinding(string binding)
            : base(BackingScripts.FindBindings, binding, true)
        {
            _description = "By.ExactBinding: " + binding;
        }
    }
}
