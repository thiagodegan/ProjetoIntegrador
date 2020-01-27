using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoIntregador.Dados.Model
{
    public class RegistroModelo
    {
        public int Filial { get; set; }
        public int Secao { get; set; }
        public int Grupo { get; set; }
        public int SubGrupo { get; set; }
        public double MeanAbsoluteError { get; set; }
        public double MeanSquaredError { get; set; }
        public double RootMeanSquaredError { get; set; }
        public double LossFunc { get; set; }
        public double RSquared { get; set; }
        public byte[] Modelo { get; set; }
        public string DFilial { get; set; }
        public string Categoria { get; set; }
        public double Size { get; set; }
    }
}
