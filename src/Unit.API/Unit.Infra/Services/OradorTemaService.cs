using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Unit.Application.Base;
using Unit.Application.DTOs.Request;
using Unit.Application.Services;
using Unit.Application.Util;
using Unit.Domain.Entities.Cadastro;

namespace Unit.Infra.Services
{
    public class OradorTemaService : IOradorTemaService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public OradorTemaService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Reply> AddAsync(OradorTemaNewRequest novo)
        {
            Reply retorno = new Reply();

            try
            {
                var _novo = new OradorTema()
                {
                    PubID = novo.PubId,
                    TemaID = novo.TemaId,
                    Publico = novo.Publico | false,
                    Criado = DateTime.Now
                };

                var resultado = _unitOfWork.OradorTemas.AddAsync(_novo);

                if (resultado.IsCompletedSuccessfully)
                {
                    await _unitOfWork.CommitAsync();

                    retorno.Success = true;
                    retorno.Messages.Add("Adicionado com sucesso.");
                    retorno.Data = novo;
                }
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Não foi possível realizar a requisição.");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }

        public async Task<Reply> GetAll(OradorTemaQueryRequest condicao)
        {
            Reply retorno = new Reply();

            try
            {
                var query = _unitOfWork.OradorTemas
                                       .AsQueryable();

                if (condicao.PubId.HasValue && condicao.PubId.Value > 0)
                {
                    query = query.Where(x => x.PubID == condicao.PubId.Value);
                }

                if (condicao.TemaId.HasValue && condicao.TemaId.Value > 0)
                {
                    query = query.Where(x => x.TemaID == condicao.TemaId.Value);
                }

                var resultado = await query.ToListAsync();

                retorno.Success = true;
                retorno.Messages.Add("Consulta realizada com sucesso.");
                retorno.Data = resultado;

            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Não foi possível realizar a consulta.");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }

        public async Task<Reply> GetById(int id)
        {
            Reply retorno = new Reply();

            try
            {
                var resultado = await _unitOfWork.OradorTemas
                                                .AsQueryable()
                                                .Include(x => x.Orador)
                                                .Include(x => x.Tema)
                                                .Where(x => x.ID == id)
                                                .FirstOrDefaultAsync();
                retorno.Success = true;
                retorno.Messages.Add("Requisição realizada com sucesso.");
                retorno.Data = resultado;

            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Não foi possível realizar a requisição.");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }

        public async Task<bool> RemoveAsync(int id)
        {
            bool retorno = false;
            var delete = await _unitOfWork.OradorTemas
                                                .AsQueryable()
                                                .Where(x => x.ID == id)
                                                .FirstOrDefaultAsync();
            if (delete != null)
            {
                _unitOfWork.OradorTemas.Delete(delete);
                await _unitOfWork.CommitAsync();
                retorno = true;
            }
            return retorno;
        }

        public async Task<Reply> UpdateAsync(OradorTemaUpdateRequest dados)
        {
            Reply retorno = new Reply();

            try
            {
                var registro = await _unitOfWork.OradorTemas
                                                .AsQueryable()
                                                .Where(x => x.ID == dados.ID)
                                                .FirstOrDefaultAsync();

                if (registro != null)
                {
                    registro.Alterado = System.DateTime.Now;
                    registro.PubID = dados.PubId;
                    registro.TemaID = dados.TemaId;
                    registro.Publico = dados.Publico;

                    _unitOfWork.OradorTemas.Update(registro);
                    await _unitOfWork.CommitAsync();

                    retorno.Success = true;
                    retorno.Messages.Add("Operação realizada com sucesso.");
                    retorno.Data = registro;
                }
                else
                {
                    retorno.Success = true;
                    retorno.Messages.Add("Registro não encontrado.");
                    retorno.Data = registro;
                }
            }
            catch (Exception ex)
            {
                retorno.Messages.Add("Não foi possível realizar a operação");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }
    }
}
