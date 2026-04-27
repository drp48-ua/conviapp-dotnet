using System;
using System.Data;
using System.Data.SQLite;
using System.Collections.Generic;
using ConviAppWeb.Models;

namespace ConviAppWeb.DataAccess
{
    public class CADMensaje
    {
        private string constring => DbConfig.ConnectionString;

        // CREATE — método desconectado
        public bool CrearMensaje(ENMensaje en)
        {
            bool creado = false;
            DataSet bdVirtual = new DataSet();
            SQLiteConnection c = new SQLiteConnection(constring);
            try
            {
                SQLiteDataAdapter da = new SQLiteDataAdapter("SELECT * FROM Mensaje LIMIT 0", c);
                da.Fill(bdVirtual, "mensaje");
                DataTable t = bdVirtual.Tables["mensaje"];
                DataRow nueva = t.NewRow();
                nueva["contenido"]   = en.Contenido ?? (object)DBNull.Value;
                nueva["fecha_envio"] = en.FechaEnvio.ToString("o");
                nueva["leido"]       = en.Leido ? 1 : 0;
                nueva["emisor_id"]   = en.EmisorId;
                nueva["piso_id"]     = en.PisoId.HasValue ? (object)en.PisoId.Value : DBNull.Value;
                t.Rows.Add(nueva);
                SQLiteCommandBuilder cb = new SQLiteCommandBuilder(da);
                da.Update(bdVirtual, "mensaje");
                creado = true;
            }
            catch (Exception) { creado = false; }
            finally { c.Close(); }
            return creado;
        }

        // READ ALL — método conectado
        public List<ENMensaje> ListarTodos(int? pisoId = null)
        {
            var lista = new List<ENMensaje>();
            SQLiteConnection c = new SQLiteConnection(constring);
            try
            {
                c.Open();
                var sql = pisoId.HasValue ? "SELECT * FROM Mensaje WHERE piso_id=@p ORDER BY fecha_envio ASC" : "SELECT * FROM Mensaje ORDER BY fecha_envio ASC";
                SQLiteCommand com = new SQLiteCommand(sql, c);
                if (pisoId.HasValue) com.Parameters.AddWithValue("@p", pisoId.Value);
                SQLiteDataReader dr = com.ExecuteReader();
                while (dr.Read()) lista.Add(MapRow(dr));
                dr.Close();
            }
            catch (Exception) { lista = new List<ENMensaje>(); }
            finally { c.Close(); }
            return lista;
        }

        private ENMensaje MapRow(SQLiteDataReader dr) => new ENMensaje
        {
            Id         = Convert.ToInt32(dr["id"]),
            Contenido  = dr["contenido"] != DBNull.Value ? dr["contenido"].ToString() : null,
            FechaEnvio = dr["fecha_envio"] != DBNull.Value ? Convert.ToDateTime(dr["fecha_envio"]) : DateTime.Now,
            Leido      = dr["leido"] != DBNull.Value && Convert.ToInt32(dr["leido"]) == 1,
            EmisorId   = dr["emisor_id"] != DBNull.Value ? Convert.ToInt32(dr["emisor_id"]) : 0,
            PisoId     = dr["piso_id"] != DBNull.Value ? (int?)Convert.ToInt32(dr["piso_id"]) : null,
        };
    }
}
