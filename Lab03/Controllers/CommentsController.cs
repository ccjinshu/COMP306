using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lab03.Models;
using Lab03.AWS;

namespace Lab03.Controllers
{
    public class CommentsController : AuthController
    {
        private readonly Lab3MovieWebContext _context;
        private readonly DynamoDBHelper _dynamoDbHelper = new DynamoDBHelper();

        public CommentsController(Lab3MovieWebContext context)
        {
            _context = context;
        }

        // GET: Comments
        public async Task<IActionResult> Index()
        {
              return _context.Comments != null ? 
                          View(await _context.Comments.ToListAsync()) :
                          Problem("Entity set 'Lab3MovieWebContext.Comments'  is null.");
        }

        // GET: Comments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Comments == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments
                .FirstOrDefaultAsync(m => m.Id.Equals( id));
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // GET: Comments/Create
        public IActionResult Create(Movie movie)
        {
             Comment comment = new Comment();
            comment.MovieId = movie.Id; 
            comment.UserId = HttpContext.Session.GetString("loginUserId");
            comment.UpdateTime = DateTime.Now;
            comment.Id = Guid.NewGuid().ToString();
            ViewBag.Movie=movie;

            return View(comment);
        }

        // POST: Comments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Content,UserId,MovieId,UpdateTime,Rating")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                //_context.Add(comment);
                //await _context.SaveChangesAsync();
                //return RedirectToAction(nameof(Index));

                //get movie by movie id
                var movie = _dynamoDbHelper.GetMovieAsync (comment.MovieId).Result;
                if (movie == null)
                {
                    return NotFound();
                }
                //add comment to movie
                movie.Comments.Add(comment);
                await _dynamoDbHelper.UpdateMovieAsync(movie);
                 
                //retun to movie details page
                return RedirectToAction("Details", "Movies", new { id = comment.MovieId });

            }
            return View(comment);
        }

        // GET: Comments/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Comments == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }
            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Content,UserId,UpdateTime,Rating")] Comment comment)
        {
            if (!id.Equals( comment.Id))
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(comment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentExists(comment.Id))
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
            return View(comment);
        }

        // GET: Comments/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Comments == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments
                .FirstOrDefaultAsync(m => m.Id == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Comments == null)
            {
                return Problem("Entity set 'Lab3MovieWebContext.Comments'  is null.");
            }
            var comment = await _context.Comments.FindAsync(id);
            if (comment != null)
            {
                _context.Comments.Remove(comment);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CommentExists(string id)
        {
          return (_context.Comments?.Any(e => e.Id.Equals(id))).GetValueOrDefault();
        }
    }
}
