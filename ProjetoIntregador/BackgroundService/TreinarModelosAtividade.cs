using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProjetoIntregador.BackgroundService.Base;
using ProjetoIntregador.Dados.Bll.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoIntregador.BackgroundService
{
    public class TreinarModelosAtividade : BaseAtividade
    {
        private readonly ILogger<TreinarModelosAtividade> loggerService;
        private readonly IConfiguration configuration;
        public TreinarModelosAtividade(IServiceScopeFactory serviceScopeFactory, 
            ILogger<TreinarModelosAtividade> logger,
            IConfiguration configuration) : base(serviceScopeFactory, logger)
        {
            loggerService = logger;
            this.configuration = configuration;
        }

        public override string NomeAtividade { get => "Treinar Modelos"; set => throw new NotImplementedException(); }
        public override string DescricaoAtividade { get => "Treina modelos para todas filiais e todas categorias."; set => throw new NotImplementedException(); }

        public override async Task ProcessInScope(IServiceProvider serviceProvider)
        {
            try
            {
                var treinarModelos = serviceProvider.GetService<ITreinarModelos>();
                loggerService.LogDebug("Executa atividade: {0}", NomeAtividade);

                await treinarModelos.Treinar();
                
                loggerService.LogDebug("Término atividade: {0}", NomeAtividade);
            }
            catch (Exception ex)
            {
                loggerService.LogError(ex, "Erro ao processar atividade!");
                throw ex;
            }
        }
    }
}
