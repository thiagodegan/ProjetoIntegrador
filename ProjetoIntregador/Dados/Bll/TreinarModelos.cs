﻿using Microsoft.Extensions.Configuration;
using ProjetoIntregador.Dados.Dal;
using ProjetoIntregador.Dados.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.Data;
using ProjetoIntregador.ML.Modelo;
using ProjetoIntregador.Dados.Bll.Contract;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;

namespace ProjetoIntregador.Dados.Bll
{
    public class TreinarModelos : ITreinarModelos
    {
        readonly IConfiguration configuration;
        private ILogger<TreinarModelos> logger;
        public TreinarModelos(IConfiguration configuration, ILogger<TreinarModelos> logger)
        {
            this.configuration = configuration;
            this.logger = logger;
        }

        public List<RegistroFilial> ListarFiliais()
        {
            DalConnection dal = new DalConnection(configuration, logger);

            try
            {
                List<RegistroFilial> registroFilials = new List<RegistroFilial>();
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("SELECT codigo FILIAL,");
                sb.AppendLine("'LOJA '||codigo NOME");
                sb.AppendLine("FROM gs_mvw_filiais");
                sb.AppendLine("WHERE tipofilial = 'L'");
                sb.AppendLine("AND natureza = 'LS'");
                sb.AppendLine("AND codigo >= 5"); // TODO: REMOVER EM PRODUCAO ESTA TRAVADO APENAS UMA FILIAL
                sb.AppendLine("ORDER BY codigo");

                var dt = dal.ExecuteQuery(sb, null);

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow dataRow in dt.Rows)
                    {
                        RegistroFilial registro = new RegistroFilial();

                        foreach (DataColumn dataColumn in dt.Columns)
                        {
                            switch (dataColumn.ColumnName.ToUpper())
                            {
                                case "FILIAL": registro.Filial = Convert.ToInt32(dataRow[dataColumn]); break;
                                case "NOME": registro.Nome = Convert.ToString(dataRow[dataColumn]); break;
                                default:
                                    break;
                            }
                        }

                        registroFilials.Add(registro);
                    }
                }

