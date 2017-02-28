using LossRunsNavClient.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace LossRunsNavClient.Request
{
    class BatchRequest
    {
        Uri baseUrl;
        BatchData jsonBatch;

        public BatchRequest()
        {
            baseUrl = new Uri("https://dev.btisinc.com/LossRuns/");
        }

        public BatchRequest(string passedUrl)
        {
            baseUrl = new Uri(passedUrl);
        }

        public async Task<BatchData> GetNextBatchAsync()
        {
            var result = await RunAsync(BatchDataHttpMethod.GET);
            Debug.WriteLine("Started Batch Request");
            return result;
        }

        public async Task UpdateBatch(BatchData batch)
        {
            await RunAsync(BatchDataHttpMethod.POST);
        }

        public async Task<GlobalStats> GetStats()
        {
            return await RunStatsAsync("api/globalstats/1");
        }

        async Task<BatchData> RunAsync(BatchDataHttpMethod type)
        {
            BatchData batch = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = baseUrl;

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11;

                // Get:
                if (type == BatchDataHttpMethod.GET)
                {
                    HttpResponseMessage response = await client.GetAsync("api/batchdata/1");
                    if (response.IsSuccessStatusCode)
                    {
                        Debug.WriteLine("Batch Request Recieved");
                        return batch = await response.Content.ReadAsAsync<BatchData>(); // Install-Package Microsoft.AspNet.WebApi.Client
                                                                                        //Console.WriteLine("{0}\t${1}\t{2}", batch.Key, batch.Data, batch.LastComplete);
                    }
                    else
                    {
                        Debug.WriteLine("Batch Request Failed");
                        return batch;
                    }
                }

                // Post
                if (type == BatchDataHttpMethod.POST)
                {
                    HttpResponseMessage response = await client.PostAsJsonAsync<BatchData>("api/incbatchdata", jsonBatch);
                    if (response.IsSuccessStatusCode)
                    {
                        Debug.WriteLine("Batch Posted");
                        return null;                                                          
                    }
                    else
                    {
                        Debug.WriteLine("Batch Request Failed");
                        return null;
                    }
                }
                return null;
            }
        }

        async Task<GlobalStats> RunStatsAsync(string method)
        {
            GlobalStats batch = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = baseUrl;

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11;

                // New code:
                HttpResponseMessage response = await client.GetAsync(method);
                if (response.IsSuccessStatusCode)
                {
                    Debug.WriteLine("Batch Request Recieved");
                    return batch = await response.Content.ReadAsAsync<GlobalStats>(); // Install-Package Microsoft.AspNet.WebApi.Client
                    //Console.WriteLine("{0}\t${1}\t{2}", batch.Key, batch.Data, batch.LastComplete);
                }
                else
                {
                    Debug.WriteLine("Batch Request Failed");
                    return batch;
                }
            }
        }

        enum BatchDataHttpMethod
        {
            GET,
            PUT,
            POST
        }
    }
}
