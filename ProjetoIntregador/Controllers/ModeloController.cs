using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ProjetoIntregador.Dados.Bll;
using ProjetoIntregador.Dados.Model;

namespace ProjetoIntregador.Controllers
{
    public class ModeloController : Controller
    {
        private readonly IConfiguration configuration;
        public ModeloController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetDados([DataSourceRequest]DataSourceRequest request)
        {
            List<RegistroModelo> registroModelos = null;
            TreinarModelos treinarModelos = new TreinarModelos(configuration);
            EfetuarPrevisao efetuarPrevisao = new EfetuarPrevisao(configuration);

            registroModelos = efetuarPrevisao.CarregaModelosMetricas();

            if (registroModelos == null)
            {
                registroModelos = new List<RegistroModelo>();
            }

            var lstFiliais = treinarModelos.ListarFiliais();
            var lstCategorias = treinarModelos.ListarCategorias();

            foreach (var filial in lstFiliais)
            {
                var qry = from p in registroModelos
                          where p.Filial == filial.Filial
                          select p;

                if (qry != null && qry.Any())
                {
                    foreach (var modelo in qry)
                    {
                        modelo.DFilial = filial.Nome;
                    }
                }
            }

            foreach (var categoria in lstCategorias)
            {
                var qry = from p in registroModelos
                          where p.Secao == categoria.Secao
                          && p.Grupo == categoria.Grupo
                          && p.SubGrupo == categoria.SubGrupo
                          select p;

                if (qry != null && qry.Any())
                {
                    foreach (var modelo in qry)
                    {
                        modelo.Categoria = categoria.Categoria;
                    }
                }
            }

            DataSourceResult result = registroModelos.ToDataSourceResult(request);
            return Json(result);
        }
    }
}