                return registroFilials;
            }
            catch 
            {
                throw;
            }
            finally
            {
                if (dal != null)
                {
                    dal = null;
                }
            }
        }

        public List<RegistroCategoria> ListarCategorias()
        {
            DalConnection dal = new DalConnection(configuration, logger);

            try
            {
                List<RegistroCategoria> registroCategorias = new List<RegistroCategoria>();

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("select secao,");
                sb.AppendLine("       grupo,");
                sb.AppendLine("       subgrupo,");
                sb.AppendLine("       dsecao,");
                sb.AppendLine("       dgrupo,");
                sb.AppendLine("       dsubgrupo");
                sb.AppendLine("from gs_mvw_estr_mercadologica");
                sb.AppendLine("where secao > 0");
                sb.AppendLine("and grupo > 0");
                sb.AppendLine("and subgrupo > 0");
                sb.AppendLine("and categoria = 0");
                sb.AppendLine("order by secao, grupo, subgrupo");

                var dt = dal.ExecuteQuery(sb, null);

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow dataRow in dt.Rows)
                    {
                        RegistroCategoria registro = new RegistroCategoria();

                        foreach (DataColumn dataColumn in dt.Columns)
                        {
                            switch (dataColumn.ColumnName.ToUpper())
                            {
                                case "SECAO": registro.Secao = Convert.ToInt32(dataRow[dataColumn]); break;
                                case "GRUPO": registro.Grupo = Convert.ToInt32(dataRow[dataColumn]); break;
                                case "SUBGRUPO": registro.SubGrupo = Convert.ToInt32(dataRow[dataColumn]); break;
                                case "DSECAO": registro.DSecao = Convert.ToString(dataRow[dataColumn]); break;
                                case "DGRUPO": registro.DGrupo = Convert.ToString(dataRow[dataColumn]); break;
                                case "DSUBGRUPO": registro.DSubGrupo = Convert.ToString(dataRow[dataColumn]); break;
                                default:
                                    break;
                            }
                        }

                        registroCategorias.Add(registro);
                    }
                }

                return registroCategorias;
            }
            catch 
            {
                throw;
            }
            finally
            {
                dal = null;
            }
        }

        private List<RegistroCmv> ListarHistorico(int Filial, int Secao, int Grupo, int SubGrupo)
        {
            DalConnection dal = new DalConnection(configuration, logger);

            try
            {
                List<RegistroCmv> registroCmvs = new List<RegistroCmv>();

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("select RMS7TO_DATE(g.dia) DIA,");
                sb.AppendLine("       case when df.dia_meta is null");
                sb.AppendLine("              and d.dia_meta is null then 0 else 1 end feriado,");
                sb.AppendLine("       C.MAXTEMPC,");
                sb.AppendLine("       C.MINTEMPC,");
                sb.AppendLine("       C.AVGTEMPC,");
                sb.AppendLine("       C.PRECIPMM,");
                sb.AppendLine("       G.VDA_CMV VALOR");
                sb.AppendLine("from gs_agg_coml_sgrp_dia g");
                sb.AppendLine("inner join gs_mvw_filiais f on f.codigo = g.filial");
                sb.AppendLine("left join clima c on c.city = f.cidade");
                sb.AppendLine("                 and c.dia = g.dia");
                sb.AppendLine("left join gs_meta_diasatipico_filial df on df.filial = g.filial");
                sb.AppendLine("                                        and df.dia_meta = rms7to_date(g.dia)");
                sb.AppendLine("left join gs_meta_diasatipico d on d.dia_meta = rms7to_date(g.dia)");
                sb.AppendLine("where g.filial = &FILIAL");
                sb.AppendLine("and g.secao = &SECAO");
                sb.AppendLine("and g.grp = &GRUPO");
                sb.AppendLine("and g.sgrp = &SUBGRUPO");
                sb.AppendLine("and g.dia between 1170101 and 1190831"); // TODO: REMOVER ESSE FILTRO DE DATA EM PRODUÇÃO
                sb.AppendLine("and g.vda_cmv > 0");
                sb.AppendLine("order by g.dia");

                Dictionary<string, object> param = new Dictionary<string, object>
                {
                    { "FILIAL", Filial },
                    { "SECAO", Secao },
                    { "GRUPO", Grupo },
                    { "SUBGRUPO", SubGrupo }
                };

                var dt = dal.ExecuteQuery(sb, param);

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow dataRow in dt.Rows)
                    {
                        RegistroCmv registro = new RegistroCmv();

                        foreach (DataColumn dataColumn in dt.Columns)
                        {
                            switch (dataColumn.ColumnName.ToUpper())
                            {
                                case "DIA": registro.Dia = Convert.ToDateTime(dataRow[dataColumn]); break;
                                case "VALOR": registro.Valor = (float)Convert.ToDouble(dataRow[dataColumn]); break;
                                case "FERIADO": registro.Feriado = Convert.ToInt32(dataRow[dataColumn]) == 1; break;
                                case "MAXTEMPC": registro.MaxTempC = dataRow[dataColumn] != DBNull.Value ? (float)Convert.ToDouble(dataRow[dataColumn]) : 0; break;
                                case "MIMTEMPC": registro.MinTempC = dataRow[dataColumn] != DBNull.Value ? (float)Convert.ToDouble(dataRow[dataColumn]) : 0; break;
                                case "AVGTEMPC": registro.AvgTempC = dataRow[dataColumn] != DBNull.Value ? (float)Convert.ToDouble(dataRow[dataColumn]) : 0; break;
                                case "PRECIPMM": registro.PrecipMm = dataRow[dataColumn] != DBNull.Value ? (float)Convert.ToDouble(dataRow[dataColumn]) : 0; break;
                                default:
                                    break;
                            }
                        }

                        registroCmvs.Add(registro);
                    }
                }

                return registroCmvs;
            }
            catch (OracleException ex)
            {
                if (ex.Message.ToLower().Contains("timeout"))
                {
                    logger.LogError(ex, $"Time out ao consultar historico, sera realizada uma nova tentativa! Filial: {Filial} Categoria: {Secao}/{Grupo}/{SubGrupo}");
                    Task.Delay(1000).Wait();
                    return ListarHistorico(Filial, Secao, Grupo, SubGrupo);
                }
                else
                {
                    throw ex;
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                dal = null;
            }
        }

        private void GravarModelo(RegistroModelo registroModelo)
        {
            DalConnection dal = new DalConnection(configuration, logger);

            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("DELETE FROM DSDH_MODELOS_CMV");
                sb.AppendLine("WHERE FILIAL = &FIL");
                sb.AppendLine("AND SECAO = &SEC");
                sb.AppendLine("AND GRUPO = &GRP");
                sb.AppendLine("AND SUBGRUPO = &SGRP");

                Dictionary<string, object> param = new Dictionary<string, object>
                {
                    { "FIL", registroModelo.Filial },
                    { "SEC", registroModelo.Secao },
                    { "GRP", registroModelo.Grupo },
                    { "SGRP", registroModelo.SubGrupo }
                };

                dal.ExecuteNonQuery(sb, param);

                sb = new StringBuilder();
                sb.AppendLine("INSERT INTO DSDH_MODELOS_CMV (");
                sb.AppendLine("FILIAL, SECAO, GRUPO, SUBGRUPO,");
                sb.AppendLine("MEANABSOLUTEERROR,MEANSQUAREDERROR,ROOTMEANSQUAREDERROR,");
                sb.AppendLine("LOSSFUNC,RSQUARED,MODELO)");
                sb.AppendLine("VALUES (&FIL, &SEC, &GRP, &SGRP,");
                sb.AppendLine("&MAB, &MSE, &RMSE, &LSFNC, &RSQ, &MDL)");

                param = new Dictionary<string, object>
                {
                    { "FIL", registroModelo.Filial },
                    { "SEC", registroModelo.Secao },
                    { "GRP", registroModelo.Grupo },
                    { "SGRP", registroModelo.SubGrupo },
                    { "MAB", registroModelo.MeanAbsoluteError },
                    { "MSE", registroModelo.MeanSquaredError },
                    { "RMSE", registroModelo.RootMeanSquaredError },
                    { "LSFNC", registroModelo.LossFunc },
                    { "RSQ", registroModelo.RSquared },
                    { "MDL", registroModelo.Modelo }
                };

                dal.ExecuteNonQuery(sb, param);
              

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dal = null;
            }
        }

        public async Task Treinar()
        {
            var filiais = ListarFiliais();
            var categorias = ListarCategorias();

            TransformData transformData = new TransformData();
            ModelBuilder modelBuilder = new ModelBuilder();
            List<Task> lstTsks = new List<Task>();

            foreach (var filial in filiais)
            {           
                if (filial.Filial >=5)
                {       
                    var tsk = Task.Run(() => {
                        foreach (var categoria in categorias)
                        {
                            try
                            {
                                logger.LogInformation($"Inicio Treina Filial {filial.Filial} Categoria {categoria.Secao}/{categoria.Grupo}/{categoria.SubGrupo}");
                                var historico = ListarHistorico(filial.Filial, categoria.Secao, categoria.Grupo, categoria.SubGrupo);
        
                                if (historico != null && historico.Where(f=> f.Dia >= new DateTime(2018,1,1)).Count() > 365 && historico.Where(f=> f.Dia >= new DateTime(2019,8,1)).Sum(m => m.Valor) > 100)
                                {                                
                                    var transformedData = transformData.TransformaDados(historico);
                                    logger.LogInformation($"Cria modelo Filial {filial.Filial} Categoria {categoria.Secao}/{categoria.Grupo}/{categoria.SubGrupo}");
                                    var modelo = modelBuilder.CreateModel(transformedData);
                                    modelo.Filial = filial.Filial;
                                    modelo.Secao = categoria.Secao;
                                    modelo.Grupo = categoria.Grupo;
                                    modelo.SubGrupo = categoria.SubGrupo;
                                    logger.LogInformation($"Grava modelo Filial {filial.Filial} Categoria {categoria.Secao}/{categoria.Grupo}/{categoria.SubGrupo}");
                                    GravarModelo(modelo);                                
                                }
                                else
                                {
                                    logger.LogInformation($"Treina Filial {filial.Filial} Categoria {categoria.Secao}/{categoria.Grupo}/{categoria.SubGrupo} sem historico!");
                                }

                                historico = null;
                                logger.LogInformation($"Fim Treina Filial {filial.Filial} Categoria {categoria.Secao}/{categoria.Grupo}/{categoria.SubGrupo}");
                                
                            }
                            catch (Exception ex) 
                            { 
                                    logger.LogError(ex,$"Treina Filial {filial.Filial} Categoria {categoria.Secao}/{categoria.Grupo}/{categoria.SubGrupo}");
                            }
                        }
                    });   

                    lstTsks.Add(tsk);
                }              
            }
             
            if (lstTsks.Count > 0)
                await Task.WhenAll(lstTsks);
        }
    }
}
