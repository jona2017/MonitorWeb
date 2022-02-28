using Newtonsoft.Json;
using Queue.DAL;
using Queue.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web.Mvc;
using System.Web;
using System.Globalization;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Threading.Tasks;

namespace Queue.Controllers
{
    public class ReportGanttController : Controller
    {
        private QueueContext db = new QueueContext();
        // GET: ReportGantt
        public ActionResult Index()
        {
            try
            {
                ViewBag.DateFrom = DateTime.Now.AddDays(-2);
                ViewBag.DateTo = DateTime.Now;


                return View();
            }
            catch (Exception err)
            {

                throw err;
            }
        }

        // GET: ReportGantt/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ReportGantt/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ReportGantt/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: ReportGantt/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ReportGantt/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: ReportGantt/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        public async Task<JsonResult> NewChart(DateTime? dateFrom, DateTime? dateTo, int periods)
        {
            //try
            //{
            var company = Request.RequestContext.HttpContext.Session["Company"].ToString();
            OperationController opc = new OperationController();
            var result = await opc.GetactivityData(company, dateFrom.Value, dateTo.Value, periods);
            return Json(result, JsonRequestBehavior.AllowGet);
            //}
            //catch (Exception err)
            //{
            //    throw err;
            //}

        }
        #region OLD

        //public JsonResult NewChart(string dateFrom, string dateTo)
        //{
        //    try
        //    {
        //        var company = Request.RequestContext.HttpContext.Session["Company"].ToString();
        //        OperationController opc = new OperationController();
        //        DateTime fromdate = Convert.ToDateTime(dateFrom);
        //        DateTime todate = Convert.ToDateTime(dateFrom);
        //        DateTime horasEnd = todate.AddHours(23).AddMinutes(59).AddSeconds(59);
        //        BasicStatsModel bm = opc.ImproductiveUsedApp(company, fromdate, horasEnd);
        //        BasicUserModel user = opc.GetUsers(company, fromdate, horasEnd);
        //        BasicStatsModel bm = opc.MoreUsedApp(company, fromdate, todate);
        //        var UserByDeparment = db.Agent_Employee.Select(c => c.Usuario).Distinct().ToList();

        //        List<object> iDataImpro = new List<object>();

        //        var programs = db.Agent_ProgramClasification.Where(t => t.Agent_Empresa.IdCompany.ToString() == company).ToList();

        //        Agent_ProgramClasification p = new Agent_ProgramClasification();

        //        TimesUser entities = new TimesUser();

        //        DataTable dt = new DataTable();

        //        dt.Columns.Add("Aplicaciones", System.Type.GetType("System.String"));
        //        dt.Columns.Add("Tiempo", System.Type.GetType("System.Int32"));



        //        for (var i = 0; i < bm.labels.Count; i++)
        //        {
        //            foreach (var n in programs)
        //            {
        //                p.name = n.name;
        //                p.clasification = n.clasification;

        //                if (bm.labels[i] == p.name)
        //                {
        //                    if (p.clasification == 2)
        //                    {
        //                        Creating sample data  
        //                        DataRow dr = dt.NewRow();
        //                        dr["Aplicaciones"] = bm.labels[i].ToString();
        //                        dr["Tiempo"] = bm.data[i];
        //                        dt.Rows.Add(dr);
        //                    }
        //                }

        //            }
        //        }

        //        foreach (DataColumn dc in dt.Columns)
        //        {
        //            List<object> x = new List<object>();
        //            x = (from DataRow drr in dt.Rows select drr[dc.ColumnName]).ToList();
        //            iDataImpro.Add(x);
        //        }


        //        var nomApp = iDataImpro.ToList();
        //        var resultadolist = ((IList)nomApp[0]);




        //        var UserDist = user.User.Distinct().ToList();

        //        List<string> UserFilter = new List<string>();

        //        for (var contUser = 0; contUser < UserByDeparment.Count; contUser++)
        //        {
        //            for (var distinct = 0; distinct < UserDist.Count; distinct++)
        //            {
        //                if (UserByDeparment[contUser] == UserDist[distinct])
        //                {
        //                    UserFilter.Add(UserDist[distinct]);
        //                }
        //            }
        //        }


        //        var ApplicationDist = user.Application.Distinct().ToList();
        //        var DateHours = user.DateTime.Cast<DateTime>().OrderBy(x => x).ToList();





