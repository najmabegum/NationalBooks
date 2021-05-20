using System;
using System.Collections.Generic;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NationalBooks.Data;
using NationalBooks.Data.Models;
using SendGrid.Helpers.Mail;

namespace NationalBooks.Functions
{
    public static class SendEmailWhenCookiesAreOrderedFunction
    {
        [FunctionName("SendEmailWhenCookiesAreOrderedFunction")]
        public static async void Run([CosmosDBTrigger(
            databaseName: "BooksDatabase",
            collectionName: "Orders",
            ConnectionStringSetting = "CosmosDBConnectionString",
            LeaseCollectionName = "leases", CreateLeaseCollectionIfNotExists =true)]
            IReadOnlyList<Document> input,
            [SendGrid(ApiKey = "SendGridKey")] IAsyncCollector<SendGridMessage> messageCollector,
            ExecutionContext context)
        {
            if (input != null && input.Count > 0)
            {
                Order order = (Order)(dynamic)input[0];
                IConfigurationRoot config = new ConfigurationBuilder()
                    .SetBasePath(context.FunctionAppDirectory)
                    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();

                var websiteUrl = config["NationalCookiesUrl"];

                var content = "You have a new order <br/><br/>" +
                    "Oder date: " + order.Date.ToString("ddMMyyyy") + "<br/>" +
                    "Price: €" + order.Price + "<br/><br/>" +
                    "More details <a href='" + websiteUrl + "/Order/Detail?id=" + order.Id + "'> here</a>";

                SendGridMessage message = new SendGridMessage();
                message.AddTo(""); /* To address - This person receives the email (We must change the website such that on order the user enters his email. For testing I hardcoded this to my email address) */
                message.AddContent("text/html", content);
                message.SetFrom(new EmailAddress("")); /* This email address must be verified in your send grid account as a verified sender. Else the email will not be sent*/
                message.SetSubject("You have a new order!");

                await messageCollector.AddAsync(message);

            }
        }
    }
}
