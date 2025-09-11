using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Unit.Application.Base;
using Unit.Application.DTOs.Request;
using Unit.Application.DTOs.Response;
using Unit.Application.Services;
using Unit.Application.Util;
using Unit.Domain.Entities.Cadastro;

namespace Unit.Infra.Services
{
    public class NVMCParteService : INVMCParteService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public NVMCParteService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Reply> Add(NVMCParte entidade)
        {
            Reply retorno = new Reply();

            try
            {
                var novo = new NVMCParte()
                {
                    NVMCID = entidade.NVMCID,
                    NomeSecao = entidade.NomeSecao,
                    Titulo = entidade.Titulo,
                    Descricao = entidade.Descricao,
                    DesignadoID = null,
                    AjudanteID = null,
                    Privilegio = entidade.Privilegio,
                    Criado = DateTime.Now,
                };

                var resultado = _unitOfWork.NVMCPartes.AddAsync(novo);

                if (resultado.IsCompletedSuccessfully)
                {
                    await _unitOfWork.CommitAsync();
                    retorno.Success = true;
                    retorno.Messages.Add("Parte criada com sucesso.");
                    retorno.Data = novo;
                }
                else
                {
                    retorno.Success = false;
                    retorno.Messages.Add("Erro ao criar parte.");
                    retorno.Errors.Add(resultado.Exception.Message);
                    return retorno;
                }
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Erro ao criar parte");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }

        public async Task<Reply> GetAll(QueryNVMCParteRequest condicao)
        {
            Reply retorno = new Reply();

            try
            {
                var query = _unitOfWork.NVMCPartes
                                        .AsQueryable()
                                        .Include(x => x.NVMC)
                                        .AsQueryable();


                if (condicao.Id.HasValue && condicao.Id.Value > 0)
                {
                    query = query.Where(x => x.ID == condicao.Id.Value);
                }
                if (condicao.NVMCId > 0)
                {
                    query = query.Where(x => x.NVMCID == condicao.NVMCId);
                }
                if (!string.IsNullOrEmpty(condicao.Data))
                {
                    var _data = DateTime.Parse(condicao.Data);
                    query = query.Where(x => x.NVMC.Data.Date == _data.Date);
                }

                var resultado = await query.ToListAsync();

                retorno.Success = true;

                if (resultado == null || resultado.Count == 0)
                {
                    retorno.Messages.Add("Nenhuma parte encontrada.");
                    retorno.Data = new List<NVMCParte>();
                }
                else
                {
                    retorno.Messages.Add("Parte(s) encontrada(s) com sucesso.");
                    retorno.Data = resultado;
                }


            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Não foi possível pesquisar as partes.");
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

        public async Task<Reply> Update(UpdateNVMCParteRequest entidade)
        {
            Reply retorno = new Reply();

            try
            {
                var existente = await _unitOfWork.NVMCPartes.GetByIdAsync(entidade.Id);

                if (existente == null)
                {
                    retorno.Success = false;
                    retorno.Messages.Add("Parte não encontrada.");
                }
                else
                {
                    existente.Alterado = DateTime.Now;
                    existente.NVMCID = entidade.NVMCId;
                    existente.NomeSecao = entidade.NomeSecao;
                    existente.Titulo = entidade.Titulo;
                    existente.Descricao = entidade.Descricao;
                    existente.DesignadoID = entidade.DesignadoId > 0 ? entidade.DesignadoId : null;
                    existente.AjudanteID = entidade.AjudanteId > 0 ? entidade.AjudanteId : null;
                    existente.Privilegio = entidade.Privilegio;


                    _unitOfWork.NVMCPartes.Update(existente);
                    await _unitOfWork.CommitAsync();

                    retorno.Success = true;
                    retorno.Messages.Add("Parte atualizada com sucesso.");
                    retorno.Data = existente;
                }
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Erro ao atualizar parte");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }
    }
}
