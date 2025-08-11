using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unit.Application.Base;
using Unit.Application.DTOs.Request;
using Unit.Application.DTOs.Response;
using Unit.Application.Services;
using Unit.Application.Sevices;
using Unit.Application.Util;
using Unit.Domain.Entities.Acesso;
using Unit.Domain.Entities.Config;
using Unit.Domain.Entities.Extra;

namespace Unit.Infra.Services
{
    public class QueueService : IQueueService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IEvolutionService _evolution;
        private readonly IEmailService _emailService;

        public QueueService(IUnitOfWork unitOfWork, IMapper mapper, IEvolutionService evolution, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _evolution = evolution;
            _emailService = emailService;
        }
        public async Task<Reply> Add(CreateQueueRequest entidade)
        {
            Reply retorno = new Reply();

            try
            {
                var novo = _mapper.Map<Unit.Domain.Entities.Extra.Queue>(entidade);
                novo.Criado = DateTime.Now;
                novo.processed = false;
                novo.success = false;
                novo.enabled = true;

                var resultado = _unitOfWork.Queues.AddAsync(novo);

                if (resultado.IsCompletedSuccessfully)
                {
                    await _unitOfWork.CommitAsync();
                    retorno.Success = true;
                    retorno.Messages.Add("Mensagem enfilerada com sucesso.");
                    retorno.Data = _mapper.Map<QueueResponse>(novo);
                }
                else
                {
                    retorno.Success = false;
                    retorno.Messages.Add("Erro enfilerar mensagem.");
                    retorno.Errors.Add(resultado.Exception.Message);
                    return retorno;
                }
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Erro ao criar usuário");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }

        public async Task<Reply> GetAll(QueryQueueRequest condicao)
        {
            Reply retorno = new Reply();

            try
            {
                var query = _unitOfWork.Queues.AsQueryable();

                if (condicao.Id.HasValue && condicao.Id.Value > 0)
                {
                    query = query.Where(x => x.ID == condicao.Id.Value);
                }
                if (!string.IsNullOrEmpty(condicao.Source))
                {
                    query = query.Where(x => x.Source.ToLower().Contains(condicao.Source.ToLower()));
                }
                if (condicao.MessageMode > 0)
                {
                    query = query.Where(x => x.MessageMode == condicao.MessageMode);
                }
                if (!string.IsNullOrEmpty(condicao.Instance))
                {
                    query = query.Where(x => x.Instance.ToLower().Contains(condicao.Instance.ToLower()));
                }

                var resultado = query.ToList();

                retorno.Success = true;

                if (resultado == null || resultado.Count == 0)
                {
                    retorno.Messages.Add("Nenhum item da fila foi encontrado.");
                    retorno.Data = new List<QueueResponse>();
                }

                retorno.Messages.Add("Itens da fila encontrado(s) com sucesso.");
                retorno.Data = _mapper.Map<List<QueueResponse>>(resultado);
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Erro ao pesquisar fila: ");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }

        public async Task<Reply> GetForProcess(QueryQueueForProcess condicao)
        {
            Reply retorno = new Reply();
            DateTime now = System.DateTime.Now;
            try
            {
                var query = _unitOfWork.Queues.AsQueryable()
                            .Where(x => x.processed == false && x.enabled == true && x.success == false)
                            .Where(x => DateTime.Compare((DateTime)x.SendDate, now) < 0);

                if (condicao.MessageMode > 0)
                {
                    query = query.Where(x => x.MessageMode == condicao.MessageMode);
                }

                var resultado = query.ToList().Take(condicao.Quantity);

                retorno.Success = true;

                if (resultado == null)
                {
                    retorno.Messages.Add("Nenhum item da fila foi encontrado.");
                    retorno.Data = new List<QueueResponse>();
                }
                else
                {
                    retorno.Messages.Add("Itens da fila encontrado(s) com sucesso.");
                    retorno.Data = _mapper.Map<List<QueueResponse>>(resultado);
                }
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Erro ao pesquisar fila: ");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }

        public async Task<Reply> GetGraphics(QueryQueueGraphicsRequest condicao)
        {
            Reply retorno = new Reply();

            try
            {
                var _inicio = DateTime.Parse(condicao.Inicio);
                var _fim = DateTime.Parse($"{condicao.Fim} 23:59:59");

                var dados = await _unitOfWork.Queues.AsQueryable()
                    .Where(x => x.SendDate >= _inicio && x.SendDate <= _fim)
                    .ToListAsync();

                var grouped = dados
                    .GroupBy(x => new { x.Status })
                    .Select(g => new GraphicResponse
                    {
                        Status = g.Key.Status,
                        Quantidade = g.Count()
                    })
                    .OrderBy(x => x.Status)
                    .ToList();

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
                var one = await _unitOfWork.Queues.AsQueryable()
                            .Where(x => x.ID == id)
                            .FirstOrDefaultAsync();

                retorno.Success = true;
                retorno.Status = System.Net.HttpStatusCode.OK;
                retorno.Messages.Add("Item encontrado com sucesso.");
                retorno.Data = one != null ? _mapper.Map<QueueResponse>(one) : new QueueResponse();
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Erro ao buscar item: ");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }

        public async Task<Reply> ProcessQueue(QueryQueueForProcess condicao)
        {
            Reply retorno = new Reply();
            List<UpdateQueueRequest> listaUpdate = new List<UpdateQueueRequest>();

            try
            {
                var fila = await this.GetForProcess(condicao);

                if (fila.Success == true && fila.Data != null)
                {
                    foreach (var item in fila.Data as List<QueueResponse>)
                    {
                        #region Update Item

                        UpdateQueueRequest updateItem = new UpdateQueueRequest
                        {
                            Id = item.Id,
                            Source = item.Source,
                            MessageMode = item.MessageMode,
                            Instance = item.Instance,
                            Name = item.Name,
                            Address = item.Address,
                            Message = item.Message,
                            SendDate = item.SendDate,
                            log = string.Empty
                        };

                        #endregion

                        switch (item.MessageMode)
                        {
                            case 1:  //Whatsapp

                                var _whatsapp = await _evolution.SendMessage(new SendEvolutionMessageRequest()
                                {
                                    Instance = item.Instance,
                                    Number = item.Address,
                                    TextMessage = new TextMessage { Text = item.Message }
                                });

                                if (_whatsapp.Success)
                                {
                                    retorno.Messages.Add($"Whatsapp para {item.Name} {item.Address} enviado com sucesso.");
                                    item.processed = true;
                                    item.success = true;
                                    item.attempts = item.attempts + 1;
                                    item.log = _whatsapp.Messages.FirstOrDefault();

                                    #region Update

                                    updateItem.processed = true;
                                    updateItem.success = true;
                                    updateItem.enabled = false;
                                    updateItem.log = $"Whatsapp para {item.Name} {item.Address} enviado com sucesso.";
                                    updateItem.attempts = item.attempts + 1;

                                    #endregion
                                }
                                else
                                {
                                    retorno.Messages.Add($"Erro ao enviar Whatsapp para {item.Name} {item.Address}: {string.Join(", ", _whatsapp.Errors)}");
                                    item.processed = true;
                                    item.success = false;
                                    item.attempts = item.attempts + 1;
                                    item.enabled = item.attempts < 3; // Desabilita após 3 tentativas
                                    item.log = string.Join(", ", _whatsapp.Errors);

                                    #region Update

                                    updateItem.processed = true;
                                    updateItem.success = false;
                                    updateItem.enabled = updateItem.attempts + 1 < 3; // Desabilita após 3 tentativas
                                    updateItem.log = $"Erro ao enviar Whatsapp para {item.Name} {item.Address}: {string.Join(", ", _whatsapp.Errors)}";
                                    updateItem.attempts = updateItem.attempts + 1;

                                    #endregion
                                }
                                break;

                            default: // E-mail

                                var SendEmail = await _emailService.SendEmailAsync(new SendEmailRequest
                                {
                                    To = item.Address,
                                    Subject = item.Name,
                                    Body = item.Message
                                });

                                retorno.Messages.Add($"Processando item {item.Id} para modo {item.MessageMode}.");
                                break;
                        }

                        listaUpdate.Add(updateItem);
                    }

                    #region Update processed items
                    foreach (var item in listaUpdate)
                    {
                        var updateResult = await this.Update(item);
                        if (updateResult.Success)
                        {
                            retorno.Messages.Add($"Item {item.Id} atualizado com sucesso.");
                        }
                        else
                        {
                            retorno.Messages.Add($"Erro ao atualizar item {item.Id}: {string.Join(", ", updateResult.Errors)}");
                        }
                    }
                    #endregion
                }
                else
                {
                    retorno.Messages.Add("Nenhum item da fila foi encontrado para processamento.");
                }
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Erro ao processar fila: ");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }

        public async Task<Reply> Update(UpdateQueueRequest entidade)
        {
            Reply retorno = new Reply();

            try
            {
                var existente = await _unitOfWork.Queues.GetByIdAsync(entidade.Id);

                if (existente == null)
                {
                    retorno.Success = false;
                    retorno.Messages.Add("Item não encontrado.");
                }
                else
                {
                    var registro = _mapper.Map<Queue>(existente);
                    registro.Alterado = DateTime.Now;
                    registro.Source = entidade.Source;
                    registro.Instance = entidade.Instance;
                    registro.MessageMode = entidade.MessageMode;
                    registro.Name = entidade.Name;
                    registro.Address = entidade.Address;
                    registro.SendDate = DateTime.Parse(entidade.SendDate);
                    registro.processed = entidade.processed;
                    registro.processed = entidade.success;
                    registro.enabled = entidade.enabled;
                    registro.log = entidade.log;
                    registro.attempts = existente.attempts + 1;

                    _unitOfWork.Queues.Update(registro);
                    await _unitOfWork.CommitAsync();

                    retorno.Success = true;
                    retorno.Messages.Add("Item atualizado com sucesso.");
                    retorno.Data = _mapper.Map<QueueResponse>(registro);
                }
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Erro ao atualizar perfil");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }
    }
}
