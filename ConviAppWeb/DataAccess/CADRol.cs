using System;
using System.Data;
using System.Data.SQLite;
using System.Collections.Generic;
using ConviAppWeb.Models;

namespace ConviAppWeb.DataAccess
{
    public class CADRol
    {
        private string constring => DbConfig.ConnectionString;

        // CREATE — metodo desconectado
        public bool CrearRol(ENRol en)
        {
            bool creado = false;
            DataSet bdVirtual = new DataSet();
            using (SQLiteConnection c = new SQLiteConnection(constring))
            {
                try
                {
                    SQLiteDataAdapter da = new SQLiteDataAdapter("SELECT * FROM Rol LIMIT 0", c);
                    da.Fill(bdVirtual, "rol");

                    DataTable t = bdVirtual.Tables["rol"];
                    DataRow nueva = t.NewRow();
                    nueva["nombre"] = en.Nombre ?? (object)DBNull.Value;
                    nueva["descripcion"] = en.Descripcion ?? (object)DBNull.Value;
                    nueva["puede_gestionar_pisos"] = en.PuedeGestionarPisos ? 1 : 0;
                    nueva["puede_ver_contratos"] = en.PuedeVerContratos ? 1 : 0;
                    nueva["puede_gestionar_usuarios"] = en.PuedeGestionarUsuarios ? 1 : 0;
                    t.Rows.Add(nueva);

                    SQLiteCommandBuilder cb = new SQLiteCommandBuilder(da);
                    da.Update(bdVirtual, "rol");
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
        public ENRol LeerRol(int id)
        {
            ENRol en = null;
            using (SQLiteConnection c = new SQLiteConnection(constring))
            {
                try
                {
                    c.Open();
                    SQLiteCommand com = new SQLiteCommand("SELECT * FROM Rol WHERE id = @id", c);
                    com.Parameters.AddWithValue("@id", id);
                    using (SQLiteDataReader dr = com.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            en = new ENRol();
                            en.Id = dr["id"] != DBNull.Value ? Convert.ToInt32(dr["id"]) : 0;
                            en.Nombre = dr["nombre"] != DBNull.Value ? dr["nombre"].ToString() : null;
                            en.Descripcion = dr["descripcion"] != DBNull.Value ? dr["descripcion"].ToString() : null;
                            en.PuedeGestionarPisos = dr["puede_gestionar_pisos"] != DBNull.Value && Convert.ToInt32(dr["puede_gestionar_pisos"]) == 1;
                            en.PuedeVerContratos = dr["puede_ver_contratos"] != DBNull.Value && Convert.ToInt32(dr["puede_ver_contratos"]) == 1;
                            en.PuedeGestionarUsuarios = dr["puede_gestionar_usuarios"] != DBNull.Value && Convert.ToInt32(dr["puede_gestionar_usuarios"]) == 1;
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
        public bool ActualizarRol(ENRol en)
        {
            bool actualizado = false;
            DataSet bdVirtual = new DataSet();
            using (SQLiteConnection c = new SQLiteConnection(constring))
            {
                try
                {
                    SQLiteDataAdapter da = new SQLiteDataAdapter("SELECT * FROM Rol", c);
                    da.Fill(bdVirtual, "rol");
                    DataTable t = bdVirtual.Tables["rol"];
                    DataRow[] filas = t.Select("id = " + en.Id);

                    if (filas.Length > 0)
                    {
                        filas[0]["nombre"] = en.Nombre ?? (object)DBNull.Value;
                        filas[0]["descripcion"] = en.Descripcion ?? (object)DBNull.Value;
                        filas[0]["puede_gestionar_pisos"] = en.PuedeGestionarPisos ? 1 : 0;
                        filas[0]["puede_ver_contratos"] = en.PuedeVerContratos ? 1 : 0;
                        filas[0]["puede_gestionar_usuarios"] = en.PuedeGestionarUsuarios ? 1 : 0;

                        SQLiteCommandBuilder cb = new SQLiteCommandBuilder(da);
                        da.Update(bdVirtual, "rol");
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
        public bool BorrarRol(ENRol en)
        {
            bool borrado = false;
            DataSet bdVirtual = new DataSet();
            using (SQLiteConnection c = new SQLiteConnection(constring))
            {
                try
                {
                    SQLiteDataAdapter da = new SQLiteDataAdapter("SELECT * FROM Rol", c);
                    da.Fill(bdVirtual, "rol");
                    DataTable t = bdVirtual.Tables["rol"];
                    DataRow[] filas = t.Select("id = " + en.Id);

                    if (filas.Length > 0)
                    {
                        filas[0].Delete();
                        SQLiteCommandBuilder cb = new SQLiteCommandBuilder(da);
                        da.Update(bdVirtual, "rol");
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
