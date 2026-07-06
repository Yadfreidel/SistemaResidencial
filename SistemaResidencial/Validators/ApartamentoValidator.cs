using FluentValidation;
using SistemaResidencial.Models;

namespace SistemaResidencial.Validators
{
    public class ApartamentoValidator : AbstractValidator<Apartamento>
    {
        public ApartamentoValidator()
        {
            RuleFor(a => a.Numero).NotEmpty().WithMessage("El número de apartamento es requerido.");
            RuleFor(a => a.PrecioAlquiler).GreaterThan(0).WithMessage("El precio de alquiler debe ser mayor a 0.");
            RuleFor(a => a.Estado).IsInEnum().WithMessage("Estado no válido.");
        }
    }
}
