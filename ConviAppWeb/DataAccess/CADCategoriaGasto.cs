using System;
using System.Data;
using System.Data.SQLite;
using System.Collections.Generic;
using ConviAppWeb.Models;

namespace ConviAppWeb.DataAccess
{
    public class CADCategoriaGasto
    {
        private string constring => DbConfig.ConnectionString;

        // CREATE — metodo desconectado
        public bool CrearCategoriaGasto(ENCategoriaGasto en)
        {
            bool creado = false;
            DataSet bdVirtual = new DataSet();
            using (SQLiteConnection c = new SQLiteConnection(constring))
            {
                try
                {
                    SQLiteDataAdapter da = new SQLiteDataAdapter("SELECT * FROM CategoriaGasto LIMIT 0", c);
                    da.Fill(bdVirtual, "categoriagasto");

                    DataTable t = bdVirtual.Tables["categoriagasto"];
                    DataRow nueva = t.NewRow();
                    nueva["nombre"] = en.Nombre ?? (object)DBNull.Value;
                    nueva["descripcion"] = en.Descripcion ?? (object)DBNull.Value;
                    nueva["icono"] = en.Icono ?? (object)DBNull.Value;
                    t.Rows.Add(nueva);

                    SQLiteCommandBuilder cb = new SQLiteCommandBuilder(da);
                    da.Update(bdVirtual, "categoriagasto");
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
        public ENCategoriaGasto LeerCategoriaGasto(int id)
        {
            ENCategoriaGasto en = null;
            using (SQLiteConnection c = new SQLiteConnection(constring))
            {
                try
                {
                    c.Open();
                    SQLiteCommand com = new SQLiteCommand("SELECT * FROM CategoriaGasto WHERE id = @id", c);
                    com.Parameters.AddWithValue("@id", id);
                    using (SQLiteDataReader dr = com.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            en = new ENCategoriaGasto();
                            en.Id = dr["id"] != DBNull.Value ? Convert.ToInt32(dr["id"]) : 0;
                            en.Nombre = dr["nombre"] != DBNull.Value ? dr["nombre"].ToString() : null;
                            en.Descripcion = dr["descripcion"] != DBNull.Value ? dr["descripcion"].ToString() : null;
                            en.Icono = dr["icono"] != DBNull.Value ? dr["icono"].ToString() : null;
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
        public bool ActualizarCategoriaGasto(ENCategoriaGasto en)
        {
            bool actualizado = false;
            DataSet bdVirtual = new DataSet();
            using (SQLiteConnection c = new SQLiteConnection(constring))
            {
                try
                {
                    SQLiteDataAdapter da = new SQLiteDataAdapter("SELECT * FROM CategoriaGasto", c);
                    da.Fill(bdVirtual, "categoriagasto");
                    DataTable t = bdVirtual.Tables["categoriagasto"];
                    DataRow[] filas = t.Select("id = " + en.Id);

                    if (filas.Length > 0)
                    {
                        filas[0]["nombre"] = en.Nombre ?? (object)DBNull.Value;
                        filas[0]["descripcion"] = en.Descripcion ?? (object)DBNull.Value;
                        filas[0]["icono"] = en.Icono ?? (object)DBNull.Value;

                        SQLiteCommandBuilder cb = new SQLiteCommandBuilder(da);
                        da.Update(bdVirtual, "categoriagasto");
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
        public bool BorrarCategoriaGasto(ENCategoriaGasto en)
        {
            bool borrado = false;
            DataSet bdVirtual = new DataSet();
            using (SQLiteConnection c = new SQLiteConnection(constring))
            {
                try
                {
                    SQLiteDataAdapter da = new SQLiteDataAdapter("SELECT * FROM CategoriaGasto", c);
                    da.Fill(bdVirtual, "categoriagasto");
                    DataTable t = bdVirtual.Tables["categoriagasto"];
                    DataRow[] filas = t.Select("id = " + en.Id);

                    if (filas.Length > 0)
                    {
                        filas[0].Delete();
                        SQLiteCommandBuilder cb = new SQLiteCommandBuilder(da);
                        da.Update(bdVirtual, "categoriagasto");
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
