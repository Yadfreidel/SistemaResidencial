using System;
using FluentValidation;
using SistemaResidencial.Models;

namespace SistemaResidencial.Validators
{
    public class ContratoValidator : AbstractValidator<Contrato>
    {
        public ContratoValidator()
        {
            RuleFor(c => c.FechaFin)
                .GreaterThan(c => c.FechaInicio).WithMessage("La fecha de fin debe ser posterior a la de inicio.")
                .Must((contrato, fechaFin) => fechaFin >= contrato.FechaInicio.AddYears(1))
                .WithMessage("El contrato debe tener una duración mínima de 1 año.");

            RuleFor(c => c.MontoMensual).GreaterThan(0).WithMessage("El monto mensual debe ser mayor a 0.");
        }
    }
}
