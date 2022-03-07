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
        var apiUrl = _configuration["EchoAPIServer"];
        var response = client.GetStringAsync($"http://{apiUrl}/echo/{Message}").Result;
        ViewData["EchoedMessage"] = $"Echo: {response}";
    }
}
