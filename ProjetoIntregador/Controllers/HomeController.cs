using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjetoIntregador.BackgroundService.Contract;
using ProjetoIntregador.Dados.Bll;
using ProjetoIntregador.Dados.Model;

namespace ProjetoIntregador.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly IServiceCollection services;
        private readonly IServiceProvider serviceProvider;
        public HomeController(IConfiguration configuration,
            IServiceCollection services,
            IServiceProvider serviceProvider)
        {
            this.configuration = configuration;
            this.services = services;
            this.serviceProvider = serviceProvider;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";
            
            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        public IActionResult GetModelos()
        {
            EfetuarPrevisao efetuaModelos = new EfetuarPrevisao(configuration);
            var modelos = efetuaModelos.CarregaModeloMetrica();

            if (modelos == null)
            {
                modelos = new RegistroModelo();
            }

            Dictionary<string, object> param = new Dictionary<string, object>
            {
                { "filiais", modelos.Filial.ToString() },
                { "modelos", modelos.Secao.ToString() },
                { "rmse", modelos.RootMeanSquaredError.ToString("N2") }
            };
            return  Json(param);
        }
    }
}
