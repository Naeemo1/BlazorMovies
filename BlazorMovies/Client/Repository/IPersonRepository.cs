using BlazorMovies.Shared.DTOS;
using BlazorMovies.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorMovies.Client.Repository
{
    public interface IPersonRepository
    {
        Task CreatePerson(Person person);
        Task DeletePerson(int id);
        //Task<List<Person>> GetPeople();
        Task<PaginatedResponse<List<Person>>> GetPeople(PaginationDTO paginationDTO);

        //Task<List<Person>> GetPeopleById(int id);
        Task<Person> GetPeopleById(int id);
        Task<List<Person>> GetPeopleByName(string Name);
        Task UpdatePerson(Person person);
    }
}
