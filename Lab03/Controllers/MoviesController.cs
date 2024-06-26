﻿using System;
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
using Microsoft.AspNetCore.Authorization;

namespace Lab03.Controllers
{
    public class MoviesController : AuthController
    {
        //private readonly Lab3MovieWebContext _context;
        DynamoDBHelper _dynamoDbHelper = new DynamoDBHelper();
        S3Helper _s3Helper = new S3Helper();

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
        public async Task<IActionResult> Create([Bind("Id,Title,Genre,Director,ReleaseTime,Rating,CoverFile,MovieFile")] Movie movie)
        {
            ModelState.Remove("UploaderId");
            ModelState.Remove("Comments");

            ModelState.Remove("CoverImage");
            ModelState.Remove("CoverFile");
            ModelState.Remove("MovieFile");
            ModelState.Remove("FileKey");
            ModelState.Remove("FileUrl");

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
                return View(movie);
            }


            if (ModelState.IsValid)
            {
                //_context.Add(movie);
                //await _context.SaveChangesAsync();
                movie.UploaderId = base.LoginUserId;
                movie.UpldateTime = DateTime.Now;

 

                //save movie file to s3 bucket

                if (movie.MovieFile != null && movie.MovieFile.Length > 0)
                {
                    //save by s3helper
                    var filename = movie.Id + "-mov-" + movie.MovieFile.FileName;
                    var fileKey = await _s3Helper.UploadFileAsync(filename, movie.MovieFile.OpenReadStream());
                    if (string.IsNullOrEmpty(fileKey))
                    {
                        return NotFound();
                    }
                    movie.FileKey = fileKey;
                    movie.FileUrl = _s3Helper.GetFileUrl(fileKey);
                }

                //save cover file to s3 bucket
                if (movie.CoverFile != null && movie.CoverFile.Length > 0)
                {
                    var filename = movie.Id + "-cover-" + movie.CoverFile.FileName;
                    //save by s3helper
                    var fileKey = await _s3Helper.UploadFileAsync(filename, movie.CoverFile.OpenReadStream());
                    if (string.IsNullOrEmpty(fileKey))
                    {
                        return NotFound();
                    }
                    movie.CoverImage = _s3Helper.GetFileUrl(fileKey);
                }



                movie.MovieFile = null;
                movie.CoverFile = null;



                var x = await _dynamoDbHelper.AddMovieAsync(movie);
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

            //check movie uploader is current login user
            if (movie.UploaderId != null && movie.UploaderId != base.LoginUserId)
            {
                var msg = "It is not your movie. You can not edit it.";
                //return base.ShowError(msg);

            }


            return View(movie);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost] 
        //[Authorize]
        public async Task<IActionResult> Edit(string id, Movie movie )
        {   

            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            if (id != movie.Id)
            {
                return NotFound();
            }

            var movie1 = await _dynamoDbHelper.GetMovieAsync(id);
            if (movie1 == null)
            {
                return NotFound();
            }

            //check movie uploader is current login user
            if (movie1.UploaderId !=null && movie1.UploaderId != base.LoginUserId)
            {
                var msg = "It is not your movie. You can not edit it.";
                return base.ShowError(msg);

            }

        
            try
            {
                movie1.UploaderId = base.LoginUserId;
                movie1.UpldateTime = DateTime.Now; 
                movie1.Title = movie.Title;
                movie1.Genre = movie.Genre;
                movie1.Director = movie.Director;
                movie1.ReleaseTime = movie.ReleaseTime;
                movie1.Rating = movie.Rating;



                //save movie file to s3 bucket

                if (movie.MovieFile != null && movie.MovieFile.Length > 0)
                {
                    //save by s3helper
                    var filename = movie.Id+"-mov-"+ movie.MovieFile.FileName;
                    var fileKey = await _s3Helper.UploadFileAsync(filename, movie.MovieFile.OpenReadStream());
                    if (string.IsNullOrEmpty(fileKey))
                    {
                        return NotFound();
                    }
                    movie1.FileKey = fileKey;
                    movie1.FileUrl = _s3Helper.GetFileUrl(fileKey); 
                }

                //save cover file to s3 bucket
                if (movie.CoverFile != null && movie.CoverFile.Length > 0)
                {
                    var filename = movie.Id + "-cover-" + movie.CoverFile.FileName;
                    //save by s3helper
                    var fileKey = await _s3Helper.UploadFileAsync(filename, movie.CoverFile.OpenReadStream());
                    if (string.IsNullOrEmpty(fileKey))
                    {
                        return NotFound();
                    }
                    movie1.CoverImage = _s3Helper.GetFileUrl(fileKey); 
                }
                      

                var result = await _dynamoDbHelper.UpdateMovieAsync(movie1);
                Console.WriteLine("Update movie result: " + result);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "error when update movie：" + ex.Message);
                return View(movie);
            }
             

