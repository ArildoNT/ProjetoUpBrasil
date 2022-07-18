using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Dapper;
using ProjetoUpBrasil.Models;

namespace ProjetoUpBrasil.Controllers
{
    public class HomeController : Controller
    {
        protected string mainConnection = ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString;

        public ActionResult Index()
        {
            PublicarClientes();
            ViewBag.Buscou = false;
            return View();
        }

        public ActionResult CadastrarCliente()
        {
            return View();
        }

        private void PublicarClientes()
        {
            SqlConnection sqlConnect = new SqlConnection(mainConnection);

            string sqlQuery = "select idcliente, NomeCliente, descricao, ativo, Dtcadastro from cliente";

            var clientes = sqlConnect.Query<Cliente>(sqlQuery);
            ViewBag.Clientes = clientes;
        }

        [HttpPost]
        public ActionResult CadastrarCliente(string descricao, string nomeCliente)
        {
            try
            {
                using (var con = new SqlConnection(mainConnection))
                {
                    var command = "Insert into [cliente] (nomeCliente, descricao, ativo, dtCadastro) values (@nomeCliente, @descricao, 1, SYSDATETIME());select @@IDENTITY;";
                    con.Execute(command, new { nomeCliente = nomeCliente, descricao = descricao });
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("CadastrarCliente");
            }
        }

        [HttpPost]
        public ActionResult ExcluirCadastro(int codCliente)
        {
            using (var con = new SqlConnection(mainConnection))
            {
                var command = "delete from [cliente] where idCliente = @idCliente";
                con.Execute(command, new { idCliente = codCliente });
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult EditarCadastro(int idCliente, string descricao, string nomeCliente, int ativo)
        {
            using (var con = new SqlConnection(mainConnection))
            {
                var command = "update [cliente] set descricao = @descricao where idCliente = @idCliente";
                con.Execute(command, new { idCliente = idCliente, descricao = descricao });
                var command1 = "update [cliente] set nomeCliente = @nomeCliente where idCliente = @idCliente";
                con.Execute(command1, new { idCliente = idCliente, nomeCliente = nomeCliente });
                var command2 = "update [cliente] set ativo = @ativo where idCliente = @idCliente";
                con.Execute(command2, new { idCliente = idCliente, ativo = ativo });
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Busca(string nomeSearch,string codSearch, int? ativosearch, DateTime? dtInicial, DateTime? dtFinal)
        {
            StringBuilder comandoSql = new StringBuilder();
            SqlConnection sqlConnect = new SqlConnection(mainConnection);
            bool primeiraCondicao = true;

            comandoSql.AppendLine("SET DATEFORMAT DMY;");
            comandoSql.AppendLine("select idcliente, NomeCliente, descricao, ativo, Dtcadastro from cliente");

            if (!string.IsNullOrEmpty(nomeSearch))
            {
                if (primeiraCondicao)
                {
                    comandoSql.AppendLine(" Where NomeCliente like '%" + nomeSearch + "%'");
                    primeiraCondicao = false;
                }
                else
                {
                    comandoSql.AppendLine(" And NomeCliente like '%" + nomeSearch + "%'");
                }
            }

            if (!string.IsNullOrEmpty(codSearch))
            {
                if (primeiraCondicao)
                {
                    comandoSql.AppendLine($" Where idcliente = {codSearch}");
                    primeiraCondicao = false;
                }
                else
                {
                    comandoSql.AppendLine($" And idcliente = {codSearch}");
                }
            }

            if (ativosearch != null)
            {
                if (primeiraCondicao)
                {
                    comandoSql.AppendLine($" Where ativo = {ativosearch}");
                    primeiraCondicao = false;
                }
                else
                {
                    comandoSql.AppendLine($" And ativo = {ativosearch}");
                }
            }

            if (dtInicial != null && dtFinal != null)
            {
                if(dtInicial == dtFinal)
                {
                    if (primeiraCondicao)
                    {
                        comandoSql.AppendLine($" Where YEAR(Dtcadastro) = YEAR('{dtInicial.ToString().Replace(" 00:00:00", "")}') and MONTH(Dtcadastro) = MONTH('{dtInicial.ToString().Replace(" 00:00:00", "")}') and DAY(Dtcadastro) = DAY('{dtInicial.ToString().Replace(" 00:00:00", "")}')");
                        primeiraCondicao = false;
                    }
                    else
                    {
                        comandoSql.AppendLine($" AND YEAR(Dtcadastro) = YEAR('{dtInicial.ToString().Replace(" 00:00:00", "")}') and MONTH(Dtcadastro) = MONTH('{dtInicial.ToString().Replace(" 00:00:00", "")}') and DAY(Dtcadastro) = DAY('{dtInicial.ToString().Replace(" 00:00:00", "")}')");
                    }
                }                
                else if (primeiraCondicao)
                {
                    comandoSql.AppendLine($" Where DAY(Dtcadastro) between DAY('{dtInicial.ToString().Replace(" 00:00:00", "")}') and DAY('{dtFinal.ToString().Replace(" 00:00:00", "")}')");
                    comandoSql.AppendLine($" AND YEAR(Dtcadastro) between YEAR('{dtInicial.ToString().Replace(" 00:00:00", "")}') and YEAR('{dtFinal.ToString().Replace(" 00:00:00", "")}')");
                    comandoSql.AppendLine($" AND MONTH(Dtcadastro) between MONTH('{dtInicial.ToString().Replace(" 00:00:00", "")}') and MONTH('{dtFinal.ToString().Replace(" 00:00:00", "")}')");
                    primeiraCondicao = false;
                }
                else
                {
                    comandoSql.AppendLine($" AND DAY(Dtcadastro) between DAY('{dtInicial.ToString().Replace(" 00:00:00", "")}') and DAY('{dtFinal.ToString().Replace(" 00:00:00", "")}')");
                    comandoSql.AppendLine($" AND YEAR(Dtcadastro) between YEAR('{dtInicial.ToString().Replace(" 00:00:00", "")}') and YEAR('{dtFinal.ToString().Replace(" 00:00:00", "")}')");
                    comandoSql.AppendLine($" AND MONTH(Dtcadastro) between MONTH('{dtInicial.ToString().Replace(" 00:00:00", "")}') and MONTH('{dtFinal.ToString().Replace(" 00:00:00", "")}')");
                }
            }

            comandoSql.AppendLine(" Order by NomeCliente");

            var clientes = sqlConnect.Query<Cliente>(comandoSql.ToString());
            ViewBag.Clientes = clientes;
            ViewBag.Buscou = true;

            return View("Index");
        }
    }
}