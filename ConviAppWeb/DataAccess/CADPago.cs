using System;
using System.Data;
using System.Data.SQLite;
using System.Collections.Generic;
using ConviAppWeb.Models;

namespace ConviAppWeb.DataAccess
{
    public class CADPago
    {
        private string constring => DbConfig.ConnectionString;

        // CREATE — método desconectado
        public bool CrearPago(ENPago en)
        {
            bool creado = false;
            DataSet bdVirtual = new DataSet();
            SQLiteConnection c = new SQLiteConnection(constring);
            try
            {
                SQLiteDataAdapter da = new SQLiteDataAdapter("SELECT * FROM Pago LIMIT 0", c);
                da.Fill(bdVirtual, "pago");
                DataTable t = bdVirtual.Tables["pago"];
                DataRow nueva = t.NewRow();
                nueva["amount"]      = en.Amount;
                nueva["date"]        = en.Date.ToString("o");
                nueva["method"]      = en.Method ?? (object)DBNull.Value;
                nueva["status"]      = en.Status ?? "pagado";
                nueva["concept"]     = en.Concept ?? (object)DBNull.Value;
                nueva["reference"]   = en.Reference ?? (object)DBNull.Value;
                nueva["contrato_id"] = en.ContratoId;
                nueva["user_id"]     = en.UserId;
                t.Rows.Add(nueva);
                SQLiteCommandBuilder cb = new SQLiteCommandBuilder(da);
                da.Update(bdVirtual, "pago");
                creado = true;
            }
            catch (Exception) { creado = false; }
            finally { c.Close(); }
            return creado;
        }

        // READ por id — método conectado
        public ENPago LeerPago(int id)
        {
            ENPago en = null;
            SQLiteConnection c = new SQLiteConnection(constring);
            try
            {
                c.Open();
                SQLiteCommand com = new SQLiteCommand("SELECT * FROM Pago WHERE id=@id", c);
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
        public List<ENPago> ListarTodos(int? contratoId = null)
        {
            var lista = new List<ENPago>();
            SQLiteConnection c = new SQLiteConnection(constring);
            try
            {
                c.Open();
                var sql = contratoId.HasValue ? "SELECT * FROM Pago WHERE contrato_id=@c ORDER BY date DESC" : "SELECT * FROM Pago ORDER BY date DESC";
                SQLiteCommand com = new SQLiteCommand(sql, c);
                if (contratoId.HasValue) com.Parameters.AddWithValue("@c", contratoId.Value);
                SQLiteDataReader dr = com.ExecuteReader();
                while (dr.Read()) lista.Add(MapRow(dr));
                dr.Close();
            }
            catch (Exception) { lista = new List<ENPago>(); }
            finally { c.Close(); }
            return lista;
        }

        // DELETE — método desconectado
        public bool BorrarPago(ENPago en)
        {
            bool borrado = false;
            DataSet bdVirtual = new DataSet();
            SQLiteConnection c = new SQLiteConnection(constring);
            try
            {
                SQLiteDataAdapter da = new SQLiteDataAdapter($"SELECT * FROM Pago WHERE id={en.Id}", c);
                da.Fill(bdVirtual, "pago");
                DataTable t = bdVirtual.Tables["pago"];
                DataRow[] filas = t.Select("id=" + en.Id);
                if (filas.Length > 0) { filas[0].Delete(); SQLiteCommandBuilder cb = new SQLiteCommandBuilder(da); da.Update(bdVirtual, "pago"); borrado = true; }
            }
            catch (Exception) { borrado = false; }
            finally { c.Close(); }
            return borrado;
        }

        private ENPago MapRow(SQLiteDataReader dr) => new ENPago
        {
            Id         = Convert.ToInt32(dr["id"]),
            Amount     = dr["amount"] != DBNull.Value ? Convert.ToDecimal(dr["amount"]) : 0,
            Date       = dr["date"] != DBNull.Value ? Convert.ToDateTime(dr["date"]) : DateTime.Now,
            Method     = dr["method"] != DBNull.Value ? dr["method"].ToString() : null,
            Status     = dr["status"] != DBNull.Value ? dr["status"].ToString() : null,
            Concept    = dr["concept"] != DBNull.Value ? dr["concept"].ToString() : null,
            Reference  = dr["reference"] != DBNull.Value ? dr["reference"].ToString() : null,
            ContratoId = dr["contrato_id"] != DBNull.Value ? Convert.ToInt32(dr["contrato_id"]) : 0,
            UserId     = dr["user_id"] != DBNull.Value ? Convert.ToInt32(dr["user_id"]) : 0,
        };
    }
}
