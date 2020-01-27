using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoIntregador.Dados.Model
{
    public class RegistroCategoria
    {
        public int Secao { get; set; }
        public int Grupo { get; set; }
        public int SubGrupo { get; set; }
        public string DSecao { get; set; }
        public string DGrupo { get; set; }
        public string DSubGrupo { get; set; }

        public int Id
        {
            get { return (Secao * 1000 + Grupo) * 1000 + SubGrupo; }
        }
        public string Categoria
        {
            get { return $"{DSecao} / {DGrupo} / {DSubGrupo}"; }
        }
    }
}
