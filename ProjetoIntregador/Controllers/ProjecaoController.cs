using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ProjetoIntregador.Dados.Bll;
using ProjetoIntregador.Dados.Bll.Contract;
using ProjetoIntregador.Dados.Model;
using ProjetoIntregador.Models;

namespace ProjetoIntregador.Controllers
{
    public class ProjecaoController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<ProjecaoController> logger;
        private readonly ITreinarModelos treinarModelos;

        public ProjecaoController(IConfiguration configuration, ILogger<ProjecaoController> logger, ITreinarModelos treinarModelos)
        {
            this.configuration = configuration;
            this.logger = logger;
            this.treinarModelos = treinarModelos;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GetDados([FromBody]dynamic filtro)
        {
            EfetuarPrevisao efetuarPrevisao = new EfetuarPrevisao(configuration, logger);

            try
            {
                ProjecaoFiltroViewModel filtroViewModel = JsonConvert.DeserializeObject<ProjecaoFiltroViewModel>(filtro.ToString());

                var dados = efetuarPrevisao.CarregaPrevisao(filtroViewModel.DtIniTrat, filtroViewModel.DtFimTrat, filtroViewModel.Filiais, filtroViewModel.Categorias);

                if (dados == null)
                {
                    dados = new List<RegistroCmv>();
                }

                return Json(dados);
            }
            catch
            {
                throw;
            }
            finally
            {
                efetuarPrevisao = null;
            }
        }

        [HttpPost]
        public IActionResult Getdetalhes([FromBody]dynamic filtro)
        {
            EfetuarPrevisao efetuarPrevisao = new EfetuarPrevisao(configuration, logger);

            try
            {
                ProjecaoFiltroViewModel filtroViewModel = JsonConvert.DeserializeObject<ProjecaoFiltroViewModel>(filtro.ToString());

                var dados = efetuarPrevisao.CarregaPrevisaoDetalhada(filtroViewModel.DtIniTrat, filtroViewModel.DtFimTrat, filtroViewModel.Filiais, filtroViewModel.Categorias);

                if (dados == null)
                {
                    dados = new List<RegistroCmv>();
                }

                var filiais = treinarModelos.ListarFiliais();
                var categorias = treinarModelos.ListarCategorias();

                foreach(var previsao in dados)
                {
                    var qryFil = from p in filiais
                                 where p.Filial == previsao.Filial
                                 select p;
                    
                    if (qryFil != null && qryFil.Any())
                    {
                        previsao.Nome = qryFil.First().Nome;
                    }

                    var qrySec = from p in categorias
                                 where p.Secao == previsao.Secao
                                 && p.Grupo == previsao.Grupo
                                 && p.SubGrupo == previsao.SubGrupo
                                 select p;
                    
                    if (qrySec != null && qrySec.Any())
                    {
                        previsao.Categoria = qrySec.First().Categoria;
                    }
                }

                return Json(dados);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                efetuarPrevisao = null;
            }
        }

        [HttpPost]
        public IActionResult GetFiliais()
        {            
            try
            {
                var dados = treinarModelos.ListarFiliais();

                if (dados == null)
                {
                    dados = new List<RegistroFilial>();
                }

                return Json(dados);
            }
            catch
            {
                throw;
            }
        }

        [HttpPost]
        public IActionResult GetCategorias()
        {            

            try
            {
                var dados = treinarModelos.ListarCategorias();

                if (dados == null)
                {
                    dados = new List<RegistroCategoria>();
                }
                
                return Json(dados);
            }
            catch
            {
                throw;
            }
        }
    }
}