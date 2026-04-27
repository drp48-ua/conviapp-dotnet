using System;
using System.Data;
using System.Data.SQLite;
using System.Collections.Generic;
using ConviAppWeb.Models;

namespace ConviAppWeb.DataAccess
{
    public class CADImagen
    {
        private string constring => DbConfig.ConnectionString;

        // CREATE — metodo desconectado
        public bool CrearImagen(ENImagen en)
        {
            bool creado = false;
            DataSet bdVirtual = new DataSet();
            using (SQLiteConnection c = new SQLiteConnection(constring))
            {
                try
                {
                    SQLiteDataAdapter da = new SQLiteDataAdapter("SELECT * FROM Imagen LIMIT 0", c);
                    da.Fill(bdVirtual, "imagen");

                    DataTable t = bdVirtual.Tables["imagen"];
                    DataRow nueva = t.NewRow();
                    nueva["url"] = en.Url ?? (object)DBNull.Value;
                    nueva["descripcion"] = en.Descripcion ?? (object)DBNull.Value;
                    nueva["es_principal"] = en.EsPrincipal ? 1 : 0;
                    nueva["fecha_subida"] = en.FechaSubida.ToString("o");
                    nueva["habitacion_id"] = en.HabitacionId ?? (object)DBNull.Value;
                    nueva["piso_id"] = en.PisoId ?? (object)DBNull.Value;
                    t.Rows.Add(nueva);

                    SQLiteCommandBuilder cb = new SQLiteCommandBuilder(da);
                    da.Update(bdVirtual, "imagen");
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
        public ENImagen LeerImagen(int id)
        {
            ENImagen en = null;
            using (SQLiteConnection c = new SQLiteConnection(constring))
            {
                try
                {
                    c.Open();
                    SQLiteCommand com = new SQLiteCommand("SELECT * FROM Imagen WHERE id = @id", c);
                    com.Parameters.AddWithValue("@id", id);
                    using (SQLiteDataReader dr = com.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            en = new ENImagen();
                            en.Id = dr["id"] != DBNull.Value ? Convert.ToInt32(dr["id"]) : 0;
                            en.Url = dr["url"] != DBNull.Value ? dr["url"].ToString() : null;
                            en.Descripcion = dr["descripcion"] != DBNull.Value ? dr["descripcion"].ToString() : null;
                            en.EsPrincipal = dr["es_principal"] != DBNull.Value && Convert.ToInt32(dr["es_principal"]) == 1;
                            en.FechaSubida = dr["fecha_subida"] != DBNull.Value ? Convert.ToDateTime(dr["fecha_subida"]) : DateTime.MinValue;
                            en.HabitacionId = dr["habitacion_id"] != DBNull.Value ? Convert.ToInt32(dr["habitacion_id"]) : (int?)null;
                            en.PisoId = dr["piso_id"] != DBNull.Value ? Convert.ToInt32(dr["piso_id"]) : (int?)null;
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
        public bool ActualizarImagen(ENImagen en)
        {
            bool actualizado = false;
            DataSet bdVirtual = new DataSet();
            using (SQLiteConnection c = new SQLiteConnection(constring))
            {
                try
                {
                    SQLiteDataAdapter da = new SQLiteDataAdapter("SELECT * FROM Imagen", c);
                    da.Fill(bdVirtual, "imagen");
                    DataTable t = bdVirtual.Tables["imagen"];
                    DataRow[] filas = t.Select("id = " + en.Id);

                    if (filas.Length > 0)
                    {
                        filas[0]["url"] = en.Url ?? (object)DBNull.Value;
                        filas[0]["descripcion"] = en.Descripcion ?? (object)DBNull.Value;
                        filas[0]["es_principal"] = en.EsPrincipal ? 1 : 0;
                        filas[0]["fecha_subida"] = en.FechaSubida.ToString("o");
                        filas[0]["habitacion_id"] = en.HabitacionId ?? (object)DBNull.Value;
                        filas[0]["piso_id"] = en.PisoId ?? (object)DBNull.Value;

                        SQLiteCommandBuilder cb = new SQLiteCommandBuilder(da);
                        da.Update(bdVirtual, "imagen");
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
        public bool BorrarImagen(ENImagen en)
        {
            bool borrado = false;
            DataSet bdVirtual = new DataSet();
            using (SQLiteConnection c = new SQLiteConnection(constring))
            {
                try
                {
                    SQLiteDataAdapter da = new SQLiteDataAdapter("SELECT * FROM Imagen", c);
                    da.Fill(bdVirtual, "imagen");
                    DataTable t = bdVirtual.Tables["imagen"];
                    DataRow[] filas = t.Select("id = " + en.Id);

                    if (filas.Length > 0)
                    {
                        filas[0].Delete();
                        SQLiteCommandBuilder cb = new SQLiteCommandBuilder(da);
                        da.Update(bdVirtual, "imagen");
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
