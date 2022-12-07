namespace Services.WebsiteStatusService;

using JobService;
using WebsiteService;

public class WebsiteJobReconciler
{
    public static string JobCheck(string obs, JobActionType jobActionType)
    {
        string status = "";

        switch(jobActionType)
        {
            case JobActionType.CHECK_STATUS:
                if(obs == "OK")
                {
                    status = "OK";
                } 
                else{
                    status = "FAULT";
                }
                break;

            default:
                status = "INVALID JOB ACTION";
                break;                
        }

        return status;
    }
} 


public class WebsiteObservation
{
    public string? statusCode { get; set; }
    
}