        //        List<object> iData = new List<object>();
        //        ArrayUsersModel UserModel = new ArrayUsersModel();
        //        Random rnd = new Random();
        //        string[] ResultApp = new string[UserFilter.Count];
        //        double SumTime = 0;
        //        string UpdateHours = "";


        //        for (var i = 0; i < UserFilter.Count; i++)
        //        {
        //            ArrayUsersModelGantt UserModel = new ArrayUsersModelGantt();
        //            UserModel.User = UserFilter[i];

        //            for (var k = 0; k < ApplicationDist.Count; k++)
        //            {
        //                for (var a = 0; a < resultadolist.Count; a++)
        //                {
        //                    if (ApplicationDist[k] == resultadolist[a].ToString())
        //                    {

        //                        UserModel.AppImpro.Add(ApplicationDist[k]);
        //                        for (int ap = 0; ap < bm.labels.Count; ap++)
        //                        {
        //                            if (resultadolist[a].ToString() == bm.labels[ap].ToString())
        //                            {
        //                                BasicStatsDate date = opc.DateUsedApp(bm.labels[ap], fromdate, horasEnd);
        //                                for (int t = 0; t < date.DateTime.Count; t++)
        //                                {
        //                                    UserModel.TimeImpro.Add(date.DateTime[t]);
        //                                }
        //                            }
        //                        }


        //                    }
        //                }

        //                UserModel.Application.Add(ApplicationDist[k]);
        //                for (var cont = 0; cont < user.User.Count; cont++)
        //                {
        //                    if (user.User[cont] == UserFilter[i] && user.Application[cont] == ApplicationDist[k])
        //                    {
        //                        SumTime = SumTime + user.Time[cont];
        //                        UserModel.Date.Add(DateHours[cont].ToString("H:mm:ss"));
        //                    }
        //                }
        //                var round = SumTime / 3600;
        //                UserModel.Time.Add(round);

        //                SumTime = 0;
        //                UserModel.Time.Add(rnd.Next(1, 50));
        //            }

        //            ResultApp[i] = JsonConvert.SerializeObject(UserModel);

        //            iData.Add(ResultApp[i]);

        //        }


        //        }

        //        return Json(iData, JsonRequestBehavior.AllowGet);





        //    }
        //    catch (Exception err)
        //    {
        //        throw err;
        //    }

        //}
        #endregion
        public JsonResult UserbyDepartamentData(string dateFrom, string dateTo, string idUser, string idDeparment)
        {
            try
            {

                var company = Request.RequestContext.HttpContext.Session["Company"].ToString();
                OperationController opc = new OperationController();
                DateTime fromdate = Convert.ToDateTime(dateFrom);
                DateTime todate = Convert.ToDateTime(dateTo);

                //BasicStatsModel bm = opc.MoreUsedApp(company, fromdate, todate);

                var UserByDeparment = db.Agent_Employee.Where(x => x.Agent_CompanyDepartment.Id.ToString() == idDeparment && x.idEmployee.ToString() == idUser)
                .Distinct().ToList();
                BasicUserModel User = opc.GetUserByName(company, fromdate, todate, UserByDeparment[0].Usuario);
                var ApplicationDist = User.Application.Distinct().ToList();

                ArrayUsersModel UserModel = new ArrayUsersModel();
                List<object> iData = new List<object>();
                Random rnd = new Random();
                string UpdateDate = "";
                double SumTime = 0;

                for (var k = 0; k < ApplicationDist.Count; k++)
                {
                    UserModel.Application.Add(ApplicationDist[k]);
                    for (var cont = 0; cont < User.User.Count; cont++)
                    {
                        if (User.Application[cont] == ApplicationDist[k])
                        {
                            SumTime = SumTime + User.Time[cont];
                        }
                    }
                    var round = Math.Round((SumTime / 3600), 2);
                    UserModel.Time.Add(round);
                    SumTime = 0;
                    UserModel.User = User.User[0];
                };
                //if (User.User[0].ToString()== idUser)
                //{
                //    UserModel.User = idUser;
                //}


                UpdateDate = JsonConvert.SerializeObject(UserModel);

                iData.Add(UpdateDate);


                return Json(iData, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        public JsonResult GetUser(string idDeparment)
        {
            try
            {
                var company = Request.RequestContext.HttpContext.Session["Company"].ToString();
                OperationController opc = new OperationController();
                var result = db.Agent_Employee.Where(d => d.Agent_CompanyDepartment.Id.ToString() == idDeparment).ToList();

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception err)
            {
                throw err;
            }

        }
        // POST: ReportGantt/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
