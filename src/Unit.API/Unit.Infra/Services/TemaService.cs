using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Unit.Application.Base;
using Unit.Application.DTOs.Request;
using Unit.Application.DTOs.Response;
using Unit.Application.Services;
using Unit.Application.Util;
using Unit.Domain.Entities.Cadastro;

namespace Unit.Infra.Services
{
    public class TemaService : ITemaService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TemaService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Reply> AddAsync(TemaNewRequest novo)
        {
            Reply retorno = new Reply();

            try
            {
                var _novo = new Tema()
                {
                    Codigo = novo.Numero,
                    Nome = novo.Nome,
                    Criado = System.DateTime.Now
                };

                var resultado = _unitOfWork.Temas.AddAsync(_novo);

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

        public async Task<Reply> GetAll(TemaQueryRequest condicao)
        {
            Reply retorno = new Reply();

            try
            {
                #region Dados
                var comProferimento = (from tema in _unitOfWork.Temas.AsQueryable()

                                       select new Tema
                                       {
                                           ID = tema.ID,
                                           Codigo = tema.Codigo,
                                           Nome = tema.Nome,
                                           Criado = tema.Criado,
                                           Alterado = tema.Criado,
                                           Discursos = (from discurso in _unitOfWork.Discursos
                                                                        .AsQueryable()
                                                                        .Include(x => x.Arranjo).ThenInclude(x => x.Cong)
                                                        where discurso.TemaID == tema.ID
                                                        && discurso.Recebido == true
                                                        select discurso).ToList(),
                                           Oradores = (from orador in _unitOfWork.OradorTemas
                                                                                .AsQueryable()
                                                                                .Include(x => x.Orador)
                                                       where orador.TemaID == tema.ID
                                                       select orador).ToList(),
                                       }).ToList();

                var semProferimento = (from tema in _unitOfWork.Temas.AsQueryable()
                                       where !(from discurso in _unitOfWork.Discursos.AsQueryable()
                                               where discurso.TemaID == tema.ID
                                               && discurso.Recebido == true
                                               select discurso.TemaID).Any()
                                       select new Tema
                                       {
                                           ID = tema.ID,
                                           Codigo = tema.Codigo,
                                           Nome = tema.Nome,
                                           Criado = tema.Criado,
                                           Alterado = tema.Criado,
                                           Discursos = new List<Discurso>(),
                                           Oradores =new List<OradorTema?>()
                                       }).ToList();

                var dados = comProferimento.Union(semProferimento).ToList();
                #endregion

                if (condicao.Id.HasValue && condicao.Id.Value > 0)
                {
                    dados = dados.Where(x => x.ID == condicao.Id.Value).ToList();
                }

                if (!string.IsNullOrEmpty(condicao.Nome))
                {
                    dados = dados.Where(x => x.Nome.ToLower().Contains(condicao.Nome.ToLower())).ToList();
                }
                if (!string.IsNullOrEmpty(condicao.Numero))
                {
                    dados = dados.Where(x => x.Codigo.ToLower().Contains(condicao.Numero.ToLower())).ToList();
                }

                retorno.Success = true;

                if (dados == null || dados.Count == 0)
                {
                    retorno.Messages.Add("Nenhum registro encontrado.");
                    retorno.Data = new List<TemaResponse>();
                }
                else
                {
                    // Substitua esta linha:
                    // dados = dados.OrderBy(x => x.Codigo).ToList();

                    // Por esta implementação para ordenar corretamente números e alfanuméricos:
                    dados = dados.OrderBy(x =>
                    {
                        // Tenta converter para número
                        if (int.TryParse(x.Codigo, out int num))
                            return (num, string.Empty);
                        // Se não for número, ordena pelo texto
                        return (int.MaxValue, x.Codigo);
                    }).ToList();
                    retorno.Messages.Add("Registro(s) encontrado(s) com sucesso.");
                    retorno.Data = dados;
                }
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Não foi possível pesquisar o endereço.");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }

        public async Task<Reply> GetById(int id)
        {
            Reply retorno = new Reply();

            try
            {
                var dados = await (from tema in _unitOfWork.Temas.AsQueryable()
                                   where tema.ID == id
                                   select new Tema
                                   {
                                       ID = tema.ID,
                                       Codigo = tema.Codigo,
                                       Nome = tema.Nome,
                                       Criado = tema.Criado,
                                       Alterado = tema.Criado,
                                       Discursos = (from discurso in _unitOfWork.Discursos
                                                                      .AsQueryable()
                                                                      .Include(x => x.Arranjo).ThenInclude(x => x.Cong)
                                                    where discurso.TemaID == tema.ID
                                                    && discurso.Recebido == true
                                                    select discurso).ToList(),
                                       Oradores = (from orador in _unitOfWork.OradorTemas
                                                                  .AsQueryable()
                                                                  .Include(x => x.Orador)
                                                   where orador.TemaID == tema.ID
                                                   select orador).ToList()
                                   }).FirstOrDefaultAsync();

                var _dados = dados;
                retorno.Success = true;
                retorno.Messages.Add("Registro(s) encontrado(s) com sucesso.");
                retorno.Data = _dados;
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Não possível adicionar o registro");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }

        public async Task<Reply> UpdateAsync(TemaUpdateRequest dados)
        {
            Reply retorno = new Reply();

            try
            {
                var registro = await _unitOfWork.Temas.GetByIdAsync(dados.Id);

                if (registro != null)
                {
                    registro.Nome = dados.Nome;
                    registro.Codigo = dados.Numero;
                    registro.Nome = dados.Nome;
                    registro.Alterado = DateTime.Now;

                    _unitOfWork.Temas.Update(registro);
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
