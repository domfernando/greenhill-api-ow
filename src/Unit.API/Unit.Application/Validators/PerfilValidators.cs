using FluentValidation;
using Unit.Application.DTOs.Request;

namespace Unit.Application.Validators
{
    public class CreatePerfilRequestValidator : AbstractValidator<CreatePerfilRequest>
    {
        public CreatePerfilRequestValidator()
        {
            RuleFor(x => x.Nome)
                .NotEmpty().WithMessage("Nome é obrigatório")
                .Length(2, 100).WithMessage("Nome deve ter entre 2 e 100 caracteres")
                .Matches(@"^[a-zA-ZÀ-ÿ\s]+$").WithMessage("Nome deve conter apenas letras");
        }
    }

    public class UpdatePerfilRequestValidator : AbstractValidator<UpdatePerfilRequest>
    {
        public UpdatePerfilRequestValidator()
        {
            RuleFor(x => x.Nome)
                .Length(2, 100).WithMessage("Nome deve ter entre 2 e 100 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Nome));
        }
    }
}
