using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Unit.Application.Base;
using Unit.Application.DTOs.Request;
using Unit.Application.Sevices;
using Unit.Application.Util;
using Unit.Domain.Entities.Cadastro;

namespace Unit.Infra.Services
{
    public class GrupoService : IGrupoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GrupoService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Reply> Add(CreateGrupoRequest entidade)
        {
            Reply retorno = new Reply();

            try
            {
                Grupo novo = new Grupo()
                {
                    Nome = entidade.Nome,
                    Criado = DateTime.Now,
                };

                var resultado = _unitOfWork.Grupos.AddAsync(novo);

                if (resultado.IsCompletedSuccessfully)
                {
                    await _unitOfWork.CommitAsync();
                    retorno.Success = true;
                    retorno.Messages.Add("Grupo criado com sucesso.");
                    retorno.Data = novo;
                }
                else
                {
                    retorno.Success = false;
                    retorno.Messages.Add("Erro ao criar um grupo.");
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

        public async Task<Reply> GetAll(QueryGrupoRequest condicao)
        {
            Reply retorno = new Reply();

            try
            {
                var query = _unitOfWork.Grupos.AsQueryable();

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
                    retorno.Messages.Add("Nenhum Grupo encontrado.");
                    retorno.Data = new Grupo();
                }

                retorno.Messages.Add("Papel(s) encontrado(s) com sucesso.");
                retorno.Data = resultado;
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Não foi possível pesquisar o grupo.");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }

        public async Task<Reply> GetGrupoPub(int id)
        {
            Reply retorno = new Reply();

            try
            {
                var one = await _unitOfWork.GrupoPubs
                                .AsQueryable()
                                .Include(x => x.Grupo)
                                .Include(x => x.Pub)
                                .Where(x => x.ID == id)
                                .FirstOrDefaultAsync();

                retorno.Success = true;
                retorno.Status = System.Net.HttpStatusCode.OK;
                retorno.Messages.Add("Publicador encontrado com sucesso.");
                retorno.Data = one;
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Erro ao buscar grupo: ");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }

        public async Task<Reply> GetOne(int id)
        {
            Reply retorno = new Reply();

            try
            {
                var one = await _unitOfWork.Grupos
                    .AsQueryable()
                    .Include(x => x.Membros).ThenInclude(x => x.Pub)
                    .Where(x => x.ID == id)
                    .FirstOrDefaultAsync();

                if (one != null && one.Membros != null)
                {
                    //one.Membros = one.Membros
                    //    .OrderBy(m =>
                    //        m.Papel == "SG" ? 0 :
                    //        m.Papel == "DG" ? 1 :
                    //        m.Papel == "AJ" ? 2 :3)
                    one.Membros = one.Membros
                        .OrderBy(m =>
                            m.Papel == "SG" ? 0 :
                            m.Papel == "DG" ? 1 :
                            m.Papel == "AJ" ? 2 : 3)
                        .ThenBy(m => m.Pub.Nome)
                        .ToList();
                }
                retorno.Success = true;
                retorno.Status = System.Net.HttpStatusCode.OK;
                retorno.Messages.Add("Grupo encontrado com sucesso.");
                retorno.Data = one;
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Erro ao buscar grupo: ");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }

        public async Task<Reply> Update(UpdateGrupoRequest entidade)
        {
            Reply retorno = new Reply();

            try
            {
                var existente = await _unitOfWork.Grupos.GetByIdAsync(entidade.Id);

                if (existente == null)
                {
                    retorno.Success = false;
                    retorno.Messages.Add("Grupo não encontrado.");
                }
                else
                {
                    existente.Nome = entidade.Nome;
                    existente.Endereco = entidade.Endereco;
                    existente.Alterado = System.DateTime.Now;

                    _unitOfWork.Grupos.Update(existente);
                    await _unitOfWork.CommitAsync();

                    retorno.Success = true;
                    retorno.Messages.Add("Grupo atualizado com sucesso.");
                    retorno.Data = existente;
                }
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Erro ao atualizar grupo");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }

        public async Task<Reply> UpdateGrupoPub(UpdateGrupoPubRequest entidade)
        {
            Reply retorno = new Reply();

            try
            {
                var existente = await _unitOfWork.GrupoPubs.GetByIdAsync(entidade.Id);

                if (existente == null)
                {
                    retorno.Success = false;
                    retorno.Messages.Add("Publicador não encontrado.");
                }
                else
                {
                    existente.GrupoID = entidade.GrupoId;
                    existente.Papel = entidade.Papel;
                    existente.Alterado = System.DateTime.Now;

                    _unitOfWork.GrupoPubs.Update(existente);
                    await _unitOfWork.CommitAsync();

                    retorno.Success = true;
                    retorno.Messages.Add("Publicador atualizado com sucesso.");
                    retorno.Data = existente;
                }
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Erro ao atualizar grupo");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }
    }
}
