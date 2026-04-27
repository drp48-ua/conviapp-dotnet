using System;
using System.Data;
using System.Data.SQLite;
using System.Collections.Generic;
using ConviAppWeb.Models;

namespace ConviAppWeb.DataAccess
{
    public class CADNotificacion
    {
        private string constring => DbConfig.ConnectionString;

        // CREATE — metodo desconectado
        public bool CrearNotificacion(ENNotificacion en)
        {
            bool creado = false;
            DataSet bdVirtual = new DataSet();
            using (SQLiteConnection c = new SQLiteConnection(constring))
            {
                try
                {
                    SQLiteDataAdapter da = new SQLiteDataAdapter("SELECT * FROM Notificacion LIMIT 0", c);
                    da.Fill(bdVirtual, "notificacion");

                    DataTable t = bdVirtual.Tables["notificacion"];
                    DataRow nueva = t.NewRow();
                    nueva["titulo"] = en.Titulo ?? (object)DBNull.Value;
                    nueva["mensaje"] = en.Mensaje ?? (object)DBNull.Value;
                    nueva["tipo"] = en.Tipo ?? (object)DBNull.Value;
                    nueva["leida"] = en.Leida ? 1 : 0;
                    nueva["fecha_creacion"] = en.FechaCreacion.ToString("o");
                    nueva["fecha_lectura"] = en.FechaLectura?.ToString("o") ?? (object)DBNull.Value;
                    nueva["usuario_id"] = en.UsuarioId;
                    t.Rows.Add(nueva);

                    SQLiteCommandBuilder cb = new SQLiteCommandBuilder(da);
                    da.Update(bdVirtual, "notificacion");
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
        public ENNotificacion LeerNotificacion(int id)
        {
            ENNotificacion en = null;
            using (SQLiteConnection c = new SQLiteConnection(constring))
            {
                try
                {
                    c.Open();
                    SQLiteCommand com = new SQLiteCommand("SELECT * FROM Notificacion WHERE id = @id", c);
                    com.Parameters.AddWithValue("@id", id);
                    using (SQLiteDataReader dr = com.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            en = new ENNotificacion();
                            en.Id = dr["id"] != DBNull.Value ? Convert.ToInt32(dr["id"]) : 0;
                            en.Titulo = dr["titulo"] != DBNull.Value ? dr["titulo"].ToString() : null;
                            en.Mensaje = dr["mensaje"] != DBNull.Value ? dr["mensaje"].ToString() : null;
                            en.Tipo = dr["tipo"] != DBNull.Value ? dr["tipo"].ToString() : null;
                            en.Leida = dr["leida"] != DBNull.Value && Convert.ToInt32(dr["leida"]) == 1;
                            en.FechaCreacion = dr["fecha_creacion"] != DBNull.Value ? Convert.ToDateTime(dr["fecha_creacion"]) : DateTime.MinValue;
                            en.FechaLectura = dr["fecha_lectura"] != DBNull.Value ? Convert.ToDateTime(dr["fecha_lectura"]) : (DateTime?)null;
                            en.UsuarioId = dr["usuario_id"] != DBNull.Value ? Convert.ToInt32(dr["usuario_id"]) : 0;
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
        public bool ActualizarNotificacion(ENNotificacion en)
        {
            bool actualizado = false;
            DataSet bdVirtual = new DataSet();
            using (SQLiteConnection c = new SQLiteConnection(constring))
            {
                try
                {
                    SQLiteDataAdapter da = new SQLiteDataAdapter("SELECT * FROM Notificacion", c);
                    da.Fill(bdVirtual, "notificacion");
                    DataTable t = bdVirtual.Tables["notificacion"];
                    DataRow[] filas = t.Select("id = " + en.Id);

                    if (filas.Length > 0)
                    {
                        filas[0]["titulo"] = en.Titulo ?? (object)DBNull.Value;
                        filas[0]["mensaje"] = en.Mensaje ?? (object)DBNull.Value;
                        filas[0]["tipo"] = en.Tipo ?? (object)DBNull.Value;
                        filas[0]["leida"] = en.Leida ? 1 : 0;
                        filas[0]["fecha_creacion"] = en.FechaCreacion.ToString("o");
                        filas[0]["fecha_lectura"] = en.FechaLectura?.ToString("o") ?? (object)DBNull.Value;
                        filas[0]["usuario_id"] = en.UsuarioId;

                        SQLiteCommandBuilder cb = new SQLiteCommandBuilder(da);
                        da.Update(bdVirtual, "notificacion");
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
        public bool BorrarNotificacion(ENNotificacion en)
        {
            bool borrado = false;
            DataSet bdVirtual = new DataSet();
            using (SQLiteConnection c = new SQLiteConnection(constring))
            {
                try
                {
                    SQLiteDataAdapter da = new SQLiteDataAdapter("SELECT * FROM Notificacion", c);
                    da.Fill(bdVirtual, "notificacion");
                    DataTable t = bdVirtual.Tables["notificacion"];
                    DataRow[] filas = t.Select("id = " + en.Id);

                    if (filas.Length > 0)
                    {
                        filas[0].Delete();
                        SQLiteCommandBuilder cb = new SQLiteCommandBuilder(da);
                        da.Update(bdVirtual, "notificacion");
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
