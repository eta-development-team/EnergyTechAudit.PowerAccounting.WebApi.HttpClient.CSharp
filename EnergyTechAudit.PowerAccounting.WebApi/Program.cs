﻿using System;
using System.IO;
using System.Net;
using System.Net.Http;

namespace EnergyTechAudit.PowerAccounting.WebApi
{
    internal class Program
    {
        public static void Main()
        {
            using (var client = new HttpClient())
            {
                var startDateTime = DateTime.Parse("2015-04-01");
                var endDateTime = DateTime.Parse("2015-04-30");

                // для возможности использования самоподписанного сертификата сервера 
                // после установки на сервер заверенного сертификата вызов данного обработчика будет не нужен 
                ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) =>
                {
                    return true;
                };
                client.BaseAddress = new Uri("https://eta.asd116.ru:8433"); // 

                client.DefaultRequestHeaders.Add("login", "Archive.Downloader");
                client.DefaultRequestHeaders.Add("password", "xxx");

                var requestUri = string.Format
                (
                    "/api/package/archive?" +
                    "ar.measurementDeviceId=234&" +
                    "ar.periodTypeId=3&" +
                    "ar.withDictionaries=false&" +
                    "ar.periodBegin={0:yyyy-MM-dd}&" +
                    "ar.periodEnd={1:yyyy-MM-dd}&" +
                    "responseToFile=true",

                    startDateTime,
                    endDateTime
                );

                var response = client
                    .GetAsync(requestUri)
                    .Result;

                if (response != null && response.StatusCode == HttpStatusCode.OK)
                {
                    byte[] content = response.Content.ReadAsByteArrayAsync().Result;

                    string path = @"d:\ArchivePackageCs.xml";
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                    File.WriteAllBytes(path, content);

                }
            }
        }
    }
}
