using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unit.Application.Base;
using Unit.Application.DTOs.Request;
using Unit.Application.DTOs.Response;
using Unit.Application.Sevices;
using Unit.Application.Util;
using Unit.Domain.Entities.Acesso;
using Unit.Domain.Entities.Config;

namespace Unit.Infra.Services
{
    public class MensagemService : IMensagemService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MensagemService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Reply> Add(CreateMensagemRequest entidade)
        {
            Reply retorno = new Reply();

            try
            {
                var novo = _mapper.Map<Mensagem>(entidade);
                novo.Criado = DateTime.Now;

                var resultado = _unitOfWork.Mensagens.AddAsync(novo);

                if (resultado.IsCompletedSuccessfully)
                {
                    await _unitOfWork.CommitAsync();
                    retorno.Success = true;
                    retorno.Messages.Add("Mensagem criada com sucesso.");
                    retorno.Data = _mapper.Map<MensagemResponse>(novo);
                }
                else
                {
                    retorno.Success = false;
                    retorno.Messages.Add("Erro ao criar uma mensagem.");
                    retorno.Errors.Add(resultado.Exception.Message);
                    return retorno;
                }
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Erro ao criar mensagem");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }

        public async Task<Reply> GetAll(QueryMensagemRequest condicao)
        {
            Reply retorno = new Reply();

            try
            {
                var query = _unitOfWork.Mensagens.AsQueryable();

                if (condicao.Id > 0)
                {
                    query = query.Where(x => x.ID == condicao.Id);
                }
                if (!string.IsNullOrEmpty(condicao.Nome))
                {
                    query = query.Where(x => x.Nome.ToLower().Contains(condicao.Nome.ToLower()));
                }

                var resultado = query.ToList();

                retorno.Success = true;

                if (resultado == null || resultado.Count == 0)
                {
                    retorno.Messages.Add("Nenhuma mensagem encontrada.");
                    retorno.Data = new List<MensagemResponse>();
                }

                retorno.Messages.Add("Mensagem(s) encontrada(s) com sucesso.");
                retorno.Data = _mapper.Map<List<MensagemResponse>>(resultado);
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Não foi possível pesquisar a mensagem.");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }

        public async Task<Reply> GetOne(int id)
        {
            Reply retorno = new Reply();

            try
            {
                var one = await _unitOfWork.Mensagens.AsQueryable()
                            .Where(x => x.ID == id)
                            .FirstOrDefaultAsync();

                retorno.Success = true;
                retorno.Status = System.Net.HttpStatusCode.OK;
                retorno.Messages.Add("Mensagem encontrado com sucesso.");
                retorno.Data = one != null ? _mapper.Map<MensagemResponse>(one) : new MensagemResponse();
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Erro ao buscar mensagem: ");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }

        public async Task<Reply> Render(Mensagem msg)
        {
            Reply retorno = new Reply();

            try
            {
                if (msg != null)
                {
                    var conteudo = msg.ConteudoRenderizado;

                    if (msg.Marcadores != null && msg.Marcadores.Count > 0)
                    {
                        foreach (var marcador in msg.Marcadores)
                        {
                            conteudo = conteudo.Replace(marcador.Marcador, marcador.Valor);
                        }
                    }

                    retorno.Success = true;
                    retorno.Messages.Add("Mensagem renderizada com sucesso.");
                    retorno.Data = conteudo;
                }
                else
                {
                    retorno.Success = false;
                    retorno.Messages.Add("Mensagem não informada.");
                }
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Erro ao renderizar mensagem: ");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }

        public async Task<Reply> Update(UpdateMensagemRequest entidade)
        {
            Reply retorno = new Reply();

            try
            {
                var existente = await _unitOfWork.Mensagens.GetByIdAsync(entidade.Id);

                if (existente == null)
                {
                    retorno.Success = false;
                    retorno.Messages.Add("Mensagem não encontrada.");
                }
                else
                {
                    var registro = _mapper.Map<Mensagem>(existente);
                    registro.Alterado = DateTime.Now;
                    registro.Nome = entidade.Nome;
                    registro.MessageMode = (int)entidade.MessageMode;
                    registro.Conteudo = entidade.Conteudo;

                    _unitOfWork.Mensagens.Update(registro);
                    await _unitOfWork.CommitAsync();

                    retorno.Success = true;
                    retorno.Messages.Add("Mensagem atualizado com sucesso.");
                    retorno.Data = _mapper.Map<MensagemResponse>(registro);
                }
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Erro ao atualizar mensagem");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }
    }
}
