﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BOC.Models;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Nancy.Json;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BOC.Controllers
{
    public class FlightDateMobileController : Controller
    {
        [HttpGet]
        public IActionResult FlightView(LoungeModel model)
        {

            //Create viewbag content Route
            IEnumerable<SelectListItem> Routing = new[]
            {

                new SelectListItem { Value = "ALL", Text = "ALL" },
                new SelectListItem { Value = "INT", Text = "INT" },
                new SelectListItem { Value = "DOM", Text = "DOM" },

            };

            ViewBag.List = Routing;
            //Get Session Token
            var token = HttpContext.Session.GetString("Token");
            //Access API with Header
            Url airportlst = new Url();
            string uri = airportlst.Get("AirportList");


            HttpClient Client = new HttpClient();

            Client.DefaultRequestHeaders.Add("Authorization", token);
            var req = new HttpRequestMessage(HttpMethod.Post, uri);

            string Content;
            HttpResponseMessage res;
            res = Client.SendAsync(req).Result;
            Content = res.Content.ReadAsStringAsync().Result;
            var oData = JObject.Parse(Content);

            //Bind Json To List 
            JavaScriptSerializer ser = new JavaScriptSerializer();
            List<AirportList> lst = ser.Deserialize<List<AirportList>>(oData["Data"].ToString());//str is JSON string.
            for (int i = 0; i < lst.Count; i++)
            {
                lst[i].ID = i + 1;
            }
            model.ListAirport = lst;
            return View(model);
        }
        [HttpPost]
        public IActionResult Detail(FlightModel model)
        {
            //Get Session TypeOfDevice
            var typeofdevices = HttpContext.Session.GetString("TypeOfDevice");
            if (model.TimeZone == null) { model.TimeZone = "HAN"; }
            //Set Session TimeZone
            HttpContext.Session.SetString("TimeZone", model.TimeZone.ToString());

            if (model.AirportChoose == null)
            {
                model.AirportChoose = "";
            }

            //Set Session AirportChoose
            HttpContext.Session.SetString("AirportChoose", model.AirportChoose.ToString());

            if (model.Key == null)
            {
                model.Key = "";
            }
            var KeySearch = model.Key.ToString();
            if (model.ViewType == null)
            {
                model.ViewType = "";
            }
            var ViewType = model.ViewType.ToString();


            //Parse string date from dd/MM/yyyy to yyyy-MM-dd
            DateTime dt = DateTime.ParseExact(model.Date.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);

            var date = dt.ToString("yyyy-MM-dd");
            ViewData["FlightID"] = "0";
            ViewData["TimeZone"] = model.TimeZone.ToString();
            ViewData["Date"] = date;
            ViewData["KeySsearch"] = KeySearch.ToString();
            ViewData["Int_Dom"] = model.SelectedRouting.ToString();
            ViewData["ViewType"] = ViewType.ToString();
            ViewData["AutoHide"] = model.AutoHide.ToString();

            return RedirectToAction("Index", "FlightDateMobile", new { @TimeZone = model.TimeZone.ToString(), @Date = date, @Key = KeySearch.ToString(), @SelectedRouting = model.SelectedRouting.ToString(), @VType = ViewType.ToString(), @AutoHide = model.AutoHide.ToString() });

            
        }

        [HttpPost]
        public IActionResult Index(string TimeZone, string Date, string Key, string SelectedRouting, string VType, string AutoHide)
        {
            if (TimeZone == null) { TimeZone = "HAN"; }
        
            var dd = Date.ToString();
            string[] collection = dd.Split('/');
            // to get the full month name
            string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(Int16.Parse(collection[1]));
            string day = collection[0];
            Date = collection[2] + '-' + collection[1] + '-' + collection[0];
            //TempData["Date"] = day + monthName;
            HttpContext.Session.SetString("Date", day + monthName);
            ////Get Session AirportChoose
            var AirportChoose = HttpContext.Session.GetString("AirportChoose");

            if (AirportChoose == null)
            {
                AirportChoose = "";
            }
            if (Key == null)
            {
                Key = "";
            }
            var KeySearch = Key.ToString();
            if (VType == null)
            {
                VType = "";
            }
            var ViewType = VType.ToString();


            //Get Session Token
            var token = HttpContext.Session.GetString("Token");
            //Access API with Header
            Url flightget = new Url();
            string uri = flightget.Get("FlightGet");

            HttpClient Client = new HttpClient();

            var nvc = new List<KeyValuePair<string, string>>();
            nvc.Add(new KeyValuePair<string, string>("FlightID", "0"));
            nvc.Add(new KeyValuePair<string, string>("AreaCode", AirportChoose));
            nvc.Add(new KeyValuePair<string, string>("TimeZone", TimeZone.ToString()));
            nvc.Add(new KeyValuePair<string, string>("Date", Date.ToString()));
            nvc.Add(new KeyValuePair<string, string>("KeySearch", KeySearch));
            nvc.Add(new KeyValuePair<string, string>("Int_Dom", SelectedRouting.ToString()));
            nvc.Add(new KeyValuePair<string, string>("ViewType", ViewType));
            nvc.Add(new KeyValuePair<string, string>("AutoHide", AutoHide.ToString()));



            Client.DefaultRequestHeaders.Add("Authorization", token);
            var req = new HttpRequestMessage(HttpMethod.Post, uri) { Content = new FormUrlEncodedContent(nvc) };

            string Content;
            HttpResponseMessage res;
            res = Client.SendAsync(req).Result;
            Content = res.Content.ReadAsStringAsync().Result;

            JToken _FltItem;
            JObject ser = JObject.Parse(Content);
            Int32 _Result = (int)ser.SelectToken("ResultCode");

            List<Flight> lst = new List<Flight>();
            var ser2 = ser.SelectToken("Data");
            List<JToken> data = new List<JToken>(ser2.Children());

            foreach (var item in data)
            {
                Flight Flt = new Flight();

                item.CreateReader();
                Int32 _FlightIDs = Int32.Parse(item.SelectToken("FlightID").ToString());
                Flt.FlightID = _FlightIDs;


                _FltItem = item.SelectToken("Date");
                Flt.Date = (string)_FltItem.SelectToken("Value");
                Flt.Date_Color = (string)_FltItem.SelectToken("BackColor");

                _FltItem = item.SelectToken("FltNo");
                Flt.FltNo = (string)_FltItem.SelectToken("Value");
                Flt.FltNo_Color = (string)_FltItem.SelectToken("BackColor");

                _FltItem = item.SelectToken("RegisterNo");
                Flt.RegisterNo = (string)_FltItem.SelectToken("Value");
                Flt.RegisterNo_Color = (string)_FltItem.SelectToken("BackColor");

                _FltItem = item.SelectToken("Aircraft");
                Flt.Aircraft = (string)_FltItem.SelectToken("Value");
                Flt.Aircraft_Color = (string)_FltItem.SelectToken("BackColor");

                _FltItem = item.SelectToken("Route");
                Flt.Route = (string)_FltItem.SelectToken("Value");
                HttpContext.Session.SetString("Routing", Flt.Route.ToString());
                Flt.Route_Color = (string)_FltItem.SelectToken("BackColor");

                _FltItem = item.SelectToken("DateTime_ATA");
                Flt.DateTime_ATA = (string)_FltItem.SelectToken("Value");
                Flt.DateTime_ATA_Color = (string)_FltItem.SelectToken("BackColor");

                _FltItem = item.SelectToken("STD");
                Flt.STD = (string)_FltItem.SelectToken("Value");
                Flt.STD_Color = (string)_FltItem.SelectToken("BackColor");

                _FltItem = item.SelectToken("ETD");
                Flt.ETD = (string)_FltItem.SelectToken("Value");
                Flt.ETD_Color = (string)_FltItem.SelectToken("BackColor");

                _FltItem = item.SelectToken("BDT");
                Flt.BDT = (string)_FltItem.SelectToken("Value");
                Flt.BDT_Color = (string)_FltItem.SelectToken("BackColor");

                _FltItem = item.SelectToken("DoorClose");
                Flt.DoorClose = (string)_FltItem.SelectToken("Value");
                Flt.DoorClose_Color = (string)_FltItem.SelectToken("BackColor");

                _FltItem = item.SelectToken("TOff");
                Flt.TOff = (string)_FltItem.SelectToken("Value");
                Flt.TOff_Color = (string)_FltItem.SelectToken("BackColor");

                _FltItem = item.SelectToken("STA");
                Flt.STA = (string)_FltItem.SelectToken("Value");
                Flt.STA_Color = (string)_FltItem.SelectToken("BackColor");

                _FltItem = item.SelectToken("ETA");
                Flt.ETA = (string)_FltItem.SelectToken("Value");
                Flt.ETA_Color = (string)_FltItem.SelectToken("BackColor");

                _FltItem = item.SelectToken("TDown");
                Flt.TDown = (string)_FltItem.SelectToken("Value");
                Flt.TDown_Color = (string)_FltItem.SelectToken("BackColor");

                _FltItem = item.SelectToken("ATD");
                Flt.ATD = (string)_FltItem.SelectToken("Value");
                Flt.ATD_Color = (string)_FltItem.SelectToken("BackColor");


                _FltItem = item.SelectToken("ATA");
                Flt.ATA = (string)_FltItem.SelectToken("Value");
                Flt.ATA_Color = (string)_FltItem.SelectToken("BackColor");


                _FltItem = item.SelectToken("Terminal");
                Flt.Terminal = (string)_FltItem.SelectToken("Value");
                Flt.Terminal_Color = (string)_FltItem.SelectToken("BackColor");

                _FltItem = item.SelectToken("Gate");
                Flt.Gate = (string)_FltItem.SelectToken("Value");
                Flt.Gate_Color = (string)_FltItem.SelectToken("BackColor");

                _FltItem = item.SelectToken("Belt_Dep");
                Flt.Belt_Dep = (string)_FltItem.SelectToken("Value");
                Flt.Belt_Dep_Color = (string)_FltItem.SelectToken("BackColor");

                _FltItem = item.SelectToken("Bay_Dep");
                Flt.Bay_Dep = (string)_FltItem.SelectToken("Value");
                Flt.Bay_Dep_Color = (string)_FltItem.SelectToken("BackColor");

                _FltItem = item.SelectToken("Bay_Arr");
                Flt.Bay_Arr = (string)_FltItem.SelectToken("Value");
                Flt.Bay_Arr_Color = (string)_FltItem.SelectToken("BackColor");

                lst.Add(Flt);
            }
            for (int i = 0; i < lst.Count; i++)
            {
                lst[i].ID = i + 1;
            }
            //var model = new FlightModel();
            //model.Ds = lst;
            return View(lst);
        }
        public IActionResult Detail(string FlightID,string FlightNo,string Routing)
        {
            ViewBag.FlightID = FlightID;
            ViewBag.FlightNo = FlightNo;
            ViewBag.Routing = Routing;
            return View();
        }
        public IActionResult FlightOne(string FlightID)
        {
            //Get Session TimeZone
            var TimeZone = HttpContext.Session.GetString("TimeZone")==null?"HAN": HttpContext.Session.GetString("TimeZone");
            
            //Get Session Token
            var token = HttpContext.Session.GetString("Token");

            ////Access API to get One Flight 
            Url flightget = new Url();
            string uri = flightget.Get("FlightOneGet");

            HttpClient Client = new HttpClient();
            var nvc = new List<KeyValuePair<string, string>>();
            nvc.Add(new KeyValuePair<string, string>("FlightID", FlightID));
            nvc.Add(new KeyValuePair<string, string>("TimeZone", TimeZone));
            Client.DefaultRequestHeaders.Add("Authorization", token);
            var req = new HttpRequestMessage(HttpMethod.Post, uri) { Content = new FormUrlEncodedContent(nvc) };

            string Content;
            HttpResponseMessage res;
            res = Client.SendAsync(req).Result;
            Content = res.Content.ReadAsStringAsync().Result;

            JObject ser = JObject.Parse(Content);
            Int32 _Result = (int)ser.SelectToken("ResultCode");

            List<Items> lst = new List<Items>();
            var ser2 = ser.SelectToken("Data");
            List<JToken> data = new List<JToken>(ser2.Children());
            var model = new FlightOneModel();

            foreach (var item in data)
            {

                var PaxEst_Full = item.SelectToken("PaxEst_Full");
                model.PaxEst_Full = JsonConvert.DeserializeObject<Items>(PaxEst_Full.ToString());

                var PaxEst_Total = item.SelectToken("PaxEst_Total");
                model.PaxEst_Total = JsonConvert.DeserializeObject<Items>(PaxEst_Total.ToString());

                var Pax_Full = item.SelectToken("Pax_Full");
                model.Pax_Full = JsonConvert.DeserializeObject<Items>(Pax_Full.ToString());

                var Pax_Total = item.SelectToken("Pax_Total");
                model.Pax_Total = JsonConvert.DeserializeObject<Items>(Pax_Total.ToString());

                var Crew = item.SelectToken("Crew");
                model.Crew = JsonConvert.DeserializeObject<Items>(Crew.ToString());

                var Cargo_Full = item.SelectToken("Cargo_Full");
                model.Cargo_Full = JsonConvert.DeserializeObject<Items>(Cargo_Full.ToString());

                var Mail = item.SelectToken("Mail");
                model.Mail = JsonConvert.DeserializeObject<Items>(Mail.ToString());

                var Co_Mat_Full = item.SelectToken("Co_Mat_Full");
                model.Co_Mat_Full = JsonConvert.DeserializeObject<Items>(Co_Mat_Full.ToString());

                var Fuel_Remmain = item.SelectToken("Fuel_Remmain");
                model.Fuel_Remmain = JsonConvert.DeserializeObject<Items>(Fuel_Remmain.ToString());

                var Fuel_Topup = item.SelectToken("Fuel_Topup");
                model.Fuel_Topup = JsonConvert.DeserializeObject<Items>(Fuel_Topup.ToString());

                var Fuel_Trip = item.SelectToken("Fuel_Trip");
                model.Fuel_Trip = JsonConvert.DeserializeObject<Items>(Fuel_Trip.ToString());

                string FlightId = item.SelectToken("FlightID").ToString();
                string TimeKey = item.SelectToken("TimeKey").ToString();

                var Date = item.SelectToken("Date");
                model.Date = JsonConvert.DeserializeObject<Items>(Date.ToString());
                var dd = model.Date.Value.ToString();
                string[] collection = dd.Split('-');
                // to get the full month name
                string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(Int16.Parse(collection[1]));
                string day = collection[2];
                //TempData["Date"] = day + monthName;
                HttpContext.Session.SetString("Date", day + monthName);

                var FltNo = item.SelectToken("FltNo");
                model.FltNo = JsonConvert.DeserializeObject<Items>(FltNo.ToString());
                //TempData["FlightNo"] = model.FltNo.Value.ToString();
                HttpContext.Session.SetString("FlightNo", model.FltNo.Value.ToString());

                var RegisterNo = item.SelectToken("RegisterNo");
                model.RegisterNo = JsonConvert.DeserializeObject<Items>(RegisterNo.ToString());

                var Aircraft = item.SelectToken("Aircraft");
                model.Aircraft = JsonConvert.DeserializeObject<Items>(Aircraft.ToString());

                var Route = item.SelectToken("Route");
                model.Route = JsonConvert.DeserializeObject<Items>(Route.ToString());
                //TempData["Routing"] = model.Route.Value.ToString();
                HttpContext.Session.SetString("Routing", model.Route.Value.ToString());

                var DateTime_ATD = item.SelectToken("DateTime_ATD");
                model.DateTime_ATD = JsonConvert.DeserializeObject<Items>(DateTime_ATD.ToString());

                var DateTime_ATA = item.SelectToken("DateTime_ATA");
                model.DateTime_ATA = JsonConvert.DeserializeObject<Items>(DateTime_ATA.ToString());

                var STD = item.SelectToken("STD");
                model.STD = JsonConvert.DeserializeObject<Items>(STD.ToString());

                var ETD = item.SelectToken("ETD");
                model.ETD = JsonConvert.DeserializeObject<Items>(ETD.ToString());

                var BDT = item.SelectToken("BDT");
                model.BDT = JsonConvert.DeserializeObject<Items>(BDT.ToString());

                var DoorClose = item.SelectToken("DoorClose");
                model.DoorClose = JsonConvert.DeserializeObject<Items>(DoorClose.ToString());

                var TOff = item.SelectToken("TOff");
                model.TOff = JsonConvert.DeserializeObject<Items>(TOff.ToString());

                var STA = item.SelectToken("STA");
                model.STA = JsonConvert.DeserializeObject<Items>(STA.ToString());

                var ETA = item.SelectToken("ETA");
                model.ETA = JsonConvert.DeserializeObject<Items>(ETA.ToString());

                var TDown = item.SelectToken("TDown");
                model.TDown = JsonConvert.DeserializeObject<Items>(TDown.ToString());

                var ATD = item.SelectToken("ATD");
                model.ATD = JsonConvert.DeserializeObject<Items>(ATD.ToString());

                var ATA = item.SelectToken("ATA");
                model.ATA = JsonConvert.DeserializeObject<Items>(ATA.ToString());

                var Terminal = item.SelectToken("Terminal");
                model.Terminal = JsonConvert.DeserializeObject<Items>(Terminal.ToString());

                var Gate = item.SelectToken("Gate");
                model.Gate = JsonConvert.DeserializeObject<Items>(Gate.ToString());

                var Belt_Dep = item.SelectToken("Belt_Dep");
                model.Belt_Dep = JsonConvert.DeserializeObject<Items>(Belt_Dep.ToString());

                var Bay_Dep = item.SelectToken("Bay_Dep");
                model.Bay_Dep = JsonConvert.DeserializeObject<Items>(Bay_Dep.ToString());

                var Bay_Arr = item.SelectToken("Bay_Arr");
                model.Bay_Arr = JsonConvert.DeserializeObject<Items>(Bay_Arr.ToString());
            }
           
            return View(model);
           
        }

        public IActionResult FlightDoc(string FlightId, string FlightNo, string Routing)
        {
            if (FlightNo != null)
            {
                HttpContext.Session.SetString("FlightNo", FlightNo);
            }
            if (Routing != null)
            {
                HttpContext.Session.SetString("Routing", Routing);
            }
            //Get Session Token
            var token = HttpContext.Session.GetString("Token");
            ////Access API to get Doccument Flight 
            Url flightget = new Url();
            string uri = flightget.Get("FlightDocumentGet");

            HttpClient Client = new HttpClient();
            var nvc = new List<KeyValuePair<string, string>>();
            nvc.Add(new KeyValuePair<string, string>("FlightDocID", "0"));
            nvc.Add(new KeyValuePair<string, string>("FlightID", FlightId));
            Client.DefaultRequestHeaders.Add("Authorization", token);
            var req = new HttpRequestMessage(HttpMethod.Post, uri) { Content = new FormUrlEncodedContent(nvc) };

            string Content;
            HttpResponseMessage res;
            res = Client.SendAsync(req).Result;
            Content = res.Content.ReadAsStringAsync().Result;

            JToken _LstItem;
            JObject ser = JObject.Parse(Content);
            Int32 _Result = (int)ser.SelectToken("ResultCode");

            List<FlightDoc> lst = new List<FlightDoc>();
            var ser2 = ser.SelectToken("Data");
            List<JToken> data = new List<JToken>(ser2.Children());

            foreach (var item in data)
            {
                FlightDoc lstdoc = new FlightDoc();

                lstdoc.FlightDocID = (int)item.SelectToken("FlightDocID");

                lstdoc.FlightID = (int)item.SelectToken("FlightID");

                _LstItem = item.SelectToken("Airport");
                lstdoc.Airport = _LstItem.ToString();

                _LstItem = item.SelectToken("RouteType");
                lstdoc.RouteType = _LstItem.ToString();

                _LstItem = item.SelectToken("DocumentType");
                lstdoc.DocumentType = _LstItem.ToString();

                _LstItem = item.SelectToken("Content");
                lstdoc.Content = _LstItem.ToString();

                _LstItem = item.SelectToken("UserUpdate");
                lstdoc.UserUpdate = _LstItem.ToString();

                _LstItem = item.SelectToken("RecDate");
                lstdoc.RecDate = _LstItem.ToString();

                _LstItem = item.SelectToken("FileName");
                lstdoc.FileName = _LstItem.ToString();

                lst.Add(lstdoc);
            }
            for (int i = 0; i < lst.Count; i++)
            {
                lst[i].OddEvent = i % 2;
            }


            return View(lst);



        }
        public IActionResult Discussion(string FlightID)
        {
            //Get Session TimeZone
            var TimeZone = HttpContext.Session.GetString("TimeZone");
            //Get Session Token
            var token = HttpContext.Session.GetString("Token");
            var model = new FlightOneModel();
            //Access API to get Conference Flight 
            Url flightconference = new Url();
            string url = flightconference.Get("ConferenceList");

            HttpClient Client = new HttpClient();
            var nvc = new List<KeyValuePair<string, string>>();
            nvc.Add(new KeyValuePair<string, string>("FlightID", FlightID));
            Client.DefaultRequestHeaders.Add("Authorization", token);
            var req1 = new HttpRequestMessage(HttpMethod.Post, url) { Content = new FormUrlEncodedContent(nvc) };

            string Contentconf;
            HttpResponseMessage resconf;
            resconf = Client.SendAsync(req1).Result;
            Contentconf = resconf.Content.ReadAsStringAsync().Result;

            JObject serconf = JObject.Parse(Contentconf);
            Int32 _ResultConf = (int)serconf.SelectToken("ResultCode");

            List<Conference> lstconf = new List<Conference>();
            var ser_ = serconf.SelectToken("Data");
            List<JToken> dataconf = new List<JToken>(ser_.Children());

            foreach (var itemconf in dataconf)
            {
                var conflight = new Conference();

                var ID = Int32.Parse(itemconf.SelectToken("ConferenceID").ToString());
                conflight.id = ID;

                var FlightId = Int32.Parse(itemconf.SelectToken("FlightID").ToString());
                conflight.FlightID = FlightId;

                var ConferenceID = Int32.Parse(itemconf.SelectToken("ConferenceID").ToString());
                conflight.ConferenceID = ConferenceID;

                var ParentID = Int32.Parse(itemconf.SelectToken("ParentID").ToString());
                conflight.parentId = ParentID;

                var MessageType = itemconf.SelectToken("MessageType").ToString();
                conflight.MessageType = MessageType;

                var ScheduleDate = itemconf.SelectToken("ScheduleDate").ToString();
                conflight.ScheduleDate = ScheduleDate;

                var DivisionName = itemconf.SelectToken("DivisionName").ToString();
                conflight.DivisionName = DivisionName;

                var Remark = itemconf.SelectToken("Remark").ToString();
                conflight.Remark = Remark;

                var Scope = itemconf.SelectToken("Scope").ToString();
                conflight.Scope = Scope;

                var Airport = itemconf.SelectToken("Airport").ToString();
                conflight.Airport = Airport;

                var RouteType = itemconf.SelectToken("RouteType").ToString();
                conflight.RouteType = RouteType;

                var Status = itemconf.SelectToken("Status").ToString();
                conflight.Status = JsonConvert.DeserializeObject<Items>(Status.ToString());

                var CreateDate = itemconf.SelectToken("CreateDate").ToString();
                conflight.CreateDate = CreateDate;

                var CreateUser = itemconf.SelectToken("CreateUser").ToString();
                conflight.CreateUser = CreateUser;

                var DeleteDate = itemconf.SelectToken("DeleteDate").ToString();
                conflight.DeleteDate = DeleteDate;

                var DeleteUser = itemconf.SelectToken("DeleteUser").ToString();
                conflight.DeleteUser = DeleteUser;

                lstconf.Add(conflight);
            }
            //var obj = lstconf.ToArray();
            //ViewData["ConferenceFlight"] = new JavaScriptSerializer().Serialize(obj);

            //model.ConferenceFlight = lstconf;

            return View(lstconf);
        }


    }
}
