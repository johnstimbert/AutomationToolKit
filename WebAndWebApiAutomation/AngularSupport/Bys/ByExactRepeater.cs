using WebAndWebApiAutomation.AngularSupport.Bys.Base;
using WebAndWebApiAutomation.AngularSupport.Scripts;

namespace WebAndWebApiAutomation.AngularSupport.Bys
{
    internal class ByExactRepeater : AngularBaseBy
    {
        /// <summary>
        /// Creates a new instance of <see cref="ByExactRepeater"/>.
        /// </summary>
        /// <param name="repeat">The exact text of the repeater, e.g. 'cat in cats'.</param>
        public ByExactRepeater(string repeat)
            : base(BackingScripts.FindAllRepeaterRows, repeat, true)
        {
            _description = "By.ExactRepeater: " + repeat;
        }
    }
}
