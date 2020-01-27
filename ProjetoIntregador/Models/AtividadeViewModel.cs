using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoIntregador.Models
{
    public class AtividadeViewModel
    {
        [DisplayName("Atividade")]
        [ReadOnly(true)]
        public string NomeAtividade { get; set; }
        [DisplayName("Descrição")]
        [ReadOnly(true)]
        public string DescricaoAtividade { get; set; }

        [DisplayName("Status")]
        public string Status { get; set; }
        [DisplayName("Última Execução")]
        public DateTime? UltimaExecucao { get; set; }
        [DisplayName("Próxima Execução")]
        public DateTime ProximaExecucao { get; set; }
        [DisplayName("Último Tempo Exec.")]
        public DateTime UltimoTempo { get; set; }

    }
}
