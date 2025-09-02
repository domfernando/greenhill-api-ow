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
    public class PapelService : IPapelService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PapelService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Reply> Add(CreatePapelRequest entidade)
        {
            Reply retorno = new Reply();

            try
            {
                var novo = _mapper.Map<Papel>(entidade);
                novo.Criado = DateTime.Now;
                novo.Ativo = true;

                var resultado = _unitOfWork.Papeis.AddAsync(novo);

                if (resultado.IsCompletedSuccessfully)
                {
                    await _unitOfWork.CommitAsync();
                    retorno.Success = true;
                    retorno.Messages.Add("Papel criado com sucesso.");
                    retorno.Data = _mapper.Map<PapelResponse>(novo);
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

        public async Task<Reply> GetAll(QueryPapelRequest condicao)
        {
            Reply retorno = new Reply();

            try
            {
                var query = _unitOfWork.Papeis.AsQueryable();

                if (condicao.Id.HasValue && condicao.Id.Value > 0)
                {
                    query = query.Where(x => x.ID == condicao.Id.Value);
                }
                if (!string.IsNullOrEmpty(condicao.Nome))
                {
                    query = query.Where(x => x.Nome.ToLower().Contains(condicao.Nome.ToLower()) || x.Descricao.ToLower().Contains(condicao.Nome));
                }

                var resultado = query.ToList();

                retorno.Success = true;

                if (resultado == null || resultado.Count == 0)
                {
                    retorno.Messages.Add("Nenhum papel encontrado.");
                    retorno.Data = new List<PapelResponse>();
                }

                retorno.Messages.Add("Papel(s) encontrado(s) com sucesso.");
                retorno.Data = _mapper.Map<List<PapelResponse>>(resultado);
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Não foi possível pesquisar o serviço.");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }

        public async Task<Reply> GetOne(int id)
        {
            Reply retorno = new Reply();

            try
            {
                var one = await _unitOfWork.Papeis
                                            .AsQueryable()
                                            .Include(x => x.Pubs).ThenInclude(x => x.Pub)
                                            .Where(x => x.ID == id)
                                            .FirstOrDefaultAsync();

                retorno.Success = true;
                retorno.Status = System.Net.HttpStatusCode.OK;
                retorno.Messages.Add("Papel encontrado com sucesso.");
                retorno.Data = one != null ? _mapper.Map<PapelResponse>(one) : new PapelResponse();
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Erro ao buscar papel: ");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }

        public async Task<Reply> Update(UpdatePapelRequest entidade)
        {
            Reply retorno = new Reply();

            try
            {
                var existente = await _unitOfWork.Papeis.GetByIdAsync(entidade.Id);

                if (existente == null)
                {
                    retorno.Success = false;
                    retorno.Messages.Add("Papel não encontrado.");
                }
                else
                {
                    var registro = _mapper.Map<Papel>(existente);
                    registro.Alterado = DateTime.Now;
                    registro.Nome = entidade.Nome;
                    registro.Descricao = entidade.Descricao;
                    registro.Ativo = entidade.Ativo;

                    _unitOfWork.Papeis.Update(registro);
                    await _unitOfWork.CommitAsync();

                    retorno.Success = true;
                    retorno.Messages.Add("Papel atualizado com sucesso.");
                    retorno.Data = _mapper.Map<PapelResponse>(registro);
                }
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Erro ao atualizar papel");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }
    }
}
