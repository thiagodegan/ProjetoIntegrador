using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using ProjetoIntregador.BackgroundService.Contract;
using ProjetoIntregador.Models;

namespace ProjetoIntregador.Controllers
{
    public class AtividadeController : Controller
    {
        private readonly IServiceProvider serviceProvider;
        public AtividadeController(IServiceProvider provider)
        {
            serviceProvider = provider;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetAtividade([DataSourceRequest]DataSourceRequest request)
        {
            List<AtividadeViewModel> atividadeViewModels = new List<AtividadeViewModel>();

            var atividadeServ = serviceProvider.GetServices(typeof(Microsoft.Extensions.Hosting.IHostedService));
            foreach (var itemTask in atividadeServ)
            {
                if (itemTask is IAtividade atividade)
                {
                    atividadeViewModels.Add(new AtividadeViewModel()
                    {
                        DescricaoAtividade = atividade.GetDescricaoAtividade(),
                        NomeAtividade = atividade.GetNomeAtividade(),
                        Status = atividade.GetStatus(),
                        ProximaExecucao = atividade.GetProximaExecucao().ToLocalTime(),
                        UltimaExecucao = atividade.GetUltimaExecucao(),
                        UltimoTempo = new DateTime(atividade.GetUltimoTempo().Ticks)
                    });
                }

            }
            DataSourceResult result = atividadeViewModels.ToDataSourceResult(request);
            return Json(result);
        }

        [HttpPost]
        public IActionResult StartAtividade([FromQuery]string NomeAtividade)
        {
            if (string.IsNullOrWhiteSpace(NomeAtividade))
            {
                return NotFound();
            }

            var atividadeServ = serviceProvider.GetServices(typeof(Microsoft.Extensions.Hosting.IHostedService));

            foreach (var item in atividadeServ)
            {
                if (item is IAtividade atividade)
                {
                    if (atividade.GetNomeAtividade() == NomeAtividade)
                    {
                        atividade.ExecutaAtividade();
                    }
                }
            }

            return Ok();
        }
    }
}