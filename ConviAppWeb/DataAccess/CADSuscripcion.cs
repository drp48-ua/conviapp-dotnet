using System;
using System.Data;
using System.Data.SQLite;
using System.Collections.Generic;
using ConviAppWeb.Models;

namespace ConviAppWeb.DataAccess
{
    public class CADSuscripcion
    {
        private string constring => DbConfig.ConnectionString;

        // CREATE — metodo desconectado
        public bool CrearSuscripcion(ENSuscripcion en)
        {
            bool creado = false;
            DataSet bdVirtual = new DataSet();
            using (SQLiteConnection c = new SQLiteConnection(constring))
            {
                try
                {
                    SQLiteDataAdapter da = new SQLiteDataAdapter("SELECT * FROM Suscripcion LIMIT 0", c);
                    da.Fill(bdVirtual, "suscripcion");

                    DataTable t = bdVirtual.Tables["suscripcion"];
                    DataRow nueva = t.NewRow();
                    nueva["plan"] = en.Plan ?? (object)DBNull.Value;
                    nueva["precio_mensual"] = en.PrecioMensual;
                    nueva["fecha_inicio"] = en.FechaInicio.ToString("o");
                    nueva["fecha_fin"] = en.FechaFin.ToString("o");
                    nueva["activa"] = en.Activa ? 1 : 0;
                    nueva["usuario_id"] = en.UsuarioId;
                    t.Rows.Add(nueva);

                    SQLiteCommandBuilder cb = new SQLiteCommandBuilder(da);
                    da.Update(bdVirtual, "suscripcion");
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
        public ENSuscripcion LeerSuscripcion(int id)
        {
            ENSuscripcion en = null;
            using (SQLiteConnection c = new SQLiteConnection(constring))
            {
                try
                {
                    c.Open();
                    SQLiteCommand com = new SQLiteCommand("SELECT * FROM Suscripcion WHERE id = @id", c);
                    com.Parameters.AddWithValue("@id", id);
                    using (SQLiteDataReader dr = com.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            en = new ENSuscripcion();
                            en.Id = dr["id"] != DBNull.Value ? Convert.ToInt32(dr["id"]) : 0;
                            en.Plan = dr["plan"] != DBNull.Value ? dr["plan"].ToString() : null;
                            en.PrecioMensual = dr["precio_mensual"] != DBNull.Value ? Convert.ToDecimal(dr["precio_mensual"]) : 0;
                            en.FechaInicio = dr["fecha_inicio"] != DBNull.Value ? Convert.ToDateTime(dr["fecha_inicio"]) : DateTime.MinValue;
                            en.FechaFin = dr["fecha_fin"] != DBNull.Value ? Convert.ToDateTime(dr["fecha_fin"]) : DateTime.MinValue;
                            en.Activa = dr["activa"] != DBNull.Value && Convert.ToInt32(dr["activa"]) == 1;
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
        public bool ActualizarSuscripcion(ENSuscripcion en)
        {
            bool actualizado = false;
            DataSet bdVirtual = new DataSet();
            using (SQLiteConnection c = new SQLiteConnection(constring))
            {
                try
                {
                    SQLiteDataAdapter da = new SQLiteDataAdapter("SELECT * FROM Suscripcion", c);
                    da.Fill(bdVirtual, "suscripcion");
                    DataTable t = bdVirtual.Tables["suscripcion"];
                    DataRow[] filas = t.Select("id = " + en.Id);

                    if (filas.Length > 0)
                    {
                        filas[0]["plan"] = en.Plan ?? (object)DBNull.Value;
                        filas[0]["precio_mensual"] = en.PrecioMensual;
                        filas[0]["fecha_inicio"] = en.FechaInicio.ToString("o");
                        filas[0]["fecha_fin"] = en.FechaFin.ToString("o");
                        filas[0]["activa"] = en.Activa ? 1 : 0;
                        filas[0]["usuario_id"] = en.UsuarioId;

                        SQLiteCommandBuilder cb = new SQLiteCommandBuilder(da);
                        da.Update(bdVirtual, "suscripcion");
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
        public bool BorrarSuscripcion(ENSuscripcion en)
        {
            bool borrado = false;
            DataSet bdVirtual = new DataSet();
            using (SQLiteConnection c = new SQLiteConnection(constring))
            {
                try
                {
                    SQLiteDataAdapter da = new SQLiteDataAdapter("SELECT * FROM Suscripcion", c);
                    da.Fill(bdVirtual, "suscripcion");
                    DataTable t = bdVirtual.Tables["suscripcion"];
                    DataRow[] filas = t.Select("id = " + en.Id);

                    if (filas.Length > 0)
                    {
                        filas[0].Delete();
                        SQLiteCommandBuilder cb = new SQLiteCommandBuilder(da);
                        da.Update(bdVirtual, "suscripcion");
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
