using BM7Tutorial.DAL.Models;
using Moq;
using Nexus.Base.CosmosDBRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BM7Tutorial.BLL.Test
{
    public class PersonServiceTest
    {
        public class GetPersonById
        {
            [Theory]
            [InlineData("1")]
            [InlineData("2")]
            public async Task GetPersonByIdExists_ReturnPerson(string id)
            {
                // arrange
                var repo = new Mock<IDocumentDBRepository<Person>>();

                IEnumerable<Person> persons = new List<Person>
                {
                    {new Person() { Id = "1", FirstName = "abcd", LastName = "efgh"} },
                    {new Person() { Id = "2", FirstName = "ijkl", LastName = "mnop"} }
                };

                var personData = persons.Where(p => p.Id == id).FirstOrDefault();

                repo.Setup(c => c.GetByIdAsync(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>()
                )).Returns(
                    Task.FromResult<Person>(personData)
                );

                var svc = new PersonService(repo.Object);

                // act
                var act = await svc.GetPersonById(id, null);

                // assert
                Assert.Equal(personData, act);
            }
        }
    }
}
