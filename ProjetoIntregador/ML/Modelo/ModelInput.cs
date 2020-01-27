using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoIntregador.ML.Modelo
{
    public class ModelInput
    {
        [ColumnName("ANO"), LoadColumn(0)]
        public float ANO { get; set; }


        [ColumnName("MES_FEV"), LoadColumn(1)]
        public float MES_FEV { get; set; }


        [ColumnName("MES_MAR"), LoadColumn(2)]
        public float MES_MAR { get; set; }


        [ColumnName("MES_ABR"), LoadColumn(3)]
        public float MES_ABR { get; set; }


        [ColumnName("MES_MAI"), LoadColumn(4)]
        public float MES_MAI { get; set; }


        [ColumnName("MES_JUN"), LoadColumn(5)]
        public float MES_JUN { get; set; }


        [ColumnName("MES_JUL"), LoadColumn(6)]
        public float MES_JUL { get; set; }


        [ColumnName("MES_AGO"), LoadColumn(7)]
        public float MES_AGO { get; set; }


        [ColumnName("MES_SET"), LoadColumn(8)]
        public float MES_SET { get; set; }


        [ColumnName("MES_OUT"), LoadColumn(9)]
        public float MES_OUT { get; set; }


        [ColumnName("MES_NOV"), LoadColumn(10)]
        public float MES_NOV { get; set; }


        [ColumnName("MES_DEZ"), LoadColumn(11)]
        public float MES_DEZ { get; set; }


        [ColumnName("SEM_MES_SEG"), LoadColumn(12)]
        public float SEM_MES_SEG { get; set; }


        [ColumnName("SEM_MES_TER"), LoadColumn(13)]
        public float SEM_MES_TER { get; set; }


        [ColumnName("SEM_MES_QUA"), LoadColumn(14)]
        public float SEM_MES_QUA { get; set; }


        [ColumnName("SEM_MES_QUI"), LoadColumn(15)]
        public float SEM_MES_QUI { get; set; }


        [ColumnName("DIA_SEG"), LoadColumn(16)]
        public float DIA_SEG { get; set; }


        [ColumnName("DIA_TER"), LoadColumn(17)]
        public float DIA_TER { get; set; }


        [ColumnName("DIA_QUA"), LoadColumn(18)]
        public float DIA_QUA { get; set; }


        [ColumnName("DIA_QUI"), LoadColumn(19)]
        public float DIA_QUI { get; set; }


        [ColumnName("DIA_SEX"), LoadColumn(20)]
        public float DIA_SEX { get; set; }


        [ColumnName("DIA_SAB"), LoadColumn(21)]
        public float DIA_SAB { get; set; }


        [ColumnName("Label"), LoadColumn(22)]
        public float Label { get; set; }
    }
}
