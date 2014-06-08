using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Collections;
using Newtonsoft.Json;
using System.Diagnostics;
using MongoDB.Bson;
using MongoDB.Driver;



namespace WorkerRole1
{
    public class Customer 
    { 
        public ObjectId Id { get; set; }
        public int SqlId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
    }

    
    
    public class dbSynchro
    {


        public static String GetData()
        {
            //string results;
            string conn = "server=mssql.oldtowndining.com;database=oldtowndining;uid=oldtowndining;pwd=y5EQJ5m7C3;pooling=true;connection lifetime=120;max pool size=25;";
            string query = "Select CustomerID, COALESCE(CLastName, '') as CLastName, COALESCE(CFirstName, '') as CFirstName, COALESCE(CEmail, '') as CEmail from tblCustomers where CLastName = 'Baily'";
            List<Customer> customers = new List<Customer>();
            using (SqlConnection connection = new SqlConnection(conn))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                


                //get the data reader, etc.
                while (reader.Read())
                {
                    customers.Add(new Customer
                    {
                        SqlId = (int)reader["CustomerId"],
                        LastName = (String)reader["CLastName"],
                        FirstName = (String)reader["CFirstName"],
                        Email = (String)reader["CEmail"]
                    });
                }


                //clean up datareader


                //Console.WriteLine(JsonConvert.SerializeObject(objs));
                //Console.ReadLine();
                //results = JsonConvert.SerializeObject(objs);


            }
            //return new string[] { "Bid1", "Bid2" };
            //Trace.Write(results);

            var connectionString = "mongodb://localhost/tipminer";
            var client = new MongoClient(connectionString);
            var server = client.GetServer();
            var database = server.GetDatabase("tipminer");
            var collection = database.GetCollection<Customer>("customers");
            //var entity = new Entity { Name = "Chris" };

            foreach (Customer c in customers)
            {
                collection.Insert(c);
                var id = c.Id;
                Trace.Write("the document id is " + id.ToString());
            }
            
            



            return "hello";
        }
    }

}
