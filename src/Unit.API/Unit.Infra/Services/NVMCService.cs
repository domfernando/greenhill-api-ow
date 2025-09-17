using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Unit.Application.Base;
using Unit.Application.DTOs.Request;
using Unit.Application.Services;
using Unit.Application.Util;
using Unit.Domain.Entities.Cadastro;

namespace Unit.Infra.Services
{
    public class NVMCService : INVMCService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public NVMCService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Reply> Add(NVMC entidade)
        {
            Reply retorno = new Reply();

            try
            {
                //var _data = DateTime.ParseExact(entidade.Data, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                //var _mes = DateTime.ParseExact(entidade.Mes, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

                var novo = new NVMC()
                {
                    Mes = entidade.Mes,
                    Data = entidade.Data,
                    Semana = entidade.Semana,
                    Presidente = entidade.Presidente,
                    OracaoInicial = entidade.OracaoInicial,
                    OracaoFinal = entidade.OracaoFinal,
                    Conteudo = entidade.Conteudo ?? string.Empty,
                    Criado = DateTime.Now,
                };

                var resultado = _unitOfWork.NVMCs.AddAsync(novo);

                if (resultado.IsCompletedSuccessfully)
                {
                    await _unitOfWork.CommitAsync();
                    retorno.Success = true;
                    retorno.Messages.Add("Semana criada com sucesso.");
                    retorno.Data = novo ?? new NVMC();
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
                retorno.Messages.Add("Erro ao criar semana");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }

        public async Task<Reply> GetAll(QueryNVMCRequest condicao)
        {
            Reply retorno = new Reply();

            try
            {
                var query = _unitOfWork.NVMCs
                                       .AsQueryable()
                                       .Include(x => x.Partes).ThenInclude(x => x.Designado)
                                       .Include(x => x.Partes).ThenInclude(x => x.Ajudante)
                                       .AsQueryable();
                if (condicao.Id > 0)
                {
                    query = query.Where(x => x.ID == condicao.Id.Value);
                }
                if (!string.IsNullOrEmpty(condicao.Mes))
                {
                    var _mes = DateTime.ParseExact(condicao.Mes, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                    query = query.Where(x => x.Mes.Month == _mes.Month && x.Mes.Year == _mes.Year);
                }
                if (!string.IsNullOrEmpty(condicao.Data))
                {
                    var _data = DateTime.ParseExact(condicao.Data, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                    query = query.Where(x => x.Data.Date == _data.Date);
                }

                var resultado = await query.ToListAsync();

                retorno.Success = true;

                if (resultado == null || resultado.Count == 0)
                {
                    retorno.Messages.Add("Nenhuma semana encontrada.");
                    retorno.Data = new List<NVMC>();
                }
                else
                {
                    var semanasAgrupadas = resultado.Select(nvmc => new
                    {
                        Id = nvmc.ID,
                        DataFormatada = nvmc.DataFormatada,
                        Semana = new
                        {
                            Id = nvmc.ID,
                            DataFormatada = nvmc.DataFormatada,
                            NVMC = nvmc,
                            PartesPorSecao = nvmc.Partes
                                .GroupBy(p => p.NomeSecao)
                                .ToDictionary(
                                    g => g.Key switch
                                    {
                                        "Presidencia" => "Presidencia",
                                        "TESOUROS DA PALAVRA DE DEUS" => "Tesouros",
                                        "FAÇA SEU MELHOR NO MINISTÉRIO" => "Faca",
                                        "NOSSA VIDA CRISTÃ" => "Vida",
                                        _ => g.Key
                                    },
                                    g => g.ToList()
                                )
                        }
                    }).ToList();

                    retorno.Messages.Add("Semana(s) encontrada(s) com sucesso.");
                    retorno.Data = semanasAgrupadas;
                }
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Não foi possível pesquisar as semanas.");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }
       

        public async Task<Reply> GetList(QueryNVMCRequest condicao)
        {
            Reply retorno = new Reply();

            try
            {
                var query = _unitOfWork.NVMCs
                                       .AsQueryable();
                if (condicao.Id > 0)
                {
                    query = query.Where(x => x.ID == condicao.Id.Value);
                }
                if (!string.IsNullOrEmpty(condicao.Mes))
                {
                    var _mes = DateTime.ParseExact(condicao.Mes, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                    query = query.Where(x => x.Mes.Month == _mes.Month && x.Mes.Year == _mes.Year);
                }
                if (!string.IsNullOrEmpty(condicao.Data))
                {
                    var _data = DateTime.ParseExact(condicao.Data, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                    query = query.Where(x => x.Data.Date == _data.Date);
                }

                var resultado = await query.ToListAsync();

                retorno.Success = true;

                if (resultado == null || resultado.Count == 0)
                {
                    retorno.Messages.Add("Nenhuma semana encontrada.");
                }
                else
                {
                    var semanasAgrupadas = resultado.Select(nvmc => new
                    {
                        Id = nvmc.ID,
                        DataFormatada = nvmc.DataFormatada,
                        Semana = nvmc.Semana
                    }).OrderBy(x => x.DataFormatada).ToList();

                    retorno.Messages.Add("Semana(s) encontrada(s) com sucesso.");
                    retorno.Data = semanasAgrupadas;
                }
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Não foi possível pesquisar as semanas.");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }

        public async Task<Reply> GetOne(int id)
        {
            Reply retorno = new Reply();

            try
            {
                var one = await _unitOfWork.NVMCs
                                           .AsQueryable()
                                           .Include(x => x.Partes).ThenInclude(x => x.Designado)
                                           .Include(x => x.Partes).ThenInclude(x => x.Ajudante)
                                           .Where(x => x.ID == id)
                                           .FirstOrDefaultAsync();

                var registro = new
                {
                    Id = one.ID,
                    DataFormatada = one.DataFormatada,                   
                    Detalhes = new
                    {
                        Id = one.ID,
                        DataFormatada = one.DataFormatada,
                        Semana = one.Semana,
                        NVMC = one,
                        PartesPorSecao = one.Partes
                               .GroupBy(p => p.NomeSecao)
                               .ToDictionary(
                                   g => g.Key switch
                                   {
                                       "Presidencia" => "Presidencia",
                                       "TESOUROS DA PALAVRA DE DEUS" => "Tesouros",
                                       "FAÇA SEU MELHOR NO MINISTÉRIO" => "Faca",
                                       "NOSSA VIDA CRISTÃ" => "Vida",
                                       _ => g.Key
                                   },
                                   g => g.ToList()
                               )
                    }
                };

                retorno.Success = true;
                retorno.Status = System.Net.HttpStatusCode.OK;
                retorno.Messages.Add("Semana encontrada com sucesso.");
                retorno.Data = registro;
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Erro ao buscar semana: ");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }

        public async Task<Reply> GetOneParte(int id)
        {
            Reply retorno = new Reply();

            try
            {
                var one = await _unitOfWork.NVMCPartes
                                           .AsQueryable()
                                           .Include(x => x.NVMC)
                                           .Include(x => x.Designado)
                                           .Include(x => x.Ajudante)
                                           .Where(x => x.ID == id)
                                           .FirstOrDefaultAsync();

                retorno.Success = true;
                retorno.Status = System.Net.HttpStatusCode.OK;
                retorno.Messages.Add("Parte encontrada com sucesso.");
                retorno.Data = one;
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Erro ao buscar parte: ");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }

        public async Task<Reply> GetPartesByPub(int pubId)
        {
            Reply retorno = new Reply();

            try
            {
                var historico = await _unitOfWork.NVMCPartes
                                               .AsQueryable()
                                               .Include(x => x.NVMC)
                                               .Include(x => x.Designado)
                                               .Include(x => x.Ajudante)
                                               .Where(x => x.DesignadoID == pubId || x.AjudanteID == pubId)
                                               .OrderByDescending(x => x.NVMC.Data)
                                               .ToListAsync();
               
                retorno.Success = true;
                retorno.Status = System.Net.HttpStatusCode.OK;
                retorno.Messages.Add("Semana encontrada com sucesso.");
                retorno.Data = historico;
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Erro ao buscar semana: ");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }

        public async Task<Reply> GetGraficoPartesByPub(int pubId)
        {
            Reply retorno = new Reply();

            try
            {
                var historico = await _unitOfWork.NVMCPartes
                    .AsQueryable()
                    .Include(x => x.NVMC)
                    .Include(x => x.Designado)
                    .Include(x => x.Ajudante)
                    .Where(x => x.DesignadoID == pubId || x.AjudanteID == pubId)
                    .OrderByDescending(x => x.NVMC.Data)
                    .ToListAsync();

                var grafico = historico
                            .GroupBy(x => x.DescricaoParte)
                            .Select(g => new
                            {
                                DescricaoParte = g.Key,
                                Quantidade = g.Count(),
                                Partes = g.ToList()
                            })
                            .OrderByDescending(x => x.Quantidade)
                            .ToList();

                retorno.Success = true;
                retorno.Status = System.Net.HttpStatusCode.OK;
                retorno.Messages.Add("Gráfico de partes encontrado com sucesso.");
                retorno.Data = grafico;
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Erro ao buscar gráfico de partes: ");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }

        public async Task<Reply> Update(UpdateNVMCRequest entidade)
        {
            Reply retorno = new Reply();

            try
            {
                var existente = await _unitOfWork.NVMCs.GetByIdAsync(entidade.Id);

                if (existente == null)
                {
                    retorno.Success = false;
                    retorno.Messages.Add("Papel não encontrado.");
                }
                else
                {
                    var _data = DateTime.ParseExact(entidade.Data, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                    var _mes = DateTime.ParseExact(entidade.Mes, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                    existente.Data = _data;
                    existente.Mes = _mes;

                    existente.Presidente = entidade.Presidente;
                    existente.OracaoInicial = entidade.OracaoInicial;
                    existente.OracaoFinal = entidade.OracaoFinal;
                    existente.Conteudo = entidade.Conteudo ?? string.Empty;
                    existente.Alterado = DateTime.Now;

                    _unitOfWork.NVMCs.Update(existente);
                    await _unitOfWork.CommitAsync();

                    retorno.Success = true;
                    retorno.Messages.Add("Semana atualizado com sucesso.");
                    retorno.Data = existente;
                }
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Erro ao atualizar semana");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }

        public async Task<Reply> UpdateParte(UpdateNVMCParteRequest entidade)
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
                    existente.Titulo = entidade.Titulo;
                    existente.Descricao = entidade.Descricao;
                    existente.DesignadoID = entidade.DesignadoId;
                    existente.AjudanteID = entidade.AjudanteId ?? null;
                    existente.Alterado = DateTime.Now;

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
