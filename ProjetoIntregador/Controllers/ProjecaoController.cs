using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ProjetoIntregador.Dados.Bll;
using ProjetoIntregador.Dados.Model;
using ProjetoIntregador.Models;

namespace ProjetoIntregador.Controllers
{
    public class ProjecaoController : Controller
    {
        private readonly IConfiguration configuration;

        public ProjecaoController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GetDados([FromBody]dynamic filtro)
        {
            EfetuarPrevisao efetuarPrevisao = new EfetuarPrevisao(configuration);

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
        public IActionResult GetFiliais()
        {
            TreinarModelos treinarModelos = new TreinarModelos(configuration);

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
            finally
            {
                treinarModelos = null;
            }
        }

        [HttpPost]
        public IActionResult GetCategorias()
        {
            TreinarModelos treinarModelos = new TreinarModelos(configuration);

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
            finally
            {
                treinarModelos = null;
            }
        }
    }
}