            //return to movie details page
            return RedirectToAction("Details", "Movies", movie); //  redirect to index page

           

            //return View(movie);
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

            //check movie uploader is current login user
            if (movie.UploaderId != base.LoginUserId)
            {
                var msg = "It is not your movie. You can not delete it.";
                return base.ShowError(msg);
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

            var movie1 = await _dynamoDbHelper.GetMovieAsync(id);
            if (movie1 == null)
            {
                return NotFound();
            }

            //check movie uploader is current login user
            if (movie1.UploaderId != base.LoginUserId)
            {
                var msg = "It is not your movie. You can not delete it.";
                return base.ShowError(msg);

            }


            try
            {

                //delete file from s3
                if (!string.IsNullOrEmpty(movie1.FileKey))
                {
                    await _s3Helper.DeleteFileAsync(movie1.FileKey);
                }

                //delete cover file from s3
                if (!string.IsNullOrEmpty(movie1.CoverImage))
                {
                    await _s3Helper.DeleteFileByUrlAsync(movie1.CoverImage);
                }

            }
            catch (Exception ex)
            {

                
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

        //add movie
        [HttpPost]
        public async Task<IActionResult> AddMovie(Movie movie)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _dynamoDbHelper.AddMovieAsync(movie);

                    return RedirectToAction("Index", "Movies"); //  redirect to index page
                }
                else
                {
                    return View(movie);
                }
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("", "error when add movie：" + ex.Message);
                return View(movie);
            }
        }

        //  get all movies
        [HttpGet]
        public async Task<IActionResult> Index()
        {

            var allMovies = await _dynamoDbHelper.GetAllMoviesAsync();
            allMovies = allMovies.OrderByDescending(m => m.UpldateTime).ThenByDescending(m => m.Rating).ToList();
            return View(allMovies);
        }

        //get movies by genre and rating
        [HttpPost]
        public async Task<IActionResult> Index(MovieGenre selectedGenre, double? minRating, double? maxRating)
        {

            var movies = await _dynamoDbHelper.QueryMoviesByFiltersAsync(selectedGenre, minRating, maxRating); 
          


            //return minRating to view
            ViewData["minRating"] = minRating;
            ViewData["maxRating"] = maxRating;
            ViewData["selectedGenre"] = selectedGenre;





            return View(movies);
        }


        //  generate demo data
        [HttpGet]
        public async Task<IActionResult> Init()
        {
            //create some demo data
            await _dynamoDbHelper.GenerateMoviesData();


            return Ok("Init movie data success");
        }


