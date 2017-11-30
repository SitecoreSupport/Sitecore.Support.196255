namespace Sitecore.Support.Pipelines.Save
{
    using Sitecore;
    using Sitecore.Collections;
    using Sitecore.Data.Validators;
    using Sitecore.Diagnostics;
    using Sitecore.Globalization;
    using Sitecore.Web;
    using Sitecore.Web.UI.Sheer;
    using System;
    using Sitecore.Pipelines.Save;

    public class Validators
    {
        public void Process(SaveArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            this.ProcessInternal(args);
        }

        protected void ProcessInternal(ClientPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            if (args.IsPostBack)
            {
                if (args.Result == "no")
                {
                    args.AbortPipeline();
                }
                args.IsPostBack = false;
            }
            else
            {
                string formValue = WebUtil.GetFormValue("scValidatorsKey");
                if (!string.IsNullOrEmpty(formValue))
                {
                    ValidatorCollection validators = ValidatorManager.GetValidators(ValidatorsMode.ValidatorBar, formValue);
                    ValidatorOptions options = new ValidatorOptions(true);
                    ValidatorManager.Validate(validators, options);
                    Pair<ValidatorResult, BaseValidator> pair = ValidatorManager.GetStrongestResult(validators, true, false); //Alex20171130: Fixed to pass FALSE so it will not skip FATAL
                    ValidatorResult result = pair.Part1;
                    BaseValidator failedValidator = pair.Part2;
                    if ((failedValidator != null) && failedValidator.IsEvaluating)
                    {
                        SheerResponse.Alert("The fields in this item have not been validated.\n\nWait until validation has been completed and then save your changes.", new string[0]);
                        args.AbortPipeline();
                    }
                    else
                    {
                        switch (result)
                        {
                            case ValidatorResult.CriticalError:
                                {
                                    string text = Translate.Text("Some of the fields in this item contain critical errors.\n\nAre you sure you want to save this item?");
                                    if (MainUtil.GetBool(args.CustomData["showvalidationdetails"], false) && (failedValidator != null))
                                    {
                                        text = text + ValidatorManager.GetValidationErrorDetails(failedValidator);
                                    }
                                    SheerResponse.Confirm(text);
                                    args.WaitForPostBack();
                                    return;
                                }
                            case ValidatorResult.FatalError:
                                {
                                    string str3 = Translate.Text("Some of the fields in this item contain fatal errors.\n\nYou must resolve these errors before you can save this item.");
                                    if (MainUtil.GetBool(args.CustomData["showvalidationdetails"], false) && (failedValidator != null))
                                    {
                                        str3 = str3 + ValidatorManager.GetValidationErrorDetails(failedValidator);
                                    }
                                    SheerResponse.Alert(str3, new string[0]);
                                    SheerResponse.SetReturnValue("failed");
                                    args.AbortPipeline();
                                    break;
                                }
                        }
                    }
                }
            }
        }
    }
}
