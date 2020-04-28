using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BlazorMovies.Server.Helpers;
using BlazorMovies.Shared.DTOS;
using BlazorMovies.Shared.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorMovies.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PeopleController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IFileStorageService fileStorageService;
        private readonly IMapper mapper;
        private string ContainerName = "people";

        public PeopleController(ApplicationDbContext context, IFileStorageService fileStorageService, IMapper mapper)
        {
            this.context = context;
            this.fileStorageService = fileStorageService;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<Person>>> GetPeople([FromQuery]PaginationDTO paginationDTO)
        {
            var queryable = context.Person.AsQueryable();
            await HttpContext.InsertPagination(queryable, paginationDTO.RecordsPerPage);

            return await queryable.Paginate(paginationDTO).ToListAsync();
        }

        [HttpGet("search/{searchText}")]
        public async Task<ActionResult<List<Person>>> FilterByName(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText)) { return new List<Person>(); }

            return await context.Person.Where(p => p.Name.Contains(searchText))
                .Take(5)
                .ToListAsync();

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Person>> GetPeopleId(int id)
        {
            var person = await context.Person.FirstOrDefaultAsync(x => x.Id == id);
            if (person == null) { return NotFound(); }

            return person;
        }


        [HttpPost]
        public async Task<ActionResult<int>> Post(Person person)
        {   
            if (!string.IsNullOrWhiteSpace(person.Picture))
            {
                var personImage = Convert.FromBase64String(person.Picture);
                person.Picture = await fileStorageService.SaveFile(personImage, "jpg", ContainerName);
            }
            
            context.Add(person);
            await context.SaveChangesAsync();
            return person.Id;
        }

        [HttpPut]
        public async Task<ActionResult> Put(Person person)
        {
            var personDB = await context.Person.FirstOrDefaultAsync(x => x.Id == person.Id);

            if (personDB == null)
            {
                return NotFound();
            }

            personDB = mapper.Map(person, personDB);

            if (!string.IsNullOrWhiteSpace(person.Picture))
            {
                var personImage = Convert.FromBase64String(person.Picture);
                personDB.Picture = await fileStorageService.EditFile(personImage, "jpg", ContainerName, personDB.Picture);
            }

            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var person = await context.Person.FirstOrDefaultAsync(x => x.Id == id);

            if (person == null) { return NotFound(); }

            context.Remove(person);
            await context.SaveChangesAsync();

            return NoContent();
        }


    }
}