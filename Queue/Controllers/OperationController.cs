using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Queue.Models;
using Queue.ViewModels;
using MongoDB.Driver.Linq;
using System.Collections.Concurrent;

namespace Queue.Controllers
{
    public class OperationController : Controller
    {
        public OperationController()
        {
            MongoHelper.ConnectToMongoService();
        }

        #region AddMethods
        public bool AddHardware(List<InstalledHardwareViewModel> o)
        {
            try
            {
                MongoHelper.HardWareList = MongoHelper.database.GetCollection<InstalledHardwareViewModel>("Hardware");
                MongoHelper.HardWareList.InsertMany(o);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool AddSoftware(List<InstalledProgramsViewModel> o)
        {
            try
            {
                MongoHelper.SoftWareList = MongoHelper.database.GetCollection<InstalledProgramsViewModel>("Software");
                MongoHelper.SoftWareList.InsertMany(o);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool AddTracker(List<TrakerBase> tb)
        {
            try
            {
                MongoHelper.TrakerBase = MongoHelper.database.GetCollection<TrakerBase>("TrackerTime");
                MongoHelper.TrakerBase.InsertMany(tb);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool AddCapture(CaptureBase cb)
        {
            try
            {
                MongoHelper.UserCapture = MongoHelper.database.GetCollection<CaptureBase>("WindowsCapture");
                MongoHelper.UserCapture.InsertOne(cb);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool AddLog(LogsViewModel lg)
        {
            try
            {
                MongoHelper.Logs = MongoHelper.database.GetCollection<LogsViewModel>("Log");
                MongoHelper.Logs.InsertOne(lg);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #endregion

        #region UpdateMethods
        public void UpdateHardware(InstalledHardwareViewModel o)
        {
            try
            {
                var collection = MongoHelper.database.GetCollection<InstalledHardwareViewModel>("Hardware");
                var builder = Builders<InstalledHardwareViewModel>.Filter;
                var filter = builder.Eq("_id", o._id);
                var update = Builders<InstalledHardwareViewModel>.Update.Set("status", false).Set("uninstalldate", DateTime.Now);
                var result = collection.UpdateOne(filter, update);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateSotfware(InstalledProgramsViewModel o)
        {
            try
            {
                var collection = MongoHelper.database.GetCollection<InstalledProgramsViewModel>("Software");
                var builder = Builders<InstalledProgramsViewModel>.Filter;
                var filter = builder.Eq("_id", o._id);
                var update = Builders<InstalledProgramsViewModel>.Update.Set("Status", false).Set("uninstalldate", DateTime.Now);
                var result = collection.UpdateOne(filter, update);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region GetMethods
        public List<InstalledProgramsViewModel> GetSoftWare(string IdCompany, string Pc)
        {
            try
            {
                MongoHelper.SoftWareList = MongoHelper.database.GetCollection<InstalledProgramsViewModel>("Software");
                var builder = Builders<InstalledProgramsViewModel>.Filter;
                var filter = builder.Eq("IdCompany", IdCompany) & builder.Eq("Pc", Pc) & builder.Eq("Status", true);

                var results = MongoHelper.SoftWareList.Find(filter).ToList();
                return results;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<InstalledHardwareViewModel> GetHardware(string IdCompany, string Pc)
        {
            try
            {
                MongoHelper.HardWareList = MongoHelper.database.GetCollection<InstalledHardwareViewModel>("Hardware");
                var builder = Builders<InstalledHardwareViewModel>.Filter;
                var filter = builder.Eq("IdCompany", IdCompany) & builder.Eq("Pc", Pc) & builder.Eq("status", true);

                var results = MongoHelper.HardWareList.Find(filter).ToList();
                return results;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<CaptureBase> GeImage(string IdCompany)
        {
            try
            {
                MongoHelper.UserCapture = MongoHelper.database.GetCollection<CaptureBase>("WindowsCapture");
                var builder = Builders<CaptureBase>.Filter;
                var filter = builder.Eq("IdCompany", IdCompany);

                var results = MongoHelper.UserCapture.Find(filter).ToList();
                return results;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<AutomaticTakeTimeModel> GetSoftWareClasification(string IdCompany)
        {
            try
            {
                //MongoHelper.SoftWareList = MongoHelper.database.GetCollection<InstalledProgramsViewModel>("Software");
                //var builder = Builders<InstalledProgramsViewModel>.Filter;

                //var filter = builder.Eq("IdCompany", IdCompany) & builder.Eq("Status", true);

                //var results = MongoHelper.SoftWareList.Find(filter).ToList();


                var query = (from e in MongoHelper.database.GetCollection<AutomaticTakeTimeModel>("TrackerTime").AsQueryable<AutomaticTakeTimeModel>()
                             where e.IdEmpresa == IdCompany
                             select new AutomaticTakeTimeModel
                             {
                                 Application = e.Application

                             }).Distinct().ToList();

                return query;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion


        #region Stadisticas

        public BasicStatsModel MoreUsedApp(string idcompany, DateTime fromdate, DateTime todate)
        {
            BasicStatsModel bm = new BasicStatsModel();
            var query = (from e in MongoHelper.database.GetCollection<AutomaticTakeTimeModel>("TrackerTime").AsQueryable<AutomaticTakeTimeModel>()
                         where e.IdEmpresa == idcompany
                         && e.Date >= fromdate && e.Date <= todate
                         select new AutomaticTakeTimeModel
                         {
                             Application = e.Application,
                             Time = e.Activity,
                             Date = e.Date
                         }).Distinct().ToList();

            foreach (var grouping in query.OrderByDescending(x => x.Time).GroupBy(g => g.Application).ToList())
            {
                var item = grouping;

                double? time = query.Where(t => t.Application == item.Key).Select(f => f.Time).Sum();
                bm.labels.Add(item.Key);
                double? totalminutes = 0;

                if (time != null && time > 0)
                    totalminutes = (time / 60);

                bm.data.Add(Math.Round(totalminutes.Value, 2));
            }

            return bm;
        }

        public BasicUserModel GetUsers(string idcompany, DateTime fromdate, DateTime todate)
        {
            BasicUserModel bm = new BasicUserModel();
            var query = (from e in MongoHelper.database.GetCollection<AutomaticTakeTimeModel>("TrackerTime").AsQueryable<AutomaticTakeTimeModel>()
                         where e.IdEmpresa == idcompany
                         && e.Date >= fromdate && e.Date <= todate
                         select new AutomaticTakeTimeModel
                         {
                             UserName = e.UserName,
                             Application = e.Application,
                             Time = e.Activity,
                             Date = e.Date,
                         }).Distinct().ToList();

            for (var i = 0; i < query.Count; i++)
            {
                bm.User.Add(query[i].UserName);
                bm.Application.Add(query[i].Application);
                bm.Time.Add((double)query[i].Time);
                bm.DateTime.Add((DateTime)query[i].Date);

            }
            return bm;
        }
        public BasicUserModel GetUserByName(string idcompany, DateTime fromdate, DateTime todate, string Name)
        {
            //string fromDate = fromdate.ToString("yyyy-MM-dd");
            //string toDate = todate.ToString("yyyy-MM-dd");
            BasicUserModel bm = new BasicUserModel();
            var query = (from e in MongoHelper.database.GetCollection<AutomaticTakeTimeModel>("TrackerTime").AsQueryable<AutomaticTakeTimeModel>()
                         where e.IdEmpresa == idcompany
                         && e.Date >= fromdate && e.Date <= todate
                         && e.UserName == Name
                         select new AutomaticTakeTimeModel
                         {
                             UserName = e.UserName,
                             Application = e.Application,
                             Time = e.Activity,
                             Date = e.Date
                         }).Distinct().ToList();

            for (var i = 0; i < query.Count; i++)
            {
                bm.User.Add(query[i].UserName);
                bm.Application.Add(query[i].Application);
                bm.Time.Add((double)query[i].Time);
            }
            return bm;
        }
        public BasicStatsModel TypeApp(string idcompany)
        {
            DateTime dateFrom = DateTime.Today.AddDays(-50);
            DateTime dateTo = dateFrom.AddHours(23);
            BasicStatsModel bm = new BasicStatsModel();
            var query = (from e in MongoHelper.database.GetCollection<AutomaticTakeTimeModel>("TrackerTime").AsQueryable<AutomaticTakeTimeModel>()
                         where e.IdEmpresa == idcompany && e.Date >= dateFrom && e.Date <= dateTo
                         select new AutomaticTakeTimeModel
                         {
                             Application = e.Application,
                             Time = e.Activity,
                             Date = e.Date
                         }).Distinct().ToList();

            foreach (var grouping in query.OrderByDescending(x => x.Time).GroupBy(g => g.Application).ToList())
            {
                var item = grouping;

                double? time = query.Where(t => t.Application == item.Key).Select(f => f.Time).Sum();
                bm.labels.Add(item.Key);
                double? totalminutes = 0;

                if (time != null && time > 0)
                    totalminutes = (time / 60);

                bm.data.Add(Math.Round(totalminutes.Value, 2));
            }

            return bm;
        }

        public BasicStatsModel WebUsedApp(string idcompany, DateTime fromdate, DateTime todate)
        {
            BasicStatsModel bm = new BasicStatsModel();
            var query = (from e in MongoHelper.database.GetCollection<AutomaticTakeTimeModel>("TrackerTime").AsQueryable<AutomaticTakeTimeModel>()
                         where e.IdEmpresa == idcompany && e.Application == "chrome"
                         && e.Date >= fromdate && e.Date <= todate
                         select new AutomaticTakeTimeModel
                         {
                             Application = e.Application,
                             Time = e.Activity,
                             Date = e.Date,
                             Title = e.Title
                         }).Distinct().ToList();

            foreach (var grouping in query.OrderByDescending(x => x.Time).GroupBy(g => g.Title).ToList())
            {
                var item = grouping;

                double? time = query.Where(t => t.Title == item.Key).Select(f => f.Time).Sum();
                bm.labels.Add(item.Key);
                double? totalminutes = 0;

                if (time != null && time > 0)
                    totalminutes = (time / 60);

                bm.data.Add(Math.Round(totalminutes.Value, 2));
            }

            return bm;
        }

        //public BasicStatsModel ImproductiveUsedApp(string idcompany, DateTime fromdate, DateTime todate)
        //{
        //    BasicStatsModel bm = new BasicStatsModel();
        //    var query = (from e in MongoHelper.database.GetCollection<AutomaticTakeTimeModel>("TrackerTime").AsQueryable<AutomaticTakeTimeModel>()
        //                 where e.IdEmpresa == idcompany
        //                 && e.Date >= fromdate && e.Date <= todate
        //                 select new AutomaticTakeTimeModel
        //                 {
        //                     Application = e.Application,
        //                     Time = e.Activity,
        //                     Date = e.Date
        //                 }).Distinct().ToList();

        //    foreach (var grouping in query.OrderByDescending(x => x.Time).GroupBy(g => g.Application).ToList())
        //    {
        //        var item = grouping;

        //        double? time = query.Where(t => t.Application == item.Key).Select(f => f.Time).Sum();
        //        bm.labels.Add(item.Key);
        //        var date = query.Where(t => t.Application == item.Key).Select(f => f.Date).ToList();

        //        double? totalminutes = 0;
        //        for (int i = 0; i < date.Count; i++)
        //        {
        //            bm.DateTime.Add(date[i].ToString("H:mm:ss"));
        //        }
        //        if (time != null && time > 0)
        //            totalminutes = (time / 60);

        //        bm.data.Add(Math.Round(totalminutes.Value, 2));
        //    }

        //    return bm;
        //}


        public string GetNameTimesApps(string Application, double Seconds)
        {
            return "<b>" + Application + ":</b> " + GetNameTimes(Seconds);
        }

        public string GetNameTimes(double Seconds)
        {
            double _Seconds = Math.Round(Seconds);
            double _Minutes = Math.Round(_Seconds / 60);
            double _Hours = Math.Round(_Minutes / 60);

            string diff_show = _Seconds + " segundos";
            if (_Seconds >= 60)
            {
                var _SecondsDiff = Math.Abs((_Minutes * 60) - _Seconds);
                diff_show = _Minutes + " minuto(s) " + (_SecondsDiff > 1 ? _SecondsDiff + " segundos" : string.Empty);
                if (_Minutes >= 60)
                {
                    var _MinutesDiff = Math.Abs((_Hours * 60) - _Minutes);
                    diff_show = _Hours + " hora(s) " + (_MinutesDiff > 1 ? _MinutesDiff + " minuto(s)" : string.Empty);
                }
            }

            return diff_show;
        }

        public async Task<List<UsersReportGanttModel>> GetactivityData(string idcompany, DateTime fromdate, DateTime todate, int periods)
        {


            var _queryFiltre = MongoHelper.database.GetCollection<AutomaticTakeTimeModel>("TrackerTime").AsQueryable<AutomaticTakeTimeModel>().
                Where(e => e.IdEmpresa == idcompany);


            var _startTest = new DateTime(fromdate.Year, fromdate.Month, fromdate.Day);
            var _endTest = new DateTime(todate.Year, todate.Month, todate.Day);
            _endTest = _endTest.Add(new TimeSpan(23, 59, 59));

            //_queryFiltre = _queryFiltre.Where(x => x.FocusTime == fromdate);
            _queryFiltre = _queryFiltre.Where(s => s.FocusTime >= _startTest && s.FocusTime <= _endTest);



            var queryPrincipal = _queryFiltre.
                //Where(e => e.FocusTime >= fromdate.Date && e.FocusTime <= todate.Date).
                GroupBy(e => e.UserName)
                .Select(e =>
                           new UsersReportGanttModel
                           {
                               UserName = e.Key,
                               ReportSytems = e.Select(x => new UsersReportGanttModel.ReportTime()
                               {
                                   Application = x.Application,
                                   FocusTime = x.FocusTime,
                                   Activity = x.Activity
                               }).ToList()
                           }).ToList();



            ConcurrentQueue<UsersReportGanttModel> queryAuxParallel = new ConcurrentQueue<UsersReportGanttModel>();

            await Task.FromResult(Parallel.ForEach(queryPrincipal, new ParallelOptions { MaxDegreeOfParallelism = 4 }, async (Item) =>
            {
                UsersReportGanttModel InsertArray = new UsersReportGanttModel() { UserName = Item.UserName };

                ConcurrentQueue<UsersReportGanttModel.ReportTime> _ReportSytemsParallel = new ConcurrentQueue<UsersReportGanttModel.ReportTime>();

                await Task.FromResult(Parallel.ForEach(Item.ReportSytems, new ParallelOptions { MaxDegreeOfParallelism = 6 }, (Item2) =>
                {
                    string _NameApps = Item2.Application.Trim().ToUpper();
                    int IdClassication = 0;
                    string NameClassication = "Sin clasificar";

                    using (DAL.QueueContext db = new DAL.QueueContext())
                    {

                        var programsFound = db.Agent_ProgramClasification.FirstOrDefault(t => t.Agent_Empresa.IdCompany.ToString() == idcompany && t.name.Trim().ToUpper() == _NameApps);

                        if (programsFound != null)
                        {
                            switch (programsFound.clasification)
                            {
                                case 1:
                                    IdClassication = 1;
                                    NameClassication = "Productivas";
                                    break;
                                case 2:
                                    IdClassication = 2;
                                    NameClassication = "Improductiva";
                                    break;
                                case 3:
                                    IdClassication = 3;
                                    NameClassication = "Neutrales";
                                    break;
                            }
                        }
                    }

                    var NewReportTimes = new UsersReportGanttModel.ReportTime
                    {
                        Application = _NameApps,
                        FocusTime = Item2.FocusTime,
                        Activity = Item2.Activity,
                        AppsImproClassify = IdClassication,
                        AppImproName = NameClassication
                    };

                    _ReportSytemsParallel.Enqueue(NewReportTimes);


                }));

                InsertArray.ReportSytems = _ReportSytemsParallel.OrderBy(x => x.FocusTime).ToList();

                queryAuxParallel.Enqueue(InsertArray);

            }));





            List<UsersReportGanttModel> queryAux = new List<UsersReportGanttModel>();

            foreach (var item in queryAuxParallel)
            {
                var NewRecord = new UsersReportGanttModel() { UserName = item.UserName };
                var listAppsGroupClasification = item.ReportSytems.Select(x => x.AppsImproClassify).Distinct().ToList();
                

                List<UsersReportGanttModel.ReportTime> _reportTimestmp = new List<UsersReportGanttModel.ReportTime>();

                var RecordTimes = item.ReportSytems.OrderBy(c => c.FocusTime);

               
                string _IdAppsPrevious = string.Empty;

                foreach (var itemReportTimes in RecordTimes)
                {
                    var insertData = true;
                    if (_IdAppsPrevious == itemReportTimes.Application)
                    {
                        var sumaActivityRecordAppsLocal = itemReportTimes.Activity;
                        var infoTimesApps = _reportTimestmp.Where(x => x.Application == itemReportTimes.Application).OrderBy(x => x.FocusTimeEnd);
                        if (infoTimesApps.Any())
                        {
                            var timesApps = infoTimesApps.Last();

                            TimeSpan diff = timesApps.FocusTimeEnd - itemReportTimes.FocusTime;
                            double _Seconds = Math.Abs(Math.Truncate(diff.TotalSeconds));
                            if (!(_Seconds > 0))
                            {
                                timesApps.Activity += itemReportTimes.Activity;
                                insertData = false;
                            }
                        }
                    }

                    if (insertData)
                    {
                        var NewReportTimes = new UsersReportGanttModel.ReportTime
                        {
                            Application = itemReportTimes.Application,
                            FocusTime = itemReportTimes.FocusTime,
                            Activity = itemReportTimes.Activity,
                            AppsImproClassify = itemReportTimes.AppsImproClassify,
                            AppImproName = itemReportTimes.AppImproName
                        };

                        _reportTimestmp.Add(NewReportTimes);
                    }

                    _IdAppsPrevious = itemReportTimes.Application;
                }

                double _periods = periods * 60;
                _reportTimestmp = _reportTimestmp.OrderBy(x => x.FocusTime).ToList();

                int AppsImproClassifyPrevious = _reportTimestmp.First().AppsImproClassify;
                double ActivitysPrevious = 0;

                //var infoApps = new List<string>();
                var firtTime = true;

                //var listAppsGroupClasificationAA = _reportTimestmp.Select(x => x.Application).Distinct().ToList();
                //NewRecord.UserName = item.UserName + "|" + string.Join(",", listAppsGroupClasificationAA);

                foreach (var itemReportTimes in _reportTimestmp)
                {

                    if (itemReportTimes.Application.ToLower() == "sourcetree")
                    {
                        string asds = "";
                    }

                    bool newGroup = false;

                    string ShowInfo = GetNameTimesApps(itemReportTimes.Application, itemReportTimes.Activity);

                    if (firtTime)
                    {
                        firtTime = false;
                        var NewReportTimes = new UsersReportGanttModel.ReportTime
                        {
                            Application = ShowInfo,
                            //GroupApplication = new List<string>() { ShowInfo },
                            FocusTime = itemReportTimes.FocusTime,
                            Activity = 0, //se sumará mas adelante
                            AppsImproClassify = itemReportTimes.AppsImproClassify,
                            AppImproName = itemReportTimes.AppImproName
                        };

                        NewRecord.ReportSytems.Add(NewReportTimes);
                    }

                    var foundRecordTimes = NewRecord.ReportSytems.Last();
                    //infoApps.Add(ShowInfo);


                    if (AppsImproClassifyPrevious != itemReportTimes.AppsImproClassify)
                    {
                        newGroup = true;
                    }

                    if (!newGroup)
                    {
                        if (_periods > 0)
                        {
                            if ((foundRecordTimes.Activity + itemReportTimes.Activity) > _periods)
                            {
                                newGroup = true;
                            }
                        }
                    }

                    //if (!newGroup)
                    //{
                    //    if (ApplicationPrevious != itemReportTimes.Application)
                    //    {
                    //        newGroup = true;
                    //    }
                    //}


                    if (newGroup)
                    {
                        var _activitys = itemReportTimes.Activity;
                        var _FocusTime = itemReportTimes.FocusTime;

                        double _aux = 0;
                        if (foundRecordTimes.Activity == 0)
                        {
                            foundRecordTimes.Application = GetNameTimesApps(itemReportTimes.Application, _periods);
                            foundRecordTimes.GroupApplication = new List<string>() { foundRecordTimes.Application };
                            foundRecordTimes.MessageDuration = GetNameTimes(_periods);
                            foundRecordTimes.Activity = _periods;
                            _FocusTime = foundRecordTimes.FocusTimeEnd;

                            bool contineCorrection = true;
                            while (contineCorrection)
                            {
                                var _sumActivitys = _aux + _activitys;
                                if (_sumActivitys > _periods)
                                {
                                    _aux = +_periods;
                                    //crear grupos
                                    var _NewReportTimes = new UsersReportGanttModel.ReportTime
                                    {
                                        Application = foundRecordTimes.Application,
                                        GroupApplication = new List<string>() { foundRecordTimes.Application },
                                        MessageDuration = GetNameTimes(_periods),
                                        FocusTime = _FocusTime,
                                        Activity = _periods,
                                        AppsImproClassify = itemReportTimes.AppsImproClassify,
                                        AppImproName = itemReportTimes.AppImproName
                                    };

                                    NewRecord.ReportSytems.Add(_NewReportTimes);
                                    _activitys -= _periods;
                                    _FocusTime = _NewReportTimes.FocusTimeEnd;
                                }
                                else
                                {
                                    contineCorrection = false;
                                    _activitys = Math.Abs(_activitys);
                                }
                            }


                        }

                        ShowInfo = GetNameTimesApps(itemReportTimes.Application, _activitys);

                        //Cierre grupo
                        var NewReportTimes = new UsersReportGanttModel.ReportTime
                        {
                            Application = ShowInfo,
                            GroupApplication = new List<string>() { ShowInfo },
                            MessageDuration = GetNameTimes(_activitys),
                            FocusTime = _FocusTime,
                            Activity = _activitys,
                            AppsImproClassify = itemReportTimes.AppsImproClassify,
                            AppImproName = itemReportTimes.AppImproName
                        };

                        NewRecord.ReportSytems.Add(NewReportTimes);

                        //infoApps = new List<string>();

                    }
                    else
                    {
                        //foundRecordTimes.Application += string.Join("</br>", infoApps);
                        foundRecordTimes.GroupApplication.Add(ShowInfo);
                        foundRecordTimes.Activity += itemReportTimes.Activity;
                        foundRecordTimes.MessageDuration = GetNameTimes(foundRecordTimes.Activity);
                    }


                    AppsImproClassifyPrevious = itemReportTimes.AppsImproClassify;
                    ActivitysPrevious = itemReportTimes.Activity;
                }

                //if (infoApps.Any())
                //{

                //    var foundRecordTimes = NewRecord.ReportSytems.Last();
                //    foundRecordTimes.Application = string.Join("</br>", infoApps);
                //    foundRecordTimes.Activity += ActivitysPrevious;
                //}



                //DateTime dateStart = _reportTimestmp.Min(x => x.FocusTime);
                //DateTime dateEnd = _reportTimestmp.Max(x => x.FocusTimeEnd);

                //if (_periods == 0) { dateEnd = _reportTimestmp.Max(x => x.FocusTimeEnd); }
                //else { dateEnd = dateEnd.AddSeconds(_periods); }

                //int AppsImproClassifyPrevious = _reportTimestmp.First().AppsImproClassify;
                //string AppImproNamePrevious = _reportTimestmp.First().AppImproName;
                //DateTime FocusTimePrevious = _reportTimestmp.First().FocusTime;
                //bool reset = false;


                //while (dateStart <= _endTest)
                //{

                //    var infoApps = new List<string>();



                //    var infoReportTimes = _reportTimestmp.Where(x => x.FocusTime >= dateStart && x.FocusTime <= dateEnd).OrderBy(x => x.FocusTime);
                //    DateTime firstFocusTime = infoReportTimes.Min(x => x.FocusTime);
                //    double sumaActivityRecordAppsAll = 0;



                //    foreach (var itemTimes in infoReportTimes)
                //    {
                //        double sumaActivityRecordAppsApps = itemTimes.Activity;

                //        //TimeSpan diff = itemTimes.FocusTimeEnd - dateEnd;
                //        //double _Secondsdiff = Math.Truncate(diff.TotalSeconds);
                //        //if (_Secondsdiff > 0)
                //        //{
                //        //    sumaActivityRecordAppsApps -= _Secondsdiff;
                //        //}


                //        double _Seconds = Math.Round(sumaActivityRecordAppsApps);
                //        double _Minutes = Math.Truncate(_Seconds / 60);
                //        double _Hours = Math.Truncate(_Minutes / 60);

                //        string diff_show = _Seconds + " segundos";
                //        if (_Seconds >= 60)
                //        {
                //            var _SecondsDiff = Math.Abs(((_Minutes * 60) - _Seconds));
                //            diff_show = _Minutes + " minuto(s) " + (_SecondsDiff > 0 ? _SecondsDiff + " segundos" : string.Empty);
                //            if (_Minutes >= 60)
                //            {
                //                var _MinutesDiff = Math.Abs(((_Hours * 60) - _Minutes));
                //                diff_show = _Hours + " hora(s) " + (_MinutesDiff > 0 ? _MinutesDiff + " minuto(s)" : string.Empty);
                //            }
                //        }

                //        string ShowInfo = "<b>" + itemTimes.Application + ":</b> " + diff_show;

                //        if (AppsImproClassifyPrevious != itemTimes.AppsImproClassify)
                //        {
                //            var NewReportTimes = new UsersReportGanttModel.ReportTime
                //            {
                //                Application = string.Join("</br>", infoApps),
                //                FocusTime = firstFocusTime,
                //                Activity = sumaActivityRecordAppsAll,
                //                AppsImproClassify = AppsImproClassifyPrevious,
                //                AppImproName = AppImproNamePrevious
                //            };

                //            NewRecord.ReportSytems.Add(NewReportTimes);

                //            infoApps = new List<string>();
                //            sumaActivityRecordAppsAll = 0;
                //            reset = true;

                //            firstFocusTime = itemTimes.FocusTimeEnd;
                //        }

                //        if (_periods > 0)
                //        {
                //            if (!reset)
                //            {
                //                if ((sumaActivityRecordAppsAll + sumaActivityRecordAppsApps) > _periods)
                //                {
                //                    var NewReportTimes = new UsersReportGanttModel.ReportTime
                //                    {
                //                        Application = string.Join("</br>", infoApps),
                //                        FocusTime = firstFocusTime,
                //                        Activity = sumaActivityRecordAppsAll,
                //                        AppsImproClassify = itemTimes.AppsImproClassify,
                //                        AppImproName = itemTimes.AppImproName
                //                    };

                //                    NewRecord.ReportSytems.Add(NewReportTimes);

                //                    infoApps = new List<string>();
                //                    sumaActivityRecordAppsAll = 0;
                //                    firstFocusTime = itemTimes.FocusTimeEnd;
                //                }
                //            }
                //            else { reset = false; }
                //        }

                //        infoApps.Add(ShowInfo);
                //        sumaActivityRecordAppsAll += sumaActivityRecordAppsApps;

                //        AppsImproClassifyPrevious = itemTimes.AppsImproClassify;
                //        AppImproNamePrevious = itemTimes.AppImproName;
                //    }

                //    if (infoApps.Any())
                //    {
                //        var NewReportTimes = new UsersReportGanttModel.ReportTime
                //        {
                //            Application = string.Join("</br>", infoApps),
                //            FocusTime = firstFocusTime,
                //            Activity = sumaActivityRecordAppsAll,
                //            AppsImproClassify = AppsImproClassifyPrevious,
                //            AppImproName = AppImproNamePrevious
                //        };

                //        NewRecord.ReportSytems.Add(NewReportTimes);
                //    }

                //    if (_periods == 0)
                //    {
                //        dateStart = _endTest.AddMinutes(30);
                //    }
                //    else
                //    {
                //        dateStart = dateStart.AddSeconds(_periods);
                //    }
                //}



                //foreach (var itemAppsClasification in listAppsGroupClasification.OrderBy(x => x))
                //{
                //    var RecordTimes = _reportTimestmp.Where(x => x.AppsImproClassify == itemAppsClasification).OrderBy(c => c.FocusTime);

                //    //DateTime dateStart = RecordTimes.Min(x => x.FocusTime);
                //    //DateTime dateEnd = dateStart;


                //    //while (dateStart <= _endTest)
                //    //{
                //    double sumaActivityRecordAppsAll = 0;
                //    var infoApps = new List<string>();

                //    if (_periods == 0) { dateEnd = RecordTimes.Max(x => x.FocusTimeEnd); }
                //    else { dateEnd = dateEnd.AddSeconds(_periods); }

                //    var infoReportTimes = RecordTimes.Where(x => x.FocusTime >= dateStart && x.FocusTime <= dateEnd);
                //    var groupApps = infoReportTimes.Select(x => x.Application).Distinct();

                //    foreach (var itemapp in groupApps)
                //    {
                //        double sumaActivityRecordApps = 0;

                //        var infoReportTimesApps = infoReportTimes.Where(x => x.Application == itemapp);
                //        sumaActivityRecordApps = Math.Abs(Math.Truncate(infoReportTimesApps.Sum(x => x.Activity)));

                //        TimeSpan diff = infoReportTimesApps.Max(x => x.FocusTimeEnd) - dateEnd;
                //        double _Secondsdiff = Math.Truncate(diff.TotalSeconds);

                //        if (_Secondsdiff > 0)
                //        {
                //            sumaActivityRecordApps = sumaActivityRecordApps - _Secondsdiff;
                //        }


                //        double _Seconds = sumaActivityRecordApps;
                //        double _Minutes = Math.Truncate(_Seconds / 60);
                //        double _Hours = Math.Truncate(_Minutes / 60);

                //        string diff_show = _Seconds + " segundos";
                //        if (_Seconds > 60)
                //        {
                //            diff_show = _Minutes + " minuto(s) " + Math.Abs(((_Minutes * 60) - _Seconds)) + " segundos";
                //            if (_Minutes > 60)
                //            {
                //                diff_show = _Hours + " hora(s) " + Math.Abs(((_Hours * 60) - _Minutes)) + " minuto(s)";
                //            }
                //        }

                //        string ShowInfo = "<b>" + itemapp + ":</b> " + diff_show;

                //        if (_periods > 0)
                //        {
                //            if ((sumaActivityRecordAppsAll + sumaActivityRecordApps) > _periods)
                //            {
                //                var NewReportTimes = new UsersReportGanttModel.ReportTime
                //                {
                //                    Application = string.Join("</br>", infoApps),
                //                    FocusTime = dateStart,
                //                    Activity = sumaActivityRecordAppsAll,
                //                    AppsImproClassify = itemAppsClasification,
                //                    AppImproName = RecordTimes.First().AppImproName
                //                };

                //                NewRecord.ReportSytems.Add(NewReportTimes);

                //                infoApps = new List<string>();
                //                sumaActivityRecordAppsAll = 0;
                //            }
                //        }

                //        infoApps.Add(ShowInfo);
                //        sumaActivityRecordAppsAll += sumaActivityRecordApps;

                //    }

                //    if (infoApps.Any())
                //    {
                //        var NewReportTimes = new UsersReportGanttModel.ReportTime
                //        {
                //            Application = string.Join("</br>", infoApps),
                //            FocusTime = dateStart,
                //            Activity = sumaActivityRecordAppsAll,
                //            AppsImproClassify = itemAppsClasification,
                //            AppImproName = RecordTimes.First().AppImproName
                //        };

                //        NewRecord.ReportSytems.Add(NewReportTimes);
                //    }



                //    if (_periods == 0)
                //    {
                //        dateStart = _endTest.AddMinutes(30);
                //    }
                //    else
                //    {
                //        dateStart = dateStart.AddSeconds(_periods);
                //    }


                //}




                //}



                if (NewRecord.ReportSytems.Any())
                {
                    queryAux.Add(NewRecord);
                }

            }






            //foreach (var item in queryPrincipal)
            //{
            //    var NewRecord = new UsersReportGanttModel() { UserName = item.UserName };
            //    var listApps = item.ReportSytems.Select(x => x.Application).Distinct().ToList();

            //    foreach (var itemApps in listApps)
            //    {
            //        var RecordTimes = item.ReportSytems.Where(x => x.Application == itemApps);
            //        var sumaActivity = Math.Truncate(RecordTimes.Sum(x => x.Activity));

            //        if (!(sumaActivity > 0))
            //        {
            //            continue;
            //        }

            //        int IdClassication = 0;
            //        string NameClassication = "Sin clasificar";

            //        using (DAL.QueueContext db = new DAL.QueueContext())
            //        {
            //            string _NameApps = item.UserName.Trim().ToUpper();
            //            var programsFound = db.Agent_ProgramClasification.FirstOrDefault(t => t.Agent_Empresa.IdCompany.ToString() == idcompany && t.name.Trim().ToUpper() == _NameApps);

            //            if (programsFound != null)
            //            {
            //                switch (programsFound.clasification)
            //                {
            //                    case 1:
            //                        NameClassication = "Productivas";
            //                        break;
            //                    case 2:
            //                        NameClassication = "Improductiva";
            //                        break;
            //                    case 3:
            //                        NameClassication = "Neutrales";
            //                        break;
            //                }
            //            }
            //        }


            //        var NewReportTimes = new UsersReportGanttModel.ReportTime
            //        {
            //            Application = itemApps,
            //            FocusTime = RecordTimes.OrderBy(x => x.FocusTime).First().FocusTime,
            //            Activity = sumaActivity,
            //            AppsImproClassify = IdClassication,
            //            AppImproName = NameClassication
            //        };

            //        NewRecord.ReportSytems.Add(NewReportTimes);

            //    }


            //    queryAux.Add(NewRecord);
            //}


            return queryAux;
        }

        public BasicStatsModel ImproductiveUsedApp(string idcompany, DateTime fromdate, DateTime todate)
        {
            BasicStatsModel bm = new BasicStatsModel();
            var query = (from e in MongoHelper.database.GetCollection<AutomaticTakeTimeModel>("TrackerTime").AsQueryable<AutomaticTakeTimeModel>()
                         where e.IdEmpresa == idcompany
                         && e.Date >= fromdate && e.Date <= todate
                         select new AutomaticTakeTimeModel
                         {
                             Application = e.Application,
                             Time = e.Activity,
                             Date = e.Date,
                         }).Distinct().ToList();

            foreach (var grouping in query.OrderByDescending(x => x.Time).GroupBy(g => g.Application).ToList())
            {
                var item = grouping;

                double? time = query.Where(t => t.Application == item.Key).Select(f => f.Time).Sum();
                bm.labels.Add(item.Key);
                var date = query.Where(t => t.Application == item.Key).Select(f => f.Date).ToList();

                double? totalminutes = 0;
                for (int i = 0; i < date.Count; i++)
                {
                    bm.DateTime.Add(date[i].ToString("H:mm:ss"));
                }
                if (time != null && time > 0)
                    totalminutes = (time / 60);

                bm.data.Add(Math.Round(totalminutes.Value, 2));
            }

            return bm;
        }

        //public BasicStatsModel hh(string idcompany, DateTime fromdate, DateTime todate)
        //{
        //    BasicStatsModel bm = new BasicStatsModel();
        //    var query = (from e in MongoHelper.database.GetCollection<AutomaticTakeTimeModel>("TrackerTime").AsQueryable<AutomaticTakeTimeModel>()
        //                 where e.IdEmpresa == idcompany
        //                 && e.Date >= fromdate && e.Date <= todate
        //                 select new AutomaticTakeTimeModel
        //                 {
        //                     Application = e.Application,
        //                     Time = e.Activity,
        //                     Date = e.Date
        //                 }).Distinct().ToList();

        //    foreach (var grouping in query.OrderByDescending(x => x.Time).GroupBy(g => g.Application).ToList())
        //    {
        //        var item = grouping;

        //        double? time = query.Where(t => t.Application == item.Key).Select(f => f.Time).Sum();
        //        bm.labels.Add(item.Key);
        //        var date = query.Where(t => t.Application == item.Key).Select(f => f.Date).ToList();

        //        double? totalminutes = 0;
        //        for (int i = 0; i < date.Count; i++)
        //        {
        //            bm.DateTime.Add(date[i].ToString("H:mm:ss"));
        //        }
        //        if (time != null && time > 0)
        //            totalminutes = (time / 60);

        //        bm.data.Add(Math.Round(totalminutes.Value, 2));
        //    }

        //    return bm;
        //}
        public BasicStatsDate DateUsedApp(string app, DateTime fromdate, DateTime todate)
        {
            BasicStatsDate bm = new BasicStatsDate();
            var query = (from e in MongoHelper.database.GetCollection<AutomaticTakeTimeModel>("TrackerTime").AsQueryable<AutomaticTakeTimeModel>()
                         where e.Application == app
                         && e.Date >= fromdate && e.Date <= todate
                         select new AutomaticTakeTimeModel
                         {
                             Application = e.Application,
                             Time = e.Activity,
                             Date = e.Date
                         }).Distinct().ToList();

            foreach (var grouping in query.OrderByDescending(x => x.Time).GroupBy(g => g.Application).ToList())
            {
                var item = grouping;

                double? time = query.Where(t => t.Application == item.Key).Select(f => f.Time).Sum();

                var date = query.Where(t => t.Application == item.Key).Select(f => f.Date).ToList();


                for (int i = 0; i < date.Count; i++)
                {
                    bm.DateTime.Add(date[i].ToString("H:mm:ss"));
                }

            }

            return bm;
        }
        #endregion
    }
}