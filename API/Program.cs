using Services.WebsiteService;
using Services.JobService;
using Services.JobDataService;
using Services.WebsiteStatusService;
using JobWorker;
using System;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<Worker>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var websites = new[]{""};

//Endpoints
//get all website jobs
app.MapGet(
    "/jobs/all", async () =>
    {
        List<Job> jobs = await JobScheduler.GetJobsAsync();
        return jobs;
    });


app.MapGet("/jobs/due", async () => {
    List<Job> jobs = await JobScheduler.GetScheduledJobsToRunAsync();
    return jobs;
});    

//post a new website job
app.MapPost(
    "/jobs/create", 
    async (Job? job) => {
        var output = await JobScheduler.ScheduleJobAsync(job!);
        return output;
    });

// GET /obs/{station}/raw
app.MapGet(
    "/obs/{id}/raw", 
    async (string id) => {

        string? obs = await WebsiteServiceAPI.GetLatestObservationAsync(id);
        Console.WriteLine($"Returned value: {obs}");
        return obs;
    }
);

app.MapGet("/jobresults/all", async () => {
    List<JobResult> jobresults = await JobScheduler.GetJobResultsAsync();
    return jobresults;
});


app.Run("https://localhost:3000");

