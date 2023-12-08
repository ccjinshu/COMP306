using CCBnb_Admin_Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Policy;
using System.Text;

namespace CCBnb_Admin_Web.Controllers
{
    public class RoomController : ApiController
    {
        // GET: RoomController
        public async Task<ActionResult> IndexAsync()
        {

  //          curl - X 'GET' \
  //'http://v6-510460395.ca-central-1.elb.amazonaws.com/api/Room' \
  //-H 'accept: text/plain'


            //call restapi to get all rooms
            var apiUrl= "/api/Room";
            var client = base.Client;
            //client.DefaultRequestHeaders.Add("Accept", "application/json");
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
        public async Task<ActionResult> DetailsAsync(int id)
        {
            // 调用 REST API 获取指定房间的详细信息
            var apiUrl = $"/api/Room/{id}";
            var client = base.Client;
            var response = await client.GetAsync(apiUrl);
            Console.WriteLine($"apiUrl: {apiUrl}");
            Console.WriteLine($"response: {response}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var room = JsonConvert.DeserializeObject<RoomDTO>(content);
                return View(room);
            }
            else
            {
                ViewData["Error"] = $"Error retrieving details for room {id}";
                return View();
            }
        }

        // GET: RoomController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RoomController/Create 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RoomDTO room)
        {
            var apiUrl = "/api/Room";
            var client = base.Client;
            var json = JsonConvert.SerializeObject(room);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ViewData["Error"] = "Error creating room";
                return View(room);
            }
        }

        // GET: RoomController/Edit/5
        public async Task<ActionResult> EditAsync(int id)
        {
            
            return await DetailsAsync(id);
           

        }

        // POST: RoomController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAsync(int id, RoomDTO roomDTO)
        {
         
            //API Method , PUT : api/Room/5
            var apiUrl = $"/api/Room/{id}";
            var client = base.Client;
            var response = await client.PutAsync(apiUrl,
                new StringContent(JsonConvert.SerializeObject(roomDTO), Encoding.UTF8, "application/json")
                );
            Console.WriteLine($"apiUrl: {apiUrl}");
            Console.WriteLine($"response: {response}");
            
            if (!response.IsSuccessStatusCode)
            {
                ViewData["Error"] = $"Error when update for room {id}";
                return View();
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }   
             


        }

        // GET: RoomController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            return await DetailsAsync(id);
        }

        // POST: Room/Delete/5 
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public  async Task<ActionResult> DeleteAsync(int id)
        {
  //curl - X 'DELETE' \
  //'https://34.128.145.217.nip.io/bnb_auth_v1/api/Room/888' \
  //-H 'accept: */*'
           // var apiUrl = $"/api/Room/{id}";
            var apiUrl = $"/api/Room/{id}";
            var client = base.Client;
            var response = await client.DeleteAsync(apiUrl);
            Console.WriteLine($"apiUrl: {apiUrl}");
            Console.WriteLine($"response: {response}");

            if (!response.IsSuccessStatusCode)
            {
                ViewData["Error"] = $"Error when delete for room {id}";
                return View();
            }
            else
            {
                return RedirectToAction(nameof(Index));
            } 


        }
           


















 
    }
}
