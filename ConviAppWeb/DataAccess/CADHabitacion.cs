using System;
using System.Data;
using System.Data.SQLite;
using System.Collections.Generic;
using ConviAppWeb.Models;

namespace ConviAppWeb.DataAccess
{
    public class CADHabitacion
    {
        private string constring => DbConfig.ConnectionString;

        // CREATE — metodo desconectado
        public bool CrearHabitacion(ENHabitacion en)
        {
            bool creado = false;
            DataSet bdVirtual = new DataSet();
            using (SQLiteConnection c = new SQLiteConnection(constring))
            {
                try
                {
                    SQLiteDataAdapter da = new SQLiteDataAdapter("SELECT * FROM Habitacion LIMIT 0", c);
                    da.Fill(bdVirtual, "habitacion");

                    DataTable t = bdVirtual.Tables["habitacion"];
                    DataRow nueva = t.NewRow();
                    nueva["numero"] = en.Numero ?? (object)DBNull.Value;
                    nueva["precio"] = en.Precio;
                    nueva["metros"] = en.Metros;
                    nueva["disponible"] = en.Disponible ? 1 : 0;
                    nueva["tiene_bano"] = en.TieneBano ? 1 : 0;
                    nueva["descripcion"] = en.Descripcion ?? (object)DBNull.Value;
                    nueva["piso_id"] = en.PisoId;
                    t.Rows.Add(nueva);

                    SQLiteCommandBuilder cb = new SQLiteCommandBuilder(da);
                    da.Update(bdVirtual, "habitacion");
                    creado = true;
                }
                catch (Exception)
                {
                    creado = false;
                }
            }
            return creado;
        }

        // READ — metodo conectado
        public ENHabitacion LeerHabitacion(int id)
        {
            ENHabitacion en = null;
            using (SQLiteConnection c = new SQLiteConnection(constring))
            {
                try
                {
                    c.Open();
                    SQLiteCommand com = new SQLiteCommand("SELECT * FROM Habitacion WHERE id = @id", c);
                    com.Parameters.AddWithValue("@id", id);
                    using (SQLiteDataReader dr = com.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            en = new ENHabitacion();
                            en.Id = dr["id"] != DBNull.Value ? Convert.ToInt32(dr["id"]) : 0;
                            en.Numero = dr["numero"] != DBNull.Value ? dr["numero"].ToString() : null;
                            en.Precio = dr["precio"] != DBNull.Value ? Convert.ToDecimal(dr["precio"]) : 0;
                            en.Metros = dr["metros"] != DBNull.Value ? Convert.ToDouble(dr["metros"]) : 0;
                            en.Disponible = dr["disponible"] != DBNull.Value && Convert.ToInt32(dr["disponible"]) == 1;
                            en.TieneBano = dr["tiene_bano"] != DBNull.Value && Convert.ToInt32(dr["tiene_bano"]) == 1;
                            en.Descripcion = dr["descripcion"] != DBNull.Value ? dr["descripcion"].ToString() : null;
                            en.PisoId = dr["piso_id"] != DBNull.Value ? Convert.ToInt32(dr["piso_id"]) : 0;
                        }
                    }
                }
                catch (Exception)
                {
                    en = null;
                }
            }
            return en;
        }

        // UPDATE — metodo desconectado
        public bool ActualizarHabitacion(ENHabitacion en)
        {
            bool actualizado = false;
            DataSet bdVirtual = new DataSet();
            using (SQLiteConnection c = new SQLiteConnection(constring))
            {
                try
                {
                    SQLiteDataAdapter da = new SQLiteDataAdapter("SELECT * FROM Habitacion", c);
                    da.Fill(bdVirtual, "habitacion");
                    DataTable t = bdVirtual.Tables["habitacion"];
                    DataRow[] filas = t.Select("id = " + en.Id);

                    if (filas.Length > 0)
                    {
                        filas[0]["numero"] = en.Numero ?? (object)DBNull.Value;
                        filas[0]["precio"] = en.Precio;
                        filas[0]["metros"] = en.Metros;
                        filas[0]["disponible"] = en.Disponible ? 1 : 0;
                        filas[0]["tiene_bano"] = en.TieneBano ? 1 : 0;
                        filas[0]["descripcion"] = en.Descripcion ?? (object)DBNull.Value;
                        filas[0]["piso_id"] = en.PisoId;

                        SQLiteCommandBuilder cb = new SQLiteCommandBuilder(da);
                        da.Update(bdVirtual, "habitacion");
                        actualizado = true;
                    }
                }
                catch (Exception)
                {
                    actualizado = false;
                }
            }
            return actualizado;
        }

        // DELETE — metodo desconectado
        public bool BorrarHabitacion(ENHabitacion en)
        {
            bool borrado = false;
            DataSet bdVirtual = new DataSet();
            using (SQLiteConnection c = new SQLiteConnection(constring))
            {
                try
                {
                    SQLiteDataAdapter da = new SQLiteDataAdapter("SELECT * FROM Habitacion", c);
                    da.Fill(bdVirtual, "habitacion");
                    DataTable t = bdVirtual.Tables["habitacion"];
                    DataRow[] filas = t.Select("id = " + en.Id);

                    if (filas.Length > 0)
                    {
                        filas[0].Delete();
                        SQLiteCommandBuilder cb = new SQLiteCommandBuilder(da);
                        da.Update(bdVirtual, "habitacion");
                        borrado = true;
                    }
                }
                catch (Exception)
                {
                    borrado = false;
                }
            }
            return borrado;
        }
    }
}
