using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Unit.Application.Base;
using Unit.Application.DTOs.Request;
using Unit.Application.DTOs.Response;
using Unit.Application.Services;
using Unit.Application.Util;
using Unit.Domain.Entities.Cadastro;

namespace Unit.Infra.Services
{
    public class DiscursoService : IDiscursoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DiscursoService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Reply> AddAsync(DiscursoNewRequest novo)
        {
            Reply retorno = new Reply();

            try
            {
                var _novo = new Discurso()
                {
                    ArranjoID = novo.ArranjoId,
                    Data = DateTime.Parse(novo.Data),
                    TemaID = novo.TemaId,
                    Contato = novo.Contato,
                    Orador = novo.Orador,
                    Recebido = novo.Recebido,
                    Status = novo.Status,
                    Criado = System.DateTime.Now,
                };

                var resultado = _unitOfWork.Discursos.AddAsync(_novo);

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

        public async Task<bool> DeleteById(int id)
        {
            bool retorno = false;
            var delete = _unitOfWork.Discursos.AsQueryable()
                                .Where(x => x.ID == id)
                               .FirstOrDefault();
            if (delete != null)
            {
                _unitOfWork.Discursos.Delete(delete);
                await _unitOfWork.CommitAsync();
                retorno = true;
            }

            return retorno;
        }

        public async Task<bool> DeleteNotificationById(int id)
        {
            bool retorno = false;
            var delete = await _unitOfWork.DiscursoNotificacoes
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
                    await _unitOfWork.CommitAsync();
                    retorno = true;
                }
            }

            return retorno;
        }

        public async Task<string> GenerateContent(int id)
        {
            string _conteudo = "";
            var _discurso = await _unitOfWork.Discursos.AsQueryable()
                                              .Include(x => x.Arranjo).ThenInclude(x => x.Cong)
                                              .Include(x => x.Tema)
                                              .Where(x => x.ID.Equals(id))
                                              .FirstOrDefaultAsync();

            _conteudo = "\n*Discurso público*\n\n";
            _conteudo += $"*Data:* {_discurso.DataFormatada}\n";
            _conteudo += $"*Tema:* {_discurso.Tema.Codigo} - {_discurso.Tema.Nome}\n\n";
            _conteudo += $"*Congregação:* {_discurso.Arranjo.Cong.Nome}\n";
            _conteudo += $"*Reunião:* {_discurso.Arranjo.Cong.Dia} às {_discurso.Arranjo.Cong.Horario}\n";
            _conteudo += $"*Endereço:* {_discurso.Arranjo.Cong.Endereco} \n\n";
            _conteudo += $"*Contato:* {_discurso.Arranjo.Cong.Responsavel} \n";
            _conteudo += $"{_discurso.Arranjo.Cong.Fone}\n\n";

            return _conteudo;
        }

        public async Task<string> GenerateNotification(int id, int messageId, string conteudo, bool whatsapp)
        {
            var _discurso = await _unitOfWork.Discursos
                                    .AsQueryable()
                                    .Where(x => x.ID.Equals(id)).FirstOrDefaultAsync();

            var _mensagem = await _unitOfWork.Mensagens
                                        .AsQueryable()
                                        .Where(x => x.ID == messageId).FirstOrDefaultAsync();

            string _conteudo = whatsapp ? _mensagem.ConteudoRenderizado : _mensagem.Conteudo;

            _conteudo = _conteudo.Replace("@Nome", _discurso.Orador);
            _conteudo = _conteudo.Replace("@Mes", _discurso.Mes);
            _conteudo = _conteudo.Replace("@Data", _discurso.DataFormatada);
            _conteudo = _conteudo.Replace("@Conteudo", conteudo);

            return _conteudo;
        }

