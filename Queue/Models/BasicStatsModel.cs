using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Queue.Models
{
    public class BasicStatsModel
    {
        public List<string> labels = new List<string>();
        public List<double?> data = new List<double?>();
        public List<string> DateTime = new List<string>();
    }
    public class BasicStatsDate
    {

        public List<string> DateTime = new List<string>();
    }

    public class BasicUserModel
    {
        public List<string> User = new List<string>();
        public List<string> Application = new List<string>();
        public List<double> Time = new List<double>();
        public List<DateTime> DateTime = new List<DateTime>();
    }

    public class ArrayUsersModel
    {
        public string User;
        public List<string> Application = new List<string>();
        public List<double> Time = new List<double>();
        public List<string> Date = new List<string>();
        //public List<ApplicationModel> Data = new List<ApplicationModel>();
    }
    public class ArrayUsersModelGantt
    {
        public string User;
        public List<string> Application = new List<string>();
        public List<double> Time = new List<double>();
        public List<string> Date = new List<string>();
        public List<string> AppImpro = new List<string>();
        public List<string> TimeImpro = new List<string>();
        //public List<ApplicationModel> Data = new List<ApplicationModel>();
    }

    public class ApplicationModel
    {
        public string Application;
        public double Time;
    }



    public class UsersReportGanttModel
    {
        public string UserName { get; set; }
        public List<string> GroupApps { get { return ReportSytems.Select(x => x.Application).Distinct().ToList(); } }
        public List<ReportTime> ReportSytems { get; set; } = new List<ReportTime>();

        public class ReportTime
        {
            public string Application { get; set; }
            public string ApplicationT { get { return string.Join("</br>", GroupApplication); } }
            public List<string> GroupApplication { get; set; } = new List<string>();
            public string MessageDuration { get; set; }
            public DateTime FocusTime { get; set; }
            public DateTime FocusTimeEnd { get { return FocusTime.AddSeconds(this.Activity); } }
            public string FocusTimeT { get { return FocusTime.ToString("yyyy-MM-dd HH:mm:ss"); } }
            public double Activity { get; set; }
            public string AppImproName { get; set; }
            public int AppsImproClassify { get; set; }
            public string DateEnd { get { return FocusTimeEnd.ToString("yyyy-MM-dd HH:mm:ss"); } }
        }

    }

}