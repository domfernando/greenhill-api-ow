using FluentValidation;
using Unit.Application.DTOs.Request;

namespace Unit.Application.Validators
{
    public class CreatePessoaRequestValidator : AbstractValidator<CreatePessoaRequest>
    {
        public CreatePessoaRequestValidator()
        {
            RuleFor(x => x.Nome)
                .NotEmpty().WithMessage("Nome é obrigatório")
                .Length(3, 100).WithMessage("Nome deve ter entre 2 e 100 caracteres");
        }
    }

    public class UpdatePessoaRequestValidator : AbstractValidator<UpdatePessoaRequest>
    {
        public UpdatePessoaRequestValidator()
        {
            RuleFor(x => x.Nome)
                .Length(3, 100).WithMessage("Nome deve ter entre 3 e 100 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Nome));

        }
    }
}
