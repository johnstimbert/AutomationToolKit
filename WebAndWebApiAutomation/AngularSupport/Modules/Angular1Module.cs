namespace WebAndWebApiAutomation.AngularSupport.Modules
{
    /// <summary>
    /// Module automatically installed by Protractor when a page is loaded with Angular 1.
    /// </summary>
    internal class Angular1Module : Module
    {
        internal Angular1Module() : base(ModuleName, ModuleScript){}

        private const string ModuleName = "protractorBaseModule_";

        private const string ModuleScript = "angular.module('" + ModuleName + @"', [])
.config([
  '$compileProvider',
  function($compileProvider) {
    if ($compileProvider.debugInfoEnabled) {
      $compileProvider.debugInfoEnabled(true);
    }
  }
]);";

    }
}
