using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lab03.Models;
using NuGet.Protocol;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Lab03.AWS;
using Microsoft.Extensions.Azure;

namespace Lab03.Controllers
{
    public class MoviesController : Controller
    {
        //private readonly Lab3MovieWebContext _context;
        DynamoDBHelper _dynamoDbHelper = new DynamoDBHelper();

        public MoviesController(Lab3MovieWebContext context)
        {
            //_context = context;
        }

        // GET: Movies
        //public async Task<IActionResult> Index()
        //{
        //    return _context.Movies != null ?
        //                View(await _context.Movies.ToListAsync()) :
        //                Problem("Entity set 'Lab3MovieWebContext.Movies'  is null."); 
        //}
      
        // GET: Movies/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var movie = await _dynamoDbHelper.GetMovieAsync(id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie); 
        }

        // GET: Movies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Movies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Genre,Director,ReleaseTime,Rating,CoverImage,FileKey,FileUrl")] Movie movie)
        {
            ModelState.Remove("Comments");

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
                String msg=errors.Aggregate("", (current, error) => current + error.ErrorMessage);
                ViewData["ErrorMessage"] = "Please correct the validation errors." + msg;
                return View(movie);
            }


            if (ModelState.IsValid)
            {
                //_context.Add(movie);
                //await _context.SaveChangesAsync();

              var x=     await   _dynamoDbHelper.AddMovieAsync(movie);
                //print log
                Console.WriteLine("Add movie:" + x);  

                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var movie = await _dynamoDbHelper.GetMovieAsync(id);
            if (movie == null)
            {
                return NotFound();
            } 
            return View(movie);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Title,Genre,Director,ReleaseTime,Rating,CoverImage,FileKey,FileUrl")] Movie movie)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            if (id != movie.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var result = await _dynamoDbHelper.UpdateMovieAsync(movie);
                    Console.WriteLine("Update movie result: " + result);

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "更新电影时出错：" + ex.Message);
                    return View(movie);
                }
            }

            return View(movie);
        }

        // GET: Movies/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var movie = await _dynamoDbHelper.GetMovieAsync(id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

              await _dynamoDbHelper.DeleteMovieAsync(id);
            Console.WriteLine("Delete movie  : " + id);

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> MovieExists(string id)
        {
            var movie = await _dynamoDbHelper.GetMovieAsync(id);
            return movie != null;
        }

        //

        // 处理添加电影的 POST 请求
        [HttpPost]
        public async Task<IActionResult> AddMovie(Movie movie)
        {
            try
            {
                if (ModelState.IsValid)
                {


                    // 创建 Movie 对象并设置 Id


                    // 将电影信息存储到 DynamoDB 表中
                   await _dynamoDbHelper.AddMovieAsync(movie);



                    return RedirectToAction("Index", "Movies"); // 添加成功后重定向到电影列表页面
                }
                else
                {
                    return View(movie);
                }
            }
            catch (Exception ex)
            {
                // 处理异常情况
                ModelState.AddModelError("", "添加电影时出错：" + ex.Message);
                return View(movie);
            }
        }

        // 查询全部电影列表
        [HttpGet]
        public async Task<IActionResult> Index()
        {
          

            var allMovies = await _dynamoDbHelper.GetAllMoviesAsync(); 
            return View(allMovies);
        }
        [HttpPost]
        public async Task<IActionResult> Index(MovieGenre selectedGenre, double? minRating, double? maxRating)
        {

            var movies    = await _dynamoDbHelper.QueryMoviesByGenreAsync(selectedGenre);  // 获取所有电影数据

             

            return View(movies);
        }


        // 查询全部电影列表
        [HttpGet]
        public async Task<IActionResult> Init()
        {
            //create some demo data
            await _dynamoDbHelper.GenerateMoviesData();

 
            return Ok("Init movie data success");
        }


    }
}
