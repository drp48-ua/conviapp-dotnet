using System;
using System.Data;
using System.Data.SQLite;
using System.Collections.Generic;
using ConviAppWeb.Models;

namespace ConviAppWeb.DataAccess
{
    public class CADZonaComun
    {
        private string constring => DbConfig.ConnectionString;

        // CREATE — metodo desconectado
        public bool CrearZonaComun(ENZonaComun en)
        {
            bool creado = false;
            DataSet bdVirtual = new DataSet();
            using (SQLiteConnection c = new SQLiteConnection(constring))
            {
                try
                {
                    SQLiteDataAdapter da = new SQLiteDataAdapter("SELECT * FROM ZonaComun LIMIT 0", c);
                    da.Fill(bdVirtual, "zonacomun");

                    DataTable t = bdVirtual.Tables["zonacomun"];
                    DataRow nueva = t.NewRow();
                    nueva["nombre"] = en.Nombre ?? (object)DBNull.Value;
                    nueva["descripcion"] = en.Descripcion ?? (object)DBNull.Value;
                    nueva["capacidad"] = en.Capacidad;
                    nueva["disponible"] = en.Disponible ? 1 : 0;
                    nueva["piso_id"] = en.PisoId;
                    t.Rows.Add(nueva);

                    SQLiteCommandBuilder cb = new SQLiteCommandBuilder(da);
                    da.Update(bdVirtual, "zonacomun");
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
        public ENZonaComun LeerZonaComun(int id)
        {
            ENZonaComun en = null;
            using (SQLiteConnection c = new SQLiteConnection(constring))
            {
                try
                {
                    c.Open();
                    SQLiteCommand com = new SQLiteCommand("SELECT * FROM ZonaComun WHERE id = @id", c);
                    com.Parameters.AddWithValue("@id", id);
                    using (SQLiteDataReader dr = com.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            en = new ENZonaComun();
                            en.Id = dr["id"] != DBNull.Value ? Convert.ToInt32(dr["id"]) : 0;
                            en.Nombre = dr["nombre"] != DBNull.Value ? dr["nombre"].ToString() : null;
                            en.Descripcion = dr["descripcion"] != DBNull.Value ? dr["descripcion"].ToString() : null;
                            en.Capacidad = dr["capacidad"] != DBNull.Value ? Convert.ToInt32(dr["capacidad"]) : 0;
                            en.Disponible = dr["disponible"] != DBNull.Value && Convert.ToInt32(dr["disponible"]) == 1;
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
        public bool ActualizarZonaComun(ENZonaComun en)
        {
            bool actualizado = false;
            DataSet bdVirtual = new DataSet();
            using (SQLiteConnection c = new SQLiteConnection(constring))
            {
                try
                {
                    SQLiteDataAdapter da = new SQLiteDataAdapter("SELECT * FROM ZonaComun", c);
                    da.Fill(bdVirtual, "zonacomun");
                    DataTable t = bdVirtual.Tables["zonacomun"];
                    DataRow[] filas = t.Select("id = " + en.Id);

                    if (filas.Length > 0)
                    {
                        filas[0]["nombre"] = en.Nombre ?? (object)DBNull.Value;
                        filas[0]["descripcion"] = en.Descripcion ?? (object)DBNull.Value;
                        filas[0]["capacidad"] = en.Capacidad;
                        filas[0]["disponible"] = en.Disponible ? 1 : 0;
                        filas[0]["piso_id"] = en.PisoId;

                        SQLiteCommandBuilder cb = new SQLiteCommandBuilder(da);
                        da.Update(bdVirtual, "zonacomun");
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
        public bool BorrarZonaComun(ENZonaComun en)
        {
            bool borrado = false;
            DataSet bdVirtual = new DataSet();
            using (SQLiteConnection c = new SQLiteConnection(constring))
            {
                try
                {
                    SQLiteDataAdapter da = new SQLiteDataAdapter("SELECT * FROM ZonaComun", c);
                    da.Fill(bdVirtual, "zonacomun");
                    DataTable t = bdVirtual.Tables["zonacomun"];
                    DataRow[] filas = t.Select("id = " + en.Id);

                    if (filas.Length > 0)
                    {
                        filas[0].Delete();
                        SQLiteCommandBuilder cb = new SQLiteCommandBuilder(da);
                        da.Update(bdVirtual, "zonacomun");
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
