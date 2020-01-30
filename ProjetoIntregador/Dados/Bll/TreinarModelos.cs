using Microsoft.Extensions.Configuration;
using ProjetoIntregador.Dados.Dal;
using ProjetoIntregador.Dados.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.Data;
using ProjetoIntregador.ML.Modelo;

namespace ProjetoIntregador.Dados.Bll
{
    public class TreinarModelos
    {
        readonly IConfiguration configuration;
        public TreinarModelos(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public List<RegistroFilial> ListarFiliais()
        {
            DalConnection dal = new DalConnection(configuration);

            try
            {
                List<RegistroFilial> registroFilials = new List<RegistroFilial>();
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("SELECT codigo FILIAL,");
                sb.AppendLine("'LOJA '||codigo NOME");
                sb.AppendLine("FROM gs_mvw_filiais");
                sb.AppendLine("WHERE tipofilial = 'L'");
                sb.AppendLine("AND natureza = 'LS'");
               // sb.AppendLine("AND codigo = 8"); // TODO: REMOVER EM PRODUCAO ESTA TRAVADO APENAS UMA FILIAL
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
            DalConnection dal = new DalConnection(configuration);

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
            DalConnection dal = new DalConnection(configuration);

            try
            {
                List<RegistroCmv> registroCmvs = new List<RegistroCmv>();

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("select RMS7TO_DATE(g.dia) DIA,");
                sb.AppendLine("       case when df.dia_meta is null");
                sb.AppendLine("              and d.dia_meta is null then 0 else 1 end feriado,");
                sb.AppendLine("       G.VDA_CMV VALOR");
                sb.AppendLine("from gs_agg_coml_sgrp_dia g");
                sb.AppendLine("left join gs_meta_diasatipico_filial df on df.filial = g.filial");
                sb.AppendLine("                                        and df.dia_meta = rms7to_date(g.dia)");
                sb.AppendLine("left join gs_meta_diasatipico d on d.dia_meta = rms7to_date(g.dia)");
                sb.AppendLine("where g.filial = &FILIAL");
                sb.AppendLine("and g.secao = &SECAO");
                sb.AppendLine("and g.grp = &GRUPO");
                sb.AppendLine("and g.sgrp = &SUBGRUPO");
                sb.AppendLine("and g.dia between 1150101 and 1190831"); // TODO: REMOVER ESSE FILTRO DE DATA EM PRODUÇÃO
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
                                default:
                                    break;
                            }
                        }

                        registroCmvs.Add(registro);
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

        private void GravarModelo(RegistroModelo registroModelo)
        {
            DalConnection dal = new DalConnection(configuration);

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

            var qrySecao = from p in categorias
                           group p by p.Secao into g
                           select new RegistroCategoria
                           {
                               Secao = g.Key
                           };
            var secoes = qrySecao.ToList();
            TransformData transformData = new TransformData();
            ModelBuilder modelBuilder = new ModelBuilder();
            List<Task> lstTsks = new List<Task>();

            foreach (var filial in filiais)
            {
                foreach (var secao in secoes)
                {    
                    var qryCategoria = from p in categorias
                                           where p.Secao == secao.Secao
                                           select p;
                    var categoriaSecao = qryCategoria.ToList();                                       
                    var tsk = Task.Run(() =>
                    {                        
                        foreach (var categoria in categoriaSecao)
                        {
                            try
                            {
                                var historico = ListarHistorico(filial.Filial, categoria.Secao, categoria.Grupo, categoria.SubGrupo);

                                if (historico != null && historico.Count > 365 && historico.Sum(m => m.Valor) > 0)
                                {                                
                                    var transformedData = transformData.TransformaDados(historico);

                                    var modelo = modelBuilder.CreateModel(transformedData);

                                    modelo.Filial = filial.Filial;
                                    modelo.Secao = categoria.Secao;
                                    modelo.Grupo = categoria.Grupo;
                                    modelo.SubGrupo = categoria.SubGrupo;
                                    GravarModelo(modelo);                                
                                }
                                historico = null;
                            }
                            catch (Exception ex) 
                            { 
                                //throw ex;
                                ex = null;
                            }
                        }
                    });

                    lstTsks.Add(tsk);
                }
            }

            await Task.WhenAll(lstTsks);
        }
    }
}
