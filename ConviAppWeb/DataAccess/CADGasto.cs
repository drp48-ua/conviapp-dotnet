using System;
using System.Data;
using System.Data.SQLite;
using System.Collections.Generic;
using ConviAppWeb.Models;

namespace ConviAppWeb.DataAccess
{
    public class CADGasto
    {
        private string constring => DbConfig.ConnectionString;

        // CREATE — método desconectado
        public bool CrearGasto(ENGasto en)
        {
            bool creado = false;
            DataSet bdVirtual = new DataSet();
            SQLiteConnection c = new SQLiteConnection(constring);
            try
            {
                SQLiteDataAdapter da = new SQLiteDataAdapter("SELECT * FROM Gasto LIMIT 0", c);
                da.Fill(bdVirtual, "gasto");
                DataTable t = bdVirtual.Tables["gasto"];
                DataRow nueva = t.NewRow();
                nueva["concepto"]          = en.Concepto ?? (object)DBNull.Value;
                nueva["importe"]           = en.Importe;
                nueva["fecha"]             = en.Fecha.ToString("o");
                nueva["pagado"]            = en.Pagado ? 1 : 0;
                nueva["descripcion"]       = en.Descripcion ?? (object)DBNull.Value;
                nueva["registrado_por_id"] = en.RegistradoPorId;
                nueva["piso_id"]           = en.PisoId.HasValue ? (object)en.PisoId.Value : DBNull.Value;
                t.Rows.Add(nueva);
                SQLiteCommandBuilder cb = new SQLiteCommandBuilder(da);
                da.Update(bdVirtual, "gasto");
                creado = true;
            }
            catch (Exception) { creado = false; }
            finally { c.Close(); }
            return creado;
        }

        // READ ALL — método conectado
        public List<ENGasto> ListarTodos(int? pisoId = null)
        {
            var lista = new List<ENGasto>();
            SQLiteConnection c = new SQLiteConnection(constring);
            try
            {
                c.Open();
                var sql = pisoId.HasValue ? "SELECT * FROM Gasto WHERE piso_id=@p ORDER BY fecha DESC" : "SELECT * FROM Gasto ORDER BY fecha DESC";
                SQLiteCommand com = new SQLiteCommand(sql, c);
                if (pisoId.HasValue) com.Parameters.AddWithValue("@p", pisoId.Value);
                SQLiteDataReader dr = com.ExecuteReader();
                while (dr.Read()) lista.Add(MapRow(dr));
                dr.Close();
            }
            catch (Exception) { lista = new List<ENGasto>(); }
            finally { c.Close(); }
            return lista;
        }

        // DELETE — método desconectado
        public bool BorrarGasto(ENGasto en)
        {
            bool borrado = false;
            DataSet bdVirtual = new DataSet();
            SQLiteConnection c = new SQLiteConnection(constring);
            try
            {
                SQLiteDataAdapter da = new SQLiteDataAdapter($"SELECT * FROM Gasto WHERE id={en.Id}", c);
                da.Fill(bdVirtual, "gasto");
                DataTable t = bdVirtual.Tables["gasto"];
                DataRow[] filas = t.Select("id=" + en.Id);
                if (filas.Length > 0)
                { filas[0].Delete(); SQLiteCommandBuilder cb = new SQLiteCommandBuilder(da); da.Update(bdVirtual, "gasto"); borrado = true; }
            }
            catch (Exception) { borrado = false; }
            finally { c.Close(); }
            return borrado;
        }

        private ENGasto MapRow(SQLiteDataReader dr) => new ENGasto
        {
            Id              = Convert.ToInt32(dr["id"]),
            Concepto        = dr["concepto"] != DBNull.Value ? dr["concepto"].ToString() : null,
            Importe         = dr["importe"] != DBNull.Value ? Convert.ToDecimal(dr["importe"]) : 0,
            Fecha           = dr["fecha"] != DBNull.Value ? Convert.ToDateTime(dr["fecha"]) : DateTime.Now,
            Pagado          = dr["pagado"] != DBNull.Value && Convert.ToInt32(dr["pagado"]) == 1,
            Descripcion     = dr["descripcion"] != DBNull.Value ? dr["descripcion"].ToString() : null,
            RegistradoPorId = dr["registrado_por_id"] != DBNull.Value ? Convert.ToInt32(dr["registrado_por_id"]) : 0,
            PisoId          = dr["piso_id"] != DBNull.Value ? (int?)Convert.ToInt32(dr["piso_id"]) : null,
        };
    }
}
