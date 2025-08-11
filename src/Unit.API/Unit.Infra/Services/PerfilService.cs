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
using Unit.Application.Sevices;
using Unit.Application.Util;
using Unit.Domain.Entities.Acesso;

namespace Unit.Infra.Services
{
    public class PerfilService : IPerfilService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public PerfilService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
        }
        public async Task<Reply> Add(CreatePerfilRequest entidade)
        {
            Reply retorno = new Reply();

            try
            {
                var novo = _mapper.Map<Perfil>(entidade);
                novo.Criado = DateTime.Now;

                var resultado = _unitOfWork.Perfis.AddAsync(novo);

                if (resultado.IsCompletedSuccessfully)
                {
                    await _unitOfWork.CommitAsync();
                    retorno.Success = true;
                    retorno.Messages.Add("Perfil criado com sucesso.");
                    retorno.Data = _mapper.Map<PerfilResponse>(novo);
                }
                else
                {
                    retorno.Success = false;
                    retorno.Messages.Add("Erro ao criar um perfil.");
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

        public async Task<Reply> GetAll(QueryPerfilRequest condicao)
        {
            Reply retorno = new Reply();

            try
            {
                var query = _unitOfWork.Perfis.AsQueryable();

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
                    retorno.Messages.Add("Nenhum perfil encontrado.");
                    retorno.Data = new List<PerfilResponse>();
                }

                retorno.Messages.Add("Perfil(s) encontrado(s) com sucesso.");
                retorno.Data = _mapper.Map<List<PerfilResponse>>(resultado);
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Erro ao criar perfil: ");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }

        public async Task<Reply> GetOne(int id)
        {
            Reply retorno = new Reply();

            try
            {
                var one = await _unitOfWork.Perfis.AsQueryable()
                            .Include(x => x.Usuarios)
                            .Where(x => x.ID == id)
                            .FirstOrDefaultAsync();

                retorno.Success = true;
                retorno.Status = System.Net.HttpStatusCode.OK;
                retorno.Messages.Add("Perfil encontrado com sucesso.");
                retorno.Data = one != null ? _mapper.Map<PerfilResponse>(one) : new PerfilResponse();
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Erro ao buscar perfil: ");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }

        public async Task<Reply> GetUsersByPerfil(int id)
        {
            Reply retorno = new Reply();

            try
            {
                var dados = await _unitOfWork.UsuarioPerfis.AsQueryable()
                            .Include(x => x.Usuario)
                            .Where(x => x.PerfilId == id)
                            .Select(x => new UsuarioPerfilResponse
                            {
                                ID = x.ID,
                                PerfilNome = x.Perfil.Nome,
                                UsuarioNome = x.Usuario.Nome
                            }).ToListAsync();

                retorno.Success = true;
                retorno.Status = System.Net.HttpStatusCode.OK;
                retorno.Messages.Add("Usuário(s) encontrado(s) com sucesso.");
                retorno.Data = dados != null ? dados : new List<UsuarioPerfilResponse>();
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Erro ao buscar usuários do perfil: ");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }

        public async Task<Reply> GetPerfisByUsuario(int id)
        {
            Reply retorno = new Reply();

            try
            {
                var dados = await _unitOfWork.UsuarioPerfis.AsQueryable()
                            .Include(x => x.Perfil)
                            .Where(x => x.UsuarioId == id)
                            .Select(x => new UsuarioPerfilResponse
                            {
                                ID = x.ID,
                                PerfilNome = x.Perfil.Nome,
                                UsuarioNome = x.Usuario.Nome
                            }).ToListAsync();

                retorno.Success = true;
                retorno.Status = System.Net.HttpStatusCode.OK;
                retorno.Messages.Add("Perfil(s) encontrado(s) com sucesso.");
                retorno.Data = dados != null ? dados : new List<UsuarioPerfilResponse>();
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Erro ao buscar usuários do perfil: ");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }

        public async Task<Reply> Update(UpdatePerfilRequest entidade)
        {
            Reply retorno = new Reply();

            try
            {
                var existente = await _unitOfWork.Perfis.GetByIdAsync(entidade.Id);

                if (existente == null)
                {
                    retorno.Success = false;
                    retorno.Messages.Add("Perfil não encontrado.");
                }
                else
                {
                    var registro = _mapper.Map<Perfil>(existente);
                    registro.Alterado = DateTime.Now;
                    registro.Nome = entidade.Nome;

                    _unitOfWork.Perfis.Update(registro);
                    await _unitOfWork.CommitAsync();

                    retorno.Success = true;
                    retorno.Messages.Add("Perfil atualizado com sucesso.");
                    retorno.Data = _mapper.Map<PerfilResponse>(registro);
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
