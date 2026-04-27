using System;
using System.Data;
using System.Data.SQLite;
using System.Collections.Generic;
using ConviAppWeb.Models;

namespace ConviAppWeb.DataAccess
{
    public class CADReserva
    {
        private string constring => DbConfig.ConnectionString;

        // CREATE — método desconectado
        public bool CrearReserva(ENReserva en)
        {
            bool creado = false;
            DataSet bdVirtual = new DataSet();
            SQLiteConnection c = new SQLiteConnection(constring);
            try
            {
                SQLiteDataAdapter da = new SQLiteDataAdapter("SELECT * FROM Reserva LIMIT 0", c);
                da.Fill(bdVirtual, "reserva");
                DataTable t = bdVirtual.Tables["reserva"];
                DataRow nueva = t.NewRow();
                nueva["fecha_inicio"]   = en.FechaInicio.ToString("o");
                nueva["fecha_fin"]      = en.FechaFin.ToString("o");
                nueva["estado"]         = en.Estado ?? "pendiente";
                nueva["motivo"]         = en.Motivo ?? (object)DBNull.Value;
                nueva["usuario_id"]     = en.UsuarioId;
                nueva["zona_comun_id"]  = en.ZonaComunId;
                t.Rows.Add(nueva);
                SQLiteCommandBuilder cb = new SQLiteCommandBuilder(da);
                da.Update(bdVirtual, "reserva");
                creado = true;
            }
            catch (Exception) { creado = false; }
            finally { c.Close(); }
            return creado;
        }

        // READ ALL — método conectado
        public List<ENReserva> ListarTodas(int? usuarioId = null)
        {
            var lista = new List<ENReserva>();
            SQLiteConnection c = new SQLiteConnection(constring);
            try
            {
                c.Open();
                var sql = usuarioId.HasValue ? "SELECT * FROM Reserva WHERE usuario_id=@u ORDER BY fecha_inicio ASC" : "SELECT * FROM Reserva ORDER BY fecha_inicio ASC";
                SQLiteCommand com = new SQLiteCommand(sql, c);
                if (usuarioId.HasValue) com.Parameters.AddWithValue("@u", usuarioId.Value);
                SQLiteDataReader dr = com.ExecuteReader();
                while (dr.Read()) lista.Add(MapRow(dr));
                dr.Close();
            }
            catch (Exception) { lista = new List<ENReserva>(); }
            finally { c.Close(); }
            return lista;
        }

        // DELETE — método desconectado
        public bool CancelarReserva(ENReserva en)
        {
            bool borrado = false;
            DataSet bdVirtual = new DataSet();
            SQLiteConnection c = new SQLiteConnection(constring);
            try
            {
                SQLiteDataAdapter da = new SQLiteDataAdapter($"SELECT * FROM Reserva WHERE id={en.Id}", c);
                da.Fill(bdVirtual, "reserva");
                DataTable t = bdVirtual.Tables["reserva"];
                DataRow[] filas = t.Select("id=" + en.Id);
                if (filas.Length > 0)
                { filas[0].Delete(); SQLiteCommandBuilder cb = new SQLiteCommandBuilder(da); da.Update(bdVirtual, "reserva"); borrado = true; }
            }
            catch (Exception) { borrado = false; }
            finally { c.Close(); }
            return borrado;
        }

        private ENReserva MapRow(SQLiteDataReader dr) => new ENReserva
        {
            Id          = Convert.ToInt32(dr["id"]),
            FechaInicio = dr["fecha_inicio"] != DBNull.Value ? Convert.ToDateTime(dr["fecha_inicio"]) : DateTime.Now,
            FechaFin    = dr["fecha_fin"] != DBNull.Value ? Convert.ToDateTime(dr["fecha_fin"]) : DateTime.Now,
            Estado      = dr["estado"] != DBNull.Value ? dr["estado"].ToString() : "pendiente",
            Motivo      = dr["motivo"] != DBNull.Value ? dr["motivo"].ToString() : null,
            UsuarioId   = dr["usuario_id"] != DBNull.Value ? Convert.ToInt32(dr["usuario_id"]) : 0,
            ZonaComunId = dr["zona_comun_id"] != DBNull.Value ? Convert.ToInt32(dr["zona_comun_id"]) : 0,
        };
    }
}
