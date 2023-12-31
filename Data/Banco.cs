﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data.Entity;
using System.Data;
using System.Windows.Forms;
using System.Drawing;
using System.Security.Cryptography.X509Certificates;

namespace Academia_AMS
{
    internal class Banco
    {
        private static SQLiteConnection connection;
        public static string bancos { get; private set; } = Pessoa.bancoUsuario;

        public static SQLiteConnection OpenConnection(string banco)
        {   
            
            connection = new SQLiteConnection(banco);
            connection.Open();
            return connection;

        }

        public static DataTable ObterUsuarios()
        {
            SQLiteDataAdapter adapter = null;
            DataTable dataTable = new DataTable();

            try
            {

                using (var cmd = OpenConnection(bancos).CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM fit_info";
                    adapter = new SQLiteDataAdapter(cmd.CommandText, OpenConnection(bancos));
                    adapter.Fill(dataTable);

                    OpenConnection(bancos).Close();
                    return dataTable;

                }

            }
            catch (Exception ex)
            {
                throw ex;
                
            }
           

        }

       

        ///informaçoes do usuario (Cadastro)

        public virtual void NovoUsuario(Pessoa u)
        {
            if (VerificarNumber(u))
            {
                MessageBox.Show("Numero De Telefone Já Cadastrado");
                return;

            }
            try
            {
                var cmd = OpenConnection(bancos).CreateCommand();
                cmd.CommandText = "INSERT INTO fit_info (T_NAME, N_DATA, T_HORARIO, N_TELEFONE, T_OBS, N_CPF) VALUES (@nome, @data, @horario, @telefone, @obs, @cpf)";

                cmd.Parameters.AddWithValue("@nome", u.T_NAME);
                cmd.Parameters.AddWithValue("@data", u.N_DATA);     
                cmd.Parameters.AddWithValue("@horario", u.T_HORARIO);
                cmd.Parameters.AddWithValue("@telefone", u.N_TELEFONE);  
                cmd.Parameters.AddWithValue("@obs", u.T_OBS);
                cmd.Parameters.AddWithValue("@cpf", u.N_CPF);

                cmd.ExecuteNonQuery();
                MessageBox.Show("Usuario Cadastrado");

                OpenConnection(bancos).Close();


            }

            catch
            {
                MessageBox.Show("Não Foi Possivel Adicionar O Usuario Tente Novamnte");
               
            }
            

        }

        //Rotinas Gerais

        protected static bool VerificarNumber(Pessoa u)
        {

            bool resultado;
            SQLiteDataAdapter adapter = null;
            DataTable dataTable = new DataTable();

            using (var cmd = OpenConnection(bancos).CreateCommand())
            {
                try
                {

                    cmd.CommandText = "SELECT N_TELEFONE FROM fit_info WHERE N_TELEFONE ='" + u.N_TELEFONE + "'";
                    adapter = new SQLiteDataAdapter(cmd.CommandText, OpenConnection(bancos));
                    adapter.Fill(dataTable);
                    if (dataTable.Rows.Count > 0)
                    {
                        resultado = true;

                    }
                    else
                    {
                        resultado = false;

                    }

                    OpenConnection(bancos).Close();
                    return resultado;
                }catch (Exception ex)
                {
                    throw ex;
                }
                
                

            }

            

        }

        public  Pessoa ObterUsuarioPorCPF(int cpf)
        {
            using (var connection = OpenConnection(bancos))
            using (var cmd = connection.CreateCommand())
            {
                try
                {
                    cmd.CommandText = "SELECT * FROM fit_info WHERE N_CPF = @cpf";
                    cmd.Parameters.AddWithValue("@cpf", cpf);

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Cria um objeto Usuario e preenche com os dados do banco
                            Pessoa usuario = new Pessoa
                            {
                                T_NAME = reader["T_NAME"].ToString(),
                                N_DATA = (reader["N_DATA"].ToString()),
                                T_HORARIO = reader["T_HORARIO"].ToString(),
                                N_TELEFONE = long.Parse(reader["N_TELEFONE"].ToString()),
                                T_OBS = reader["T_OBS"].ToString(),
                                N_CPF = long.Parse(reader["N_CPF"].ToString())
                            };

                            return usuario;
                        }
                    }

                    return null; // Retorna null se o usuário não for encontrado
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

    

        public virtual void DeletarUsuario(int cpf)
        {
                
            try
            {   
                   
                using (var vcon = OpenConnection(bancos))
                {
                    if (ObterUsuarioPorCPF(cpf) != null)
                    {
                        var cmd = vcon.CreateCommand();
                        cmd.CommandText = "DELETE FROM fit_info WHERE N_CPF = @cpf";
                        cmd.Parameters.AddWithValue("@cpf", cpf);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Usuario Excluido Com Sucesso");
                        return;
                    }
                    else
                    {
                        MessageBox.Show("Usuario Não Encontrado");
                    }
                   
                }
            }
            catch 
            {

                MessageBox.Show("Usuario Não Encontrado");
            }
        }
    }
  }