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
    public class CongService : ICongService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CongService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Reply> AddAsync(CongNewModel novo)
        {
            Reply retorno = new Reply();

            try
            {
                var _novo = _mapper.Map<Cong>(novo);
                _novo.Criado = System.DateTime.Now;
                var resultado = _unitOfWork.Congs.AddAsync(_novo);

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

        public async Task<Reply> GetAll(CongQueryModel condicao)
        {
            Reply retorno = new Reply();

            try
            {
                var query = _unitOfWork.Congs.AsQueryable();

                if (condicao.Id.HasValue && condicao.Id.Value > 0)
                {
                    query = query.Where(x => x.ID == condicao.Id.Value);
                }
                if (!string.IsNullOrEmpty(condicao.Nome))
                {
                    query = query.Where(x => x.Nome.ToLower().Contains(condicao.Nome.ToLower()));
                }
                if (!string.IsNullOrEmpty(condicao.Circuito))
                {
                    query = query.Where(x => x.Circuito.ToLower().Contains(condicao.Circuito.ToLower()));
                }
                if (!string.IsNullOrEmpty(condicao.Resposavel))
                {
                    query = query.Where(x => x.Responsavel.ToLower().Contains(condicao.Resposavel.ToLower()));
                }

                var resultado = query.ToList();

                retorno.Success = true;

                if (resultado == null || resultado.Count == 0)
                {
                    retorno.Messages.Add("Nenhum registro encontrado.");
                    retorno.Data = new List<CongResponse>();
                }

                retorno.Messages.Add("Registro(s) encontrado(s) com sucesso.");

                var resultadoFinal = new List<Cong>();

                var primeiroResigstro = resultado.OrderBy(x => x.ID).FirstOrDefault();
                resultadoFinal.Add(primeiroResigstro);

                var outrosRegistros = resultado
                                    .Where(x => x.ID != resultadoFinal.FirstOrDefault().ID)
                                    .OrderBy(x => x.Nome)
                                    .ToList();
                resultadoFinal.AddRange(outrosRegistros);

                retorno.Data = _mapper.Map<List<CongResponse>>(resultadoFinal);
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Não foi possível pesquisar o endereço.");
                retorno.Errors.Add(ex.Message);
            }

            return retorno;
        }

        public async Task<Reply> GetOne(int id)
        {
            Reply retorno = new Reply();
            try
            {
                var dados = await _unitOfWork.Congs
                                             .AsQueryable()
                                             .Include(x => x.Arranjos)
                                             .Where(x => x.ID==id)
                                             .FirstOrDefaultAsync();
                retorno.Success = true;
                retorno.Messages.Add("Registro encontrado com sucesso.");
                retorno.Data = dados;
            }
            catch (Exception ex)
            {
                retorno.Success = false;
                retorno.Messages.Add("Não foi possível consultr os registros");
                retorno.Errors.Add($"{ex.Message}");
            }
            return retorno;
        }

        public async Task<Reply> UpdateAsync(CongUpdateModel dados)
        {
            Reply retorno = new Reply();

            try
            {
                var _update = new Cong()
                {
                    ID = dados.ID,
                    Nome = dados.Nome,
                    Circuito = dados.Circuito,
                    Dia = dados.Dia,
                    Horario = dados.Horario,
                    Responsavel = dados.Responsavel,
                    Email = dados.Email,
                    Endereco = dados.Endereco,
                    Maps = dados.Maps,
                    Fone = dados.Fone
                };

                _unitOfWork.Congs.Update(_update);
                await _unitOfWork.CommitAsync();

                retorno.Success = true;
                retorno.Messages.Add("Operação realizada com sucesso.");
                retorno.Data = _update;
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
