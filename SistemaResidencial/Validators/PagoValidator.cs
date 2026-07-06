using FluentValidation;
using SistemaResidencial.Models;

namespace SistemaResidencial.Validators
{
    public class PagoValidator : AbstractValidator<Pago>
    {
        public PagoValidator()
        {
            RuleFor(p => p.FechaPago).NotEmpty().WithMessage("La fecha de pago es requerida.");
            RuleFor(p => p.Monto)
                .GreaterThan(0).WithMessage("El monto debe ser mayor a 0.")
                .Must((pago, monto) => pago.Contrato == null || monto == pago.Contrato.MontoMensual)
                .WithMessage("El monto de pago debe ser exactamente el monto mensual del contrato.");
        }
    }
}
