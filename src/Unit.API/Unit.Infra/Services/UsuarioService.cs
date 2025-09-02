using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Unit.Application.Base;
using Unit.Application.DTOs.Request;
using Unit.Application.DTOs.Response;
using Unit.Application.Services;
using Unit.Application.Sevices;
using Unit.Application.Util;
using Unit.Domain.Entities.Acesso;
using Unit.Domain.Entities.Config;
using Unit.Infra.Tools;

namespace Unit.Infra.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IMensagemService _mensagemService;
        private readonly IQueueService _queueService;
        private readonly IPubService _pubService;

        public UsuarioService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration, IMensagemService mensagemService, IQueueService queueService, IPubService pubService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
            _mensagemService = mensagemService;
            _queueService = queueService;
            _pubService = pubService;
        }
        public async Task<Reply> Add(CreateUsuarioRequest entidade)
        {
            Reply retorno = new Reply();

            try
            {
                var novo = _mapper.Map<Usuario>(entidade);
                novo.Usr = entidade.Email.ToLower();
                novo.Criado = DateTime.Now;
                var resultado = _unitOfWork.Usuarios.AddAsync(novo);

                if (resultado.IsCompletedSuccessfully)
                {
                    await _unitOfWork.CommitAsync();
                    retorno.Success = true;
                    retorno.Messages.Add("Usuário criado com sucesso.");
                    retorno.Data = _mapper.Map<UsuarioResponse>(novo);
                }
                else
                {
                    retorno.Success = false;
                    retorno.Messages.Add("Erro ao adicionar usuário.");
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
        public Task<bool> EmailExist(string email)
        {
            var dados = _unitOfWork.Usuarios.AsQueryable()
                            .Where(x => x.Email.ToLower() == email.ToLower())
                            .SingleOrDefault();
            return Task.FromResult(dados != null ? true : false);
        }
        public async Task<Reply> GetAll(QueryUsuarioRequest condicao)
        {
            Reply retorno = new Reply();

            try
            {
                var query = _unitOfWork.Usuarios.AsQueryable();

                if (condicao.Id > 0)
                {
                    query = query.Where(x => x.ID == condicao.Id);
                }
                if (!string.IsNullOrEmpty(condicao.Nome))
                {
                    query = query.Where(x => x.Nome.ToLower().Contains(condicao.Nome.ToLower()));
                }
                if (!string.IsNullOrEmpty(condicao.Email))
                {
                    query = query.Where(x => x.Email.ToLower().Contains(condicao.Email.ToLower()));
                }

                var resultado = query.ToList();

                retorno.Success = true;

                if (resultado == null || resultado.Count == 0)
                {
                    retorno.Messages.Add("Nenhum usuário encontrado.");
                    retorno.Data = new List<UsuarioResponse>();
                }

                retorno.Messages.Add("Usuário(s) encontrado(s) com sucesso.");
                retorno.Data = _mapper.Map<List<UsuarioResponse>>(resultado);
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Erro ao criar usuário: ");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }
        public async Task<Reply> GetOne(int id)
        {
            Reply retorno = new Reply();

            try
            {
                var one = await _unitOfWork.Usuarios.AsQueryable()
                            .Include(x => x.Perfis).ThenInclude(x => x.Perfil)
                            .Where(x => x.ID == id)
                            .FirstOrDefaultAsync();

                retorno.Success = true;
                retorno.Status = System.Net.HttpStatusCode.OK;
                retorno.Messages.Add("Usuário encontrado com sucesso.");
                retorno.Data = one != null ? _mapper.Map<UsuarioCRUDResponse>(one) : new UsuarioCRUDResponse();
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Erro ao buscar usuário: ");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }
        public async Task<Reply> Update(UpdateUsuarioRequest entidade)
        {
            Reply retorno = new Reply();

            try
            {
                var alterado = _mapper.Map<Usuario>(entidade);

                var existente = await _unitOfWork.Usuarios.GetByIdAsync(entidade.Id);

                if (existente == null)
                {
                    retorno.Success = false;
                    retorno.Messages.Add("Usuário não encontrado.");
                }
                else
                {
                    var registro = _mapper.Map<Usuario>(existente);
                    registro.Alterado = DateTime.Now;
                    registro.Nome = entidade.Nome;
                    registro.NomeCompleto = entidade.NomeCompleto;
                    registro.Email = entidade.Email;
                    registro.Celular = entidade.Celular;
                    if (!string.IsNullOrEmpty(entidade.Pwd))
                    {
                        registro.Pwd = Tools.Util.GenerateHashPassword(entidade.Pwd);
                    }
                    registro.Codigo = Tools.Util.GenerateCode(6);
                    registro.Verificado = entidade.Verificado;
                    registro.Ativo = entidade.Ativo;
                    registro.Travado = entidade.Travado;
                    registro.Mfa = entidade.Mfa ?? false;
                    registro.MfaModo = entidade.MfaModo;
                    registro.MfaCodigo = entidade.MfaCodigo;
                    registro.MfaExpira = entidade.MfaExpira;

                    _unitOfWork.Usuarios.Update(registro);
                    await _unitOfWork.CommitAsync();

                    retorno.Success = true;
                    retorno.Messages.Add("Usuário atualizado com sucesso.");
                    retorno.Data = _mapper.Map<UsuarioCRUDResponse>(registro);
                }
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Erro ao atualizar usuário");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }
        public async Task<Reply> Login(LoginRequest request)
        {
            Reply retorno = new Reply();

            try
            {
                var _login = Logar(request.Login, request.Pwd).Result;

                if (_login.Success)
                {
                    if (_login.Validate)
                    {
                        var _data = _mapper.Map<Usuario>(_login.Data);

                        var _userLogado = _mapper.Map<UsuarioLoginResponse>(_data);
                        _userLogado.Perfis = new List<UsuarioPerfilResponse>();

                        foreach (var p in _data.Perfis)
                        {
                            _userLogado.Perfis.Add(new UsuarioPerfilResponse
                            {
                                ID = p.Perfil.ID,
                                PerfilNome = p.Perfil.Nome
                            });
                        }

                        var _pub = await _pubService.GetByUsuarioId(_data.ID);

                        if(_pub.Success && _pub.Data != null)
                        {
                            var _dadosPub = (PubResponse)_pub.Data;

                            if (_dadosPub != null)
                            {
                                _userLogado.PubId = _dadosPub.ID;
                                _userLogado.Papeis = _dadosPub.Papeis ?? new List<PubPapelResponse>();    
                                _userLogado.Priv = _dadosPub.Privilegio;
                            }
                        }

                        if (_data.Mfa == true)
                        {
                            _login.Data = new { user = _userLogado, mfa = true };
                        }
                        else
                        {
                            _userLogado.UltimoLogin = string.Format("{0:dd/MM/yyyy HH:mm:ss}", _data.UltimoLogin);

                            var _token = Tools.Util.GenerateToken(_configuration, _userLogado);

                            _login.Data = new { user = _userLogado, token = _token, mfa = false };
                        }

                        retorno = _login;
                    }
                    else
                    {
                        _login.Messages.Add($"Usuário {request.Login} ainda não foi validado");
                        retorno = _login;
                    }
                }
                else
                {
                    retorno = _login;
                }
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Erro ao realizar login: ");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }
        private async Task<Reply> Logar(string email, string pass)
        {
            var _rtn = new Reply();
            var _config = Tools.Util.GetConfigAccess(_configuration);
            Usuario usuario = null;

            try
            {
                var _user = _unitOfWork.Usuarios.AsQueryable()
                            .Include(x => x.Perfis)
                            .Where(x => x.Usr.ToLower() == email.ToLower())
                            .FirstOrDefault();

                //Usuario existe
                if (_user == null)
                {
                    #region Usuário inexistente

                    _rtn.Success = false;
                    _rtn.Validate = false;
                    _rtn.Messages.Add($"Usuário {email} não encontrado.");

                    #endregion
                }
                else
                {
                    //Usuário travado?
                    if (!string.IsNullOrEmpty(_user.Travado))
                    {
                        #region Usuário bloqueado

                        _rtn.Success = false;
                        _rtn.Validate = false;
                        _rtn.Messages.Add($"Usuário bloqueado.");

                        #endregion
                    }
                    else
                    {
                        usuario = _mapper.Map<Usuario>(_user);

                        //Checando senha
                        if (!string.IsNullOrEmpty(_user.Pwd))
                        {

                            if (!_user.Pwd.Equals(Tools.Util.GenerateHashPassword(pass)))
                            {
                                #region Senha incorreta

                                _user.Tentativa = _user.Tentativa + 1;

                                _unitOfWork.Usuarios.Update(usuario);
                                _unitOfWork.CommitAsync().Wait();

                                _rtn.Success = false;
                                _rtn.Validate = false;

                                //Se habilitado, logar tentativa de senha
                                if (_config.BloquearTentativaAcesso)
                                {
                                    if (_user.Tentativa >= _config.TentativaAcesso)
                                    {
                                        usuario.Travado = System.DateTime.Now.ToString();

                                        _unitOfWork.Usuarios.Update(usuario);
                                        _unitOfWork.CommitAsync().Wait();

                                        _rtn.Messages.Add($"Acesso bloqueado por tentativas de acesso.");
                                    }
                                    else
                                    {
                                        _rtn.Messages.Add($"Senha incorreta. Tentativa {_user.Tentativa} de {_config.TentativaAcesso}.");
                                    }
                                }
                                else
                                {
                                    _rtn.Messages.Add("Senha incorreta");
                                }

                                #endregion
                            }
                            else
                            {
                                if (_config.ValidaEmail)
                                {
                                    if (_user.Verificado == null)
                                    {
                                        #region Usuário ainda não validado. Gerar chave para validação

                                        Guid _chave = Guid.NewGuid();

                                        usuario.Codigo = _chave.ToString();
                                        _unitOfWork.Usuarios.Update(usuario);
                                        _unitOfWork.CommitAsync().Wait();

                                        #endregion
                                    }
                                }

                                if (_user.Verificado != null)
                                {
                                    #region Login bem sucedido

                                    //Zera as tentativas com erro 
                                    usuario.Tentativa = 0;
                                    usuario.UltimoLogin = System.DateTime.Now.ToString();

                                    _unitOfWork.Usuarios.Update(usuario);
                                    _unitOfWork.CommitAsync().Wait();

                                    //Recupera os perfis do usuário
                                    _user.Perfis = new List<UsuarioPerfil?>();
                                    _user.Perfis = _unitOfWork.UsuarioPerfis.AsQueryable()
                                                   .Include(x => x.Perfil)
                                                   .Include(x => x.Usuario)
                                                   .Where(x => x.UsuarioId == _user.ID)
                                                   .Select(x => new UsuarioPerfil
                                                   {
                                                       UsuarioId = x.UsuarioId,
                                                       PerfilId = x.PerfilId,
                                                       Perfil = new Perfil
                                                       {
                                                           ID = x.Perfil.ID,
                                                           Nome = x.Perfil.Nome
                                                       }
                                                   }).ToList();

                                    _rtn.Data = _user;

                                    #endregion
                                }
                                else
                                {
                                    #region Retornar para validação

                                    _rtn.Validate = false;
                                    _rtn.Messages.Add("E-mail do usuário ainda não validado.");
                                    _rtn.Data = _user;

                                    #endregion
                                }
                            }
                        }
                        else
                        {
                            #region Cadastro incompleto

                            _rtn.Success = false;
                            _rtn.Validate = false;
                            _rtn.Messages.Add($"Cadastro incompleto para usuário {email}. É necessário definir uma senha.");

                            #endregion
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                _rtn.Success = false;
                _rtn.Validate = false;
                _rtn.Messages.Add("Não foi possível pesquisar o usuário");
                _rtn.Errors.Add(ex.Message);
            }

            return _rtn;
        }
        private async Task<Usuario> GetUser(string email)
        {

            Usuario usuario = new Usuario();
            usuario = _unitOfWork.Usuarios.AsQueryable()
                        .Where(x => x.Usr.ToLower() == email.ToLower())
                        .FirstOrDefault();

            return usuario;
        }
        public async Task<Reply> Validate(string codigo)
        {
            Reply retorno = new Reply();

            try
            {
                var _usuario = _unitOfWork.Usuarios.AsQueryable()
                                .Where(x => x.Codigo.ToLower() == codigo.ToLower())
                                .FirstOrDefault();

                if (_usuario == null)
                {
                    retorno.Success = false;
                    retorno.Validate = false;
                    retorno.Messages.Add("Código de validação inválido ou usuário não encontrado.");
                    retorno.Data = _mapper.Map<UsuarioResponse>(_usuario);
                }
                else
                {
                    retorno.Success = true;
                    retorno.Validate = true;
                    retorno.Messages.Add("Código de validação encontrado com sucesso.");
                }
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Erro ao validar usuário: ");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }
        public async Task<Reply> AddPerfil(int idUsuario, int idPerfil)
        {
            Reply retorno = new Reply();

            try
            {
                var usuarioPerfil = await _unitOfWork.UsuarioPerfis.AsQueryable()
                                  .Where(x => x.UsuarioId == idUsuario && x.PerfilId == idPerfil)
                                  .FirstOrDefaultAsync();

                if (usuarioPerfil == null)
                {
                    var novo = _unitOfWork.UsuarioPerfis.AddAsync(new UsuarioPerfil
                    {
                        UsuarioId = idUsuario,
                        PerfilId = idPerfil
                    });

                    if (novo.IsCompletedSuccessfully)
                    {
                        await _unitOfWork.CommitAsync();
                        retorno.Success = true;
                        retorno.Messages.Add("Perfil adicionado ao usuario com sucesso.");
                    }
                    else
                    {
                        retorno.Success = false;
                        retorno.Messages.Add("Erro ao adicionar perfil ao usuário.");
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
        public Task<Reply> RemPerfil(int id)
        {
            Reply retorno = new Reply();
            try
            {
                var remover = _unitOfWork.UsuarioPerfis.GetByIdAsync(id).Result;
                if (remover != null)
                {
                    _unitOfWork.UsuarioPerfis.Delete(remover);
                    _unitOfWork.CommitAsync().Wait();
                    retorno.Success = true;
                    retorno.Messages.Add("Perfil removido do usuário com sucesso.");
                }
                else
                {
                    retorno.Success = false;
                    retorno.Messages.Add("Perfil não encontrado ou já removido.");
                }
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Erro ao remover perfil do usuário.");
                retorno.Errors.Add(ex.Message);
            }
            return Task.FromResult(retorno);
        }
        public async Task<Reply> GetMfaToken(GetMfaTokenRequest request)
        {
            Reply retorno = new Reply();
            var _config = Tools.Util.GetConfigAccess(_configuration);

            try
            {
                var usuarioMfa = this.GetUser(request.Email).Result;

                if (usuarioMfa != null)
                {
                    var tokenMfa = Util.GenerateCode(6);
                    var expira = DateTime.Now.AddMinutes(_config.DuracaoTokenMfa);

                    var alterar = _mapper.Map<UpdateUsuarioRequest>(usuarioMfa);
                    alterar.MfaCodigo = tokenMfa;
                    alterar.MfaExpira = expira;

                    var update = await this.Update(alterar);

                    if (update.Success == true)
                    {
                        // Mensagem do token
                        var dadoMsg = _unitOfWork.Mensagens.AsQueryable()
                                                .Where(x => x.ID == 1)
                                                .FirstOrDefault();

                        dadoMsg.Marcadores.Add(new MensagemReplace() { Marcador = "@Nome", Valor = usuarioMfa.Nome });
                        dadoMsg.Marcadores.Add(new MensagemReplace() { Marcador = "@Token", Valor = tokenMfa });

                        var msg = await _mensagemService.Render(dadoMsg);

                        if (msg.Success == true)
                        {
                            var queue = await _queueService.Add(new CreateQueueRequest()
                            {
                                Source="Autenticação MFA",
                                MessageMode = 1,
                                Name=usuarioMfa.Nome,
                                Address = usuarioMfa.Whatsapp,
                                Instance = "NandoBento",
                                Message = msg.Data.ToString(),
                                SendDate = DateTime.Now.ToString(),
                            });

                            if(queue.Success == true)
                            {
                                retorno.Success = true;
                                retorno.Messages.Add("Token MFA gerado com sucesso.");
                                retorno.Data = new { token = "ready" };
                            }
                            else
                            {
                                retorno.Success = false;
                                retorno.Messages.Add("Não foi possível gerar o Token MFA");
                                retorno.Data = new { token = "ready" };
                            }                            
                        }
                        else
                        {
                            retorno.Success = false;
                            retorno.Messages.Add("Não foi possível gerar o Token MFA.");
                            retorno.Data = new { token = "failed" };
                        }                           
                    }
                    else
                    {
                        retorno.Success = false;
                        retorno.Messages.Add("Não foi possível gerar o token: ");
                        retorno.Errors.Add(update.Errors.FirstOrDefault() ?? "Erro desconhecido.");
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Erro ao gerar token MFA: ");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }
        public async Task<Reply> LoginMfa(LoginMfaRequest request)
        {
            Reply retorno = new Reply();

            try
            {
                var usuarioMfa = this.GetUser(request.Email).Result;

                if (usuarioMfa != null)
                {
                    if (usuarioMfa.MfaCodigo == request.Token)
                    {
                        if (DateTime.Compare((DateTime)usuarioMfa.MfaExpira, System.DateTime.Now) > 0)
                        {

                            #region Token e completar dados

                            var _userLogado = _mapper.Map<UsuarioLoginResponse>(usuarioMfa);

                            _userLogado.Perfis = new List<UsuarioPerfilResponse>();
                            _userLogado.Perfis = _unitOfWork.UsuarioPerfis.AsQueryable()
                                           .Include(x => x.Perfil)
                                           .Include(x => x.Usuario)
                                           .Where(x => x.UsuarioId == usuarioMfa.ID)
                                           .Select(x => new UsuarioPerfilResponse
                                           {
                                               ID = x.PerfilId,
                                               PerfilNome = x.Perfil.Nome
                                           }).ToList();

                            _userLogado.UltimoLogin = string.Format("{0:dd/MM/yyyy HH:mm:ss}", usuarioMfa.UltimoLogin);

                            var _token = Tools.Util.GenerateToken(_configuration, _userLogado);

                            retorno.Success = true;
                            retorno.Messages.Add("Login via MFA realizado com sucesso.");
                            retorno.Data = new { user = _userLogado, token = _token, mfa = false };

                            #endregion
                        }
                        else
                        {
                            retorno.Success = false;
                            retorno.Messages.Add("Token MFA expirado. Gere um novo token.");
                        }
                    }
                }
                else
                {
                    retorno.Success = false;
                    retorno.Messages.Add("Usuário não existe");
                }
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Não foi possível completar o login:");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }
    }
}
