using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Unit.Application.Base;
using Unit.Application.DTOs.Request;
using Unit.Application.DTOs.Response;
using Unit.Application.Services;
using Unit.Application.Util;
using Unit.Domain.Entities.Cadastro;
using Unit.Domain.Entities.Extra;

namespace Unit.Infra.Services
{
    public class ArranjoService : IArranjoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ArranjoService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Reply> AddAsync(ArranjoNewRequest novo)
        {
            Reply retorno = new Reply();

            try
            {
                var _novo = new Arranjo()
                {
                    CongId = novo.CongId,
                    Data = DateTime.Parse(novo.Data),
                    Modo = "Troca",
                    Status = "Aberto",
                    Criado = System.DateTime.Now,
                };
                var resultado = _unitOfWork.Arranjos.AddAsync(_novo);

                if (resultado.IsCompletedSuccessfully)
                {
                    await _unitOfWork.CommitAsync();

                    retorno.Success = true;
                    retorno.Messages.Add("Adicionado com sucesso.");
                    retorno.Data = _novo;
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

        public async Task<bool> DeleteNotificationById(int id)
        {
            bool retorno = false;
            var delete = await _unitOfWork.ArranjoNotificacoes
                                .AsQueryable()
                                .Where(x => x.ID == id)
                                .FirstOrDefaultAsync();
            if (delete != null)
            {
                var _queue = await _unitOfWork.Queues
                                            .AsQueryable()
                                            .Where(x => x.ID == delete.QueueID)
                                            .FirstOrDefaultAsync();
                if (_queue != null)
                {
                    _unitOfWork.Queues.Delete(_queue);                   
                }

                _unitOfWork.ArranjoNotificacoes.Delete(delete);
                await _unitOfWork.CommitAsync();

                retorno = true;
            }

            return retorno;
        }

        public async Task<string> GenerateContent(int id, string tipo)
        {
            string _conteudo = "";
            var _arranjo = await _unitOfWork.Arranjos
                                            .AsQueryable()
                                            .Where(x => x.ID.Equals(id))
                                            .Include(x => x.Cong)
                                            .Include(x => x.Discursos).ThenInclude(x => x.Tema)
            .FirstOrDefaultAsync();

            switch (tipo)
            {
                case "AvisoArranjo":
                    #region Discursos

                    var _discursos = _unitOfWork.Discursos
                                                .AsQueryable()
                                                .Where(x => x.Data.Value.Month == _arranjo.Data.Value.Month && x.Data.Value.Year == _arranjo.Data.Value.Year)
                                                 .Include(x => x.Tema)
                                                 .OrderBy(x => x.Data)
                                                 .ToListAsync();

                    _conteudo = "\n*Arranjo de oradores*\n\n";
                    _conteudo += $"*Mês:* {_arranjo.Mes}\n";
                    _conteudo += $"*Congregação:* {_arranjo.Cong.Nome}\n";
                    _conteudo += $"*Reunião:* {_arranjo.Cong.Dia} às {_arranjo.Cong.Horario}\n";
                    _conteudo += $"*Endereço:* {_arranjo.Cong.Endereco}\n";
                    _conteudo += !string.IsNullOrEmpty(_arranjo.Cong.Maps) ? $"*Maps: - {_arranjo.Cong.Maps}\n"
                                                                           : "\n";
                    _conteudo += $"*Responsável:* {_arranjo.Cong.Responsavel}\n";
                    _conteudo += $"*Contato:* {_arranjo.Cong.Fone}\n\n";
                    _conteudo += $"*Whatsapp:* https://api.whatsapp.com/send?phone=+{_arranjo.Cong.Whatsapp}\n\n";

                    _conteudo += $"*Oradores recebidos:*\n\n";
                    foreach (var item in _discursos.Result
                                                   .Where(x => x.Recebido == true).ToList())
                    {
                        _conteudo += $"*Data:* {item.DataFormatada}\n";
                        _conteudo += $"*Tema:* {item.Tema.Codigo}-{item.Tema.Nome}\n";
                        _conteudo += $"*Orador:* {item.Orador}\n\n";
                    }

                    _conteudo += $"*Oradores enviados:*\n\n";
                    foreach (var item in _discursos.Result
                                                   .Where(x => x.Recebido == false).ToList())
                    {
                        _conteudo += $"*Data:* {item.DataFormatada}\n";
                        _conteudo += $"*Tema:* {item.Tema.Codigo}-{item.Tema.Nome}\n";
                        _conteudo += $"*Orador:* {item.Orador}\n\n";
                    }

                    #endregion
                    break;

                default:
                    #region Dados arranjo

                    var _cong = await _unitOfWork.Congs.AsQueryable()
                                         .Where(x => x.ID == 1)
                                         .FirstOrDefaultAsync();

                    _conteudo = $"\n\n*Congregação:* {_cong.Nome}\n";
                    _conteudo += $"*Reunião:* {_cong.Dia} às {_cong.Horario}\n";
                    _conteudo += $"*Endereço:* {_cong.Endereco}\n";
                    _conteudo += !string.IsNullOrEmpty(_cong.Maps) ? $"*Maps:* {_cong.Maps}\n\n"
                                                                   : "\n\n";

                    #endregion
                    break;
            }

            return _conteudo;
        }

        public async Task<string> GenerateNotification(int id, int messageId, string conteudo, bool whatsapp)
        {
            var _arranjo = await _unitOfWork.Arranjos.AsQueryable()
                                  .Where(x => x.ID.Equals(id))
                                  .Include(x => x.Cong)
                                  .FirstOrDefaultAsync();

            var _mensagem = await _unitOfWork.Mensagens.AsQueryable()
                                             .Where(x => x.ID == messageId).FirstOrDefaultAsync();

            string _conteudo = whatsapp ? _mensagem.ConteudoRenderizado : _mensagem.Conteudo;

            _conteudo = _conteudo.Replace("@Nome", _arranjo.Cong.Responsavel);
            _conteudo = _conteudo.Replace("@Mes", _arranjo.Mes);
            _conteudo = _conteudo.Replace("@Conteudo", conteudo);

            return _conteudo;
        }

        public async Task<Reply> GetAll(ArranjoQueryRequest condicao)
        {
            Reply retorno = new Reply();

            try
            {
                var query = _unitOfWork.Arranjos
                                      .AsQueryable()
                                      .Include(x => x.Cong)
                                      .Include(x => x.Discursos)
                                      .Include(x => x.Notificacoes).ThenInclude(x => x.Queue)
                                      .AsQueryable(); // Ensure query is IQueryable<Arranjo>

                if (condicao.CongId.HasValue && condicao.CongId.Value > 0)
                {
                    query = query.Where(x => x.CongId == condicao.CongId.Value);
                }

                if (!string.IsNullOrEmpty(condicao.Mes) && !string.IsNullOrEmpty(condicao.Mes))
                {
                    query = query.Where(x => x.Data.Value.Month == Convert.ToInt32(condicao.Mes)
                                          && x.Data.Value.Year == Convert.ToInt32(condicao.Ano));
                }
                else
                {
                    if (!string.IsNullOrEmpty(condicao.Ano) && string.IsNullOrEmpty(condicao.Mes))
                    {
                        query = query.Where(x => x.Data.Value.Year == Convert.ToInt32(condicao.Ano));
                    }
                }

                var resultado = await query.ToListAsync();

                if (!string.IsNullOrEmpty(condicao.Ano) && string.IsNullOrEmpty(condicao.Mes))
                {
                    int[] meses = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };

                    foreach(var mes in meses)
                    {
                        if (!resultado.Any(x => x.Data.Value.Month == mes && x.Data.Value.Year == Convert.ToInt32(condicao.Ano)))
                        {
                            var mesFaltando = new DateTime(Convert.ToInt32(condicao.Ano), mes, 1);
                            resultado.Add(new Arranjo()
                            {
                                ID = 0, // ID will be set by the database
                                Data = mesFaltando,
                                CongId = 1, // Default or placeholder value
                                Cong = new Cong() { Nome = "Em Aberto" }, // Placeholder for Cong
                                Discursos = new List<Discurso>(),
                                Notificacoes = new List<ArranjoNotificacao>()
                            });
                        }
                    }
                }

                retorno.Success = true;
                retorno.Messages.Add("Consulta realizada com sucesso.");
                retorno.Data = resultado.OrderBy(x => x.Data).ToList();

            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Não foi possível realizar a consulta.");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }

        public async Task<Reply> GetAllByYear(ArranjoQueryRequest condicao, int ano)
        {
            Reply retorno = new Reply();

            try
            {
                var query = _unitOfWork.Arranjos.AsQueryable()
                                                .Where(x => x.Data.Value.Year == ano);
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
                var dados = await _unitOfWork.Arranjos.AsQueryable()
                                                .Include(x => x.Cong)
                                                .Include(x => x.Notificacoes).ThenInclude(x => x.Queue)
                                                .Where(x => x.ID == id).FirstOrDefaultAsync();

                if (dados != null)
                {
                    dados.Discursos = await _unitOfWork.Discursos.AsQueryable()
                                        .Where(x => x.ArranjoID == id)
                                        .Select(x => new Discurso()
                                        {
                                            ID = x.ID,
                                            Arranjo = new Arranjo()
                                            {
                                                ID = x.Arranjo.ID,
                                                Data = x.Arranjo.Data
                                            },
                                            Data = x.Data,
                                            Tema = new Tema()
                                            {
                                                Codigo = x.Tema.Codigo,
                                                Nome = x.Tema.Nome,
                                            },
                                            Orador = x.Orador,
                                            Status = x.Status,
                                            Obs = x.Obs,
                                            Notificacoes = x.Notificacoes
                                        }).ToListAsync() ?? new List<Discurso>();

                    dados.Notificacoes = await _unitOfWork.ArranjoNotificacoes.AsQueryable()
                                                    .Where(x => x.ArranjoID == id)
                                                    .Select(x => new ArranjoNotificacao()
                                                    {
                                                        ID = x.ID,
                                                        ArranjoID = x.ArranjoID,
                                                        QueueID = x.QueueID,
                                                        Queue = x.Queue,
                                                        Tipo = x.Tipo
                                                    }).ToListAsync();

                    retorno.Success = true;
                    retorno.Messages.Add("Arranjo encontrado com sucesso.");
                    retorno.Data = dados;
                }
                else
                {
                    retorno.Success = false;
                    retorno.Messages.Add("Arranjo não encontrado.");
                }
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Não foi possível realizar a consulta.");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }

        public async Task<Reply> UpdateAsync(ArranjoUpdateRequest dados)
        {
            Reply retorno = new Reply();

            try
            {
                var registro = await _unitOfWork.Arranjos.GetByIdAsync(dados.ID);

                if (registro != null)
                {
                    registro.Alterado = System.DateTime.Now;
                    registro.CongId = dados.CongId;
                    registro.Data = DateTime.Parse(dados.Data);
                    registro.Modo = dados.Modo;
                    registro.Status = dados.Status;
                    registro.Obs = dados.Obs;

                    _unitOfWork.Arranjos.Update(registro);
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
