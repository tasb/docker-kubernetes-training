using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using System.Net.Http;

namespace echo_webapp.Pages;

public class IndexModel : PageModel
{
    private HttpClient client;
    private readonly ILogger<IndexModel> _logger;
    private readonly IConfiguration _configuration;
    
    [BindProperty]
    public string? Message { get; set; }


    public IndexModel(ILogger<IndexModel> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
        client = new HttpClient();
    }

    public void OnGet()
    {

    }

    public void OnPost()
    {
        _logger.LogInformation("Sending message to echo service");
        var apiUrl = _configuration["EchoAPIServer"];
        var url = String.Format("http://{0}/echo/{1}" , apiUrl, Message);
        _logger.LogInformation($"Sending request to {url}");
        var response = client.GetStringAsync(url).Result;
        _logger.LogInformation($"Response from {url} is {response}");
        ViewData["EchoedMessage"] = $"Echo: {response}";
    }
}
