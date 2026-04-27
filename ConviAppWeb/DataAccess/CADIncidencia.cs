using System;
using System.Data;
using System.Data.SQLite;
using System.Collections.Generic;
using ConviAppWeb.Models;

namespace ConviAppWeb.DataAccess
{
    public class CADIncidencia
    {
        private string constring => DbConfig.ConnectionString;

        // CREATE — método desconectado
        public bool CrearIncidencia(ENIncidencia en)
        {
            bool creado = false;
            DataSet bdVirtual = new DataSet();
            SQLiteConnection c = new SQLiteConnection(constring);
            try
            {
                SQLiteDataAdapter da = new SQLiteDataAdapter("SELECT * FROM Incidencia LIMIT 0", c);
                da.Fill(bdVirtual, "incidencia");
                DataTable t = bdVirtual.Tables["incidencia"];
                DataRow nueva = t.NewRow();
                nueva["titulo"]           = en.Titulo ?? (object)DBNull.Value;
                nueva["descripcion"]      = en.Descripcion ?? (object)DBNull.Value;
                nueva["estado"]           = en.Estado ?? "abierta";
                nueva["prioridad"]        = en.Prioridad ?? "media";
                nueva["fecha_reporte"]    = en.FechaReporte.ToString("o");
                nueva["reportada_por_id"] = en.ReportadaPorId;
                nueva["piso_id"]          = en.PisoId.HasValue ? (object)en.PisoId.Value : DBNull.Value;
                t.Rows.Add(nueva);
                SQLiteCommandBuilder cb = new SQLiteCommandBuilder(da);
                da.Update(bdVirtual, "incidencia");
                creado = true;
            }
            catch (Exception) { creado = false; }
            finally { c.Close(); }
            return creado;
        }

        // READ ALL — método conectado
        public List<ENIncidencia> ListarTodas(int? pisoId = null)
        {
            var lista = new List<ENIncidencia>();
            SQLiteConnection c = new SQLiteConnection(constring);
            try
            {
                c.Open();
                var sql = pisoId.HasValue ? "SELECT * FROM Incidencia WHERE piso_id=@p ORDER BY fecha_reporte DESC" : "SELECT * FROM Incidencia ORDER BY fecha_reporte DESC";
                SQLiteCommand com = new SQLiteCommand(sql, c);
                if (pisoId.HasValue) com.Parameters.AddWithValue("@p", pisoId.Value);
                SQLiteDataReader dr = com.ExecuteReader();
                while (dr.Read()) lista.Add(MapRow(dr));
                dr.Close();
            }
            catch (Exception) { lista = new List<ENIncidencia>(); }
            finally { c.Close(); }
            return lista;
        }

        // UPDATE estado — método conectado
        public bool ActualizarEstado(int id, string estado)
        {
            bool ok = false;
            SQLiteConnection c = new SQLiteConnection(constring);
            try
            {
                c.Open();
                SQLiteCommand com = new SQLiteCommand("UPDATE Incidencia SET estado=@e WHERE id=@id", c);
                com.Parameters.AddWithValue("@e", estado);
                com.Parameters.AddWithValue("@id", id);
                ok = com.ExecuteNonQuery() > 0;
            }
            catch (Exception) { ok = false; }
            finally { c.Close(); }
            return ok;
        }

        private ENIncidencia MapRow(SQLiteDataReader dr) => new ENIncidencia
        {
            Id             = Convert.ToInt32(dr["id"]),
            Titulo         = dr["titulo"] != DBNull.Value ? dr["titulo"].ToString() : null,
            Descripcion    = dr["descripcion"] != DBNull.Value ? dr["descripcion"].ToString() : null,
            Estado         = dr["estado"] != DBNull.Value ? dr["estado"].ToString() : "abierta",
            Prioridad      = dr["prioridad"] != DBNull.Value ? dr["prioridad"].ToString() : "media",
            FechaReporte   = dr["fecha_reporte"] != DBNull.Value ? Convert.ToDateTime(dr["fecha_reporte"]) : DateTime.Now,
            ReportadaPorId = dr["reportada_por_id"] != DBNull.Value ? Convert.ToInt32(dr["reportada_por_id"]) : 0,
            PisoId         = dr["piso_id"] != DBNull.Value ? (int?)Convert.ToInt32(dr["piso_id"]) : null,
        };
    }
}
