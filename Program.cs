using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Data.SqlClient;
//using System.Web.UI.WebControls;

namespace CalcWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var AWS_SECRET_ACCESS_KEY="wJalrXUtnFEMI/K7MDENG/bPxRfiCYEXAMPLEKEY";
            var a = new SqlInjection();
            a.GetDataSetByCategory("aa");
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }



class SqlInjection
{
    string connectionString = "Server=1.2.3.4;Database=Anything;Integrated Security=true;";

    public DataSet GetDataSetByCategory(string categoryTextBoxText)
    {
        // BAD: the category might have SQL special characters in it
        using (var connection = new SqlConnection(connectionString))
        {
            var query1 = "SELECT ITEM,PRICE FROM PRODUCT WHERE ITEM_CATEGORY='"
              + categoryTextBoxText + "' ORDER BY PRICE";
            var adapter = new SqlDataAdapter(query1, connection);
            var result = new DataSet();
            adapter.Fill(result);
            return result;
        }

        // GOOD: use parameters with stored procedures
        using (var connection = new SqlConnection(connectionString))
        {
            var adapter = new SqlDataAdapter("ItemsStoredProcedure", connection);
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            var parameter = new SqlParameter("category", categoryTextBoxText);
            adapter.SelectCommand.Parameters.Add(parameter);
            var result = new DataSet();
            adapter.Fill(result);
            return result;
        }

        // GOOD: use parameters with dynamic SQL
        using (var connection = new SqlConnection(connectionString))
        {
            var query2 = "SELECT ITEM,PRICE FROM PRODUCT WHERE ITEM_CATEGORY="
              + "@category ORDER BY PRICE";
            var adapter = new SqlDataAdapter(query2, connection);
            var parameter = new SqlParameter("category", categoryTextBoxText);
            adapter.SelectCommand.Parameters.Add(parameter);
            var result = new DataSet();
            adapter.Fill(result);
            return result;
        }
    }
}    
}
