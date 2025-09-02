using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Unit.Application.Base;
using Unit.Application.DTOs.Request;
using Unit.Application.Sevices;
using Unit.Application.Util;
using Unit.Domain.Entities.Cadastro;
using Unit.Domain.Entities.Config;

namespace Unit.Infra.Services
{
    public class EventoService : IEventoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public EventoService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Reply> Add(CreateEventoRequest entidade)
        {
            Reply retorno = new Reply();

            try
            {
                var novo = new Evento()
                {
                    Criado = DateTime.Now,
                    Data = entidade.Data,
                    Ativo = true,
                    Semana = false,
                    Nome = entidade.Nome,
                    Tipo = "Evento",
                    Descricao = ""
                };

                var resultado = _unitOfWork.Eventos.AddAsync(novo);

                if (resultado.IsCompletedSuccessfully)
                {
                    await _unitOfWork.CommitAsync();
                    retorno.Success = true;
                    retorno.Messages.Add("Evento criado com sucesso.");
                    retorno.Data = novo;
                }
                else
                {
                    retorno.Success = false;
                    retorno.Messages.Add("Erro ao criar um evento.");
                    retorno.Errors.Add(resultado.Exception.Message);
                    return retorno;
                }
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Erro ao criar evento");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }

        public async Task<Reply> AddPapel(int eventoId, int papelId)
        {
            Reply retorno = new Reply();

            try
            {
                var _papel = await _unitOfWork.EventoPapeis.AsQueryable()
                                  .Where(x => x.EventoId == eventoId && x.PapelId == papelId)
                                  .FirstOrDefaultAsync();

                if (_papel == null)
                {
                    var novo = _unitOfWork.EventoPapeis.AddAsync(new EventoPapel
                    {
                        EventoId = eventoId,
                        PapelId = papelId,
                        Criado = DateTime.Now
                    });

                    if (novo.IsCompletedSuccessfully)
                    {
                        await _unitOfWork.CommitAsync();
                        retorno.Success = true;
                        retorno.Messages.Add("Papel adicionado ao evento com sucesso.");
                    }
                    else
                    {
                        retorno.Success = false;
                        retorno.Messages.Add("Erro ao adicionar papel ao evento.");
                        retorno.Errors.Add(novo.Exception.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Erro ao adicionar perfil ao usuário.");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }

        public Task<Reply> RemPapel(int id)
        {
            Reply retorno = new Reply();
            try
            {
                var remover = _unitOfWork.EventoPapeis.GetByIdAsync(id).Result;
                if (remover != null)
                {
                    _unitOfWork.EventoPapeis.Delete(remover);
                    _unitOfWork.CommitAsync().Wait();
                    retorno.Success = true;
                    retorno.Messages.Add("Papel removido do evento com sucesso.");
                }
                else
                {
                    retorno.Success = false;
                    retorno.Messages.Add("Papel não encontrado ou já removido.");
                }
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Erro ao remover papel do evento.");
                retorno.Errors.Add(ex.Message);
            }
            return Task.FromResult(retorno);
        }

        public async Task<Reply> GetAll(QueryEventoRequest condicao)
        {
            Reply retorno = new Reply();

            try
            {
                var query = _unitOfWork.Eventos.AsQueryable();

                if (condicao.Id.HasValue && condicao.Id.Value > 0)
                {
                    query = query.Where(x => x.ID == condicao.Id.Value);
                }
                if (!string.IsNullOrEmpty(condicao.Nome))
                {
                    query = query.Where(x => x.Nome.ToLower().Contains(condicao.Nome.ToLower()));
                }
                if (!string.IsNullOrEmpty(condicao.Mes) || !string.IsNullOrEmpty(condicao.Ano))
                {
                    if (!string.IsNullOrEmpty(condicao.Mes) && !string.IsNullOrEmpty(condicao.Ano))
                    {
                        query = query.Where(x => x.Data.Year == Convert.ToInt32(condicao.Ano) && x.Data.Month == Convert.ToInt32(condicao.Mes));
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(condicao.Mes))
                        {
                            query = query.Where(x => x.Data.Month == Convert.ToInt32(condicao.Mes));
                        }

                        if (!string.IsNullOrEmpty(condicao.Ano))
                        {
                            query = query.Where(x => x.Data.Year == Convert.ToInt32(condicao.Ano));
                        }
                    }
                }

                var resultado = await query.ToListAsync();

                retorno.Success = true;

                if (resultado == null || resultado.Count == 0)
                {
                    retorno.Messages.Add("Nenhum Evento encontrado.");
                    retorno.Data = new List<Evento>();
                }

                retorno.Messages.Add("Evento(s) encontrado(s) com sucesso.");
                retorno.Data = resultado.OrderByDescending(x => x.Data).ToList();
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Não foi possível pesquisar eventos.");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }

        public async Task<Reply> GetOne(int id)
        {
            Reply retorno = new Reply();

            try
            {
                var one = await _unitOfWork.Eventos.AsQueryable()
                                .Include(x => x.Papeis).ThenInclude(x => x.Papel)
                                .Where(x => x.ID == id)
                                .FirstOrDefaultAsync();

                retorno.Success = true;
                retorno.Status = System.Net.HttpStatusCode.OK;
                retorno.Messages.Add("Evento encontrado com sucesso.");
                retorno.Data = one != null ? one : new Evento();
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Erro ao buscar evento: ");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }

        public async Task<Reply> Update(UpdateEventoRequest entidade)
        {
            Reply retorno = new Reply();

            try
            {
                var existente = await _unitOfWork.Eventos.GetByIdAsync(entidade.Id);

                if (existente == null)
                {
                    retorno.Success = false;
                    retorno.Messages.Add("Evento não encontrado.");
                }
                else
                {
                    existente.Alterado = DateTime.Now;
                    existente.Nome = entidade.Nome;
                    existente.Ativo = entidade.Ativo;
                    existente.Semana = entidade.Semana;
                    existente.Descricao = entidade.Descricao;
                    existente.Data = entidade.Data;
                    existente.Tipo = !string.IsNullOrEmpty(entidade.Tipo) ? entidade.Tipo : "Evento";

                    _unitOfWork.Eventos.Update(existente);
                    await _unitOfWork.CommitAsync();

                    if (!string.IsNullOrEmpty(entidade.PapeisSelecionados))
                    {
                        foreach (string item in entidade.PapeisSelecionados.Split(","))
                        {
                            var addPapel = await this.AddPapel(entidade.Id, Convert.ToInt32(item));
                        }

                        var papeisEvento = await _unitOfWork.EventoPapeis.AsQueryable()
                                                        .AsNoTracking()
                                                        .Where(x => x.EventoId == entidade.Id)
                                                        .ToListAsync();
                        foreach (var item in papeisEvento)
                        {
                            if (!entidade.PapeisSelecionados.Split(",").Select(int.Parse).ToArray().Contains(item.PapelId))
                            {
                                var remPapel = await this.RemPapel(item.ID);
                            }
                        }
                    }

                    retorno.Success = true;
                    retorno.Messages.Add("Evento atualizado com sucesso.");
                    retorno.Data = existente;
                }
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Erro ao atualizar evento");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }

        public async Task<Reply> GetByPapel(QueryEventoByPapelRequest condicao)
        {
            Reply retorno = new Reply();

            try
            {
                var query = _unitOfWork.Eventos
                                .AsQueryable()
                                .Include(x => x.Papeis).ThenInclude(x => x.Papel);

                if (!string.IsNullOrEmpty(condicao.IdsSelecionados))
                {
                    var ids = condicao.IdsSelecionados?.Split(",").Select(int.Parse).ToArray();

                    if (ids != null && ids.Length > 0)
                    {
                        query.Where(x => x.Papeis.Any(p => ids.Contains(p.PapelId)) || !x.Papeis.Any());
                    }
                    else
                    {
                        query.Where(x => !x.Papeis.Any());
                    }
                }

                var resultado = await query
                                        .Where(x => x.Ativo && x.Data >= DateTime.Now)
                                        .ToListAsync();

                retorno.Success = true;

                if (resultado == null || resultado.Count == 0)
                {
                    retorno.Messages.Add("Nenhum Evento encontrado.");
                    retorno.Data = new List<Evento>();
                }
                else
                {
                    retorno.Messages.Add("Evento(s) encontrado(s) com sucesso.");
                    retorno.Data = resultado.OrderBy(x => x.Data).Take(condicao.Quantidade).ToList();
                }
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Não foi possível pesquisar eventos.");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }
    }
}
