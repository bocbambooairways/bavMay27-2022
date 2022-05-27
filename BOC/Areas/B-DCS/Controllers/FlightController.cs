using BOC.Areas.B_DCS.Models;
using BOC.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;

namespace BOC.Areas.B_DCS.Controllers
{
    [Area("B-DCS")]
    public class FlightController : Controller
    {
        private IHostingEnvironment Environment;
        public UriConfig UriConfig { get; }
        public FlightController(Microsoft.Extensions.Options.IOptions<UriConfig> _UriConfig, IHostingEnvironment Environment)
        {
            UriConfig = _UriConfig.Value;
            this.Environment = Environment;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult M_Index(string Airport, 
                                     string FlightDate)
        {
            if (!string.IsNullOrEmpty(Airport)||
                !string.IsNullOrEmpty(FlightDate))
            {
                ViewBag.Airport = new DataAPI().ReturnValueFromString(Airport, "_");
                ViewBag.FlightDate = new DataAPI().ReturnValueFromString(FlightDate, "_");
            }
          
            return View();  
        }
        public IActionResult M_FlightList(string FlightDate)
        {
            ViewBag.FlightDate =new DataAPI().ReturnValueFromString(FlightDate,"_");    
            string uri_Get_B_DCS_AirportList = UriConfig.uri_B_DCS_AirportList;
            IEnumerable<DCS_AirportList> Get_B_DCS_AirportList = new DataAPI().GetListAPIWithoutParams<DCS_AirportList>(UriConfig.uri_B_DCS_AirportList,
                                                                      HttpContext.Session.GetString("Token"), System.Net.Http.HttpMethod.Post, false, "Data").Result;
            return View(Get_B_DCS_AirportList);
        }
        public IActionResult M_SearchList()
        {
            return View();
        }
        [HttpPost]  
        public IActionResult M_SearchList(string Airports,
                                          string FlightDate,
                                          string currentPage,
                                          string next,
                                          string previous)
        {
           
            string uri_DCS_Flight_Get = UriConfig.uri_B_DCS_SearchList;
            List<DCS_Flight_Get> DCS_Get_Search_List = new DataAPI().GetListOjectAPI<DCS_Flight_Get>(uri_DCS_Flight_Get,
                                                          HttpContext.Session.GetString("Token"),
                                                          HttpMethod.Post, false, "Data",
                                                          "FlightDate",
                                                           (FlightDate!=null? FlightDate:string.Empty),
                                                           "Airports",
                                                           (Airports!=null?Airports:string.Empty)).Result;
                var rs_Searchlist_Pagination_DCS = new DataAPI().list_Pagination_DCS<DCS_Flight_Get>(currentPage,
                                                            next,
                                                            previous,
                                                            10,
                                                            DCS_Get_Search_List, 
                                                            out int ViewBagcurrentpage,
                                                            out int ViewBagnumSize);
            ViewBag.currentpage = ViewBagcurrentpage;
            ViewBag.numSize = ViewBagnumSize;   
            return View(rs_Searchlist_Pagination_DCS.OrderBy(m => m.FlightID).ToList());
        }
        public IActionResult M_Passenger(string FlightID,
                                         string FlightInfor)
        {
            string _FlightID = new DataAPI().ReturnValueFromString(FlightID, " ");
            string _FlightInfor = new DataAPI().ReturnValueFromString(FlightInfor, "_");
            HttpContext.Session.SetString("FlightID",_FlightID);
            HttpContext.Session.SetString("FlightInfor", _FlightInfor);
            return View();
        }
        public IActionResult M_PassengerList()
        {
            return View();
        }

        [HttpPost]
        public IActionResult M_PassengerList(string ifly_Pax_ID,
                                              string PNR,
                                              string KeySearch,
                                              string PassengerName, 
                                              string currentPage,
                                              string next,
                                              string previous)
        {
            
            string uri_DCS_Passenger_Get = UriConfig.uri_B_DCS_Passenger_Get;
            List<DCS_Passenger_Get> DCS_Get_Passenger_List = new DataAPI().GetListOjectAPI<DCS_Passenger_Get>(uri_DCS_Passenger_Get,
                                                          HttpContext.Session.GetString("Token"),
                                                          HttpMethod.Post, false, "Data",
                                                          "FlightID",
                                                          (HttpContext.Session.GetString("FlightID") != null ? 
                                                           HttpContext.Session.GetString("FlightID").ToString() : "124853"),
                                                          "ifly_Pax_ID",
                                                           (ifly_Pax_ID != null ? ifly_Pax_ID : "0"),
                                                           "PNR",
                                                           (PNR != null ? PNR : string.Empty),
                                                           "KeySearch",
                                                           (KeySearch!=null? KeySearch:string.Empty),
                                                           "PassengerName",
                                                           (PassengerName != null ? PassengerName : string.Empty)).Result;
            var rs_list_Passenger_Pagination_DCS = new DataAPI().list_Pagination_DCS<DCS_Passenger_Get>(currentPage,
                                                           next,
                                                           previous,
                                                           10,
                                                           DCS_Get_Passenger_List,
                                                           out int ViewBagcurrentpage,
                                                           out int ViewBagnumSize);
            ViewBag.currentpage = ViewBagcurrentpage;
            ViewBag.numSize = ViewBagnumSize;
            return View(rs_list_Passenger_Pagination_DCS.OrderBy(m => m.FlightID).ToList());
           
        }
        public IActionResult M_Comment(string ifly_Pax_ID,
                                       string Pax_name,
                                       string SEQ_Seat_ST,
                                       string PNR_Ticket,
                                       string Class_SSR,
                                       string Remark)
        {
            ViewBag.ifly_Pax_ID = new DataAPI().ReturnValueFromString(ifly_Pax_ID, "_");
            ViewBag.Pax_name = new DataAPI().ReturnValueFromString(Pax_name, "_");
            ViewBag.SEQ_Seat_ST = new DataAPI().ReturnValueFromString(SEQ_Seat_ST, "_");
            ViewBag.PNR_Ticket = new DataAPI().ReturnValueFromString(PNR_Ticket, "_");
            ViewBag.Class_SSR = new DataAPI().ReturnValueFromString(Class_SSR, "_");
            ViewBag.Remark = new DataAPI().ReturnValueFromString(Remark, "_");
            HttpContext.Session.SetString("ifly_Pax_ID", ifly_Pax_ID);
            return View();
        }
        [HttpPost]
        public IActionResult M_Comment()
        {
            ViewBag.NewComment = "New";
            return View();
        }
        public IActionResult M_Comment_Create_Modify (string ifly_Pax_ID,
                                                      string Pax_name,
                                                      string SEQ_Seat_ST,
                                                      string PNR_Ticket,
                                                      string Class_SSR,
                                                      string Remark)
        {

            ViewBag.ifly_Pax_ID = new DataAPI().ReturnValueFromString(ifly_Pax_ID,"_");
            ViewBag.Pax_name = new DataAPI().ReturnValueFromString(Pax_name,"_");
            ViewBag.SEQ_Seat_ST = new DataAPI().ReturnValueFromString(SEQ_Seat_ST, "_");
            ViewBag.PNR_Ticket = new DataAPI().ReturnValueFromString(PNR_Ticket,"_");
            ViewBag.Class_SSR = new DataAPI().ReturnValueFromString(Class_SSR,"_");
            ViewBag.Remark = new DataAPI().ReturnValueFromString(Remark,"_");
            ViewBag.FlightInfo = HttpContext.Session.GetString("FlightInfor");
            return View(new SelectBoxViewModel
            {
                Items = new List<string>()
                {
                    "Active",
                    "Delete"
                }
            }); 
           }
        public IActionResult M_Input_SSR(string ifly_Pax_ID,
                                         string Pax_name,
                                         string SEQ_Seat_ST,
                                         string PNR_Ticket,
                                         string Class_SSR,
                                         string Remark)
        {

            //API tao moi hoac dieu chinh 1 SSR cua hanh khach
             string uri_DCS_SSR_Passenger_Update = UriConfig.uri_DCS_SSR_Passenger_Update;

            //2	Danh mục các SSR , người dùng sẽ chọn khi tạo mới / Điều chỉnh SSR cho hành khách
            string uri_Code_SSR_Get = UriConfig.uri_Code_SSR_Get;
            List<DCS_Code_SSR_Get> List_Code_SSR_Get = new DataAPI().GetListAPIWithoutParams<DCS_Code_SSR_Get>(uri_Code_SSR_Get,
                                                       HttpContext.Session.GetString("Token"),
                                                       HttpMethod.Post,
                                                       false,
                                                       "Data").Result;

            //3	API tạo mới hoặc điều chỉnh 1 SSR của hành khách (cach1)
            string GetSearch_eLib_Issue_Department_List = new DataAPI().GetObjectAPI<string>(uri_DCS_SSR_Passenger_Update,
                                     HttpContext.Session.GetString("Token"),
                                     HttpMethod.Post,
                                     false,
                                     "Data",
                                     "SSR_Passenger_ID",
                                     "0",
                                    "ifly_Pax_ID",
                                    "38855359",
                                    "SSRID",
                                    "12",
                                    "Count",
                                    "1",
                                    "Remark",
                                    "Test he thong",
                                    "Status",
                                    "OK").Result;

            // 3	API tạo mới hoặc điều chỉnh 1 SSR của hành khách(cach2)
            eLib_Comment_New Get_eLib_Comment_New = new DataAPI().GetStringAPI(uri_DCS_SSR_Passenger_Update,
                                                                                     HttpContext.Session.GetString("Token"),
                                                                                     HttpMethod.Post,
                                                                                     false,
                                                                                     "SSR_Passenger_ID",
                                                                                     "0",
                                                                                     "ifly_Pax_ID",
                                                                                     "38855359",
                                                                                     "SSRID",
                                                                                      "12",
                                                                                      "Count",
                                                                                      "1",
                                                                                      "Remark",
                                                                                      "Test he thong",
                                                                                      "Status",
                                                                                      "OK").Result;

            //39126714
            //new DataAPI().ReturnValueFromString(ifly_Pax_ID, "_")
            //1.Lay danh sách SSR của 1 hành khách
            string uri_DCS_SSR_Passenger_Get = UriConfig.uri_DCS_SSR_Passenger_Get;
            List<DCS_SSR_Passenger_Get> DCS_Get_Passenger_List = new DataAPI().GetListOjectAPI<DCS_SSR_Passenger_Get>(uri_DCS_SSR_Passenger_Get,
                                                                 HttpContext.Session.GetString("Token"),
                                                                 HttpMethod.Post, 
                                                                 false,
                                                                 "Data",
                                                                 "SSR_Passenger_ID",
                                                                 "0",
                                                                 "ifly_Pax_ID",
                                                                 "39126714").Result;

            //  4  API Nhập liệu hành lý không có BagTag(chỉ nhập pcs/ weight)
            string uri_DCS_Passenger_Bag_NoBagTag_Update = UriConfig.uri_DCS_Passenger_Bag_NoBagTag_Update;
            eLib_Comment_New DCS_Passenger_Bag_NoBagTag_Update = new DataAPI().GetStringAPI(uri_DCS_Passenger_Bag_NoBagTag_Update,
                                                                                    HttpContext.Session.GetString("Token"),
                                                                                    HttpMethod.Post,
                                                                                    false,
                                                                                    "ifly_Pax_ID",
                                                                                    "38922866",
                                                                                    "BagPcs",
                                                                                     "2",
                                                                                    "BagWeight",
                                                                                    "11",
                                                                                    "BagPriority",
                                                                                    "No").Result;

            //5.API Nhập liệu thông tin gi chú cho 1 hành khách
            string uri_DCS_Passenger_Comment_Update = UriConfig.uri_DCS_Passenger_Comment_Update;
            eLib_Comment_New DCS_Passenger_Comment_Update = new DataAPI().GetStringAPI(uri_DCS_Passenger_Comment_Update,
                                                                        HttpContext.Session.GetString("Token"),
                                                                        HttpMethod.Post,
                                                                        false,
                                                                        "ifly_Pax_ID",
                                                                        "38922860",
                                                                        "Comment",
                                                                        "Co hoi cho tat ca moi nmbhguoi").Result;


            //6	API Offload 1 hành khách sau khi checkin
            string uri_DCS_Passenger_Offload = UriConfig.uri_DCS_Passenger_Offload;
            eLib_Comment_New DCS_Passenger_Offload = new DataAPI().GetStringAPI(uri_DCS_Passenger_Offload,
                                                                        HttpContext.Session.GetString("Token"),
                                                                        HttpMethod.Post,
                                                                        false,
                                                                        "ifly_Pax_ID",
                                                                        "38922872").Result;

            //if (Class_SSR != null)
            //{
            //    string[] _Class_SSR = new DataAPI().ReturnValueFromString(Class_SSR, "_").Split("/");
            //    string ssr = _Class_SSR[1];
            //    if (!string.IsNullOrEmpty(ssr))
            //    {
            //        string[] arr_ssr = ssr.Split(' ');
            //        ViewBag.arr_ssr = arr_ssr;
            //    }
            //}

            ViewBag.FlightInfo = HttpContext.Session.GetString("FlightInfor");
            ViewBag.Pax_name = new DataAPI().ReturnValueFromString(Pax_name,"_");
            return View(DCS_Get_Passenger_List);
        }
        public IActionResult M_Input_Baggage(string ifly_Pax_ID,
                                             string Pax_name,
                                             string SEQ_Seat_ST,
                                             string PNR_Ticket,
                                             string Class_SSR,
                                             string Remark)
        {
            ViewBag.ifly_Pax_ID = new DataAPI().ReturnValueFromString(ifly_Pax_ID, "_");
            ViewBag.Pax_name = new DataAPI().ReturnValueFromString(Pax_name, "_");
            ViewBag.SEQ_Seat_ST = new DataAPI().ReturnValueFromString(SEQ_Seat_ST, "_");
            ViewBag.PNR_Ticket = new DataAPI().ReturnValueFromString(PNR_Ticket, "_");
            ViewBag.Class_SSR = new DataAPI().ReturnValueFromString(Class_SSR, "_");
            ViewBag.Remark = new DataAPI().ReturnValueFromString(Remark, "_");
            ViewBag.FlightInfo = HttpContext.Session.GetString("FlightInfor");
            return View();
        }

        [HttpPost]
        public IActionResult M_Input_Baggage(string ifly_Pax_ID="",
                                             string Pax_name="",
                                             string SEQ_Seat_ST="",
                                             string PNR_Ticket="",
                                             string Class_SSR="",
                                             string Remark = "",
                                             string Bag_Weight = "",
                                             string Bag_PCS = "",
                                             string Bag_Tag = "")
        {
            //  4  API Nhập liệu hành lý không có BagTag(chỉ nhập pcs/ weight)
            string uri_DCS_Passenger_Bag_NoBagTag_Update = UriConfig.uri_DCS_Passenger_Bag_NoBagTag_Update;
            eLib_Comment_New DCS_Passenger_Bag_NoBagTag_Update = new DataAPI().GetStringAPI(uri_DCS_Passenger_Bag_NoBagTag_Update,
                                                                                    HttpContext.Session.GetString("Token"),
                                                                                    HttpMethod.Post,
                                                                                    false,
                                                                                    "ifly_Pax_ID",
                                                                                    "38922866",
                                                                                    "BagPcs",
                                                                                     "2",
                                                                                    "BagWeight",
                                                                                    "11",
                                                                                    "BagPriority",
                                                                                    "No").Result;
            ViewBag.ifly_Pax_ID = ifly_Pax_ID;
            ViewBag.Pax_name = Pax_name;
            ViewBag.SEQ_Seat_ST = SEQ_Seat_ST;
            ViewBag.PNR_Ticket = PNR_Ticket;
            ViewBag.Class_SSR = Class_SSR;
            ViewBag.Remark = Class_SSR;
            ViewBag.FlightInfo = HttpContext.Session.GetString("FlightInfor");
            return View();
        }
        public IActionResult M_OffLoad(string ifly_Pax_ID,
                                       string Pax_name,
                                       string SEQ_Seat_ST,
                                       string PNR_Ticket,
                                       string Class_SSR,
                                       string Remark)
        {
            ViewBag.ifly_Pax_ID = new DataAPI().ReturnValueFromString(ifly_Pax_ID, "_");
            ViewBag.Pax_name = new DataAPI().ReturnValueFromString(Pax_name, "_");
            ViewBag.SEQ_Seat_ST = new DataAPI().ReturnValueFromString(SEQ_Seat_ST, "_");
            ViewBag.PNR_Ticket = new DataAPI().ReturnValueFromString(PNR_Ticket, "_");
            ViewBag.Class_SSR = new DataAPI().ReturnValueFromString(Class_SSR, "_");
            ViewBag.Remark = new DataAPI().ReturnValueFromString(Remark, "_");
            ViewBag.FlightInfo = HttpContext.Session.GetString("FlightInfor");
            ViewData["Seat"] = new DataAPI().ReturnValueFromString(SEQ_Seat_ST, "_").Split("/")[1].ToString();
            return View();
        }
        [HttpPost]
        public IActionResult M_OffLoad(string ifly_Pax_ID="",
                                      string Pax_name="",
                                      string SEQ_Seat_ST="",
                                      string PNR_Ticket="",
                                      string Class_SSR="",
                                      string Remark="",
                                      string Bag_Weight="",
                                      string Bag_PCS="",
                                      string Bag_Tag="")
        {
            string uri_DCS_Passenger_Offload = UriConfig.uri_DCS_Passenger_Offload;
            var DCS_Passenger_Offload = new DataAPI().GetStringAPI(uri_DCS_Passenger_Offload,
                                                                        HttpContext.Session.GetString("Token"),
                                                                        HttpMethod.Post,
                                                                        false,
                                                                        "ifly_Pax_ID",
                                                                        "38922872").Result;

            ViewBag.ifly_Pax_ID = ifly_Pax_ID;
            ViewBag.Pax_name = Pax_name;
            ViewBag.SEQ_Seat_ST = SEQ_Seat_ST;
            ViewBag.PNR_Ticket = PNR_Ticket;
            ViewBag.Class_SSR = Class_SSR;
            ViewBag.Remark = Remark;
            ViewBag.FlightInfo = HttpContext.Session.GetString("FlightInfor");
            //ViewData["Seat"] = new DataAPI().ReturnValueFromString(SEQ_Seat_ST, "_").Split("/")[1].ToString();
            return View();
        }

        public IActionResult M_SeatMap(string Pax_name,
                                       string SEQ_Seat_ST,
                                       string _Action)
        {
            ViewBag.Pax_name = new DataAPI().ReturnValueFromString(Pax_name, "_");
            ViewBag.Action = new DataAPI().ReturnValueFromString(_Action, "_");
            ViewBag.SEQ_Seat_ST = new DataAPI().ReturnValueFromString(SEQ_Seat_ST, "_");
            return View();
        }
        public IActionResult M_DCS_SSR_Passenger_Get(string TableID,
                                                     string view,
                                                     string Pax_name,
                                                     string FlightInfo)
        {
            string _TableID = new DataAPI().ReturnValueFromString(TableID, "_");
            string  _view = new DataAPI().ReturnValueFromString(view, "_");

            //1	Danh mục các SSR , người dùng sẽ chọn khi tạo mới / Điều chỉnh SSR cho hành khách
            string uri_Code_SSR_Get = UriConfig.uri_Code_SSR_Get;
            ViewBag.List_Code_SSR_Get = new DataAPI().GetListAPIWithoutParams<DCS_Code_SSR_Get>(uri_Code_SSR_Get,
                                                        HttpContext.Session.GetString("Token"),
                                                        HttpMethod.Post,
                                                        false,
                                                        "Data").Result;
            ViewBag.Pax_name = new DataAPI().ReturnValueFromString(Pax_name, "_");
            ViewBag.FlightInfo = HttpContext.Session.GetString("FlightInfor");
            if (_view == "edit")
            {
                //2
                string uri_DCS_SSR_Passenger_Get = UriConfig.uri_DCS_SSR_Passenger_Get;
                List<DCS_SSR_Passenger_Get> DCS_Get_Passenger_List = new DataAPI().GetListOjectAPI<DCS_SSR_Passenger_Get>(uri_DCS_SSR_Passenger_Get,
                                                            HttpContext.Session.GetString("Token"),
                                                            HttpMethod.Post,
                                                            false,
                                                            "Data",
                                                            "SSR_Passenger_ID",
                                                            _TableID,
                                                            "ifly_Pax_ID",
                                                            "39126714").Result;
                if (DCS_Get_Passenger_List != null)
                    foreach (var item in DCS_Get_Passenger_List)
                    {
                        ViewBag.TableID = item.TableID;
                        ViewBag.FlightID = item.FlightID;
                        ViewBag.ifly_Pax_ID = item.ifly_Pax_ID;
                        ViewBag.SSRID = item.SSRID;
                        ViewBag.Count = item.Count;
                        ViewBag.SSR = item.SSR;
                        ViewBag.Remark = item.Remark;
                        ViewBag.Status = item.Status;
                        ViewBag.LastUpdate = item.LastUpdate;
                        ViewBag.UserUpdate = item.UserUpdate;
                        ViewBag.RecUserID = item.RecUserID;
                        ViewBag.Type = item.Type;
                    }
            }
            return View(new SelectBoxViewModel
            {
                Items = new List<string>()
                {
                    "Active",
                    "Delete"
                }
            });
            }
        [HttpPost]
        public IActionResult M_DCS_SSR_Passenger_Get(String SSR_Passenger_ID,
                                                      String ifly_Pax_ID,
                                                      String Count,
                                                      String Remark,
                                                      String Status,
                                                      String ssrID,
                                                      string Pax_name,
                                                      string FlightInfo
                                                      )
        {
            string _SSR_Passenger_ID = "0";
            string _ifly_Pax_ID = "38855359";
            string _Count = "1";
            string _Remark = "Test he thong";
            string _Status = "OK";
            ViewBag.Pax_name = Pax_name;
            ViewBag.FlightInfo = FlightInfo;
            string uri_DCS_SSR_Passenger_Update = UriConfig.uri_DCS_SSR_Passenger_Update;
            //3   API tạo mới hoặc điều chỉnh 1 SSR của hành khách(cach1)
            string DCS_SSR_Passenger_Update = new DataAPI().GetObjectAPI<string>(uri_DCS_SSR_Passenger_Update,
                                     HttpContext.Session.GetString("Token"),
                                     HttpMethod.Post,
                                     false,
                                     "Data",
                                     "SSR_Passenger_ID",
                                     _SSR_Passenger_ID,
                                    "ifly_Pax_ID",
                                    _ifly_Pax_ID,
                                    "SSRID",
                                    (ssrID!=null? ssrID:"0"),
                                    "Count",
                                    (_Count!=null? _Count:"0"),
                                    "Remark",
                                    _Remark,
                                    "Status",
                                    _Status).Result;

            //1	Danh mục các SSR , người dùng sẽ chọn khi tạo mới / Điều chỉnh SSR cho hành khách
            string uri_Code_SSR_Get = UriConfig.uri_Code_SSR_Get;
            ViewBag.List_Code_SSR_Get = new DataAPI().GetListAPIWithoutParams<DCS_Code_SSR_Get>(uri_Code_SSR_Get,
                                                        HttpContext.Session.GetString("Token"),
                                                        HttpMethod.Post,
                                                        false,
                                                        "Data").Result;

            return View(new SelectBoxViewModel
            {
                Items = new List<string>()
                {
                    "Active",
                    "Delete"
                }
            });
        }
        public IActionResult M_Edit_Comment()
        {
            return View(new SelectBoxViewModel
            {
                Items = new List<string>()
                {
                    "Active",
                    "Delete"
                }
            });
        }
    }
}
