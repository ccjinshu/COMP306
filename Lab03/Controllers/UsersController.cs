using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lab03.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
namespace Lab03.Controllers
{
    public class UsersController : Controller
    {
        private readonly Lab3MovieWebContext _context;

        public UsersController(Lab3MovieWebContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
              return _context.Users != null ? 
                          View(await _context.Users.ToListAsync()) :
                          Problem("Entity set 'Lab3MovieWebContext.Users'  is null.");
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,Email,Password,ConfirmPassword")] User user)
        {

            var password = user.Password;
            var confirmPassword = user.ConfirmPassword;

            //remove ConfirmPassword from ModelState

            //ModelState.Remove("ConfirmPassword");
            ModelState.Remove("PasswordHash");

            //check if password and confirmPassword are the same
            //if (password != confirmPassword)
            //{
            //    ViewData["ErrorMessage"] = "Password and Confirm Password do not match.";
            //    return View(user);
            //}



            if (!ModelState.IsValid)
            {

                foreach (var modelStateEntry in ModelState.Values)
                {
                    foreach (var error in modelStateEntry.Errors)
                    {
                        Console.WriteLine($"Model Error: {error.ErrorMessage}");
                    }
                }


                var errors = ModelState.Values.SelectMany(v => v.Errors);
                String msg = errors.Aggregate("", (current, error) => current + error.ErrorMessage);
                ViewData["ErrorMessage"] = "Please correct the validation errors." + msg;
                return View(user);
            }

            if (ModelState.IsValid)
            { 

                //check if user already exists
                var existingUser = _context.Users.FirstOrDefault(u => u.Email == user.Email);
                if (existingUser != null)
                {
                    var msg= " "+ user.Email+"   already  registered. Please login."; 
                    ViewData["ErrorMessage"] =   msg;
                    return View(user);
                }  
               
                user.PasswordHash = hashPassword(user.Password);
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,Email,Password,ConfirmPassword")] User user)
        {
            //remove passwordhash from ModelState
            ModelState.Remove("PasswordHash");


            if (id != user.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {


                    user.PasswordHash = hashPassword(user.Password);

                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'Lab3MovieWebContext.Users'  is null.");
            }
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }
            
            await _context.SaveChangesAsync(); 

            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
          return (_context.Users?.Any(e => e.UserId == id)).GetValueOrDefault();
        }



        //Logout user
        public IActionResult Logout()
        { 
            //remove session
            HttpContext.Session.Remove("loginName");
            HttpContext.Session.Remove("loginUserId");
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home"); //  redirect to index page
        }

        // GET: Users/Create
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login([Bind("Email,Password")] User user)
        {
            //remove passwordhash from ModelState
            ModelState.Remove("PasswordHash");
            ModelState.Remove("ConfirmPassword");
            if (!ModelState.IsValid)
            {

                foreach (var modelStateEntry in ModelState.Values)
                {
                    foreach (var error in modelStateEntry.Errors)
                    {
                        Console.WriteLine($"Model Error: {error.ErrorMessage}");
                    }
                }


                var errors = ModelState.Values.SelectMany(v => v.Errors);
                String msg = errors.Aggregate("", (current, error) => current + error.ErrorMessage);
                ViewData["ErrorMessage"] = "Please correct the validation errors." + msg;
                return View(user);
            }



            try
            {
                if (ModelState.IsValid)
                {
                    //check if user already exists
                    User existingUser = _context.Users.FirstOrDefault(u => u.Email == user.Email);
                    if (existingUser == null)
                    {
                        var msg= " "+ user.Email+"   not registered. Please register.";
                        ViewData["ErrorMessage"] =   msg;
                        return View(user);
                    }

                    //check password
                    var hashstr1 = this.hashPassword(user.Password);
                    var hashstr2 = this.hashPassword(user.Password);
                    if (existingUser.PasswordHash != hashPassword(user.Password))
                    {
                        var msg= " "+ user.Email+"   password is incorrect.";
                        ViewData["ErrorMessage"] =   msg;
                        return View(user);
                    }


                    //login success

                    //set session
                    HttpContext.Session.SetString("loginName", existingUser.Email);
                    HttpContext.Session.SetString("loginUserId", existingUser.UserId+"");
                    // 将用户信息传递到视图中 
                    ViewBag.loginName = existingUser.Email;

                    return RedirectToAction("Index", "Movies"); //  redirect to index page
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return View(user);
        }



        string   hashPassword(string password)
        {
            // Generate a 128-bit salt using a sequence of
            // cryptographically strong random bytes.
            //byte[] salt = RandomNumberGenerator.GetBytes(128 / 8); // divide by 8 to convert bits to bytes
            //Console.WriteLine($"Salt: {Convert.ToBase64String(salt)}");
             

            //define a salt for testing
            byte[] salt = Convert.FromBase64String("XrY7uAFWMUfzaD6xQFNgaQ==");
            // derive a 256-bit subkey (use HMACSHA256 with 100,000 iterations)
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password!,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));

             return hashed;
        }



    }
}
