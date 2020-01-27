using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProjetoIntregador.BackgroundService.Contract
{
    public interface IAtividade : IHostedService
    {
        Task Process();
        Task ProcessInScope(IServiceProvider serviceProvider);
        void ExecutaAtividade();
        Task ExecuteAsync(CancellationToken stoppingToken);
        string GetStatus();
        DateTime? GetUltimaExecucao();
        TimeSpan GetUltimoTempo();
        string GetNomeAtividade();
        string GetDescricaoAtividade();
        DateTime GetProximaExecucao();
    }
}
