using System;
using System.Data;
using System.Data.SQLite;
using System.Collections.Generic;
using ConviAppWeb.Models;

namespace ConviAppWeb.DataAccess
{
    public class CADFavorito
    {
        private string constring => DbConfig.ConnectionString;

        // CREATE — metodo desconectado
        public bool CrearFavorito(ENFavorito en)
        {
            bool creado = false;
            DataSet bdVirtual = new DataSet();
            using (SQLiteConnection c = new SQLiteConnection(constring))
            {
                try
                {
                    SQLiteDataAdapter da = new SQLiteDataAdapter("SELECT * FROM Favorito LIMIT 0", c);
                    da.Fill(bdVirtual, "favorito");

                    DataTable t = bdVirtual.Tables["favorito"];
                    DataRow nueva = t.NewRow();
                    nueva["fecha_guardado"] = en.FechaGuardado.ToString("o");
                    nueva["usuario_id"] = en.UsuarioId;
                    nueva["habitacion_id"] = en.HabitacionId ?? (object)DBNull.Value;
                    nueva["piso_id"] = en.PisoId ?? (object)DBNull.Value;
                    t.Rows.Add(nueva);

                    SQLiteCommandBuilder cb = new SQLiteCommandBuilder(da);
                    da.Update(bdVirtual, "favorito");
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
        public ENFavorito LeerFavorito(int id)
        {
            ENFavorito en = null;
            using (SQLiteConnection c = new SQLiteConnection(constring))
            {
                try
                {
                    c.Open();
                    SQLiteCommand com = new SQLiteCommand("SELECT * FROM Favorito WHERE id = @id", c);
                    com.Parameters.AddWithValue("@id", id);
                    using (SQLiteDataReader dr = com.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            en = new ENFavorito();
                            en.Id = dr["id"] != DBNull.Value ? Convert.ToInt32(dr["id"]) : 0;
                            en.FechaGuardado = dr["fecha_guardado"] != DBNull.Value ? Convert.ToDateTime(dr["fecha_guardado"]) : DateTime.MinValue;
                            en.UsuarioId = dr["usuario_id"] != DBNull.Value ? Convert.ToInt32(dr["usuario_id"]) : 0;
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
        public bool ActualizarFavorito(ENFavorito en)
        {
            bool actualizado = false;
            DataSet bdVirtual = new DataSet();
            using (SQLiteConnection c = new SQLiteConnection(constring))
            {
                try
                {
                    SQLiteDataAdapter da = new SQLiteDataAdapter("SELECT * FROM Favorito", c);
                    da.Fill(bdVirtual, "favorito");
                    DataTable t = bdVirtual.Tables["favorito"];
                    DataRow[] filas = t.Select("id = " + en.Id);

                    if (filas.Length > 0)
                    {
                        filas[0]["fecha_guardado"] = en.FechaGuardado.ToString("o");
                        filas[0]["usuario_id"] = en.UsuarioId;
                        filas[0]["habitacion_id"] = en.HabitacionId ?? (object)DBNull.Value;
                        filas[0]["piso_id"] = en.PisoId ?? (object)DBNull.Value;

                        SQLiteCommandBuilder cb = new SQLiteCommandBuilder(da);
                        da.Update(bdVirtual, "favorito");
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
        public bool BorrarFavorito(ENFavorito en)
        {
            bool borrado = false;
            DataSet bdVirtual = new DataSet();
            using (SQLiteConnection c = new SQLiteConnection(constring))
            {
                try
                {
                    SQLiteDataAdapter da = new SQLiteDataAdapter("SELECT * FROM Favorito", c);
                    da.Fill(bdVirtual, "favorito");
                    DataTable t = bdVirtual.Tables["favorito"];
                    DataRow[] filas = t.Select("id = " + en.Id);

                    if (filas.Length > 0)
                    {
                        filas[0].Delete();
                        SQLiteCommandBuilder cb = new SQLiteCommandBuilder(da);
                        da.Update(bdVirtual, "favorito");
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
