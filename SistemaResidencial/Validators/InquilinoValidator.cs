using FluentValidation;
using SistemaResidencial.Models;

namespace SistemaResidencial.Validators
{
    public class InquilinoValidator : AbstractValidator<Inquilino>
    {
        public InquilinoValidator()
        {
            RuleFor(i => i.Nombre).NotEmpty().WithMessage("El nombre es requerido.");
            RuleFor(i => i.Apellido).NotEmpty().WithMessage("El apellido es requerido.");
            RuleFor(i => i.NumeroDocumento).NotEmpty().WithMessage("El documento es requerido.");
            RuleFor(i => i.Email).EmailAddress().When(i => !string.IsNullOrEmpty(i.Email)).WithMessage("Formato de email inválido.");
        }
    }
}
