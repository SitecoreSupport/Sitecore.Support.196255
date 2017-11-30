namespace Sitecore.Support.Shell.Applications.ContentManager.ReturnFieldEditorValues
{
    using Sitecore.Pipelines.Save;
    using System;
    using Sitecore.Shell.Applications.ContentManager.ReturnFieldEditorValues;
    public class Validate : Sitecore.Support.Pipelines.Save.Validators
    {
        public void Process(ReturnFieldEditorValuesArgs args)
        {
            base.ProcessInternal(args);
        }
    }
}
