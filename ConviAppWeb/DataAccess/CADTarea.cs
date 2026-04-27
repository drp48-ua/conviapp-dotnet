using System;
using System.Data;
using System.Data.SQLite;
using System.Collections.Generic;
using ConviAppWeb.Models;

namespace ConviAppWeb.DataAccess
{
    public class CADTarea
    {
        private string constring => DbConfig.ConnectionString;

        // CREATE — método desconectado
        public bool CrearTarea(ENTarea en)
        {
            bool creado = false;
            DataSet bdVirtual = new DataSet();
            SQLiteConnection c = new SQLiteConnection(constring);
            try
            {
                SQLiteDataAdapter da = new SQLiteDataAdapter("SELECT * FROM Tarea LIMIT 0", c);
                da.Fill(bdVirtual, "tarea");
                DataTable t = bdVirtual.Tables["tarea"];
                DataRow nueva = t.NewRow();
                nueva["titulo"]         = en.Titulo ?? (object)DBNull.Value;
                nueva["descripcion"]    = en.Descripcion ?? (object)DBNull.Value;
                nueva["estado"]         = en.Estado ?? "pendiente";
                nueva["prioridad"]      = en.Prioridad ?? "media";
                nueva["fecha_creacion"] = en.FechaCreacion.ToString("o");
                nueva["creada_por_id"]  = en.CreadaPorId;
                nueva["piso_id"]        = en.PisoId.HasValue ? (object)en.PisoId.Value : DBNull.Value;
                t.Rows.Add(nueva);
                SQLiteCommandBuilder cb = new SQLiteCommandBuilder(da);
                da.Update(bdVirtual, "tarea");
                creado = true;
            }
            catch (Exception) { creado = false; }
            finally { c.Close(); }
            return creado;
        }

        // READ ALL — método conectado
        public List<ENTarea> ListarTodas(int? pisoId = null)
        {
            var lista = new List<ENTarea>();
            SQLiteConnection c = new SQLiteConnection(constring);
            try
            {
                c.Open();
                var sql = pisoId.HasValue ? "SELECT * FROM Tarea WHERE piso_id=@p" : "SELECT * FROM Tarea";
                SQLiteCommand com = new SQLiteCommand(sql, c);
                if (pisoId.HasValue) com.Parameters.AddWithValue("@p", pisoId.Value);
                SQLiteDataReader dr = com.ExecuteReader();
                while (dr.Read()) lista.Add(MapRow(dr));
                dr.Close();
            }
            catch (Exception) { lista = new List<ENTarea>(); }
            finally { c.Close(); }
            return lista;
        }

        // UPDATE estado — método conectado
        public bool ToggleEstado(int id)
        {
            bool ok = false;
            SQLiteConnection c = new SQLiteConnection(constring);
            try
            {
                c.Open();
                SQLiteCommand com = new SQLiteCommand(
                    "UPDATE Tarea SET estado = CASE WHEN estado='completada' THEN 'pendiente' ELSE 'completada' END WHERE id=@id", c);
                com.Parameters.AddWithValue("@id", id);
                ok = com.ExecuteNonQuery() > 0;
            }
            catch (Exception) { ok = false; }
            finally { c.Close(); }
            return ok;
        }

        // DELETE — método desconectado
        public bool BorrarTarea(ENTarea en)
        {
            bool borrado = false;
            DataSet bdVirtual = new DataSet();
            SQLiteConnection c = new SQLiteConnection(constring);
            try
            {
                SQLiteDataAdapter da = new SQLiteDataAdapter($"SELECT * FROM Tarea WHERE id={en.Id}", c);
                da.Fill(bdVirtual, "tarea");
                DataTable t = bdVirtual.Tables["tarea"];
                DataRow[] filas = t.Select("id=" + en.Id);
                if (filas.Length > 0)
                { filas[0].Delete(); SQLiteCommandBuilder cb = new SQLiteCommandBuilder(da); da.Update(bdVirtual, "tarea"); borrado = true; }
            }
            catch (Exception) { borrado = false; }
            finally { c.Close(); }
            return borrado;
        }

        private ENTarea MapRow(SQLiteDataReader dr) => new ENTarea
        {
            Id            = Convert.ToInt32(dr["id"]),
            Titulo        = dr["titulo"] != DBNull.Value ? dr["titulo"].ToString() : null,
            Descripcion   = dr["descripcion"] != DBNull.Value ? dr["descripcion"].ToString() : null,
            Estado        = dr["estado"] != DBNull.Value ? dr["estado"].ToString() : "pendiente",
            Prioridad     = dr["prioridad"] != DBNull.Value ? dr["prioridad"].ToString() : "media",
            FechaCreacion = dr["fecha_creacion"] != DBNull.Value ? Convert.ToDateTime(dr["fecha_creacion"]) : DateTime.Now,
            CreadaPorId   = dr["creada_por_id"] != DBNull.Value ? Convert.ToInt32(dr["creada_por_id"]) : 0,
            PisoId        = dr["piso_id"] != DBNull.Value ? (int?)Convert.ToInt32(dr["piso_id"]) : null,
        };
    }
}
