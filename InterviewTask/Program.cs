using System.Net.Mail;
using FleetManager;
using Microsoft.Data.Sqlite;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet(
        "/weatherforecast/{vesselName}",
        async (string vesselName) =>
        {
            var log = new Log();
            
            const string connStr = "Data Source=database.db;";

            using (var dbConnection = new SqliteConnection(connStr))
            {
                dbConnection.Open();

                var command = dbConnection.CreateCommand();

                if (!string.IsNullOrEmpty(vesselName))
                    command.CommandText = $"SELECT Id FROM Vessels WHERE Name = '{vesselName}'";
                else
                {
                    command.CommandText = "SELECT Id FROM Vessels";
                }

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var id = reader["Id"].ToString();

                        using var dbConnection2 = new SqliteConnection(connStr);
                        dbConnection2.Open();

                        var command2 = dbConnection2.CreateCommand();
                        command2.CommandText = "SELECT * FROM CrewMember WHERE VesselId = $id";
                        command2.Parameters.AddWithValue("$id", id);

                        using (var reader2 = command2.ExecuteReader())
                        {
                            while (reader2.Read())
                            {
                                var name = string.Join(" ", reader2["FirstName"].ToString(), reader2["LastName"].ToString());
                                var emailAddress = reader2["EmailAddress"].ToString();

                                var basicAuthenticationInfo = new System.Net.NetworkCredential("myUsername", "myPassword");
                                // Send email
                                var mySmtpClient = new SmtpClient("smtp.gmail.com");
                                mySmtpClient.UseDefaultCredentials = false;
                                mySmtpClient.Credentials = basicAuthenticationInfo;

                                var from = new MailAddress("no-reply@shippingcompany.com", "Shipping Company");
                                var to = new MailAddress(emailAddress.Trim(), name);
                                var email = new MailMessage(from, to);

                                email.Subject = "Latest document update";
                                email.Body = $"Dear {name}, please find the latest documents attached.";

                                var attachments = new List<Attachment>();
                                foreach (var file in Directory.GetFiles("Documents", "*.pdf"))
                                {
                                    attachments.Add(new Attachment(file));
                                }

                                foreach (var attachment in attachments)
                                {
                                    email.Attachments.Add(attachment);
                                }

                                log.Info($"Sending email to {emailAddress}.");

                                await mySmtpClient.SendMailAsync(email);
                            }
                        }

                        dbConnection2.Close();
                    }
                }
            }

            return "Ok";
        })
    .WithName("VesselReporterApi")
    .WithOpenApi();

app.Run();