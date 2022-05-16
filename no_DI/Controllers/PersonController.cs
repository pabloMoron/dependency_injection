using DI.Interfaces;
using DI.Models;
using DI.Services;
using Microsoft.AspNetCore.Mvc;

namespace no_DI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        IPersonRepository _personRepository = new PersonRepository();//Sí se instancia

        [HttpGet]
        public JsonResult Get([FromQuery] int? id, [FromQuery] string? name="")
        {
            Person? result = null ;
            if(id!=null)
                result = _personRepository.getById(id.Value);
            
            if(!string.IsNullOrEmpty(name) && id== null)
                result = _personRepository.getByName(name);

            return new JsonResult(new
            {
                status = 200,
                result = result
            });
        }
    }
}
