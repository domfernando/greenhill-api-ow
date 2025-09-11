using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Unit.Application.Base;
using Unit.Application.DTOs.Request;
using Unit.Application.DTOs.Response;
using Unit.Application.Enums;
using Unit.Application.Sevices;
using Unit.Application.Util;
using Unit.Domain.Entities.Acesso;
using Unit.Domain.Entities.Cadastro;

namespace Unit.Infra.Services
{
    public class RelatorioService : IRelatorioService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHubContext<MySocketService> _hubContext;

        public RelatorioService(IUnitOfWork unitOfWork, IMapper mapper, IHubContext<MySocketService> hubContext)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _hubContext = hubContext;
        }
        public async Task<Reply> Add(CreateRelatorioRequest entidade)
        {
            Reply retorno = new Reply();

            try
            {
                var novo = new Relatorio()
                {
                    Data = entidade.Mes,
                    PubId = entidade.PubId,
                    Atividade = false,
                    Auxiliar = false,
                    Regular = false,
                    Horas = 0,
                    Estudos = 0,
                    Obs = string.Empty,
                    Criado = DateTime.Now,
                };

                var resultado = _unitOfWork.Relatorios.AddAsync(novo);

                if (resultado.IsCompletedSuccessfully)
                {
                    await _unitOfWork.CommitAsync();
                    retorno.Success = true;
                    retorno.Messages.Add("Relatorio criado com sucesso.");
                    retorno.Data = novo;
                }
                else
                {
                    retorno.Success = false;
                    retorno.Messages.Add("Erro ao criar um relatório.");
                    retorno.Errors.Add(resultado.Exception.Message);
                    return retorno;
                }
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Erro ao criar relatorio");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }

        public async Task<Reply> AddBatch(CreateRelatorioBatchRequest entidade)
        {
            Reply retorno = new Reply();
            string[] ignorar = new string[] { "falecido", "mudou", "removido" };
            var _data = DateTime.Parse(entidade.Mes);

            try
            {
                var publicadores = await _unitOfWork.Pubs.AsQueryable()
                                                    .AsNoTracking()
                                                    .Where(x => !ignorar.Contains(x.Situacao.ToLower()))
                                                    .Select(p => new
                                                    {
                                                        PubId = p.ID,
                                                        Situacao = p.Situacao,
                                                        AuxiliarAte = p.AuxiliarAte,
                                                        Auxiliar = p.AuxiliarAte != null ? (p.AuxiliarAte > _data) : false
                                                    })
                                                    .ToListAsync();
                if (publicadores != null)
                {
                    foreach (var pub in publicadores)
                    {
                        if (pub.Auxiliar)
                        {

                        }


                        Relatorio relat = new Relatorio()
                        {
                            Data = _data,
                            PubId = pub.PubId,
                            Atividade = false,
                            Auxiliar = pub.Auxiliar,
                            Regular = pub.Situacao == "Regular",
                            Horas = 0,
                            Estudos = 0,
                            Obs = "",
                            Criado = System.DateTime.Now
                        };

                        var existe = await _unitOfWork.Relatorios
                                                     .AsQueryable()
                                                     .Where(x => x.Data == _data && x.PubId == pub.PubId)
                                                     .FirstOrDefaultAsync();
                        if (existe == null)
                        {
                            await _unitOfWork.Relatorios.AddAsync(relat);
                            await _unitOfWork.CommitAsync();
                        }
                    }

                    retorno.Success = true;
                    retorno.Messages.Add("Relatórios gerados com sucesso");
                    retorno.Data = publicadores;
                }
                else
                {
                    retorno.Success = false;
                    retorno.Messages.Add("Não foi possível gerar os Relatórios");
                }
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Não foi possível criar os relatórios.");
                retorno.Errors.Add(ex.Message);
            }

            if (!string.IsNullOrEmpty(entidade.Usuario))
            {
                await _hubContext.Clients.All.SendAsync("Push", entidade.Usuario, $"A criação dos relatórios foi realizada com sucesso.");
            }

            return retorno;
        }

        public async Task<Reply> AddByGrupo(CreateRelatorioByGrupoRequest entidade)
        {
            Reply retorno = new Reply();
            string[] ignorar = new string[] { "falecido", "mudou", "removido" };
            var _data = DateTime.Parse(entidade.Data);

            try
            {
                var publicadores = await _unitOfWork.GrupoPubs
                                                   .AsQueryable()
                                                   .Include(x => x.Pub)
                                                   .Where(x => x.GrupoID == entidade.GrupoId)
                                                   .Where(x => !ignorar.Contains(x.Pub.Situacao.ToLower()))
                                                   .Select(x => new { x.Pub.ID, x.Pub.Situacao, x.Pub.AuxiliarAte, Auxiliar = x.Pub.AuxiliarAte != null ? (x.Pub.AuxiliarAte > _data) : false })
                                                   .ToListAsync();

                if (publicadores != null)
                {
                    foreach (var pub in publicadores)
                    {
                        Relatorio relat = new Relatorio()
                        {
                            Data = _data,
                            PubId = pub.ID,
                            Atividade = false,
                            Auxiliar = pub.Auxiliar,
                            Regular = pub.Situacao == "Regular",
                            Horas = 0,
                            Estudos = 0,
                            Obs = "",
                            Criado = System.DateTime.Now
                        };

                        var existe = await _unitOfWork.Relatorios
                                                        .AsQueryable()
                                                        .Where(x => x.Data == _data && x.PubId == pub.ID)
                                                        .FirstOrDefaultAsync();
                        if (existe == null)
                        {
                            await _unitOfWork.Relatorios.AddAsync(relat);
                            await _unitOfWork.CommitAsync();
                        }
                    }

                    retorno.Success = true;
                    retorno.Messages.Add("Relatórios gerados com sucesso");
                    retorno.Data = publicadores;
                }
                else
                {
                    retorno.Success = false;
                    retorno.Messages.Add("Não foi possível gerar os Relatórios");
                }
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Não foi possível criar os relatórios.");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }

        public async Task<Reply> Delete(int id)
        {
            Reply retorno = new Reply();
            try
            {
                var remover = _unitOfWork.Relatorios.GetByIdAsync(id).Result;
                if (remover != null)
                {
                    _unitOfWork.Relatorios.Delete(remover);
                    _unitOfWork.CommitAsync().Wait();
                    retorno.Success = true;
                    retorno.Messages.Add("Relatório removido com sucesso.");
                }
                else
                {
                    retorno.Success = false;
                    retorno.Messages.Add("Relatório não encontrado ou já removido.");
                }
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Erro ao remover relatório.");
                retorno.Errors.Add(ex.Message);
            }
            return retorno;
        }

        public async Task<Reply> GetAll(QueryRelatorioRequest condicao)
        {
            Reply retorno = new Reply();

            try
            {
                var query = _unitOfWork.Relatorios
                                        .AsQueryable()
                                        .Include(x => x.Pub).ThenInclude(x => x.Grupos).ThenInclude(g => g.Grupo)
                                        .AsQueryable();

                if (condicao.Mes.HasValue)
                {
                    query = query.Where(x => x.Data == condicao.Mes.Value);
                }
                if (condicao.PubId > 0)
                {
                    query = query.Where(x => x.PubId == condicao.PubId);
                }
                if (condicao.GrupoId > 0)
                {
                    query = query.Where(x => x.Pub.Grupos.Any(g => g.GrupoID == condicao.GrupoId));
                }

                var resultado = await query.ToListAsync();

                retorno.Success = true;

                if (resultado == null || resultado.Count == 0)
                {
                    retorno.Messages.Add("Nenhum relatório encontrado.");
                    retorno.Data = new List<RelatorioResponse>();
                }
                else
                {
                    if (condicao.Agrupar == true)
                    {
                        var agrupado = resultado
                            .GroupBy(relats => relats.Data)
                            .Select(g => new
                            {
                                Mes = string.Format("{0: MMMM/yyyy}", g.Key),
                                Relatorios = _mapper.Map<List<RelatorioResponse>>(g.OrderBy(p =>
                                                                                            p.Pub.Grupos.FirstOrDefault()?.Papel == "SG" ? 0 :
                                                                                            p.Pub.Grupos.FirstOrDefault()?.Papel == "DG" ? 1 :
                                                                                            p.Pub.Grupos.FirstOrDefault()?.Papel == "AJ" ? 2 : 3)
                                                                                            .ThenBy(m => m.Pub.Nome).ToList())
                            })
                            .OrderBy(g => g.Mes)
                            .ToList();

                        retorno.Messages.Add("Relatório(s) agrupado(s) com sucesso.");
                        retorno.Data = agrupado;
                        retorno.Success = true;
                        return retorno;
                    }
                    else
                    {
                        resultado = resultado.OrderBy(p =>
                                                      p.Pub.Grupos.FirstOrDefault()?.Papel == "SG" ? 0 :
                                                      p.Pub.Grupos.FirstOrDefault()?.Papel == "DG" ? 1 :
                                                      p.Pub.Grupos.FirstOrDefault()?.Papel == "AJ" ? 2 : 3)
                                                      .ThenBy(m => m.Pub.Nome)
                                              .ToList();

                        retorno.Messages.Add("Relatório(s) encontrado(s) com sucesso.");
                        retorno.Data = _mapper.Map<List<RelatorioResponse>>(resultado);
                    }
                }
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
                var one = await _unitOfWork.Relatorios
                                            .AsQueryable()
                                            .Include(x => x.Pub).ThenInclude(x => x.Grupos).ThenInclude(g => g.Grupo)
                                            .Where(x => x.ID == id)
                                            .FirstOrDefaultAsync();
                retorno.Success = true;
                retorno.Status = System.Net.HttpStatusCode.OK;
                retorno.Messages.Add("Relatório encontrado com sucesso.");
                retorno.Data = one != null ? _mapper.Map<RelatorioResponse>(one) : new RelatorioResponse();
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Erro ao relatório papel: ");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }

        public async Task<Reply> Update(UpdateRelatorioRequest entidade)
        {
            Reply retorno = new Reply();

            try
            {
                var existente = await _unitOfWork.Relatorios.GetByIdAsync(entidade.Id);

                if (existente == null)
                {
                    retorno.Success = false;
                    retorno.Messages.Add("Relatório não encontrado.");
                }
                else
                {
                    existente.Data = entidade.Data;
                    existente.PubId = entidade.PubId;
                    existente.Atividade = entidade.Atividade ?? existente.Atividade;
                    existente.Auxiliar = entidade.Auxiliar ?? existente.Auxiliar;
                    existente.Horas = entidade.Horas ?? existente.Horas;
                    existente.Estudos = entidade.Estudos ?? existente.Estudos;
                    existente.Obs = entidade.Obs ?? existente.Obs;
                    existente.Entregue = entidade.Entregue ?? existente.Entregue;
                    existente.Alterado = DateTime.Now;

                    _unitOfWork.Relatorios.Update(existente);
                    await _unitOfWork.CommitAsync();

                    retorno.Success = true;
                    retorno.Messages.Add("Relatório atualizado com sucesso.");
                    retorno.Data = existente;
                }
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Erro ao atualizar relatório");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }
    }
}
