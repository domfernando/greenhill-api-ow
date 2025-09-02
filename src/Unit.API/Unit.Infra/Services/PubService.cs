using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Unit.Application.Base;
using Unit.Application.DTOs.Request;
using Unit.Application.DTOs.Response;
using Unit.Application.Enums;
using Unit.Application.Services;
using Unit.Application.Util;
using Unit.Domain.Entities.Acesso;
using Unit.Domain.Entities.Cadastro;

namespace Unit.Infra.Services
{
    public class PubService : IPubService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PubService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Reply> AddAsync(PubNewModel novo)
        {
            Reply retorno = new Reply();

            try
            {
                var _novo = new Pub()
                {
                    Nome = novo.Nome,
                    NomeCompleto = novo.Nome,
                    Situacao = Situacao.Não_Batizado.ToString().Replace("_", " "),
                    Privilegio = Privilegio.Nenhum.ToString(),
                    Criado = System.DateTime.Now
                };

                var resultado = _unitOfWork.Pubs.AddAsync(_novo);

                if (resultado.IsCompletedSuccessfully)
                {
                    await _unitOfWork.CommitAsync();

                    var addGrupo = await this.AddGrupo(_novo.ID, 1, "Membro");

                    retorno.Success = true;
                    retorno.Messages.Add("Adicionado com sucesso. Adicionado ao grupo padrão.");
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

        public async Task<Reply> AddGrupo(int pubId, int grupoId, string papel)
        {
            Reply retorno = new Reply();

            try
            {
                var grupoP = await _unitOfWork.GrupoPubs.AsQueryable()
                                  .Where(x => x.PubID == pubId && x.GrupoID == grupoId)
                                  .FirstOrDefaultAsync();

                if (grupoP == null)
                {
                    var novo = _unitOfWork.GrupoPubs.AddAsync(new GrupoPub
                    {
                        PubID = pubId,
                        GrupoID = grupoId,
                        Papel = papel,
                        Criado = DateTime.Now,
                        Alterado = DateTime.Now
                    });

                    if (novo.IsCompletedSuccessfully)
                    {
                        await _unitOfWork.CommitAsync();
                        retorno.Success = true;
                        retorno.Messages.Add("Publicador adicionado ao grupo com sucesso.");
                    }
                    else
                    {
                        retorno.Success = false;
                        retorno.Messages.Add("Erro ao adicionar publicador ao grupo.");
                        retorno.Errors.Add(novo.Exception.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Erro ao adicionar publicador ao grupo.");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }

        public async Task<Reply> AddPapel(int pubId, int papelId)
        {
            Reply retorno = new Reply();

            try
            {
                var usuarioPerfil = await _unitOfWork.PubPapeis.AsQueryable()
                                  .Where(x => x.PubId == pubId && x.PapelId == papelId)
                                  .FirstOrDefaultAsync();

                if (usuarioPerfil == null)
                {
                    var novo = _unitOfWork.PubPapeis.AddAsync(new PubPapel
                    {
                        PubId = pubId,
                        PapelId = papelId,
                        Criado = DateTime.Now
                    });

                    if (novo.IsCompletedSuccessfully)
                    {
                        await _unitOfWork.CommitAsync();
                        retorno.Success = true;
                        retorno.Messages.Add("Papel adicionado ao publicador com sucesso.");
                    }
                    else
                    {
                        retorno.Success = false;
                        retorno.Messages.Add("Erro ao adicionar papel ao publicador.");
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

        public async Task<Reply> GetAll(PubQueryModel condicao)
        {
            Reply retorno = new Reply();

            try
            {
                var query = _unitOfWork.Pubs
                                        .AsQueryable()
                                        .Include(x => x.Papeis).ThenInclude(p => p.Papel)
                                        .Include(x => x.Grupos).ThenInclude(g => g.Grupo)
                                        .AsQueryable();

                if (condicao.Id.HasValue && condicao.Id.Value > 0)
                {
                    query = query.Where(x => x.ID == condicao.Id.Value);
                }
                if (!string.IsNullOrEmpty(condicao.Nome))
                {
                    query = query.Where(x => x.Nome.ToLower().Contains(condicao.Nome.ToLower()));
                }
                if (condicao.Genero.HasValue)
                {
                    query = query.Where(x => x.Genero.ToLower() != null && x.Genero.ToLower().Contains(condicao.Genero.Value.ToString().ToLower()));
                }
                if (condicao.Privilegio.HasValue)
                {
                    query = query.Where(x => x.Privilegio != null && x.Privilegio.ToLower().Equals(condicao.Privilegio.Value.ToString().ToLower().Replace("_", " ")));
                }
                if (condicao.Situacao.HasValue)
                {
                    query = query.Where(x => x.Situacao != null && x.Situacao.ToLower().Contains(condicao.Situacao.Value.ToString().ToLower().Replace("_", " ")));
                }
                else
                {
                    string[] ignorar = { "inativo", "removido", "falecido", "mudou" };
                    query = query.Where(x => x.Situacao != null && !ignorar.Contains(x.Situacao.ToLower()));
                }
                if (condicao.Orador.HasValue)
                {
                    query = query.Where(x => x.Orador == condicao.Orador.Value);
                }
                if (!string.IsNullOrEmpty(condicao.Escola))
                {
                    query = query.Where(x => x.Escola == Convert.ToBoolean(condicao.Escola));
                }

                var resultado = await query.ToListAsync();

                if (!string.IsNullOrEmpty(condicao.Varao))
                {
                    resultado = resultado.Where(x => x.Varao == Convert.ToBoolean(condicao.Varao)).ToList();
                }
                if (!string.IsNullOrEmpty(condicao.Dianteira))
                {
                    resultado = resultado.Where(x => x.Dianteira == Convert.ToBoolean(condicao.Dianteira)).ToList();
                }
                if (condicao.Faixa.HasValue)
                {
                    resultado = resultado.Where(x => x.Faixa != null && x.Faixa.ToLower().Equals(condicao.Faixa.Value.ToString().ToLower())).ToList();
                }

                retorno.Success = true;

                if (resultado == null || resultado.Count == 0)
                {
                    retorno.Messages.Add("Nenhum registro encontrado.");
                    retorno.Data = new List<PubResponse>();
                }
                else
                {
                    resultado = resultado.OrderBy(x => x.Nome).ToList();
                    retorno.Messages.Add("Registro(s) encontrado(s) com sucesso.");
                    retorno.Data = _mapper.Map<List<PubResponse>>(resultado);
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

        public async Task<Reply> GetByUsuarioId(int id)
        {
            Reply retorno = new Reply();
            try
            {
                var dados = _unitOfWork.Pubs
                                       .AsQueryable()
                                       .Include(x => x.Papeis).ThenInclude(p => p.Papel)
                                       .Include(x => x.Usuario)
                                       .Where(x => x.UsuarioId == id)
                                       .FirstOrDefault();

                retorno.Success = true;
                retorno.Messages.Add("Registro encontrado com sucesso.");
                retorno.Data = _mapper.Map<PubResponse>(dados);
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Não foi possível consultr os registros");
                retorno.Errors.Add($"{ex.Message}");
            }
            return retorno;
        }

        public async Task<Reply> GetOne(int id)
        {
            Reply retorno = new Reply();
            try
            {
                var dados = _unitOfWork.Pubs
                                       .AsQueryable()
                                       .Include(x => x.Papeis).ThenInclude(p => p.Papel)
                                       .Include(x => x.Usuario)
                                       .Include(x => x.Grupos).ThenInclude(x => x.Grupo)
                                       .Where(x => x.ID == id)
                                       .FirstOrDefault();

                retorno.Success = true;
                retorno.Messages.Add("Registro encontrado com sucesso.");
                retorno.Data = _mapper.Map<PubResponse>(dados);
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Não foi possível consultr os registros");
                retorno.Errors.Add($"{ex.Message}");
            }
            return retorno;
        }

        public async Task<Reply> GetOrador(int id)
        {
            Reply retorno = new Reply();
            OradorResponse orador = new OradorResponse();

            try
            {
                var dados = await _unitOfWork.Pubs
                                       .AsQueryable()
                                       .Where(x => x.ID == id)
                                       .FirstOrDefaultAsync();
                if (dados != null)
                {
                    orador.Id = dados.ID;
                    orador.Nome = dados.Nome;
                    orador.Privilegio = dados.Privilegio;
                    orador.Celular = dados.Celular;
                    orador.Temas = await _unitOfWork.OradorTemas
                                    .AsQueryable()
                                    .Include(x => x.Tema)
                                    .Where(x => x.PubID == id)
                                    .Select(t => new OradorTemasResponse
                                    {
                                        Id = t.ID,
                                        Tema = new TemaResponse
                                        {
                                            Id = t.Tema.ID,
                                            Nome = t.Tema.Nome,
                                            Codigo = t.Tema.Codigo,
                                            NomeFormatado = t.Tema.Codigo + " - " + t.Tema.Nome
                                        }
                                    }).OrderBy(x => Convert.ToInt32(x.Tema.Codigo)).ToListAsync() ?? new List<OradorTemasResponse>();

                    retorno.Success = true;
                    retorno.Messages.Add("Registro encontrado com sucesso.");
                    retorno.Data = orador;
                }
                else
                {
                    retorno.Success = false;
                    retorno.Messages.Add("Orador não encontrado.");
                    retorno.Data = orador;
                }
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Não foi possível consultar os registros");
                retorno.Errors.Add($"{ex.Message}");
            }

            return retorno;
        }

        public async Task<Reply> GetOradores(OradorQueryRequest condicao)
        {
            Reply retorno = new Reply();
            OradorResponse orador = new OradorResponse();

            try
            {
                var query = _unitOfWork.Pubs
                                       .AsQueryable()
                                       .Include(x => x.Temas).ThenInclude(t => t.Tema)
                                       .Where(x => x.Orador == true);

                if (!string.IsNullOrEmpty(condicao.Orador))
                {
                    query = query.Where(x => x.Nome.ToLower().Contains(condicao.Orador.ToLower()));
                }
                if (condicao.Privilegio.HasValue)
                {
                    query = query.Where(x => x.Privilegio != null && x.Privilegio.ToLower().Equals(condicao.Privilegio.Value.ToString().ToLower().Replace("_", " ")));
                }
                if (!string.IsNullOrEmpty(condicao.Tema))
                {
                    query = query.Where(x => x.Temas.Any(t => t.Tema.Codigo.ToLower().Contains(condicao.Tema.ToLower())));
                }

                var dados = await query.ToListAsync();

                if (dados != null)
                {
                    if (!string.IsNullOrEmpty(condicao.Selecionados))
                    {
                        int[] ids = condicao.Selecionados.Split(',').Select(int.Parse).ToArray();
                        dados = dados.Where(x => ids.Contains(x.ID) && x.Temas.Count() > 0).ToList();
                    }
                    retorno.Success = true;
                    retorno.Messages.Add("Registro encontrado com sucesso.");
                    retorno.Data = dados;
                }
                else
                {
                    retorno.Success = false;
                    retorno.Messages.Add("Orador não encontrado.");
                    retorno.Data = orador;
                }
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Não foi possível consultar os registros");
                retorno.Errors.Add($"{ex.Message}");
            }

            return retorno;
        }

        public Task<Reply> RemGrupo(int id)
        {
            Reply retorno = new Reply();
            try
            {
                var remover = _unitOfWork.GrupoPubs.GetByIdAsync(id).Result;
                if (remover != null)
                {
                    _unitOfWork.GrupoPubs.Delete(remover);
                    _unitOfWork.CommitAsync().Wait();
                    retorno.Success = true;
                    retorno.Messages.Add("Publicador removido do grupo com sucesso.");
                }
                else
                {
                    retorno.Success = false;
                    retorno.Messages.Add("Publicador não encontrado ou já removido.");
                }
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Erro ao remover papel do publicador.");
                retorno.Errors.Add(ex.Message);
            }
            return Task.FromResult(retorno);
        }

        #region Papel
        public Task<Reply> RemPapel(int id)
        {
            Reply retorno = new Reply();
            try
            {
                var remover = _unitOfWork.PubPapeis.GetByIdAsync(id).Result;
                if (remover != null)
                {
                    _unitOfWork.PubPapeis.Delete(remover);
                    _unitOfWork.CommitAsync().Wait();
                    retorno.Success = true;
                    retorno.Messages.Add("Papel removido do publicador com sucesso.");
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
                retorno.Messages.Add("Erro ao remover papel do publicador.");
                retorno.Errors.Add(ex.Message);
            }
            return Task.FromResult(retorno);
        }

        public async Task<Reply> UpdateAsync(PubUpdateModel dados)
        {
            Reply retorno = new Reply();

            try
            {
                var registro = await _unitOfWork.Pubs.GetByIdAsync(dados.ID);

                if (registro != null)
                {
                    registro.Nome = dados.Nome;
                    registro.NomeCompleto = dados.NomeCompleto;
                    registro.Nascimento = !string.IsNullOrEmpty(dados.Nascimento) ? DateTime.Parse(dados.Nascimento) : null;
                    registro.Batismo = !string.IsNullOrEmpty(dados.Batismo) ? DateTime.Parse(dados.Batismo) : null;
                    registro.Situacao = dados.Situacao.Replace("_", " ");
                    registro.Privilegio = dados.Privilegio.Replace("_", " ");
                    registro.Orador = dados.Orador;
                    registro.Genero = dados.Genero.ToLower();
                    registro.Endereco = dados.Endereco;
                    registro.Complemento = dados.Complemento;
                    registro.Telefone = dados.Telefone;
                    registro.Celular = dados.Celular;
                    registro.Obs = dados.Obs;
                    registro.EmergenciaNome = dados.EmergenciaNome;
                    registro.EmergenciaContato = dados.EmergenciaContato;
                    registro.Escola = dados.Escola ?? false;
                    registro.Notificar = dados.Notificar ?? false;
                    registro.UsuarioId = dados.UsuarioId > 0 ? dados.UsuarioId : null;
                    registro.Alterado = System.DateTime.Now;
                    _unitOfWork.Pubs.Update(registro);
                    await _unitOfWork.CommitAsync();

                    if (!string.IsNullOrEmpty(dados.PapeisSelecionados))
                    {
                        foreach (string item in dados.PapeisSelecionados.Split(","))
                        {
                            var addPapel = await this.AddPapel(dados.ID, Convert.ToInt32(item));
                        }

                        var papeisPub = await _unitOfWork.PubPapeis.AsQueryable()
                                                        .AsNoTracking()
                                                        .Where(x => x.PubId == dados.ID)
                                                        .ToListAsync();
                        foreach (var item in papeisPub)
                        {
                            if (!dados.PapeisSelecionados.Split(",").Select(int.Parse).ToArray().Contains(item.PapelId))
                            {
                                var remPapel = await this.RemPapel(item.ID);
                            }
                        }
                    }

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

        #endregion

        #region Grupo

        #endregion
    }
}
