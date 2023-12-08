using CCBnb_Admin_Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Policy;

namespace CCBnb_Admin_Web.Controllers
{
    public class RoomController : Controller
    {
        // GET: RoomController
        public async Task<ActionResult> IndexAsync()
        {

  //          curl - X 'GET' \
  //'http://v6-510460395.ca-central-1.elb.amazonaws.com/api/Room' \
  //-H 'accept: text/plain'


            //call restapi to get all rooms
            var apiUrl= "http://v6-510460395.ca-central-1.elb.amazonaws.com/api/Room";
            var client = new HttpClient ();
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            var response = await client.GetAsync(apiUrl);
            //print log with varbiables name and value to console
            Console.WriteLine($"apiUrl: {apiUrl}");
            Console.WriteLine($"response: {response}");
             


            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var rooms = JsonConvert.DeserializeObject<List<RoomDTO>>(content);
                ViewData["Title"] = "Rooms";
                ViewData["Message"] = "List of all rooms";
                ViewData["Rooms"] = rooms;

                return View(rooms);
                 
            }
            else
            {
                ViewData["Error"] = "Error retrieving rooms";
                return View();
                
            } 
            


             
        }

        // GET: RoomController/Details/5
        public ActionResult Details(int id)
        {
            //call restapi to get room details


            return View();
        }

        // GET: RoomController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RoomController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(IndexAsync));
            }
            catch
            {
                return View();
            }
        }

        // GET: RoomController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: RoomController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(IndexAsync));
            }
            catch
            {
                return View();
            }
        }

        // GET: RoomController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: RoomController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(IndexAsync));
            }
            catch
            {
                return View();
            }
        }
    }
}
