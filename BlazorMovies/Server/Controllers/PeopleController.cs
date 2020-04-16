using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorMovies.Server.Helpers;
using BlazorMovies.Shared.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BlazorMovies.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PeopleController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IFileStorageService fileStorageService;

        public PeopleController(ApplicationDbContext context, IFileStorageService fileStorageService)
        {
            this.context = context;
            this.fileStorageService = fileStorageService;
        }

        [HttpPost]
        public async Task<ActionResult<int>> Post(Person person)
        {   
            if (!string.IsNullOrWhiteSpace(person.Picture))
            {
                var personImage = Convert.FromBase64String(person.Picture);
                person.Picture = await fileStorageService.SaveFile(personImage, "jpg", "people");
            }
            
            context.Add(person);
            await context.SaveChangesAsync();
            return person.Id;
        }

    }
}