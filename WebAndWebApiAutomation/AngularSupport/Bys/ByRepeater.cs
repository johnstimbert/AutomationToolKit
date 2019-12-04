using WebAndWebApiAutomation.AngularSupport.Bys.Base;
using WebAndWebApiAutomation.AngularSupport.Scripts;

namespace WebAndWebApiAutomation.AngularSupport.Bys
{
    internal class ByRepeater : AngularBaseBy
    {
        /// <summary>
        /// Creates a new instance of <see cref="ByRepeater"/>.
        /// </summary>
        /// <param name="repeat">The text of the repeater, e.g. 'cat in cats'.</param>
        public ByRepeater(string repeat)
            : base(BackingScripts.FindAllRepeaterRows, repeat, false)
        {
            _description = "By.Repeater: " + repeat;
        }
    }
}
