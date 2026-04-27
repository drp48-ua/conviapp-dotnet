using System;
using System.Data;
using System.Data.SQLite;
using System.Collections.Generic;
using ConviAppWeb.Models;

namespace ConviAppWeb.DataAccess
{
    public class CADContrato
    {
        private string constring => DbConfig.ConnectionString;

        // CREATE — método desconectado
        public bool CrearContrato(ENContrato en)
        {
            bool creado = false;
            DataSet bdVirtual = new DataSet();
            SQLiteConnection c = new SQLiteConnection(constring);
            try
            {
                SQLiteDataAdapter da = new SQLiteDataAdapter("SELECT * FROM Contrato LIMIT 0", c);
                da.Fill(bdVirtual, "contrato");
                DataTable t = bdVirtual.Tables["contrato"];
                DataRow nueva = t.NewRow();
                nueva["type"]            = en.Type ?? (object)DBNull.Value;
                nueva["start_date"]      = en.StartDate.ToString("o");
                nueva["end_date"]        = en.EndDate.ToString("o");
                nueva["monthly_rent"]    = en.MonthlyRent;
                nueva["deposit_amount"]  = en.DepositAmount;
                nueva["status"]          = en.Status ?? "activo";
                nueva["notes"]           = en.Notes ?? (object)DBNull.Value;
                nueva["commission_rate"] = en.CommissionRate;
                nueva["property_id"]     = en.PropertyId;
                nueva["user_id"]         = en.UserId;
                t.Rows.Add(nueva);
                SQLiteCommandBuilder cb = new SQLiteCommandBuilder(da);
                da.Update(bdVirtual, "contrato");
                creado = true;
            }
            catch (Exception) { creado = false; }
            finally { c.Close(); }
            return creado;
        }

        // READ por id — método conectado
        public ENContrato LeerContrato(int id)
        {
            ENContrato en = null;
            SQLiteConnection c = new SQLiteConnection(constring);
            try
            {
                c.Open();
                SQLiteCommand com = new SQLiteCommand("SELECT * FROM Contrato WHERE id=@id", c);
                com.Parameters.AddWithValue("@id", id);
                SQLiteDataReader dr = com.ExecuteReader();
                if (dr.Read()) en = MapRow(dr);
                dr.Close();
            }
            catch (Exception) { en = null; }
            finally { c.Close(); }
            return en;
        }

        // READ ALL — método conectado
        public List<ENContrato> ListarTodos(int? userId = null)
        {
            var lista = new List<ENContrato>();
            SQLiteConnection c = new SQLiteConnection(constring);
            try
            {
                c.Open();
                var sql = userId.HasValue ? "SELECT * FROM Contrato WHERE user_id=@u ORDER BY start_date DESC" : "SELECT * FROM Contrato ORDER BY start_date DESC";
                SQLiteCommand com = new SQLiteCommand(sql, c);
                if (userId.HasValue) com.Parameters.AddWithValue("@u", userId.Value);
                SQLiteDataReader dr = com.ExecuteReader();
                while (dr.Read()) lista.Add(MapRow(dr));
                dr.Close();
            }
            catch (Exception) { lista = new List<ENContrato>(); }
            finally { c.Close(); }
            return lista;
        }

        // UPDATE — método desconectado
        public bool ActualizarContrato(ENContrato en)
        {
            bool ok = false;
            DataSet bdVirtual = new DataSet();
            SQLiteConnection c = new SQLiteConnection(constring);
            try
            {
                SQLiteDataAdapter da = new SQLiteDataAdapter($"SELECT * FROM Contrato WHERE id={en.Id}", c);
                da.Fill(bdVirtual, "contrato");
                DataTable t = bdVirtual.Tables["contrato"];
                DataRow[] filas = t.Select("id=" + en.Id);
                if (filas.Length > 0)
                {
                    filas[0]["status"] = en.Status ?? "activo";
                    filas[0]["notes"]  = en.Notes ?? (object)DBNull.Value;
                    SQLiteCommandBuilder cb = new SQLiteCommandBuilder(da);
                    da.Update(bdVirtual, "contrato");
                    ok = true;
                }
            }
            catch (Exception) { ok = false; }
            finally { c.Close(); }
            return ok;
        }

        // DELETE — método desconectado
        public bool BorrarContrato(ENContrato en)
        {
            bool borrado = false;
            DataSet bdVirtual = new DataSet();
            SQLiteConnection c = new SQLiteConnection(constring);
            try
            {
                SQLiteDataAdapter da = new SQLiteDataAdapter($"SELECT * FROM Contrato WHERE id={en.Id}", c);
                da.Fill(bdVirtual, "contrato");
                DataTable t = bdVirtual.Tables["contrato"];
                DataRow[] filas = t.Select("id=" + en.Id);
                if (filas.Length > 0) { filas[0].Delete(); SQLiteCommandBuilder cb = new SQLiteCommandBuilder(da); da.Update(bdVirtual, "contrato"); borrado = true; }
            }
            catch (Exception) { borrado = false; }
            finally { c.Close(); }
            return borrado;
        }

        private ENContrato MapRow(SQLiteDataReader dr) => new ENContrato
        {
            Id             = Convert.ToInt32(dr["id"]),
            Type           = dr["type"] != DBNull.Value ? dr["type"].ToString() : null,
            StartDate      = dr["start_date"] != DBNull.Value ? Convert.ToDateTime(dr["start_date"]) : DateTime.Now,
            EndDate        = dr["end_date"] != DBNull.Value ? Convert.ToDateTime(dr["end_date"]) : DateTime.Now,
            MonthlyRent    = dr["monthly_rent"] != DBNull.Value ? Convert.ToDecimal(dr["monthly_rent"]) : 0,
            DepositAmount  = dr["deposit_amount"] != DBNull.Value ? Convert.ToDecimal(dr["deposit_amount"]) : 0,
            Status         = dr["status"] != DBNull.Value ? dr["status"].ToString() : null,
            Notes          = dr["notes"] != DBNull.Value ? dr["notes"].ToString() : null,
            CommissionRate = dr["commission_rate"] != DBNull.Value ? Convert.ToDecimal(dr["commission_rate"]) : 0,
            PropertyId     = dr["property_id"] != DBNull.Value ? Convert.ToInt32(dr["property_id"]) : 0,
            UserId         = dr["user_id"] != DBNull.Value ? Convert.ToInt32(dr["user_id"]) : 0,
        };
    }
}
