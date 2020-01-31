using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetoIntregador.Dados.Dal
{
    public class DalConnection
    {
        readonly IConfiguration configuration;
        readonly ILogger logger;
        public DalConnection(IConfiguration configuration, ILogger logger)
        {
            this.configuration = configuration;
            this.logger = logger;
        }

        private OracleConnection GetConnection()
        {
            try
            {
                OracleConnection connection = new OracleConnection(configuration.GetConnectionString("GSRetail"));
                connection.Open();
                return connection;
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "Abre Conexao");
                throw ex;
            }
        }

        public void ExecuteNonQuery(StringBuilder sb, Dictionary<string, object> param )
        {
            try
            {
            var con = GetConnection();
            var cmd = con.CreateCommand();

            cmd.BindByName = true;
            cmd.CommandType = System.Data.CommandType.Text;

            cmd.CommandText = sb.ToString().Replace("&", ":");

            if (param != null)
            {
                foreach(var parametro in param)
                {
                    cmd.Parameters.Add(parametro.Key, parametro.Value);
                }
            }

            var transaction = con.BeginTransaction();
            cmd.Transaction = transaction;

            try
            {
                cmd.ExecuteNonQuery();
                transaction.Commit();
            }
            catch (Exception ex) 
            {
                transaction.Rollback();
                throw ex;
            }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Executa Non Query");
                throw ex;
            }
        }

        public DataTable ExecuteQuery(StringBuilder sb, Dictionary<string, object> param)
        {
            try
            {
                var con = GetConnection();
                var cmd = con.CreateCommand();

                cmd.BindByName = true;
                cmd.CommandType = System.Data.CommandType.Text;

                cmd.CommandText = sb.ToString().Replace("&", ":");

                if (param != null)
                {
                    foreach (var parametro in param)
                    {
                        cmd.Parameters.Add(parametro.Key, parametro.Value);
                    }
                }
                OracleDataAdapter dt = new OracleDataAdapter(cmd);

                DataTable result = new DataTable();

                dt.Fill(result);

                con.Close();
                cmd = null;
                con = null;
                dt = null;

                return result;
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "Execute Query");
                throw ex;
            }
        }
    }
}