        //add comment ,Get
        [HttpGet]
        public async Task<IActionResult> AddComment(string id)
        {
            try
            {
                var loginUserId = HttpContext.Session.GetString("loginUserId");

                if (string.IsNullOrEmpty(loginUserId))
                {
                    //return Problem("Please login first.");
                    return RedirectToAction("Login", "Users"); //  redirect to index page
                }

                string movieId = id;
                if (string.IsNullOrEmpty(movieId))
                {
                    return NotFound();
                }

                var movie = await _dynamoDbHelper.GetMovieAsync(movieId);
                if (movie == null)
                {
                    return NotFound();
                }

                //redirect to create comment page
                //new RedirectToRouteResult(new RouteValueDictionary()
                //{
                //    {"controller","Comments" },
                //    {"action","Create" },
                //    {"movieId",movieId }
                //});
                //return RedirectToAction("Create", "Comments", movie); //  redirect to index page

                //TempData["movie"] = movie;
                ViewBag.movie = movie;
                Comment comment = new Comment();
                comment.MovieId = movieId;
                comment.UserId = loginUserId;
                comment.UpdateTime = DateTime.Now;
                comment.Id = Guid.NewGuid().ToString();
                comment.Rating = 5;
                ViewBag.comment = comment;
                return View(comment);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("", "error when add comment：" + ex.Message);
                return Problem("error when add comment：" + ex.Message);
            }
        }





        //add comment ,Post
        [HttpPost]
        public async Task<IActionResult> AddComment(Comment comment)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    //get current user's login name
                    var loginUserId = HttpContext.Session.GetString("loginUserId");

                    if (string.IsNullOrEmpty(loginUserId))
                    {
                        //return Problem("Please login first.");
                        return RedirectToAction("Login", "Users"); //  redirect to index page
                    }



                    if (string.IsNullOrEmpty(loginUserId))
                    {
                        return Problem("Please login first.");
                    }

                    //get movie
                    var movieId = comment.MovieId;
                    var movie = await _dynamoDbHelper.GetMovieAsync(movieId);
                    if (movie == null)
                    {
                        return Problem("Movie not found.");
                    }

                    //add comment
                    comment.UserId = loginUserId;
                    comment.UpdateTime = DateTime.Now;
                    comment.Id = Guid.NewGuid().ToString();
                    movie.Comments.Add(comment);

                    this.UpdateMovieRating(movie);
                     //update movie
                     await _dynamoDbHelper.UpdateMovieAsync(movie);
                   

