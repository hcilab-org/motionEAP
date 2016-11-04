// <copyright file=CommunicationManager.cs
// <copyright>
//  Copyright (c) 2016, University of Stuttgart
//  Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the Software),
//  to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
//  and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//  The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//  THE SOFTWARE IS PROVIDED AS IS, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
//  DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE
//  OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// </copyright>
// <license>MIT License</license>
// <main contributors>
//  Markus Funk, Thomas Kosch, Sven Mayer
// </main contributors>
// <co-contributors>
//  Paul Brombosch, Mai El-Komy, Juana Heusler, 
//  Matthias Hoppe, Robert Konrad, Alexander Martin
// </co-contributors>
// <patent information>
//  We are aware that this software implements patterns and ideas,
//  which might be protected by patents in your country.
//  Example patents in Germany are:
//      Patent reference number: DE 103 20 557.8
//      Patent reference number: DE 10 2013 220 107.9
//  Please make sure when using this software not to violate any existing patents in your country.
// </patent information>
// <date> 11/2/2016 12:25:59 PM</date>

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using motionEAPAdmin.ContentProviders;
using motionEAPAdmin.Backend;
using Newtonsoft.Json;
using System.Net;
using Newtonsoft.Json.Linq;

/*
 * Author: Vincenzo Baldini
 * Version: 1.0 beta
 * Info: CommunicationManager
 *       Ermöglicht über URL Befehle eine Kommunikation
 *       mit dem Server. Dabei können sowohl, über eine auf dem
 *       Server implementierte RESTful Schnittstelle, Werte
 *       übergeben und in einer Datenbank gespeichert werden,
 *       als auch Werte im JSON Format erhalten werden.
 * Wichtig: Verwendete NuGet-Pakete: 
 *          Microsoft HTTP Client Libraries
 *          JSON.NET
 * 
 */

namespace motionEAPAdmin.Network
{
    class CommunicationManager
    {
        /// <summary>
        /// the instance
        /// </summary>
        private static CommunicationManager m_Instance;
        
        //Tisch attribute

        public int PID { get; set; }
        public Boolean onlinestate { get; set; }
        public int schrittnummer { get; set; }
        public int produzierteteile { get; set; }
        public String authentification { get; set; }
        //Workflow attribute
        public int WID { get; set; }
        public String name { get; set; }
        public String jasonpfad { get; set; }
        //Aktueller Arbeitsschritt attribute
        public int CID { get; set; }
        public string timestamp { get; set; }
        public int tid { get; set; }
        public int wid { get; set; }
        public string tableName { get; set; }

        //HttpClient zum erstellen einer Verbindung
        HttpClient client = new HttpClient();

        bool m_Disabled = true;

        ServerInfo m_ServerInfo = new ServerInfo();

        public CommunicationManager()
        {
            this.authentification = SettingsManager.Instance.Settings.NetworkAuthToken;
            this.tableName = SettingsManager.Instance.Settings.NetworkTableName;
            if (SettingsManager.Instance.Settings.ServerBaseAddress == null) {
                SettingsManager.Instance.Settings.ServerBaseAddress = "http://projects.hci.simtech.uni-stuttgart.de:8081/motionEAPserver/";
            }
            client.BaseAddress = new Uri(SettingsManager.Instance.Settings.ServerBaseAddress);
            //Header for authentication
            client.DefaultRequestHeaders.Add("auth-token", authentification);
            SetupEventProcessing();
            m_ServerInfo.Tables = getTableList().Result;
            m_ServerInfo.SelfStatus = getOwnTableStatus().Result;
        }

