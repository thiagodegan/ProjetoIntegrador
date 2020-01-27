using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProjetoIntregador.BackgroundService.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProjetoIntregador.BackgroundService.Base
{
    public abstract class BaseAtividade : IAtividade
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger loggerService;
        public BaseAtividade(IServiceScopeFactory serviceScopeFactory, ILogger logger)
        {
            loggerService = logger;
            _serviceScopeFactory = serviceScopeFactory;
            NextRun = DateTime.MaxValue;
            IsRunnig = false;
            Status = "Pausada";
        }

        protected DateTime NextRun;
        protected DateTime? LastRun;
        protected TimeSpan LastTime;
        protected bool IsRunnig;
        protected string Status;
        public abstract string NomeAtividade { get; set; }
        public abstract string DescricaoAtividade { get; set; }
        private readonly CancellationTokenSource stoppingCts = new CancellationTokenSource();
        private Task executingTask;

        public void ExecutaAtividade()
        {
            if (!IsRunnig)
            {
                NextRun = DateTime.Now;
            }
        }

        public virtual async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            do
            {
                try
                {
                    Status = "Aguardando...";
                    var now = DateTime.Now;

                    if (NextRun == DateTime.MaxValue)
                    {
                        Status = "Pausada";
                        IsRunnig = false;
                    }
                    
                    if (now > NextRun)
                    {
                        Status = "Executando...";
                        LastRun = now;
                        IsRunnig = true;
                        await Process();

                        NextRun = DateTime.MaxValue;
                        IsRunnig = false;
                        Status = "Pausada";

                        if (LastRun.HasValue)
                        {
                            LastTime = DateTime.Now - LastRun.Value;
                        }
                        else
                        {
                            LastTime = DateTime.Now.AddMilliseconds(1) - DateTime.Now;
                        }
                    }
                    await Task.Delay(5000, stoppingToken); //5 seconds delay
                }
                catch (Exception ex)
                {
                    loggerService.LogError(ex, "Erro ao executar atividade!");
                    NextRun = DateTime.Now.AddMinutes(10);
                    loggerService.LogWarning($"A atividade {GetNomeAtividade()}-{GetDescricaoAtividade()} será reiniciada às: {NextRun.ToString("dd/MM/yyyy HH:mm:ss")}");
                }
            }
            while (!stoppingToken.IsCancellationRequested);
        }

        public string GetDescricaoAtividade()
        {
            return DescricaoAtividade;
        }

        public string GetNomeAtividade()
        {
            return NomeAtividade;
        }

        public string GetStatus()
        {
            return Status;
        }

        public DateTime? GetUltimaExecucao()
        {
            return LastRun;
        }

        public TimeSpan GetUltimoTempo()
        {
            return LastTime;
        }

        public DateTime GetProximaExecucao()
        {
            return NextRun;
        }

        public virtual async Task Process()
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                await ProcessInScope(scope.ServiceProvider);
            }
        }

        public abstract Task ProcessInScope(IServiceProvider serviceProvider);

        public virtual Task StartAsync(CancellationToken cancellationToken)
        {
            executingTask = ExecuteAsync(stoppingCts.Token);

            if (executingTask.IsCompleted)
            {
                return executingTask;
            }

            // Otherwise it's running
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            loggerService.LogWarning($"Encerra atividade {GetNomeAtividade()}-{GetDescricaoAtividade()}...");
            stoppingCts.Cancel();
            return Task.CompletedTask;
        }
    }
}
