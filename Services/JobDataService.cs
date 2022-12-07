namespace Services.JobDataService;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Services.WebsiteService;
using Services.JobService;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;


public class JobDbContext : DbContext
{
    public JobDbContext()
    {
        
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    
        string? dirPath = Assembly.GetExecutingAssembly().Location;
        dirPath = Path.GetDirectoryName(dirPath);


        string dbfilename = "ApiDB.sqlite";
        string connectionString = Path.GetFullPath(Path.Combine(dirPath!, dbfilename));

        
        optionsBuilder.UseSqlite($"DataSource={connectionString}");

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
        // modelBuilder.Entity<Job>().ToTable("Jobs");
        // modelBuilder.Entity<JobResult>().ToTable("JobResults");

        base.OnModelCreating(modelBuilder);
    }

    public DbSet<Job> Jobs => Set<Job>();
    public DbSet<JobResult> JobResults => Set<JobResult>();

    
    public async Task<Job> AddJobAsync(Job job)
    {
        using (var db = new JobDbContext())
        {
            db.Jobs.Add(job);
            await db.SaveChangesAsync();
        }
        return job;
    }

    
    public async Task<JobResult> AddJobResultAsync(JobResult result)
    {
        using (var db = new JobDbContext())
        {
            db.JobResults.Add(result);
            await db.SaveChangesAsync();
        }
        return result;
    }

    public async Task<Job> GetJobByIdAsync(int id)
    {

        Job? job;
        using (var db = new JobDbContext())
        {
            job = await db.Jobs
                        .Where(r => r.ID == id)
                        .FirstOrDefaultAsync<Job>();

        }
        return job!;

    }

    public async Task UpdateJobTimeStampByIdAsync(int id)
    {
        Job? job;
        using (var db = new JobDbContext())
        {
            job = await db.Jobs
                        .Where(r => r.ID == id)
                        .FirstOrDefaultAsync<Job>();

            if(job != null){
                job.JobScheduledAt = DateTime.Now;
                //save changes
                await db.SaveChangesAsync();
            }                          
        }
    }

    public async Task<List<Job>> GetJobsDueAsync()
    {

        List<Job> currentJobs;

        using (var db = new JobDbContext())
        {

            var allJobs = await db.Jobs.ToListAsync<Job>();

            currentJobs = allJobs
                .Where(j => (Math.Abs(DateTime.Now.Minute - j.JobScheduledAt.Minute)) > j.JobFrequencyInMinutes)
                .ToList<Job>();

        }
        return currentJobs!;
    }

    public bool CheckJobTimer(Job job)
    {
        TimeSpan difference = DateTime.Now - job.JobScheduledAt;
        return difference.TotalMinutes >= job.JobFrequencyInMinutes ? true : false;
    }

    public async Task<List<Job>> GetJobsAsync()
    {
        List<Job> jobs = new List<Job>();
        using (var db = new JobDbContext())
        {
            jobs = await db.Jobs.ToListAsync<Job>();
        }
        return jobs;
    }

    public async Task<List<JobResult>> GetJobResultsAsync()
    {
        List<JobResult> jobresults = new List<JobResult>();
        using (var db = new JobDbContext())
        {
            jobresults = await db.JobResults.ToListAsync<JobResult>();
        }
        return jobresults;
    }
}
