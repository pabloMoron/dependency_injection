using DI.Interfaces;
using DI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DIController : ControllerBase
    {
        private readonly ISingleton _singleton;
        private readonly IScopped   _scopped;
        private readonly ITransient _transient;

        private readonly ISingleton _singleton2;
        private readonly IScopped _scopped2;
        private readonly ITransient _transient2;
        public DIController(
            ISingleton singleton,
            IScopped scopped, 
            ITransient transient,
            ISingleton singleton2,
            IScopped scopped2,
            ITransient transient2) {
            this._singleton = singleton;
            this._scopped   = scopped;
            this._transient = transient;
            this._singleton2 = singleton2;
            this._scopped2 = scopped2;
            this._transient2 = transient2;
        }

        [HttpGet]
        public JsonResult Get()
        {

            return new JsonResult(new List<Object>
            { new{
                singletonValue = ((InjectionObject)_singleton).value,
                scoppedValue = ((InjectionObject)_scopped).value,
                transientValue = ((InjectionObject)_transient).value,
            },
            new{
                singletonValue_2 = ((InjectionObject)_singleton2).value,
                scoppedValue_2 = ((InjectionObject)_scopped2).value,
                transientValue_2 = ((InjectionObject)_transient2).value
            }});
        }
    }
}
