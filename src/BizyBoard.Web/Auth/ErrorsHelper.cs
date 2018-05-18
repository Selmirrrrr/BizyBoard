namespace BizyBoard.Web.Auth
{
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.AspNetCore.Identity;

    public static class ErrorsHelper
    {
        public static ModelStateDictionary AddErrorsToModelState(IdentityResult identityResult, ModelStateDictionary modelState)
        {
            foreach (var e in identityResult.Errors) modelState.TryAddModelError(e.Code, e.Description);

            return modelState;
        }

        public static ModelStateDictionary AddErrorToModelState(string code, string description, ModelStateDictionary modelState)
        {
            modelState.TryAddModelError(code, description);
            return modelState;
        }

        public static ModelStateDictionary AddErrorToModelState((string code, string description) error, ModelStateDictionary modelState)
        {
            modelState.TryAddModelError(error.code, error.description);
            return modelState;
        }
    }
}