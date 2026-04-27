using System;
using System.Data;
using System.Data.SQLite;
using System.Collections.Generic;
using ConviAppWeb.Models;

namespace ConviAppWeb.DataAccess
{
    public class CADPiso
    {
        private string constring => DbConfig.ConnectionString;

        // CREATE — método desconectado
        public bool CrearPiso(ENPiso en)
        {
            bool creado = false;
            DataSet bdVirtual = new DataSet();
            SQLiteConnection c = new SQLiteConnection(constring);
            try
            {
                SQLiteDataAdapter da = new SQLiteDataAdapter("SELECT * FROM Piso LIMIT 0", c);
                da.Fill(bdVirtual, "piso");
                DataTable t = bdVirtual.Tables["piso"];
                DataRow nueva = t.NewRow();
                nueva["direccion"]          = en.Direccion ?? (object)DBNull.Value;
                nueva["ciudad"]             = en.Ciudad ?? (object)DBNull.Value;
                nueva["codigo_postal"]      = en.CodigoPostal ?? (object)DBNull.Value;
                nueva["numero_habitaciones"]= en.NumeroHabitaciones;
                nueva["numero_banos"]       = en.NumeroBanos;
                nueva["precio_total"]       = en.PrecioTotal;
                nueva["descripcion"]        = en.Descripcion ?? (object)DBNull.Value;
                nueva["disponible"]         = en.Disponible ? 1 : 0;
                t.Rows.Add(nueva);
                SQLiteCommandBuilder cb = new SQLiteCommandBuilder(da);
                da.Update(bdVirtual, "piso");
                creado = true;
            }
            catch (Exception) { creado = false; }
            finally { c.Close(); }
            return creado;
        }

        // READ por id — método conectado
        public ENPiso LeerPiso(int id)
        {
            ENPiso en = null;
            SQLiteConnection c = new SQLiteConnection(constring);
            try
            {
                c.Open();
                SQLiteCommand com = new SQLiteCommand("SELECT * FROM Piso WHERE id=@id", c);
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
        public List<ENPiso> ListarTodos()
        {
            var lista = new List<ENPiso>();
            SQLiteConnection c = new SQLiteConnection(constring);
            try
            {
                c.Open();
                SQLiteCommand com = new SQLiteCommand("SELECT * FROM Piso ORDER BY id ASC", c);
                SQLiteDataReader dr = com.ExecuteReader();
                while (dr.Read()) lista.Add(MapRow(dr));
                dr.Close();
            }
            catch (Exception) { lista = new List<ENPiso>(); }
            finally { c.Close(); }
            return lista;
        }

        private ENPiso MapRow(SQLiteDataReader dr) => new ENPiso
        {
            Id                = Convert.ToInt32(dr["id"]),
            Direccion         = dr["direccion"] != DBNull.Value ? dr["direccion"].ToString() : null,
            Ciudad            = dr["ciudad"] != DBNull.Value ? dr["ciudad"].ToString() : null,
            CodigoPostal      = dr["codigo_postal"] != DBNull.Value ? dr["codigo_postal"].ToString() : null,
            NumeroHabitaciones= dr["numero_habitaciones"] != DBNull.Value ? Convert.ToInt32(dr["numero_habitaciones"]) : 0,
            NumeroBanos       = dr["numero_banos"] != DBNull.Value ? Convert.ToInt32(dr["numero_banos"]) : 0,
            PrecioTotal       = dr["precio_total"] != DBNull.Value ? Convert.ToDecimal(dr["precio_total"]) : 0,
            Descripcion       = dr["descripcion"] != DBNull.Value ? dr["descripcion"].ToString() : null,
            Disponible        = dr["disponible"] != DBNull.Value && Convert.ToInt32(dr["disponible"]) == 1,
        };
    }
}
