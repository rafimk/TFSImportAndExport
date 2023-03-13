using System.Text;
using System.Text.Json;
using TFSImportAndExport.Contracts;
using TFSImportAndExport.Entities;

namespace TFSImportAndExport.Services;

public class WorkItemService : IWorkItemService
{
    private readonly TfsOptions _options;

    public WorkItemService(TfsOptions options)
    {
        _options = options;
    }

     public async Task<List<ParentWorkItem>> GetWorkItemsWithAllChild(string epicWorkItemNo)
    {
        try
        {
            var url = $"{_options.SourceBaseUrl}/{_options.SourceProjectName}/_odata/v4.0-preview/WorkItems?";
            var query = "$select=WorkItemId, Title, WorkItemType, State &filter=WorkItemId eq={workItemNo} &$expand=" +
                        "Links($select=SourceWorkItemId, TargetWorkItemId, LinkTypeName; $filter=LinkTypeName eq 'parent';" +
                        "$expand=TargetWorkItem($select=WorkItemId))";

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", _options.SourceToken))));
                
                using (HttpResponseMessage httpResponse = client.GetAsync($"{url}{query}").Result)
                {
                    httpResponse.EnsureSuccessStatusCode();
                    string body = await httpResponse.Content.ReadAsStringAsync();

                    using JsonDocument doc = JsonDocument.Parse(body);
                    JsonElement root = doc.RootElement;

                    var data = root.GetProperty("value");
                    var parentWorkItems = JsonSerializer.Deserialize<List<ParentWorkItem>>(data);

                    return parentWorkItems;
                }
            }
            
        } 
        catch(Exception ex)
        {
            return new List<ParentWorkItem>();
        }
    }

    public async Task<WorkItemInfo> GetWorkItem(string workItemNo)
    {
        try
        {
            var url = $"{_options.SourceBaseUrl}/{_options.SourceProjectName}/_api/wit/workitems/{workItemNo}?{_options.SourceApiVersion}";
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", _options.SourceToken))));

                using (HttpResponseMessage httpResponse = client.GetAsync(url).Result)
                {
                    httpResponse.EnsureSuccessStatusCode();
                    string body = await httpResponse.Content.ReadAsStringAsync();

                    var json = JsonDocument.Parse(body);

                    var fields = json.RootElement.GetProperty("fields");

                    var assignToSection = fields.GetProperty("System.AssignedTo");

                    var id = json.RootElement.GetProperty("id").GetInt32();
                    var description = json.RootElement.GetProperty("System.Description").ToString();
                    var assignTo = json.RootElement.GetProperty("uniqueName").ToString();
                    var state = json.RootElement.GetProperty("System.State").ToString();
                    var tags = json.RootElement.GetProperty("System.Tags").ToString();
                    var iterationPath = json.RootElement.GetProperty("System.IterationPath").ToString();

                    return new WorkItemInfo
                    {
                        Id = id,
                        Description = description,
                        AssignTo = assignTo,
                        State = state,
                        IterationPath = iterationPath
                    };

                }
            }
        }
        catch(Exception ex)
        {
            return new WorkItemInfo();
        }
    }

    public async Task<int> CreateWit(WorkItem workItem, bool withParent)
    {
        HttpClient client = new HttpClient();
        
        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic",
            Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", _options.SourceToken))));

        var description = workItem.Description.Length > 0 ? workItem.Description : ".";

        List<Object> data = new List<Object>()
        {
            new { op = "add", path = "/fields/System.WorkItemType", from = string.Empty, value = workItem.Type },
            new { op = "add", path = "/fields/System.Title", from = string.Empty, value = $"{workItem.WorkItemNo.ToString()} - {workItem.Title}" },
            new { op = "add", path = "/fields/System.Description", from = string.Empty, value = description },
            new { op = "add", path = "/fields/System.State", from = string.Empty, value = workItem.State },
            new { op = "add", path = "/fields/System.Tags", from = string.Empty, value = $"{workItem.Tags};{workItem.AssignTo}" },
        };

        if (withParent)
        {
            data.Add(new 
            {
                op = "add",
                path = "/relations/-",
                from = string.Empty,
                value = new
                {
                    rel = "System.LinkTypes.Hierachy-Reverse",
                    urt = $"{_options.TargetBaseUrl}/{_options.TargetProjectName}/_apis/wit/workitems/{workItem.ClientParentId}",
                    attribute = new { comment = ""}
                }
            });
        }

        var jsonData = JsonSerializer.Serialize(data);
        string url = String.Join("?", String.Join("/", _options.TargetBaseUrl, _options.TargetProjectName, "_apis/wit/workitems", $"${workItem.Type}"), _options.TargetApiVersion);

        HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json-patch+json");

        var id = 0;

        var t = Task.Run(async delegate {
            string result = await CreateWIT(client, url, content);

            if (String.IsNullOrEmpty(result))
            {
                var jsonResult = JsonDocument.Parse(result);
                id = jsonResult.RootElement.GetProperty("id").GetInt32();
            }
        });

        t.Wait();

        return id;
    }

    private string GetProperty(JsonElement element, string propertyName)
    {
        try
        {
            return element.GetProperty(propertyName).ToString();
        }
        catch
        {
            return string.Empty;
        }
    }

    public async Task<string> CreateWIT(HttpClient client, string url, HttpContent content)
    {
        try
        {
            using (HttpResponseMessage response = await client.PostAsync(url, content))
            {
                response.EnsureSuccessStatusCode();
                return (await response.Content.ReadAsStringAsync());
            }
        }
        catch
        {
            return string.Empty;
        }
    }
}