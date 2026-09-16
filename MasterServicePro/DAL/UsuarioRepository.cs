using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using MasterServicePro.Models;
using MasterServicePro.Utils;

namespace MasterServicePro.DAL
{
    public class UsuarioRepository
    {
        private DbConnection db = new DbConnection();

        private static bool _tabelaCriada = false;
        private static readonly object _lock = new object();

        public UsuarioRepository()
        {
            try { CriarTabelaUsuarios(); } catch { }
        }

        private void CriarTabelaUsuarios()
        {
            if (_tabelaCriada) return;
            lock (_lock)
            {
                if (_tabelaCriada) return;

                // Se a tabela não existe, cria do zero
                var result = db.ExecuteScalar("SELECT object_id FROM sys.objects WHERE object_id = OBJECT_ID('Usuarios') AND type in (N'U')");
                if (result == null || result == DBNull.Value)
                {
                    db.ExecuteNonQuery(@"
                        CREATE TABLE Usuarios (
                            Id INT PRIMARY KEY IDENTITY(1,1),
                            Usuario VARCHAR(50) NOT NULL UNIQUE,
                            Senha VARCHAR(150) NOT NULL,
                            Nivel VARCHAR(20) NOT NULL,
                            Ativo BIT DEFAULT 1
                        )");

                    db.ExecuteNonQuery("INSERT INTO Usuarios (Usuario, Senha, Nivel) VALUES ('admin', 'admin', 'Admin')");
                }
                else
                {
                    // Migração: Garante que as colunas existem (sem NOT NULL inicial para evitar erro com dados existentes)
                    string[] colunas = { 
                        "Usuario VARCHAR(50)", 
                        "Senha VARCHAR(150)", 
                        "Nivel VARCHAR(20)", 
                        "Ativo BIT" 
                    };

                    foreach (var col in colunas)
                    {
                        string nomeCol = col.Split(' ')[0];
                        string checkCol = $"IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Usuarios') AND name = '{nomeCol}') " +
                                         $"ALTER TABLE Usuarios ADD {col};";
                        try { db.ExecuteNonQuery(checkCol); } catch { }
                    }
                }

                // Expand column length for PBKDF2 Hashes
                try { db.ExecuteNonQuery("ALTER TABLE Usuarios ALTER COLUMN Senha VARCHAR(150);"); } catch { }

                // Garante que o usuário admin padrão existe e está configurado corretamente
                // Migra senha antiga em texto puro para hash
                if (Convert.ToInt32(db.ExecuteScalar("SELECT COUNT(*) FROM Usuarios WHERE Usuario = 'admin'")) == 0)
                {
                    string hash = PasswordHasher.HashPassword("admin");
                    db.ExecuteNonQuery($"INSERT INTO Usuarios (Usuario, Senha, Nivel, Ativo) VALUES ('admin', '{hash}', 'Admin', 1)");
                }
                else
                {
                    // Se o admin existe mas a senha é a padrão em texto 'admin' ou nula, atualizamos para o hash
                    string hash = PasswordHasher.HashPassword("admin");
                    db.ExecuteNonQuery($"UPDATE Usuarios SET Senha = '{hash}', Nivel = 'Admin', Ativo = 1 WHERE Usuario = 'admin' AND (Senha IS NULL OR Senha = 'admin' OR Senha = '')");
                }

                _tabelaCriada = true;
            }
        }

        public bool Autenticar(string user, string password)
        {
            string query = "SELECT Id, Usuario, Senha, Nivel FROM Usuarios WHERE Usuario = @User AND Ativo = 1";
            
            using (var conn = db.GetConnection())
            {
                var row = conn.QueryFirstOrDefault(query, new { User = user });
                if (row != null)
                {
                    string storedHash = row.Senha;
                    
                    bool isValid = false;
                    if (storedHash.Length < 60)
                    {
                        isValid = (storedHash == password);
                    }
                    else
                    {
                        isValid = PasswordHasher.VerifyPassword(password, storedHash);
                    }

                    if (isValid)
                    {
                        AuthSession.Id = (int)row.Id;
                        AuthSession.Usuario = row.Usuario;
                        AuthSession.Nivel = row.Nivel;
                        return true;
                    }
                }
            }
            return false;
        }

        public List<Usuario> BuscarTodos()
        {
            using (var conn = db.GetConnection())
            {
                return conn.Query<Usuario>("SELECT Id, Usuario as Username, Senha, Nivel, Ativo FROM Usuarios WHERE Ativo = 1").ToList();
            }
        }

        public void Inserir(Usuario u)
        {
            string hash = PasswordHasher.HashPassword(u.Senha);
            using (var conn = db.GetConnection())
            {
                conn.Execute("INSERT INTO Usuarios (Usuario, Senha, Nivel, Ativo) VALUES (@Usuario, @Senha, @Nivel, 1)",
                    new { Usuario = u.Username, Senha = hash, Nivel = u.Nivel });
            }
        }

        public void Atualizar(Usuario u, bool atualizarSenha = false)
        {
            using (var conn = db.GetConnection())
            {
                if (atualizarSenha)
                {
                    string hash = PasswordHasher.HashPassword(u.Senha);
                    conn.Execute("UPDATE Usuarios SET Usuario = @Usuario, Senha = @Senha, Nivel = @Nivel WHERE Id = @Id",
                        new { Usuario = u.Username, Senha = hash, Nivel = u.Nivel, Id = u.Id });
                }
                else
                {
                    conn.Execute("UPDATE Usuarios SET Usuario = @Usuario, Nivel = @Nivel WHERE Id = @Id",
                        new { Usuario = u.Username, Nivel = u.Nivel, Id = u.Id });
                }
            }
        }

        public void Excluir(int id)
        {
            using (var conn = db.GetConnection())
            {
                conn.Execute("UPDATE Usuarios SET Ativo = 0 WHERE Id = @Id", new { Id = id });
            }
        }
    }

    public static class AuthSession
    {
        public static int Id { get; set; }
        public static string Usuario { get; set; }
        public static string Nivel { get; set; }
        public static bool IsAdmin => Nivel == "Admin";
    }
}
