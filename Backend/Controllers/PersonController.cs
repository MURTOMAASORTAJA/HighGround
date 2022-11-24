using Microsoft.AspNetCore.Mvc;
using Backend.Services;

namespace Backend.Controllers
{
    [ApiController]
    [Route("person")]
    public class PersonController
    {
        private DataService _dataService;

        public PersonController(DataService service)
        {
            _dataService = service;    
        }

        [HttpGet]
        public IActionResult GetPerson(Guid id)
        {
            var person = _dataService.GetPerson(id);

            return person != null
                ? new OkObjectResult(person)
                : new NotFoundObjectResult("No such person found.");
        }

        public IActionResult FindPeople(string nameContains)
        {
            var people = _dataService.FindPeople(nameContains);
            return people.Any()
                ? new OkObjectResult(people)
                : new NoContentResult(); // HTTP 204
        }


    }
}
