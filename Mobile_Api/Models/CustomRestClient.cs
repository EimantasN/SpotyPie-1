﻿using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mobile_Api.Models
{
    public class CustomRestClient : RestClient
    {
        private static IRestRequest GET { get; set; } = new RestRequest(Method.GET);

        private static IRestRequest POST { get; set; } = new RestRequest(Method.POST);

        private bool Active { get; set; }

        public long Id { get; set; }

        public CustomRestClient()
        {
            Id = DateTime.Now.Ticks;
        }

        public CustomRestClient(string baseUrl)
        {
            Id = DateTime.Now.Ticks;
            BaseUrl = new Uri(baseUrl);
        }

        public long GetId()
        {
            return Id;
        }

        private IRestRequest GetRequest(Method method)
        {
            switch (method)
            {
                case Method.GET:
                    return GET;
                case Method.POST:
                    return POST;
            }

            throw new Exception("Must choose request type");
        }

        public async Task<List<T>> CustomGetList<T>(Method method)
        {
            try
            {
                IRestResponse response;
                if (method == Method.GET)
                    response = await base.ExecuteGetTaskAsync(GET);
                else
                    response = await base.ExecuteTaskAsync(POST);

                if (response.IsSuccessful)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                        return new List<T>();
                    return JsonConvert.DeserializeObject<List<T>>(response.Content);
                }
                else
                {
                    return new List<T>();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<T> CustomGetObject<T>(Method method)
        {
            try
            {
                IRestResponse response;
                if (method == Method.GET)
                    response = await base.ExecuteGetTaskAsync(GET);
                else
                    response = await base.ExecuteTaskAsync(POST);

                if (response.IsSuccessful)
                {
                    return JsonConvert.DeserializeObject<T>(response.Content);
                }
                else
                {
                    return default(T);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<T> PostCustomObject<T>(string parameters)
        {
            try
            {
                IRestResponse response;
                var request = new RestRequest(Method.POST);
                request.AddHeader("content-type", "application/x-www-form-urlencoded");
                request.AddParameter("application/x-www-form-urlencoded", parameters, ParameterType.RequestBody);
                response = await base.ExecuteTaskAsync(request);
                if (response.IsSuccessful)
                {
                    return JsonConvert.DeserializeObject<T>(response.Content);
                }
                else
                {
                    return default(T);
                }

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task PostCustomObject(string parameters)
        {
            try
            {
                IRestResponse response;
                var request = new RestRequest(Method.POST);
                request.AddHeader("content-type", "application/x-www-form-urlencoded");
                request.AddParameter("application/x-www-form-urlencoded", parameters, ParameterType.RequestBody);
                response = await base.ExecuteTaskAsync(request);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<List<T>> PostCustomObjectList<T>(string parameters)
        {
            try
            {
                IRestResponse response;
                var request = new RestRequest(Method.POST);
                request.AddHeader("content-type", "application/x-www-form-urlencoded");
                request.AddParameter("application/x-www-form-urlencoded", parameters, ParameterType.RequestBody);
                response = await base.ExecuteTaskAsync(request);
                if (response.IsSuccessful)
                {
                    return JsonConvert.DeserializeObject<List<T>>(response.Content);
                }
                else
                {
                    return new List<T>();
                }

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private void TimeoutCheck(IRestRequest request, IRestResponse response)
        {
            if (response.StatusCode == 0)
            {
                //Uncomment the line below to throw a real exception.
                throw new TimeoutException("The request timed out!");
            }
        }
    }
}
