using System;
using System.Data;
using System.Data.SQLite;
using System.Collections.Generic;
using ConviAppWeb.Models;

namespace ConviAppWeb.DataAccess
{
    public class CADUsuario
    {
        private string constring => DbConfig.ConnectionString;

        // CREATE — método desconectado (DataSet + SqlDataAdapter)
        public bool CrearUsuario(ENUsuario en)
        {
            bool creado = false;
            DataSet bdVirtual = new DataSet();
            SQLiteConnection c = new SQLiteConnection(constring);

            try
            {
                SQLiteDataAdapter da = new SQLiteDataAdapter("SELECT * FROM Usuario LIMIT 0", c);
                da.Fill(bdVirtual, "usuario");

                DataTable t = bdVirtual.Tables["usuario"];
                DataRow nueva = t.NewRow();
                nueva["nombre"]         = en.Nombre ?? (object)DBNull.Value;
                nueva["apellidos"]      = en.Apellidos ?? (object)DBNull.Value;
                nueva["email"]          = en.Email;
                nueva["password_hash"]  = en.PasswordHash;
                nueva["telefono"]       = en.Telefono ?? (object)DBNull.Value;
                nueva["fecha_registro"] = en.FechaRegistro.ToString("o");
                nueva["activo"]         = en.Activo ? 1 : 0;
                nueva["rol"]            = en.Rol?.Nombre ?? "Basico";
                t.Rows.Add(nueva);

                SQLiteCommandBuilder cb = new SQLiteCommandBuilder(da);
                da.Update(bdVirtual, "usuario");
                creado = true;
            }
            catch (Exception) { creado = false; }
            finally { c.Close(); }

            return creado;
        }

        // READ por id — método conectado (SqlCommand + SqlDataReader)
        public ENUsuario LeerUsuario(int id)
        {
            ENUsuario en = null;
            SQLiteConnection c = new SQLiteConnection(constring);

            try
            {
                c.Open();
                SQLiteCommand com = new SQLiteCommand("SELECT * FROM Usuario WHERE id = @id", c);
                com.Parameters.AddWithValue("@id", id);
                SQLiteDataReader dr = com.ExecuteReader();

                if (dr.Read()) en = MapRow(dr);
                dr.Close();
            }
            catch (Exception) { en = null; }
            finally { c.Close(); }

            return en;
        }

        // READ por email — método conectado
        public ENUsuario BuscarPorEmail(string email)
        {
            ENUsuario en = null;
            SQLiteConnection c = new SQLiteConnection(constring);

            try
            {
                c.Open();
                SQLiteCommand com = new SQLiteCommand("SELECT * FROM Usuario WHERE email = @email", c);
                com.Parameters.AddWithValue("@email", email);
                SQLiteDataReader dr = com.ExecuteReader();

                if (dr.Read()) en = MapRow(dr);
                dr.Close();
            }
            catch (Exception) { en = null; }
            finally { c.Close(); }

            return en;
        }

        // EXISTS por email — método conectado
        public bool ExisteEmail(string email)
        {
            bool existe = false;
            SQLiteConnection c = new SQLiteConnection(constring);

            try
            {
                c.Open();
                SQLiteCommand com = new SQLiteCommand("SELECT COUNT(*) FROM Usuario WHERE email = @email", c);
                com.Parameters.AddWithValue("@email", email);
                existe = Convert.ToInt32(com.ExecuteScalar()) > 0;
            }
            catch (Exception) { existe = false; }
            finally { c.Close(); }

            return existe;
        }

        // UPDATE — método desconectado
        public bool ActualizarUsuario(ENUsuario en)
        {
            bool actualizado = false;
            SQLiteConnection c = new SQLiteConnection(constring);

            try
            {
                c.Open();
                SQLiteCommand com = new SQLiteCommand(
                    "UPDATE Usuario SET nombre=@n, apellidos=@a, email=@e, password_hash=@p, rol=@r WHERE id=@id", c);
                com.Parameters.AddWithValue("@n",  en.Nombre ?? "");
                com.Parameters.AddWithValue("@a",  en.Apellidos ?? "");
                com.Parameters.AddWithValue("@e",  en.Email);
                com.Parameters.AddWithValue("@p",  en.PasswordHash);
                com.Parameters.AddWithValue("@r",  en.Rol?.Nombre ?? "Basico");
                com.Parameters.AddWithValue("@id", en.Id);
                actualizado = com.ExecuteNonQuery() > 0;
            }
            catch (Exception) { actualizado = false; }
            finally { c.Close(); }

            return actualizado;
        }

        // DELETE — método desconectado
        public bool BorrarUsuario(ENUsuario en)
        {
            bool borrado = false;
            DataSet bdVirtual = new DataSet();
            SQLiteConnection c = new SQLiteConnection(constring);

            try
            {
                SQLiteDataAdapter da = new SQLiteDataAdapter($"SELECT * FROM Usuario WHERE id = {en.Id}", c);
                da.Fill(bdVirtual, "usuario");
                DataTable t = bdVirtual.Tables["usuario"];
                DataRow[] filas = t.Select("id = " + en.Id);

                if (filas.Length > 0)
                {
                    filas[0].Delete();
                    SQLiteCommandBuilder cb = new SQLiteCommandBuilder(da);
                    da.Update(bdVirtual, "usuario");
                    borrado = true;
                }
            }
            catch (Exception) { borrado = false; }
            finally { c.Close(); }

            return borrado;
        }

        private ENUsuario MapRow(SQLiteDataReader dr)
        {
            var en = new ENUsuario();
            en.Id           = Convert.ToInt32(dr["id"]);
            en.Nombre       = dr["nombre"] != DBNull.Value ? dr["nombre"].ToString() : null;
            en.Apellidos    = dr["apellidos"] != DBNull.Value ? dr["apellidos"].ToString() : null;
            en.Email        = dr["email"].ToString();
            en.PasswordHash = dr["password_hash"].ToString();
            en.Telefono     = dr["telefono"] != DBNull.Value ? dr["telefono"].ToString() : null;
            en.Activo       = dr["activo"] != DBNull.Value && Convert.ToInt32(dr["activo"]) == 1;
            en.Rol          = new ENRol { Nombre = dr["rol"] != DBNull.Value ? dr["rol"].ToString() : "Basico" };
            return en;
        }
    }
}
