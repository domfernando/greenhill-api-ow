using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Unit.Application.Base;
using Unit.Application.DTOs.Request;
using Unit.Application.DTOs.Response;
using Unit.Application.Sevices;
using Unit.Application.Util;
using Unit.Domain.Entities.Cadastro;

namespace Unit.Infra.Services
{
    public class EnderecoService : IEnderecoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public EnderecoService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Reply> Add(CreateEnderecoRequest entidade, int pessoaId)
        {
            Reply retorno = new Reply();

            try
            {
                var novo = _mapper.Map<Endereco>(entidade);
                novo.Criado = DateTime.Now;
                novo.TipoEndereco = entidade.TipoEndereco;
                novo.Logradouro = entidade.Logradouro;
                novo.Numero = entidade.Numero;

                var resultado = _unitOfWork.Enderecos.AddAsync(novo);

                if (resultado.IsCompletedSuccessfully)
                {
                    await _unitOfWork.CommitAsync();

                    if (pessoaId > 0)
                    {
                        var resultPessoa = _unitOfWork.PessoaEnderecos.AddAsync(new PessoaEndereco()
                        {
                            PessoaId = pessoaId,
                            EnderecoId = novo.ID
                        });

                        if (resultPessoa.IsCompletedSuccessfully)
                        {
                            await _unitOfWork.CommitAsync();
                        }
                    }

                    retorno.Success = true;
                    retorno.Messages.Add("Endereço criado com sucesso.");
                    retorno.Data = _mapper.Map<EnderecoResponse>(novo);
                }
                else
                {
                    retorno.Success = false;
                    retorno.Messages.Add("Erro ao criar um papel.");
                    retorno.Errors.Add(resultado.Exception.Message);
                    return retorno;
                }
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Erro ao criar serviço");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }
        public async Task<Reply> GetAll(QueryEnderecoRequest condicao)
        {
            Reply retorno = new Reply();

            try
            {
                var query = _unitOfWork.Enderecos.AsQueryable();

                if (condicao.Id.HasValue && condicao.Id.Value > 0)
                {
                    query = query.Where(x => x.ID == condicao.Id.Value);
                }
                if (!string.IsNullOrEmpty(condicao.Logradouro))
                {
                    query = query.Where(x => x.Logradouro.ToLower().Contains(condicao.Logradouro.ToLower()));
                }
                
                var resultado = query.ToList();

                retorno.Success = true;

                if (resultado == null || resultado.Count == 0)
                {
                    retorno.Messages.Add("Nenhum endereço encontrado.");
                    retorno.Data = new List<EnderecoResponse>();
                }

                retorno.Messages.Add("Endereço(s) encontrado(s) com sucesso.");
                retorno.Data = _mapper.Map<List<EnderecoResponse>>(resultado);
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Não foi possível pesquisar o endereço.");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }
        public async Task<Reply> GetOne(int id)
        {
            Reply retorno = new Reply();

            try
            {
                var one = await _unitOfWork.Enderecos.AsQueryable()
                            .Where(x => x.ID == id)
                            .FirstOrDefaultAsync();

                retorno.Success = true;
                retorno.Status = System.Net.HttpStatusCode.OK;
                retorno.Messages.Add("Endereço encontrado com sucesso.");
                retorno.Data = one != null ? _mapper.Map<EnderecoResponse>(one) : new EnderecoResponse();
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Erro ao buscar endereço: ");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }
        public async Task<Reply> Update(UpdateEnderecoRequest entidade)
        {
            Reply retorno = new Reply();

            try
            {
                var existente = await _unitOfWork.Enderecos.GetByIdAsync(entidade.Id);

                if (existente == null)
                {
                    retorno.Success = false;
                    retorno.Messages.Add("Endereço não encontrado.");
                }
                else
                {
                    var registro = _mapper.Map<Endereco>(existente);
                    registro.Alterado = DateTime.Now;
                    registro.TipoEndereco=entidade.TipoEndereco;
                    registro.Logradouro = entidade.Logradouro;
                    registro.Numero = entidade.Numero;
                    registro.Complemento = entidade.Complemento;
                    registro.Bairro = entidade.Bairro;  
                    registro.Cidade = entidade.Cidade;
                    registro.Estado = entidade.Estado;
                    registro.Cep = entidade.Cep;

                    _unitOfWork.Enderecos.Update(registro);
                    await _unitOfWork.CommitAsync();

                    retorno.Success = true;
                    retorno.Messages.Add("Endereço atualizado com sucesso.");
                    retorno.Data = _mapper.Map<EnderecoResponse>(registro);
                }
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Erro ao atualizar endereço");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }
    }
}
