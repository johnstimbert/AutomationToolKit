using WebAndWebApiAutomation.AngularSupport.Bys.Base;
using WebAndWebApiAutomation.AngularSupport.Scripts;

namespace WebAndWebApiAutomation.AngularSupport.Bys
{
    internal class ByModel : AngularBaseBy
    {
        /// <summary>
        /// Creates a new instance of <see cref="ByModel"/>.
        /// </summary>
        /// <param name="model">The model name.</param>
        public ByModel(string model)
            : base(BackingScripts.FindModel, model)
        {
            _description = "By.Model: " + model;
        }
    }
}
