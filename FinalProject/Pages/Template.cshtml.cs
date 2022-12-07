using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using FinalProject.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Services.JobDataService;
using Services.JobService;
using Services.WebsiteService;
using Services.WebsiteStatusService;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace FinalProject.Pages
{
// [Authorize]
    public class TemplateModel : PageModel
    {
        [BindProperty]
        public Job? Job { get; set; }
        public List<Job>? Jobs { get; set; }
        public JobResult? JobResult { get; set; }
        public List<JobResult>? JobResults { get; set; }

         public string? JSONOutput { get; set; }

        public async Task OnGetAsync()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:3000/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                try
                {
                    JobResults = await client.GetFromJsonAsync<List<JobResult>>("jobresults/all");
                    Jobs = await client.GetFromJsonAsync<List<Job>>("jobs/all");
                }
                catch (Exception exp)
                {
                    Console.Error.WriteLine($"Problem: {exp.Message}");
                }
            }
        }

        public async Task<IActionResult> OnPostAsync()
    {
        if (ModelState.IsValid)
        {
            Job!.JobActionType = (JobActionType)Job.JobActionType;
                       
            Job.JobScheduledAt = DateTime.Now;

            
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:3000/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                
                HttpResponseMessage response = await client.PostAsJsonAsync("jobs/create", Job);

                if (response.IsSuccessStatusCode)
                {
                   
                    Uri? returnUrl = response.Headers.Location;
                    JSONOutput += returnUrl;
                    Console.WriteLine(returnUrl);
                }
            }

          
            JSONOutput += $": {JsonSerializer.Serialize(Job)}";

        }
        
        return Page();
        }
    
    }
    
}