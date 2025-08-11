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
    public class PessoaService : IPessoaService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PessoaService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Reply> Add(CreatePessoaRequest entidade)
        {
            Reply retorno = new Reply();

            try
            {
                var novo = _mapper.Map<Pessoa>(entidade);
                novo.Criado = DateTime.Now;
                novo.Nome = entidade.Nome;
                novo.Fisica = entidade.Fisica;

                var resultado = _unitOfWork.Pessoas.AddAsync(novo);

                if (resultado.IsCompletedSuccessfully)
                {
                    await _unitOfWork.CommitAsync();
                    retorno.Success = true;
                    retorno.Messages.Add("Pessoa criado com sucesso.");
                    retorno.Data = _mapper.Map<PessoaResponse>(novo);
                }
                else
                {
                    retorno.Success = false;
                    retorno.Messages.Add("Erro ao criar uma pessoa.");
                    retorno.Errors.Add(resultado.Exception.Message);
                    return retorno;
                }
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Erro ao criar pessoa");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }

        public async Task<Reply> AddPapel(int pessoaId, int papelId)
        {
            Reply retorno = new Reply();

            try
            {
                var existe = _unitOfWork.PessoaPapeis.AsQueryable()
                                       .Where(x => x.PessoaId == pessoaId && x.PapelId == papelId)
                                       .FirstOrDefault();

                if (existe != null)
                {
                    retorno.Success = false;
                    retorno.Messages.Add("Papel já vinculado a pessoa.");
                }
                else
                {
                    var novoPapel = new PessoaPapel
                    {
                        PessoaId = pessoaId,
                        PapelId = papelId,
                        Criado = System.DateTime.Now
                    };

                    await _unitOfWork.PessoaPapeis.AddAsync(novoPapel);
                    await _unitOfWork.CommitAsync();

                    retorno.Success = true;
                    retorno.Messages.Add("Papel vinculado a pessoa com sucesso.");
                }
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Não foi possível pesquisar o registro.");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }

        public async Task<Reply> GetAll(QueryPessoaRequest condicao)
        {
            Reply retorno = new Reply();

            try
            {
                var query = _unitOfWork.Pessoas.AsQueryable();

                if (condicao.Id.HasValue && condicao.Id.Value > 0)
                {
                    query = query.Where(x => x.ID == condicao.Id.Value);
                }
                if (!string.IsNullOrEmpty(condicao.Nome))
                {
                    query = query.Where(x => x.Nome.ToLower().Contains(condicao.Nome.ToLower()));
                }

                var resultado = query.ToList();

                retorno.Success = true;

                if (resultado == null || resultado.Count == 0)
                {
                    retorno.Messages.Add("Nenhum registro encontrado.");
                    retorno.Data = new List<PessoaResponse>();
                }

                retorno.Messages.Add("Registro(s) encontrado(s) com sucesso.");
                retorno.Data = _mapper.Map<List<PessoaResponse>>(resultado);
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Não foi possível pesquisar o registro.");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }

        public async Task<Reply> GetGraphics()
        {
            Reply retorno = new Reply();

            try
            {
                // Busca todos os papeis ativos
                var papeis = await _unitOfWork.Papeis.AsQueryable().ToListAsync();

                // Busca todas as pessoas com seus papeis
                var pessoas = await _unitOfWork.Pessoas.AsQueryable()
                                    .Include(x => x.Papeis)
                                        .ThenInclude(x => x.Papel)
                                    .ToListAsync();

                // Agrupa por Papel e conta quantas pessoas possuem cada papel
                var grouped = papeis
                    .Select(papel => new GraphicResponse
                    {
                        Status = papel.Nome,
                        Quantidade = pessoas.Count(p => p.Papeis.Any(pp => pp.PapelId == papel.ID))
                    })
                    .OrderBy(x => x.Status)
                    .ToList();

                // Adiciona o total geral
                var total = new GraphicResponse
                {
                    Status = "Total",
                    Quantidade = grouped.Sum(x => x.Quantidade)
                };
                grouped.Add(total);

                retorno.Success = true;
                retorno.Status = System.Net.HttpStatusCode.OK;
                retorno.Messages.Add("Dados encontrados com sucesso.");
                retorno.Data = grouped;
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Erro ao buscar item: ");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }

        public async Task<Reply> GetOne(int id)
        {
            Reply retorno = new Reply();

            try
            {
                var one = await _unitOfWork.Pessoas.AsQueryable()
                                .Include(x => x.Papeis).ThenInclude(x => x.Papel)
                                .Include(x => x.Enderecos).ThenInclude(x => x.Endereco)
                                .Where(x => x.ID == id)
                                .FirstOrDefaultAsync();

                retorno.Success = true;
                retorno.Status = System.Net.HttpStatusCode.OK;
                retorno.Messages.Add("Registro encontrado com sucesso.");
                retorno.Data = one != null ? _mapper.Map<PessoaResponse>(one) : new PessoaResponse();
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Erro ao pesquisar: ");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }

        public async Task<Reply> RemPapel(int id)
        {
            Reply retorno = new Reply();

            try
            {
                var existe = _unitOfWork.PessoaPapeis.AsQueryable()
                                       .Where(x => x.ID == id)
                                       .FirstOrDefault();

                if (existe != null)
                {
                    _unitOfWork.PessoaPapeis.Delete(existe);
                    await _unitOfWork.CommitAsync();

                    retorno.Success = true;
                    retorno.Messages.Add("Papel desvinculado a pessoa.");
                }
                else
                {
                    retorno.Success = false;
                    retorno.Messages.Add("Papel não associado a a pessoa.");
                }
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Não foi possível pesquisar o registro.");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }

        public async Task<Reply> Update(UpdatePessoaRequest entidade)
        {
            Reply retorno = new Reply();

            try
            {
                var existente = await _unitOfWork.Pessoas.GetByIdAsync(entidade.Id);

                if (existente == null)
                {
                    retorno.Success = false;
                    retorno.Messages.Add("Registro não encontrado.");
                }
                else
                {
                    var registro = _mapper.Map<Pessoa>(existente);
                    registro.Alterado = DateTime.Now;
                    registro.Nome = entidade.Nome;
                    registro.NomeCompleto = entidade.NomeCompleto;
                    registro.Fisica = entidade.Fisica;
                    registro.Sexo = entidade.Sexo;
                    registro.Email = entidade.Email;
                    registro.Telefone = entidade.Telefone;
                    registro.Celular = entidade.Celular;
                    registro.Documento = entidade.Documento;
                    registro.SituacaoComercial = entidade.SituacaoComercial;
                    registro.Nascimento = entidade.Nascimento;
                    registro.Observacao = entidade.Observacao;

                    _unitOfWork.Pessoas.Update(registro);
                    await _unitOfWork.CommitAsync();

                    retorno.Success = true;
                    retorno.Messages.Add("Papel atualizado com sucesso.");
                    retorno.Data = _mapper.Map<PessoaResponse>(registro);
                }
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Erro ao atualizar registro");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }
    }
}
