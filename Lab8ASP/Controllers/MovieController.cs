using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lab8ASP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lab8ASP.Models.Movies;

namespace Lab8ASP.Controllers
{
    public class MovieController : Controller
    {
        private readonly MoviesContext _context;

        public MovieController(MoviesContext context)
        {
            _context = context;
        }

        // GET: Movie
        public IActionResult Index(int page = 1, int size = 10)
        {
            return View( PagingListAsync<Movie>.Create(
                (p, s) => 
                    _context.Movies
                        .OrderBy(b => b.Title)
                        .Skip((p - 1) * s)
                        .Take(s)
                        .AsAsyncEnumerable(),
                _context.Movies.Count(),
                page,
                size));
        }

        // GET: Movie/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .FirstOrDefaultAsync(m => m.MovieId == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // GET: Movie/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Title, Overview, ReleaseDate, CompanyId")]
            MovieModel model)
        {
            if (ModelState.IsValid)
            {
                int newId = _context.Movies.Any()
                    ? _context.Movies.Max(m => m.MovieId) + 1
                    : 1;
                
                var newMovie = new Movie
                {
                    MovieId = newId,
                    Title = model.Title,
                    Overview = model.Overview,
                    ReleaseDate = model.ReleaseDate
                };
                _context.Movies.Add(newMovie);
                await _context.SaveChangesAsync(); 
                
                if (!_context.ProductionCompanies.Any(c => c.CompanyId == model.CompanyId))
                {
                    throw new Exception($"Production Company with ID {model.CompanyId} does not exist.");
                }
                
                var newMovieCompany = new MovieCompany
                {
                    MovieId = newMovie.MovieId,   
                    CompanyId = model.CompanyId  
                };
                
                _context.MovieCompanies.Add(newMovieCompany);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // POST: Movie/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MovieId,Title,Budget,Homepage,Overview,Popularity,ReleaseDate,Revenue,Runtime,MovieStatus,Tagline,VoteAverage,VoteCount")] Movie movie)
        {
            if (id != movie.MovieId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.MovieId))
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
            return View(movie);
        }

        // GET: Movie/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .FirstOrDefaultAsync(m => m.MovieId == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST: Movie/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie != null)
            {
                _context.Movies.Remove(movie);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(int id)
        {
            return _context.Movies.Any(e => e.MovieId == id);
        }
    }
}
