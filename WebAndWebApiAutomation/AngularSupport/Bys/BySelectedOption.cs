using WebAndWebApiAutomation.AngularSupport.Scripts;
using WebAndWebApiAutomation.AngularSupport.Bys.Base;

namespace WebAndWebApiAutomation.AngularSupport.Bys
{
    internal class BySelectedOption : AngularBaseBy
    {
        /// <summary>
        /// Creates a new instance of <see cref="BySelectedOption"/>.
        /// </summary>
        /// <param name="model">The model name.</param>
        internal BySelectedOption(string model)
            : base(BackingScripts.FindSelectedOptions, model)
        {
            _description = "By.SelectedOption: " + model;
        }
    }
}
