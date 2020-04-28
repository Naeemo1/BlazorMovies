using BlazorMovies.Client.Helpers;
using BlazorMovies.Shared.DTOS;
using BlazorMovies.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorMovies.Client.Repository
{
    public class PersonRepository:IPersonRepository
    {
        private readonly IHttpService httpService;
        private string url = "api/people";

        public PersonRepository(IHttpService httpService)
        {
            this.httpService = httpService;
        }

        public async Task<PaginatedResponse<List<Person>>> GetPeople(PaginationDTO paginationDTO)
        {
            //var response = await httpService.Get<List<Person>>(url);
            //if (!response.Success)
            //{
            //    throw new ApplicationException(await response.GetBody());
            //}

            //return response.Response;

            return await httpService.GetHelper<List<Person>>(url, paginationDTO);

        }

        public async Task<Person> GetPeopleById(int id)
        {
            return await httpService.GetHelper<Person>($"{url}/{id}");
        }

        public async Task<List<Person>> GetPeopleByName(string Name)
        {
            var response = await httpService.Get<List<Person>>($"{url}/search/{Name}");
            if (!response.Success)
            {
                throw new ApplicationException(await response.GetBody());
            }

            return  response.Response;
        }

        public async Task CreatePerson(Person person)
        {
            var response = await httpService.Post(url, person);
            if (!response.Success)
            {
                throw new ApplicationException(await response.GetBody());
            }
        }

        public async Task UpdatePerson (Person person)
        {
            var response = await httpService.Put(url, person);
            if (!response.Success)
            {
                throw new ApplicationException(await response.GetBody());
            }
        }

        public async Task DeletePerson(int id)
        {
            await httpService.DeleteHelper($"{url}/{id}");
        }

    }
}