        public async Task<Reply> GetAll(DiscursoQueryRequest condicao)
        {
            Reply retorno = new Reply();

            try
            {
                var query = _unitOfWork.Discursos.AsQueryable()
                                        .Include(x => x.Arranjo).ThenInclude(x => x.Cong)
                                        .Include(x => x.Tema).ToList();

                if (condicao.ArranjoId > 0)
                {
                    query = query.Where(x => x.ArranjoID == condicao.ArranjoId).ToList();
                }

                if (condicao.CongId > 0)
                {
                    query = query.Where(x => x.Arranjo.CongId == condicao.CongId).ToList();
                }

                if (!string.IsNullOrEmpty(condicao.Tema) || !string.IsNullOrEmpty(condicao.Numero))
                {
                    if (!string.IsNullOrEmpty(condicao.Tema))
                    {
                        query = query.Where(x => x.Tema.Nome.ToLower().Contains(condicao.Tema.ToLower())).ToList();
                    }

                    if (!string.IsNullOrEmpty(condicao.Numero))
                    {
                        query = query.Where(x => x.Tema.Codigo.ToLower().Contains(condicao.Numero.ToLower())).ToList();
                    }
                }

                if (!string.IsNullOrEmpty(condicao.Mes) && !string.IsNullOrEmpty(condicao.Ano))
                {
                    string _mes = $"{condicao.Ano}-{condicao.Mes}";
                    query = query.Where(x => x.Data.Value.Year == Convert.ToInt32(condicao.Ano) && x.Data.Value.Month==Convert.ToInt32(condicao.Mes)).ToList();
                }
                else
                {
                    if (!string.IsNullOrEmpty(condicao.Ano))
                    {
                        query = query.Where(x => x.Data.Value.Year == Convert.ToInt32(condicao.Ano)).ToList();
                    }
                }

                if (!string.IsNullOrEmpty(condicao.Orador))
                {
                    query = query.Where(x => x.Orador.ToLower().Contains(condicao.Orador.ToLower())).ToList();
                }

                if (condicao.Recebido.HasValue)
                {
                    query = query.Where(x => x.Recebido == condicao.Recebido.Value).ToList();
                }

                query = query.OrderByDescending(x => x.Recebido).ThenBy(t => t.Data).ToList();

                retorno.Success = true;
                retorno.Messages.Add("Consulta realizada com sucesso.");
                retorno.Data = query;

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
                var dados = await _unitOfWork.Discursos
                                        .AsQueryable()
                                        .Include(x => x.Arranjo).ThenInclude(x => x.Cong)
                                        .Include(x => x.Tema)
                                        .Include(x => x.Notificacoes).ThenInclude(x => x.Queue)
                                        .Where(x => x.ID == id)
                                        .FirstOrDefaultAsync();

                retorno.Success = true;
                retorno.Messages.Add("Registro encontrado com sucesso.");
                retorno.Data = dados;
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Não foi possível consultar o registro.");
                retorno.Errors.Add($"{ex.Message}");
            }

            return retorno;
        }

        public Task<Reply> GetDiscursos(DiscursoQueryRequest condicao)
        {
            return GetAll(condicao);
        }

        public async Task<Reply> GetDiscursosByYear(DiscursoQueryRequest condicao, int ano)
        {
            Reply retorno = new Reply();

            try
            {
                var query = _unitOfWork.Discursos.AsQueryable()
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

        public async Task<Reply> UpdateAsync(DiscursoUpdateRequest dados)
        {
            Reply retorno = new Reply();

            try
            {
                var registro = await _unitOfWork.Discursos.GetByIdAsync(dados.ID);

                if (registro != null)
                {
                    registro.Alterado = System.DateTime.Now;
                    registro.ArranjoID = dados.ArranjoId;
                    registro.Data = DateTime.Parse(dados.Data);
                    registro.Orador = dados.Orador;
                    registro.TemaID = dados.TemaId;
                    registro.Status = dados.Status;
                    registro.Obs = dados.Obs;

                    _unitOfWork.Discursos.Update(registro);
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
