using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProjetoIntregador.BackgroundService.Base;
using ProjetoIntregador.Dados.Bll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoIntregador.BackgroundService
{
    public class EfetuaPrevisaoAtividade : BaseAtividade
    {
        private readonly ILogger<EfetuaPrevisaoAtividade> loggerService;
        private readonly IConfiguration configuration;
        public EfetuaPrevisaoAtividade(IServiceScopeFactory serviceScopeFactory, 
            ILogger<EfetuaPrevisaoAtividade> logger,
            IConfiguration configuration) : base(serviceScopeFactory, logger)
        {
            this.loggerService = logger;
            this.configuration = configuration;
        }

        public override string NomeAtividade { get => "Efetua Previsões"; set => throw new NotImplementedException(); }
        public override string DescricaoAtividade { get => "Efetua as previsões de CMV"; set => throw new NotImplementedException(); }

        public override async Task ProcessInScope(IServiceProvider serviceProvider)
        {
            try
            {
                loggerService.LogDebug("Executa atividade: {0}", NomeAtividade);

                EfetuarPrevisao treinarModelos = new EfetuarPrevisao(configuration, loggerService);

                await treinarModelos.EfetuaPrevisao(new DateTime(2019,8,1), new DateTime(2019,9,30));

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
