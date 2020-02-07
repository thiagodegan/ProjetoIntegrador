using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProjetoIntregador.Dados.Dal;
using ProjetoIntregador.Dados.Model;
using ProjetoIntregador.ML.Consumo;
using ProjetoIntregador.ML.Modelo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetoIntregador.Dados.Bll
{
    public class EfetuarPrevisao
    {
        private readonly IConfiguration configuration;
        private readonly ILogger logger;
        public EfetuarPrevisao(IConfiguration configuration, ILogger logger)
        {
            this.configuration = configuration;
            this.logger = logger;
        }
        public List<RegistroModelo> CarregaModelos()
        {
            DalConnection dal = new DalConnection(configuration, logger);
            List<RegistroModelo> registroModelos = null;

            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("SELECT FILIAL, SECAO, GRUPO, SUBGRUPO, MODELO,");
                sb.AppendLine("ROUND(MEANABSOLUTEERROR, 2) MEANABSOLUTEERROR, ROUND(MEANSQUAREDERROR, 2) MEANSQUAREDERROR, ROUND(ROOTMEANSQUAREDERROR, 2) ROOTMEANSQUAREDERROR");
                sb.AppendLine("FROM DSDH_MODELOS_CMV");
                sb.AppendLine("ORDER BY FILIAL, SECAO, GRUPO, SUBGRUPO");

                var dt = dal.ExecuteQuery(sb, null);

                if (dt != null && dt.Rows.Count > 0)
                {
                    registroModelos = new List<RegistroModelo>();

                    foreach (DataRow dr in dt.Rows)
                    {
                        RegistroModelo modelo = new RegistroModelo();

                        foreach(DataColumn dc in dt.Columns)
                        {
                            switch (dc.ColumnName.ToUpper())
                            {
                                case "FILIAL": modelo.Filial = Convert.ToInt32(dr[dc]); break;
                                case "SECAO": modelo.Secao = Convert.ToInt32(dr[dc]); break;
                                case "GRUPO": modelo.Grupo = Convert.ToInt32(dr[dc]); break;
                                case "SUBGRUPO": modelo.SubGrupo = Convert.ToInt32(dr[dc]); break;
                                case "MODELO": modelo.Modelo = dr[dc] != DBNull.Value ? (byte[])dr[dc] : null; break;
                                case "MEANABSOLUTEERROR": modelo.MeanAbsoluteError = Convert.ToDouble(dr[dc]); break;
                                case "MEANSQUAREDERROR": modelo.MeanSquaredError = Convert.ToDouble(dr[dc]); break;
                                case "ROOTMEANSQUAREDERROR": modelo.RootMeanSquaredError = Convert.ToDouble(dr[dc]); break;
                                default:
                                    break;
                            }
                        }

                        registroModelos.Add(modelo);
                    }
                }

                return registroModelos;
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

        public List<RegistroModelo> CarregaModelosMetricas()
        {
            DalConnection dal = new DalConnection(configuration, logger);
            List<RegistroModelo> registroModelos = null;

            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("SELECT FILIAL, SECAO, GRUPO, SUBGRUPO,");
                sb.AppendLine("ROUND(MEANABSOLUTEERROR, 2) MEANABSOLUTEERROR, ROUND(MEANSQUAREDERROR, 2) MEANSQUAREDERROR, ROUND(ROOTMEANSQUAREDERROR, 2) ROOTMEANSQUAREDERROR");
                sb.AppendLine("FROM DSDH_MODELOS_CMV");
                sb.AppendLine("ORDER BY FILIAL, SECAO, GRUPO, SUBGRUPO");

                var dt = dal.ExecuteQuery(sb, null);

                if (dt != null && dt.Rows.Count > 0)
                {
                    registroModelos = new List<RegistroModelo>();

                    foreach (DataRow dr in dt.Rows)
                    {
                        RegistroModelo modelo = new RegistroModelo();

                        foreach (DataColumn dc in dt.Columns)
                        {
                            switch (dc.ColumnName.ToUpper())
                            {
                                case "FILIAL": modelo.Filial = Convert.ToInt32(dr[dc]); break;
                                case "SECAO": modelo.Secao = Convert.ToInt32(dr[dc]); break;
                                case "GRUPO": modelo.Grupo = Convert.ToInt32(dr[dc]); break;
                                case "SUBGRUPO": modelo.SubGrupo = Convert.ToInt32(dr[dc]); break;
                                case "MODELO": modelo.Modelo = dr[dc] != DBNull.Value ? (byte[])dr[dc] : null; break;
                                case "MEANABSOLUTEERROR": modelo.MeanAbsoluteError = Convert.ToDouble(dr[dc]); break;
                                case "MEANSQUAREDERROR": modelo.MeanSquaredError = Convert.ToDouble(dr[dc]); break;
                                case "ROOTMEANSQUAREDERROR": modelo.RootMeanSquaredError = Convert.ToDouble(dr[dc]); break;
                                default:
                                    break;
                            }
                        }

                        registroModelos.Add(modelo);
                    }
                }

                return registroModelos;
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

        private bool VerificaFeriado (DateTime dia, int Filial)
        {
            DalConnection dal = new DalConnection(configuration, logger);

            try 
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("SELECT 1 feriado");
                sb.AppendLine("FROM DUAL");
                sb.AppendLine("WHERE EXISTS (SELECT 1 FROM GS_META_DIASATIPICO WHERE dia_meta = &DIA)");
                sb.AppendLine("OR exists (select 1 from gs_meta_diasatipico_filial where dia_meta = &DIA and filial = &FIL)");

                Dictionary<string, object> param = new Dictionary<string, object> 
                {
                    { "DIA", dia},
                    { "FIL", Filial}
                };

                var dt = dal.ExecuteQuery(sb, param);

                if (dt != null && dt.Rows.Count > 0)
                {
                    return Convert.ToInt32(dt.Rows[0][0]) == 1;
                }

                return false;
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

        private RegistroCmv CarregaClima(DateTime dia, int Filial)
        {
            DalConnection dal = new DalConnection(configuration, logger);
            RegistroCmv result = new RegistroCmv();

            try 
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("SELECT c.maxtempc, c.mintempc, c.avgtempc, c.precipmm");
                sb.AppendLine("FROM GS_MVW_FILIAIS f");
                sb.AppendLine("INNER JOIN clima c on c.city = f.cidade");
                sb.AppendLine("                   and c.dia = &DIA");
                sb.AppendLine("WHERE f.codigo = &FIL");

                Dictionary<string, object> param = new Dictionary<string, object> 
                {
                    { "DIA", ((dia.Year-1900)*100+dia.Month)*100+dia.Day },
                    { "FIL", Filial}
                };

                var dt = dal.ExecuteQuery(sb, param);

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach(DataRow dr in dt.Rows)
                    {
                        foreach(DataColumn dc in dt.Columns)
                        {
                            switch (dc.ColumnName.ToUpper())
                            {
                                case "MAXTEMPC": result.MaxTempC = dr[dc] != DBNull.Value ? (float)Convert.ToDouble(dr[dc]) : 0; break;
                                case "MINTEMPC": result.MinTempC = dr[dc] != DBNull.Value ? (float)Convert.ToDouble(dr[dc]) : 0; break;
                                case "AVGTEMPC": result.AvgTempC = dr[dc] != DBNull.Value ? (float)Convert.ToDouble(dr[dc]) : 0; break;
                                case "PRECIPMM": result.PrecipMm = dr[dc] != DBNull.Value ? (float)Convert.ToDouble(dr[dc]) : 0; break;
                                default: break;
                            }
                        }
                    }
                }

                return result;
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

        private List<RegistroCmv> EfetuaPrevisao(DateTime dtIni, DateTime dtFim, RegistroModelo modelo)
        {
            ConsomeModel ml = new ConsomeModel();
            TransformData transformData = new TransformData();
            List<ModelInput> modelInputs = new List<ModelInput>();
            List<RegistroCmv> registroCmvs = new List<RegistroCmv>();
            var dtAtual = dtIni;

            do
            {
                registroCmvs.Add(new RegistroCmv { Dia = dtAtual});
                dtAtual = dtAtual.AddDays(1);
                
            } while (dtAtual <= dtFim);

            foreach (var registro in registroCmvs)
            {
                registro.Feriado = VerificaFeriado(registro.Dia, modelo.Filial);  
                var clima = CarregaClima(registro.Dia, modelo.Filial);  
                
                if (clima != null)
                {
                    registro.MaxTempC = clima.MaxTempC;
                    registro.MinTempC = clima.MinTempC;
                    registro.AvgTempC = clima.AvgTempC;
                    registro.PrecipMm = clima.PrecipMm;
                }                
            }

            modelInputs = transformData.TransformaDados(registroCmvs);
            int atual = 0;
            foreach (var item in modelInputs)
            {
                var result = ml.Predict(item, modelo);
                if (result.Score < 0)
                {
                    result.Score = 0;
                }

                registroCmvs[atual].Valor = result.Score;
                atual++;
            }

            return registroCmvs;
        }

        private void GravaPrevisoes(List<RegistroCmv> previsoes, RegistroModelo modelo)
        {
            DalConnection dal = new DalConnection(configuration, logger);

            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("DELETE FROM DSDH_PREVISAO_CMV");
                sb.AppendLine("WHERE FILIAL = &FIL");
                sb.AppendLine("AND SECAO = &SEC");
                sb.AppendLine("AND GRUPO = &GRP");
                sb.AppendLine("AND SUBGRUPO = &SGRP");

                Dictionary<string, object> param = new Dictionary<string, object>
                {
                    { "FIL", modelo.Filial },
                    { "SEC", modelo.Secao },
                    { "GRP", modelo.Grupo },
                    { "SGRP", modelo.SubGrupo }
                };

                dal.ExecuteNonQuery(sb, param);

                sb = new StringBuilder();
                sb.AppendLine("INSERT INTO DSDH_PREVISAO_CMV (");
                sb.AppendLine("DIA, FILIAL, SECAO, GRUPO, SUBGRUPO, CMV )");
                sb.AppendLine("VALUES (&DIA, &FIL, &SEC, &GRP, &SGRP, &CMV)");

                foreach (var previsao in previsoes)
                {
                    param = new Dictionary<string, object>
                    {
                        { "DIA", ((previsao.Dia.Year-1900)*100+previsao.Dia.Month)*100+previsao.Dia.Day },
                        { "FIL", modelo.Filial },
                        { "SEC", modelo.Secao },
                        { "GRP", modelo.Grupo },
                        { "SGRP", modelo.SubGrupo },
                        { "CMV", Math.Round(previsao.Valor, 2) }
                    };

                    dal.ExecuteNonQuery(sb, param);
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

        public async Task EfetuaPrevisao(DateTime dtIni, DateTime dtFim)
        {
            try
            {
                var lstModelos = CarregaModelos();

                if (lstModelos != null && lstModelos.Any())
                {
                    var qryFil = from p in lstModelos
                                group p by p.Filial into g
                                select new RegistroModelo
                                {
                                    Filial = g.Key
                                };

                    List<Task> lstTasks = new List<Task>();

                    foreach (var filial in qryFil)
                    {
                        int codigo = filial.Filial;

                        var tsk = Task.Run(()=>{
                            var filialModels = lstModelos.Where(m=>m.Filial == codigo).ToList();
                            foreach (var modelo in filialModels)
                            {
                                try
                                {                        
                                    logger.LogInformation($"Inicio previsao Filial: {modelo.Filial} Categoria {modelo.Secao}/{modelo.Grupo}/{modelo.SubGrupo}");
                                    var previsoes = EfetuaPrevisao(dtIni, dtFim, modelo);
                                    logger.LogInformation($"Fim previsao Filial: {modelo.Filial} Categoria {modelo.Secao}/{modelo.Grupo}/{modelo.SubGrupo}");
                                    logger.LogInformation($"Inicio grava previsao Filial: {modelo.Filial} Categoria {modelo.Secao}/{modelo.Grupo}/{modelo.SubGrupo}");
                                    GravaPrevisoes(previsoes, modelo);
                                    logger.LogInformation($"Fim grava previsao Filial: {modelo.Filial} Categoria {modelo.Secao}/{modelo.Grupo}/{modelo.SubGrupo}");
                                }
                                catch (Exception ex)
                                {
                                    logger.LogError(ex, $"Erro na previsao Filial: {modelo.Filial} Categoria {modelo.Secao}/{modelo.Grupo}/{modelo.SubGrupo}");
                                }
                            }
                            return Task.CompletedTask;
                        });

                        lstTasks.Add(tsk);
                    }

                    if (lstTasks.Count > 0)
                        await Task.WhenAll(lstTasks);
                }
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "Erro ao gerar previsoes");
            }
        }

        public RegistroModelo CarregaModeloMetrica()
        {
            DalConnection dal = new DalConnection(configuration, logger);

            try
            {
                RegistroModelo registro = new RegistroModelo();
                StringBuilder sb = new StringBuilder();

                sb.AppendLine("SELECT count(distinct FILIAL) filial, ");
                sb.AppendLine("       count(1) MODELOS,              ");
                sb.AppendLine("       round(avg(rootmeansquarederror), 2) rmse ");      
                sb.AppendLine("FROM DSDH_MODELOS_CMV                 ");

                var dt = dal.ExecuteQuery(sb, null);

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        foreach (DataColumn dc in dt.Columns)
                        {
                            switch (dc.ColumnName.ToUpper())
                            {
                                case "FILIAL": registro.Filial = Convert.ToInt32(dr[dc]); break;
                                case "MODELOS": registro.Secao = Convert.ToInt32(dr[dc]); break;
                                case "RMSE": registro.RootMeanSquaredError = Convert.ToSingle(dr[dc]); break;
                                default:
                                    break;
                            }
                        }
                    }
                }

                return registro;
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

        public List<RegistroCmv> CarregaPrevisao(DateTime dtIni, DateTime dtFim, int[] Filiais, int[] Categorias)
        {
            DalConnection dal = new DalConnection(configuration, logger);

            try
            {
                StringBuilder sb = new StringBuilder();                
                sb.AppendLine("select rms7to_date(p.dia) dia,");
                sb.AppendLine("       sum(p.cmv) previsao,");
                sb.AppendLine("       sum((r.vda_cmv)) cmv_real");
                sb.AppendLine("from dsdh_previsao_cmv p");
                sb.AppendLine("left join gs_agg_coml_sgrp_dia r on r.dia = p.dia");
                sb.AppendLine("                                and r.filial = p.filial");
                sb.AppendLine("                                and r.secao = p.secao");
                sb.AppendLine("                                and r.grp = p.grupo");
                sb.AppendLine("                                and r.sgrp = p.subgrupo");
                sb.AppendLine("where p.dia between &DTINI and &DTFIM");

                if (Filiais != null && Filiais.Length > 0)
                {
                    sb.AppendLine("and p.filial in (");

                    for (int i = 0; i < Filiais.Length; i++)
                    {
                        sb.Append($"{Filiais[i]}");
                        if (i+1<Filiais.Length)
                        {
                            sb.Append(",");
                        }
                    }

                    sb.AppendLine(")");
                }

                if (Categorias != null && Categorias.Length > 0)
                {
                    sb.AppendLine("and (p.secao, p.grupo, p.subgrupo ) in (");

                    for (int i = 0; i < Categorias.Length; i++)
                    {
                        int secao = 0;
                        int grupo = 0;
                        int subgrupo = 0;

                        secao = (int)Math.Truncate((decimal)Categorias[i] / 1000000);
                        grupo = (int)Math.Truncate(Math.Truncate((decimal)Categorias[i] / 1000) % 1000);
                        subgrupo = (int)Math.Truncate((decimal)Categorias[i] % 1000);

                        sb.AppendLine($"select {secao} s, {grupo} g, {subgrupo} sg from dual");

                        if (i+1<Categorias.Length)
                        {
                            sb.AppendLine("UNION ALL");
                        }
                    }

                    sb.AppendLine(")");
                }

                sb.AppendLine("group by p.dia");
                sb.AppendLine("order by p.dia");

                Dictionary<string, object> param = new Dictionary<string, object>
                {
                    { "DTINI", ((dtIni.Year-1900)*100+dtIni.Month)*100+dtIni.Day },
                    { "DTFIM", ((dtFim.Year-1900)*100+dtFim.Month)*100+dtFim.Day },
                };

                var dt = dal.ExecuteQuery(sb, param);

                List<RegistroCmv> registroCmvs = null;

                if (dt != null && dt.Rows.Count > 0)
                {
                    registroCmvs = new List<RegistroCmv>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        RegistroCmv registroCmv = new RegistroCmv();

                        foreach (DataColumn dc in dt.Columns)
                        {
                            switch (dc.ColumnName.ToUpper())
                            {
                                case "DIA": registroCmv.Dia = Convert.ToDateTime(dr[dc]); break;
                                case "PREVISAO": registroCmv.Previsao = Convert.ToSingle(dr[dc]); break;
                                case "CMV_REAL":
                                    registroCmv.ValorIsNull = dr[dc] == DBNull.Value;
                                    registroCmv.Valor = dr[dc] != DBNull.Value ? Convert.ToSingle(dr[dc]) : 0;
                                    break;
                                default:
                                    break;
                            }
                        }

                        registroCmvs.Add(registroCmv);
                    }
                }

                return registroCmvs;
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

        public List<RegistroCmv> CarregaPrevisaoDetalhada(DateTime dtIni, DateTime dtFim, int[] Filiais, int[] Categorias)
        {
            DalConnection dal = new DalConnection(configuration, logger);

            try
            {
                StringBuilder sb = new StringBuilder();                
                sb.AppendLine("select p.secao, p.grupo, p.subgrupo,");
                sb.AppendLine("       sum(p.cmv) previsao,");
                sb.AppendLine("       sum((r.vda_cmv)) cmv_real");
                sb.AppendLine("from dsdh_previsao_cmv p");
                sb.AppendLine("inner join gs_agg_coml_sgrp_dia r on r.dia = p.dia");
                sb.AppendLine("                                and r.filial = p.filial");
                sb.AppendLine("                                and r.secao = p.secao");
                sb.AppendLine("                                and r.grp = p.grupo");
                sb.AppendLine("                                and r.sgrp = p.subgrupo");
                sb.AppendLine("where p.dia between &DTINI and &DTFIM");
                sb.AppendLine("and r.vda_cmv > 0");

                if (Filiais != null && Filiais.Length > 0)
                {
                    sb.AppendLine("and p.filial in (");

                    for (int i = 0; i < Filiais.Length; i++)
                    {
                        sb.Append($"{Filiais[i]}");
                        if (i+1<Filiais.Length)
                        {
                            sb.Append(",");
                        }
                    }

                    sb.AppendLine(")");
                }

                if (Categorias != null && Categorias.Length > 0)
                {
                    sb.AppendLine("and (p.secao, p.grupo, p.subgrupo ) in (");

                    for (int i = 0; i < Categorias.Length; i++)
                    {
                        int secao = 0;
                        int grupo = 0;
                        int subgrupo = 0;

                        secao = (int)Math.Truncate((decimal)Categorias[i] / 1000000);
                        grupo = (int)Math.Truncate(Math.Truncate((decimal)Categorias[i] / 1000) % 1000);
                        subgrupo = (int)Math.Truncate((decimal)Categorias[i] % 1000);

                        sb.AppendLine($"select {secao} s, {grupo} g, {subgrupo} sg from dual");

                        if (i+1<Categorias.Length)
                        {
                            sb.AppendLine("UNION ALL");
                        }
                    }

                    sb.AppendLine(")");
                }

                sb.AppendLine("group by p.secao, p.grupo, p.subgrupo");
                sb.AppendLine("order by p.secao, p.grupo, p.subgrupo");

                Dictionary<string, object> param = new Dictionary<string, object>
                {
                    { "DTINI", ((dtIni.Year-1900)*100+dtIni.Month)*100+dtIni.Day },
                    { "DTFIM", ((dtFim.Year-1900)*100+dtFim.Month)*100+dtFim.Day },
                };

                var dt = dal.ExecuteQuery(sb, param);

                List<RegistroCmv> registroCmvs = null;

                if (dt != null && dt.Rows.Count > 0)
                {
                    registroCmvs = new List<RegistroCmv>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        RegistroCmv registroCmv = new RegistroCmv();

                        foreach (DataColumn dc in dt.Columns)
                        {
                            switch (dc.ColumnName.ToUpper())
                            {
                                //case "FILIAL": registroCmv.Filial = Convert.ToInt32(dr[dc]); break;
                                case "SECAO": registroCmv.Secao = Convert.ToInt32(dr[dc]); break;
                                case "GRUPO": registroCmv.Grupo = Convert.ToInt32(dr[dc]); break;
                                case "SUBGRUPO": registroCmv.SubGrupo = Convert.ToInt32(dr[dc]); break;
                                case "PREVISAO": registroCmv.Previsao = Convert.ToSingle(dr[dc]); break;
                                case "CMV_REAL":
                                    registroCmv.ValorIsNull = dr[dc] == DBNull.Value;
                                    registroCmv.Valor = dr[dc] != DBNull.Value ? Convert.ToSingle(dr[dc]) : 0;
                                    break;
                                default:
                                    break;
                            }
                        }

                        registroCmvs.Add(registroCmv);
                    }
                }

                return registroCmvs;
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
    }
}
