using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.Extensions.Localization;

namespace LetWeCook.Web.Areas.Identity.ViewModelValidators;

public class CustomValidationAttributeAdapterProvider : IValidationAttributeAdapterProvider
{
    private readonly IValidationAttributeAdapterProvider _baseProvider = new ValidationAttributeAdapterProvider();

    public IAttributeAdapter? GetAttributeAdapter(ValidationAttribute attribute, IStringLocalizer? stringLocalizer)
    {
        if (attribute is CustomPasswordValidationAttribute customAttr)
        {
            return new CustomPasswordValidationAdapter(customAttr, stringLocalizer);
        }
        return _baseProvider.GetAttributeAdapter(attribute, stringLocalizer);
    }
}