                    return RedirectToAction("Details", "Movies", movie); //  redirect to index page
                }
                else
                {
                    return Problem("Invalid comment.");
                }
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("", "error when add comment：" + ex.Message);
                return Problem("error when add comment：" + ex.Message);
            }
        }


        public async Task<IActionResult> DeleteComment(string movieId, string commentId)
        {
            var id = movieId;
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var movie = await _dynamoDbHelper.GetMovieAsync(id);
            if (movie == null)
            {
                return NotFound();
            }

            //get comment
            var comment = movie.Comments.FirstOrDefault(c => c.Id == commentId);
            if (comment == null)
            {
                return NotFound();
            }
            ViewBag.movie = movie;
            ViewBag.comment = comment;


            //check comment author is current login user
            if (comment.UserId != base.LoginUserId)
            {
                var msg = "It is not your comment. You can not delete it.";
                return base.ShowError(msg);

            }

            var updateTime = comment.UpdateTime;
            if (updateTime != null && ((DateTime)updateTime).AddHours(24) < DateTime.Now)
            {
                var msg = "You can only update your comment within 24 hours after you update it.";
                return base.ShowError(msg);
            }


            return View(comment);
        }

        [HttpPost, ActionName("DeleteCommentConfirmed")]
        public async Task<IActionResult> DeleteCommentConfirmed(string movieId, string commentId)
        {
            var id = movieId;
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var movie = await _dynamoDbHelper.GetMovieAsync(id);
            if (movie == null)
            {
                return NotFound();
            }

            //get comment
            var comment = movie.Comments.FirstOrDefault(c => c.Id == commentId);
            if (comment == null)
            {
                return NotFound();
            }

            //delete comment
            movie.Comments.Remove(comment);

            this.UpdateMovieRating(movie);


            //update movie
            await _dynamoDbHelper.UpdateMovieAsync(movie);
            

            ViewBag.movie = movie;
            ViewBag.comment = comment;

            //return to details page
            return RedirectToAction("Details", "Movies", movie); //  redirect to index page


        }


        //GET : Movies/EditComment/5 
        public async Task<IActionResult> EditComment(string movieId, string commentId)
        {
            var id = movieId;
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var movie = await _dynamoDbHelper.GetMovieAsync(id);
            if (movie == null)
            {
                return NotFound();
            }

            //get comment
            var comment = movie.Comments.FirstOrDefault(c => c.Id == commentId);
            if (comment == null)
            {
                return NotFound();
            }

            ViewBag.movie = movie;
            ViewBag.comment = comment;

            //check comment author is current login user
            if (comment.UserId != base.LoginUserId)
            { 
                var msg = "It is not your comment. You can not edit it."; 
                return base.ShowError(msg);

            }

           
            //only can update 24h after update

            var updateTime = comment.UpdateTime;
            if (updateTime != null && ((DateTime)updateTime).AddHours(24) < DateTime.Now)
            {
                var msg = "You can only update your comment within 24 hours after you update it.";
                return base.ShowError(msg);
            } 



            return View(comment);
        }

        //Post : Movies/EditComment/5 
        [HttpPost]
        public async Task<IActionResult> UpdateComment(Comment comment)
        {

            var loginUserId = HttpContext.Session.GetString("loginUserId");




            var movieId = comment.MovieId;
            var commentId = comment.Id;
            if (string.IsNullOrEmpty(movieId))
            {
                return NotFound();
            }

            var movie = await _dynamoDbHelper.GetMovieAsync(movieId);
            if (movie == null)
            {
                return NotFound();
            }

            //get comment
            Comment comment1 = movie.Comments.FirstOrDefault(c => c.Id == commentId);
            if (comment1 == null)
            {
                return NotFound();
            }


            //check comment author is current login user
            if (comment1.UserId != loginUserId)
            {
                return Problem("You are not the author of this comment.");
            }


            //update comment
            comment1.UpdateTime = DateTime.Now;
            comment1.Content = comment.Content;
            comment1.Rating = comment.Rating;
            comment1.UserId = loginUserId;

            this.UpdateMovieRating(movie);


            //update movie
            await _dynamoDbHelper.UpdateMovieAsync(movie);
            
            ViewBag.movie = movie;
            ViewBag.comment = comment1;
            //return to details page
            return RedirectToAction("Details", "Movies", movie); //  redirect to index page 
             
        }


        //download file from s3
        [HttpGet]
        public async Task<IActionResult> Download(string id)
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

 

            //check file is exist
            if (string.IsNullOrEmpty(movie.FileKey))
            {
                return NotFound();
            }

            //download file
            var file = await _s3Helper.GetFileStreamAsync(movie.FileKey);
            if (file == null)
            {
                return NotFound();
            }

            return File(file, "application/octet-stream", movie.FileKey);
        }




        //update file to s3 ,return file url and file key

        [HttpPost]
        public async Task<IActionResult> UpdateFile(string id)
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

            //check movie uploader is current login user
            if (movie.UploaderId != base.LoginUserId)
            {
                var msg = "It is not your movie. You can not update it.";
                return base.ShowError(msg);

            }

            //upload file to s3
            var file = Request.Form.Files[0];
            if (file == null)
            {
                return NotFound();
            }

            //upload file to s3
            var fileKey = await _s3Helper.UploadFileAsync(id, file.OpenReadStream());
            if (string.IsNullOrEmpty(fileKey))
            {
                return NotFound();
            }

            //update movie
            movie.FileKey = fileKey;
            movie.FileUrl = _s3Helper.GetFileUrl(fileKey);
            await _dynamoDbHelper.UpdateMovieAsync(movie);

            //return to details page
            return RedirectToAction("Details", "Movies", movie); //  redirect to index page 
        }





        //when comments update, update movie rating

        public void UpdateMovieRating(Movie movie)
        {
            if (movie.Comments != null || movie.Comments.Count >= 0)
            { 
                //update movie rating , average rating of all comments , #.# format
                var rating = movie.Comments.Average(c => c.Rating);
                movie.Rating = Math.Round(rating, 1);
                //movie.Rating = movie.Comments.Average(c => c.Rating);
            }

        }

    }
}
