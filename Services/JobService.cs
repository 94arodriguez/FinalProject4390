namespace Services.JobService;

using Services.JobDataService;
using Services.WebsiteService;
using Services.WebsiteStatusService;

public enum JobActionType
{
    CHECK_STATUS,
}

public enum JobActionResult
{
    FAULT,
    OK,
}

public enum JobReconciliationAction
{
    WRITE_TO_DB,
}

//WebsiteJob model
public class Job
{
    public int ID { get; set; }
    public string? Name { get; set; }
    public string? URL { get; set; }
    public string? Description { get; set; }
    public JobActionType JobActionType { get; set; }
    public long JobFrequencyInMinutes { get; set; }
    public DateTime JobScheduledAt { get; set; }
    public List<JobResult>? JobResults { get; set; }
}

//WebsiteStatus model
public class JobResult
{
    public int ID { get; set; }
    public string? Status { get; set; }
    public string? Observation { get; set; }
    public int JobID { get; set; }
    public JobActionType JobActionType { get; set; }
    
}

public class JobFactory
{
    public static Job? CreateJob(string name, string description, string url, JobActionType jobActionType, long jobFrequencyInMinutes, DateTime jobScheduledAt)
    {
        return new Job
        {
            Name = name,
            Description = description,
            URL = url,
            JobActionType = jobActionType,
            JobFrequencyInMinutes = jobFrequencyInMinutes,
            JobScheduledAt = jobScheduledAt,
        };
    } 
}


public class JobScheduler
{
    public async static Task<Job> ScheduleJobAsync(Job job)
    {
        using (var db = new JobDbContext())
        {
            await db.AddJobAsync(job);
        }
        return job;
    }

    public static async Task DoScheduledJobAsync(Job job)
    {

        string obs =
            await WebsiteServiceAPI.GetLatestObservationAsync(job.URL!);

        string status = WebsiteJobReconciler.JobCheck(obs, job.JobActionType);

        //create the job result
        JobResult outcome = new JobResult()
        {
            JobID = job.ID,
            Observation = obs,
            JobActionType = job.JobActionType,
            Status = status
        };

        await JobScheduler.LogJobResultAsync(outcome);

    }

    public async static Task<JobResult> LogJobResultAsync(JobResult result)
    {
        using (var db = new JobDbContext())
        {
            await db.AddJobResultAsync(result);
        }
        return result;
    }

    public async static Task<List<Job>> GetJobsAsync()
    {
        var jobs = new List<Job>();
        using (var db = new JobDbContext())
        {
            jobs = await db.GetJobsAsync();
        }
        return jobs;
    }

    public async static Task<List<Job>> GetScheduledJobsToRunAsync()
    {
        List<Job> currentJobs = new List<Job>();
        using (var db = new JobDbContext())
        {
            currentJobs = await db.GetJobsDueAsync();
        }

        return currentJobs;
    }

    public async static Task RunScheduledJobs()
    {

        List<Job> currentJobs = await GetScheduledJobsToRunAsync();

        foreach(Job job in currentJobs)
        {
            await JobScheduler.DoScheduledJobAsync(job);
        }
    }

  
    public async static Task<List<JobResult>> GetJobResultsAsync()
    {
        var jobresults = new List<JobResult>();
        using (var db = new JobDbContext())
        {
            jobresults = await db.GetJobResultsAsync();
        }
        return jobresults;
    }
    
}