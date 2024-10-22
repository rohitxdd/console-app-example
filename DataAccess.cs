using Dapper;
using Microsoft.Extensions.Configuration;
using Reconsile.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reconsile
{
    class DataAccess
    {
        private string _connectionstr;
        public DataAccess()
        {
            var configuration = new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory)
           .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
           .Build();

            string? connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                LogManager.Logger.Error("No connection string Found");
                Environment.Exit(1);
                LogManager.Logger.Information("Finishing with error! Bye");
            }
            _connectionstr = connectionString;
        }

        public async Task<IEnumerable<Candidate>> ListOfCandidateWithSuccessButNotdate()
        {
            string q = @"select a.applid from feedetails a 
                        inner join JobApplication b on a.applid = b.applid
                        where trandate is null and feereq = 'Y' and final = 'Y' and dummy_no is not null 
                        and amount = 100 and feetype is null ";
            using var connection = new SqlConnection(_connectionstr);
            await connection.OpenAsync();
            var res = await connection.QueryAsync<Candidate>(q);
            return res;
        }
    }
}
