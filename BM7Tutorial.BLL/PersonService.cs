using BM7Tutorial.DAL.Models;
using Nexus.Base.CosmosDBRepository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BM7Tutorial.BLL
{
    public class PersonService
    {
        private readonly IDocumentDBRepository<Person> _repository;
        public PersonService(IDocumentDBRepository<Person> repository)
        {
            if (_repository == null)
            {
                _repository = repository;
            }
        }

        public async Task<Person> GetPersonById(string id, Dictionary<string, string> pk)
        {
            return await _repository.GetByIdAsync(id, pk);
        }
    }
}
