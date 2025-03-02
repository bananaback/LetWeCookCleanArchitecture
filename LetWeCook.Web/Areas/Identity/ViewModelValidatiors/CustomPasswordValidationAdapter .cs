using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;

namespace LetWeCook.Web.Areas.Identity.ViewModelValidators;

public class CustomPasswordValidationAdapter : AttributeAdapterBase<CustomPasswordValidationAttribute>
{
    public CustomPasswordValidationAdapter(CustomPasswordValidationAttribute attribute, IStringLocalizer? stringLocalizer)
        : base(attribute, stringLocalizer) { }

    public override void AddValidation(ClientModelValidationContext context)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));

        context.Attributes["data-val"] = "true";
        context.Attributes["data-val-custompassword"] = "Password must be at least 6 characters long, contain a digit, lowercase, uppercase, and special character.";
    }

    public override string GetErrorMessage(ModelValidationContextBase validationContext)
    {
        return "Password does not meet the required criteria.";
    }
}