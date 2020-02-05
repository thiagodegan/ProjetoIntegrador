using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoIntregador.Dados.Model
{
    public class RegistroCmv
    {
        public DateTime Dia { get; set; }
        public bool Feriado { get; set; }
        public float MaxTempC { get;set; }
        public float MinTempC { get; set; }
        public float AvgTempC { get; set; }
        public float PrecipMm { get; set; }
        public float Valor { get; set; }
        public float Previsao { get; set; }
        public bool ValorIsNull { get; set; }
        public float? ValorNull { get { return ValorIsNull ? null : (float?)Valor; } }
        public string DiaFmt { get { return Dia.ToString("dd/MM/yyyy"); } }
        public int Filial { get; set; }
        public int Secao { get; set; }
        public int Grupo { get; set; }
        public int SubGrupo { get; set; }
        public string Nome { get; set; }
        public string Categoria { get; set; }
        public float Erro 
        { 
            get 
            { 
                if (Valor == 0 && Previsao == 0)
                    return 0;
                if (Valor == 0)
                    return 1;
                return (Previsao-Valor)/Valor; 
            } 
        }
    }
}
