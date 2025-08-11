using FluentValidation;
using Unit.Application.Base;
using Unit.Application.DTOs.Request;
using Unit.Application.Sevices;
using Unit.Domain.Entities.Acesso;

namespace Unit.Application.Validators
{
    public class CreateUsuarioRequestValidator : AbstractValidator<CreateUsuarioRequest>
    {
        private readonly IUsuarioService _service;

        public CreateUsuarioRequestValidator(IUsuarioService service)
        {
            _service = service;

            RuleFor(x => x.Nome)
                .NotEmpty().WithMessage("Nome é obrigatório")
                .Length(2, 100).WithMessage("Nome deve ter entre 2 e 100 caracteres");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("E-mail é obrigatório")
                .EmailAddress()
                .WithMessage("E-mail deve ter formato válido")
                .Must(email =>
                {
                    var _existe = _service.EmailExist(email);
                    return !_existe.Result;
                }).WithMessage("E-mail já está em uso");
        }

        private Task<bool> EmailExiste(string email, CancellationToken token)
        {
            return _service.EmailExist(email);
        }
    }

    public class UpdateUsuarioRequestValidator : AbstractValidator<UpdateUsuarioRequest>
    {
        public UpdateUsuarioRequestValidator()
        {
            RuleFor(x => x.Nome)
                .Length(2, 100).WithMessage("Nome deve ter entre 2 e 100 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Nome));

            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Email deve ter formato válido")
                .When(x => !string.IsNullOrEmpty(x.Email));
        }
    }

}
