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
    }
}
