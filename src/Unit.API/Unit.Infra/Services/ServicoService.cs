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
    public class ServicoService : IServicoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
     
        public ServicoService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Reply> Add(CreateServicoRequest entidade)
        {
            Reply retorno = new Reply();

            try
            {
                var novo = _mapper.Map<Servico>(entidade);          
                novo.Criado = DateTime.Now;

                var resultado = _unitOfWork.Servicos.AddAsync(novo);

                if (resultado.IsCompletedSuccessfully)
                {
                    await _unitOfWork.CommitAsync();
                    retorno.Success = true;
                    retorno.Messages.Add("Serviço criado com sucesso.");
                    retorno.Data = _mapper.Map<PerfilResponse>(novo);
                }
                else
                {
                    retorno.Success = false;
                    retorno.Messages.Add("Erro ao criar um serviço.");
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

        public async Task<ConfigServico> ConfigServico(string nome)
        {
            ConfigServico config = new ConfigServico();

            var servico = await _unitOfWork.Servicos.AsQueryable()
                                .Where(x => x.Nome.ToLower() == nome.ToLower())
                .FirstOrDefaultAsync();

            if(servico != null) {
                config = JsonConvert.DeserializeObject<ConfigServico>(servico.Valor);
            }

            return config;
        }

        public async Task<Reply> GetAll(QueryServicoRequest condicao)
        {
            Reply retorno = new Reply();

            try
            {
                var query = _unitOfWork.Servicos.AsQueryable();

                if (condicao.Id.HasValue && condicao.Id.Value > 0)
                {
                    query = query.Where(x => x.ID == condicao.Id.Value);
                }
                if (!string.IsNullOrEmpty(condicao.Nome))
                {
                    query = query.Where(x => x.Nome.ToLower().Contains(condicao.Nome.ToLower()));
                }
                
                var resultado = query.ToList();

                retorno.Success = true;

                if (resultado == null || resultado.Count == 0)
                {
                    retorno.Messages.Add("Nenhum item encontrado.");
                    retorno.Data = new List<ServicoResponse>();
                }

                retorno.Messages.Add("Item(s) encontrado(s) com sucesso.");
                retorno.Data = _mapper.Map<List<ServicoResponse>>(resultado);
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Não foi possível pesquisar o item.");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }

        public async Task<Reply> GetOne(int id)
        {
            Reply retorno = new Reply();

            try
            {
                var one = await _unitOfWork.Servicos.AsQueryable()
                            .Where(x => x.ID == id)
                            .FirstOrDefaultAsync();

                retorno.Success = true;
                retorno.Status = System.Net.HttpStatusCode.OK;
                retorno.Messages.Add("Serviço encontrado com sucesso.");
                retorno.Data = one != null ? _mapper.Map<ServicoResponse>(one) : new ServicoResponse();
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Erro ao buscar serviço: ");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }

        public async Task<Reply> Update(UpdateServicoRequest entidade)
        {
            Reply retorno = new Reply();

            try
            {
                var existente = await _unitOfWork.Servicos.GetByIdAsync(entidade.Id);

                if (existente == null)
                {
                    retorno.Success = false;
                    retorno.Messages.Add("Serviço não encontrado.");
                }
                else
                {
                    var registro = _mapper.Map<Servico>(existente);
                    registro.Alterado = DateTime.Now;
                    registro.Nome = entidade.Nome;
                    registro.Valor = entidade.Valor;

                    _unitOfWork.Servicos.Update(registro);
                    await _unitOfWork.CommitAsync();

                    retorno.Success = true;
                    retorno.Messages.Add("Serviço atualizado com sucesso.");
                    retorno.Data = _mapper.Map<ServicoResponse>(registro);
                }
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Erro ao atualizar Serviço");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }
    }
}
