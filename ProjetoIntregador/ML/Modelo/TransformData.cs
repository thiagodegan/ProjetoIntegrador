using ProjetoIntregador.Dados.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoIntregador.ML.Modelo
{
    public class TransformData
    {
        public List<ModelInput> TransformaDados(List<RegistroCmv> registroCmvs)
        {
            List<ModelInput> modelOutputs = new List<ModelInput>();

            foreach (var item in registroCmvs)
            {                
                modelOutputs.Add(TransformaDados(item));
            }

            return modelOutputs;
        }

        public ModelInput TransformaDados(RegistroCmv registroCmv)
        {
            ModelInput modelOutput = new ModelInput
            {
                Label = registroCmv.Valor,
                ANO = registroCmv.Dia.Year,
                DIA_SEG = registroCmv.Dia.DayOfWeek == DayOfWeek.Monday ? 1 : 0,
                DIA_TER = registroCmv.Dia.DayOfWeek == DayOfWeek.Tuesday ? 1 : 0,
                DIA_QUA = registroCmv.Dia.DayOfWeek == DayOfWeek.Wednesday ? 1 : 0,
                DIA_QUI = registroCmv.Dia.DayOfWeek == DayOfWeek.Thursday ? 1 : 0,
                DIA_SEX = registroCmv.Dia.DayOfWeek == DayOfWeek.Friday ? 1 : 0,
                DIA_SAB = registroCmv.Dia.DayOfWeek == DayOfWeek.Saturday ? 1 : 0,
                MES_FEV = registroCmv.Dia.Month == 2 ? 1 : 0,
                MES_MAR = registroCmv.Dia.Month == 3 ? 1 : 0,
                MES_ABR = registroCmv.Dia.Month == 4 ? 1 : 0,
                MES_MAI = registroCmv.Dia.Month == 5 ? 1 : 0,
                MES_JUN = registroCmv.Dia.Month == 6 ? 1 : 0,
                MES_JUL = registroCmv.Dia.Month == 7 ? 1 : 0,
                MES_AGO = registroCmv.Dia.Month == 8 ? 1 : 0,
                MES_SET = registroCmv.Dia.Month == 9 ? 1 : 0,
                MES_OUT = registroCmv.Dia.Month == 10 ? 1 : 0,
                MES_NOV = registroCmv.Dia.Month == 11 ? 1 : 0,
                MES_DEZ = registroCmv.Dia.Month == 12 ? 1 : 0,
                SEM_MES_SEG = GetWeekNumberOfMonth(registroCmv.Dia) == 2 ? 1 : 0,
                SEM_MES_TER = GetWeekNumberOfMonth(registroCmv.Dia) == 3 ? 1 : 0,
                SEM_MES_QUA = GetWeekNumberOfMonth(registroCmv.Dia) == 4 ? 1 : 0,
                SEM_MES_QUI = GetWeekNumberOfMonth(registroCmv.Dia) == 5 ? 1 : 0,
                FERIADO = registroCmv.Feriado ? 1 : 0,
                MAXTEMPC = registroCmv.MaxTempC,
                MINTEMPC = registroCmv.MinTempC,
                AVGTEMPC = registroCmv.AvgTempC,
                PRECIPMM = registroCmv.PrecipMm,
            };

            return modelOutput;
        }

        private int GetWeekNumberOfMonth(DateTime date)
        {
            date = date.Date;
            DateTime firstMonthDay = new DateTime(date.Year, date.Month, 1);
            DateTime firstMonthMonday = firstMonthDay.AddDays((DayOfWeek.Monday + 7 - firstMonthDay.DayOfWeek) % 7);
            if (firstMonthMonday > date)
            {
                firstMonthDay = firstMonthDay.AddMonths(-1);
                firstMonthMonday = firstMonthDay.AddDays((DayOfWeek.Monday + 7 - firstMonthDay.DayOfWeek) % 7);
            }
            return (date - firstMonthMonday).Days / 7 + 1;
        }
    }
}