        public static CommunicationManager Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new CommunicationManager();
                }
                return m_Instance;
            }
        }

        //TODO: Maybe put this configuration into a dedicated file?
        private void SetupEventProcessing()
        {
            WorkflowManager.Instance.WorkingStepStarted += WorkingStepStarted;
            WorkflowManager.Instance.WorkingStepCompleted += WorkingStepCompleted;
        }

        private void WorkingStepStarted(object sender, WorkingStepStartedEventArgs e)
        {
            if (e.WorkingStepNumber == 0)
            {
                SendWorkflowStarted(e.LoadedWorkflow.Id);
            }
            if (e.WorkingStepNumber < e.LoadedWorkflow.WorkingSteps.Count - 1) SendWorkingStepStarted(e.WorkingStepNumber, e.LoadedWorkflow.Id);
        }

        private void WorkingStepCompleted(object sender, WorkingStepCompletedEventArgs e)
        {
            if (e.WorkingStepNumber == e.LoadedWorkflow.WorkingSteps.Count - 2)
            {
                SendWorkflowCompleted(e.LoadedWorkflow.Id, e.StepEndTime - e.WorkflowStartTime);
            }
            if (e.WorkingStepNumber < e.LoadedWorkflow.WorkingSteps.Count - 1) SendWorkingStepCompleted(e.WorkingStepNumber, e.StepDurationTime, e.LoadedWorkflow.Id);
        }

        private Task<HttpResponseMessage> postRequest(string url, object request)
        {
            return client.PostAsync(url, new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"));
        }

        public void SetTable(String auth)
        {
            var url = "tisch/add/" + auth;
            var send = client.GetAsync(url);
        }

        /*
        {
            errorCode: 21
            workflowId: 0
            errorCount: 0
            duration: null
            workingStepNumber: 0
            workingStepDuration: 0
        }
         */

        public void SendWorkflowStarted(string workflowId)
        {
            var url = "table/update/startedWorkflow/" + authentification;
            var request = new HttpWorkflowStartedRequest() { workflowId = 1 };
            //var send = client.PostAsJsonAsync(url, request);
            var send = postRequest(url,request);
            updateOwnTableStatus();
        }

        public void SendWorkingStepStarted(int stepNumber, string workflowId)
        {
            var url = "table/update/startedWorkingstep/" + authentification;
            var request = new HttpWorkingStepStartedRequest() { workingStepNumber = stepNumber };
            var send = postRequest(url, request);
        }

        public void SendWorkflowCompleted(string workflowId, long duration)
        {
            var url = "table/update/completedWorkflow/" + authentification;
            var request = new HttpWorkflowCompletedRequest() { workflowId = 1, duration = duration };
            //var send = client.PostAsJsonAsync(url, request);
            var send = postRequest(url, request);
            updateOwnTableStatus();
        }

        public void SendWorkingStepCompleted(int stepNumber, long duration, string workflowId)
        {
            var url = "table/update/completedWorkingstep/" + authentification;
            var request = new HttpWorkingStepCompletedRequest() { workingStepNumber = stepNumber, workingStepDuration = duration};
            var send = postRequest(url, request);
        }

        public async void updateOwnTableStatus()
        {
            m_ServerInfo.SelfStatus = await getOwnTableStatus();
        }

        public Task<NetworkTableStatus> getOwnTableStatus()
        {
            return getTableStatus(this.tableName);
        }

        public async Task<NetworkTableStatus> getTableStatus(string tableId)
        {
            if (tableId == "") return null;
            var url = "table/" + tableId + "/status";
            if (m_Disabled)
            {
                return null;
            }
            try
            {
                var jsonString = await client.GetStringAsync(url);
                JObject table = JsonConvert.DeserializeObject<JObject>(jsonString);
                NetworkTableStatus result = new NetworkTableStatus();
                foreach (JToken token in table.Children())
                {
                    if (token is JProperty)
                    {
                        var prop = token as JProperty;
                        if (prop.Name == "produzierteteile") result.ProducedParts = (int)prop.Value;
                        if (prop.Name == "schrittnummer") result.StepNumber = (int)prop.Value;
                        if (prop.Name == "estimateTime") result.EstimatedTimeLeft = (int)prop.Value;
                    }
                }
                return await Task.Run(() => result);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async void setProducedParts(int num)
        {
            var url = "table/update/produzierteteile/" + authentification + "/" + num;
            try
            {
                //This should probably be a post request
                var jsonString = await client.GetStringAsync(url);
            }
            catch (WebException)
            {

            }
        }

        public async Task<int> getTableWorkingStepNumber(string tableId)
        {
            var url = "table/"+ tableId + "/status";
            try 
            {
                var jsonString = await client.GetStringAsync(url);
                dynamic response = JsonConvert.DeserializeObject<dynamic>(jsonString);
                //TODO: Check that property matches with returned json object
                return await Task.Run(() => response.StepNumber);
            }
            catch (WebException)
            {
                return -1;
            }
        }

        public async Task<int> getTableProducedPartsNumber(string tableId)
        {
            var url = "table/" + tableId + "/status";
            try
            {
                var jsonString = await client.GetStringAsync(url);
                JObject table = JsonConvert.DeserializeObject<JObject>(jsonString);
                int result = -1;
                foreach (JToken token in table.Children())
                {
                    if (token is JProperty)
                    {
                        var prop = token as JProperty;
                        if (prop.Name == "produzierteteile") result = (int)prop.Value;
                    }
                }
                //TODO: Check that property matches with returned json object
                return await Task.Run(() => result);
            }
            catch (WebException)
            {
                return -1;
            }
        }

        public async Task<List<TableInfo>> getTableList()
        {
            var url = "table/get/json";
            if (m_Disabled)
            {
                return new List<TableInfo>();
            }
            try
            {
                var jsonString = await client.GetStringAsync(url);
                JObject[] response = JsonConvert.DeserializeObject<JObject[]>(jsonString);
                //TODO: Parse Result
                
                var results = new List<TableInfo>();
                foreach (var table in response)
                {
                    var tInfo = new TableInfo();
                    foreach (JToken token in table.Children())
                    {
                        if (token is JProperty)
                        {
                            var prop = token as JProperty;
                            switch (prop.Name)
                            {
                                case "id":
                                    tInfo.Id = prop.Value + "";
                                    break;
                                case "name":
                                    tInfo.Name = prop.Value + "";
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    results.Add(tInfo);
                }
                return await Task.Run(() => results);
            }
            catch (Exception)
            {
                return new List<TableInfo>();
            }
        }

        public void setOnlinestate(Boolean isOnline)
        {
            //Login currently Disabled
            //if (isOnline)
            //{
                
            //    var url = "tisch/login/" + this.authentification;
            //    var send = client.GetAsync(url);
            //}
            //else
            //{
            //    var url = "tisch/logout/" + this.authentification;
            //    var send = client.GetAsync(url);
            //}
        }

        public void setWorkflowName()
        {
            this.name = name;

        }

        public ServerInfo ServerInfo
        {
            get { return m_ServerInfo; }
            set { m_ServerInfo = value; }
        }



        
    }

    class HttpWorkflowStartedRequest
    {
        public int workflowId { get; set; }
    }
    
    class HttpWorkflowCompletedRequest
    {
        public int workflowId { get; set; }

        public long duration { get; set; }
    }
    
    class HttpWorkingStepStartedRequest
    {
        public int workingStepNumber { get; set; }
        //public string WorkflowId { get; set; }
    }

    class HttpWorkingStepCompletedRequest
    {
        public int workingStepNumber { get; set; }
        public long workingStepDuration { get; set; }
    }

}
