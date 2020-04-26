using AutoMapper;
using BlazorMovies.Server.Helpers;
using BlazorMovies.Shared.DTOS;
using BlazorMovies.Shared.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorMovies.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MoviesController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IFileStorageService fileStorageService;
        private readonly IMapper mapper;
        private string ContainerName = "movies";

        public MoviesController(ApplicationDbContext context, IFileStorageService fileStorageService, IMapper mapper)
        {
            this.context = context;
            this.fileStorageService = fileStorageService;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IndexPageDTO>> GetMovies()
        {
            var limit = 6;

            var moviesInTheaters = await context.Movies
                .Where(m => m.InTheaters).Take(limit)
                .OrderByDescending(m => m.ReleaseDate)
                .ToListAsync();

            var todaysDate = DateTime.Today;

            var upcomingReleases = await context.Movies
                .Where(m => m.ReleaseDate > todaysDate)
                .OrderBy(m => m.ReleaseDate).Take(limit)
                .ToListAsync();

            var response = new IndexPageDTO();
            response.InTheaters = moviesInTheaters;
            response.UpcomingReleases = upcomingReleases;

            return response;

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DetailsMovieDTO>> Get(int id)
        {
            var movie = await context.Movies.Where(x => x.Id == id)
                .Include(x => x.MoviesGenres).ThenInclude(x => x.Genre)
                .Include(x => x.MoviesActors).ThenInclude(x => x.Person)
                .FirstOrDefaultAsync();

            if (movie == null)
            {
                return NotFound();
            }

            movie.MoviesActors = movie.MoviesActors.OrderBy(x => x.Order).ToList();

            var model = new DetailsMovieDTO();
            model.Movie = movie;
            model.Genres = movie.MoviesGenres.Select(x => x.Genre).ToList();
            model.Actors = movie.MoviesActors.Select(x =>
                new Person
                {
                    Name = x.Person.Name,
                    Picture = x.Person.Picture,
                    Character = x.Character,
                    Id = x.PersonId
                }).ToList();

            return model;

        }

        [HttpGet("update/{id}")]
        public async Task<ActionResult<MovieUpdateDTO>> PutGet(int id)
        {
            var movieActionResult = await Get(id);
            if(movieActionResult.Result is NotFoundResult) { return NotFound(); }

            var movieDetailsDTO = movieActionResult.Value;
            var selectedGenresId = movieDetailsDTO.Genres.Select(x => x.Id).ToList();
            var notSelectedGenres = await context.Genres
                .Where(x => !selectedGenresId.Contains(x.Id)).ToListAsync();

            var model = new MovieUpdateDTO();
            model.Movie = movieDetailsDTO.Movie;
            model.Actors = movieDetailsDTO.Actors;
            model.SelectedGenres = movieDetailsDTO.Genres;
            model.NotSelectedGenres = notSelectedGenres;
            return model;
        }

        [HttpPost]
        public async Task<ActionResult<int>> Post(Movie movie)
        {
            if (!string.IsNullOrWhiteSpace(movie.Poster))
            {
                var poster = Convert.FromBase64String(movie.Poster);
                movie.Poster = await fileStorageService.SaveFile(poster, "jpg", ContainerName);
            }

            if (movie.MoviesActors != null)
            {
                for (int i = 0; i < movie.MoviesActors.Count; i++)
                {
                    movie.MoviesActors[i].Order = i + 1;
                }
            }

            context.Add(movie);
            await context.SaveChangesAsync();
            return movie.Id;
        }

        [HttpPut]
        public async Task<ActionResult> Put(Movie movie)
        {
            var movieDB = await context.Movies.FirstOrDefaultAsync(x => x.Id == movie.Id);

            if (movieDB == null)
            {
                return NotFound();
            }

            movieDB = mapper.Map(movie, movieDB);

            if (!string.IsNullOrWhiteSpace(movie.Poster))
            {
                var movieImage = Convert.FromBase64String(movie.Poster);
                movieDB.Poster = await fileStorageService.EditFile(movieImage, "jpg", ContainerName, movieDB.Poster);
            }

            await context.Database.ExecuteSqlInterpolatedAsync($"delete from MoviesActors where MovieId = {movie.Id}; delete from MoviesGenres where MovieId = {movie.Id}");

            if (movie.MoviesActors != null)
            {
                for (int i = 0; i < movie.MoviesActors.Count; i++)
                {
                    movie.MoviesActors[i].Order = i + 1;
                }
            }

            movieDB.MoviesActors = movie.MoviesActors;
            movieDB.MoviesGenres = movie.MoviesGenres;

            await context.SaveChangesAsync();
            return NoContent();
        }

    }
}