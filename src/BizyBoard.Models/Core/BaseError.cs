namespace BizyBoard.Models.Core
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc.ModelBinding;

    public class BaseError
    {
        public string Message { get; set; }
        public bool IsError { get; set; }
        public string Detail { get; set; }
        public List<ValidationError> Errors { get; set; }

        public BaseError(string message = "Une erreur est survenue.")
        {
            Message = message;
            IsError = true;
        }

        public BaseError(ModelStateDictionary modelState)
        {
            IsError = true;
            Message = "Validation Failed";
            Errors = modelState.Keys.SelectMany(key => modelState[key].Errors.Select(x => new ValidationError(key, x.ErrorMessage))).ToList();
        }
    }
}