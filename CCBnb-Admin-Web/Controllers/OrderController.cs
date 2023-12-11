using CCBnb_Admin_Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Policy;
using System.Text;

namespace CCBnb_Admin_Web.Controllers
{
    public class OrderController : ApiController
    {
        // GET: RoomController
        public async Task<ActionResult> IndexAsync()
        {

  //          curl - X 'GET' \
  //'http://v6-510460395.ca-central-1.elb.amazonaws.com/api/Room' \
  //-H 'accept: text/plain'


            //call restapi to get all rooms
            var apiUrl= "/api/Booking";
            var client = base.Client;
            //client.DefaultRequestHeaders.Add("Accept", "application/json");
            var response = await client.GetAsync(apiUrl);
            //print log with varbiables name and value to console
            Console.WriteLine($"apiUrl: {apiUrl}");
            Console.WriteLine($"response: {response}");
             


            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var rooms = JsonConvert.DeserializeObject<List<BookingDTO>>(content);
                ViewData["Title"] = "Orders";
                ViewData["Message"] = "List of all Order";
                ViewData["Rooms"] = rooms;

                return View(rooms);
                 
            }
            else
            {
                ViewData["Error"] = "Error retrieving Order";
                return View();
                
            } 
            


             
        }

        // GET: BookingController/Details/5
        public async Task<ActionResult> DetailsAsync(int id)
        {
            // 调用 REST API 获取详细信息
            var apiUrl = $"/api/Booking/{id}";
            var client = base.Client;
            var response = await client.GetAsync(apiUrl);
            Console.WriteLine($"apiUrl: {apiUrl}");
            Console.WriteLine($"response: {response}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var Booking = JsonConvert.DeserializeObject<BookingDTO>(content);
                return View(Booking);
            }
            else
            {
                ViewData["Error"] = $"Error retrieving details for order {id}";
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
        public async Task<ActionResult> Create(BookingDTO booking)
        {
            var apiUrl = "/api/Booking";
            var client = base.Client;
            var json = JsonConvert.SerializeObject(booking);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ViewData["Error"] = "Error creating room";
                return View(booking);
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
        public async Task<ActionResult> EditAsync(int id, BookingDTO bookingDTO)
        {

            //API Method , PUT : api/booking/5
            var apiUrl = $"/api/booking/{id}";
            var client = base.Client;
            var response = await client.PutAsync(apiUrl,
                new StringContent(JsonConvert.SerializeObject(bookingDTO), Encoding.UTF8, "application/json")
                );
            Console.WriteLine($"apiUrl: {apiUrl}");
            Console.WriteLine($"response: {response}");
            
            if (!response.IsSuccessStatusCode)
            {
                ViewData["Error"] = $"Error when update for booking {id}";
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
            //'https://34.128.145.217.nip.io/bnb_auth_v1/api/booking/888' \
            //-H 'accept: */*'
            // var apiUrl = $"/api/Booking/{id}";
            var apiUrl = $"/api/Booking/{id}";
            var client = base.Client;
            var response = await client.DeleteAsync(apiUrl);
            Console.WriteLine($"apiUrl: {apiUrl}");
            Console.WriteLine($"response: {response}");

            if (!response.IsSuccessStatusCode)
            {
                ViewData["Error"] = $"Error when delete for Booking {id}";
                return View();
            }
            else
            {
                return RedirectToAction(nameof(Index));
            } 


        }




        //Enable Room ; PATCH: api/Booking/5
        [HttpPatch]
        public async Task<ActionResult> CheckInAsync(int id)
        {
            //set patchData
            var patchData = new[]
            {
                new
                {
                    operationType = 0,
                    path = "Status",
                    op = "replace",
                    value = "CheckIn"
                }
            };

            //API Method , PUT : api/Booking/5
            var apiUrl = $"/api/Booking/{id}";
            var client = base.Client;
            var response = await client.PatchAsync(apiUrl,
                               new StringContent(JsonConvert.SerializeObject(patchData), Encoding.UTF8, "application/json")
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

        //Disable Room ; PATCH: api/Room/5
        [HttpPatch]
        public async Task<ActionResult> CheckOutAsync(int id)
        {

            //            curl - X 'PATCH' \
            //  'http://localhost:8306/api/Booking/1' \
            //  -H 'accept: */*' \
            //  -H 'Content-Type: application/json-patch+json' \
            //  -d '[
            //  {
            //                "operationType": 0,
            //    "path": "Status",
            //    "op": "replace", 
            //    "value": "Disable"
            //  }
            //]'
            //set patchData
            var patchData = new[]
            {
                new
                {
                    operationType = 0,
                    path = "Status",
                    op = "replace",
                    value = "CheckOut"
                }
            }; 


            //API Method , PUT : api/Room/5
            var apiUrl = $"/api/Booking/{id}";
            var client = base.Client;
            var response = await client.PatchAsync(apiUrl,
                                              new StringContent(JsonConvert.SerializeObject(patchData), Encoding.UTF8, "application/json")
                                                                                           );
            Console.WriteLine($"apiUrl: {apiUrl}");
            Console.WriteLine($"response: {response}");

            if (!response.IsSuccessStatusCode)
            {
                ViewData["Error"] = $"Error when update for Booking {id}";
                return View();
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

















 
    }
}
