using DI.Interfaces;
using DI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        IPersonRepository _personRepository; //No se instancia
        public PersonController(IPersonRepository personRepository) {
            this._personRepository = personRepository;
        }

        [HttpGet]
        public JsonResult Get([FromQuery]int id) {
            var result = _personRepository.getById(id);

            return new JsonResult(new {
                status=200,
                result=result
            });
        }
    }
}
