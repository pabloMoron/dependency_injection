# Acerca de mi 
[Perfil](https://github.com/pabloMoron/profile)

## Acerca de este proyecto
Este es un proyecto personal, que forma parte de mi [portfolio](https://github.com/pabloMoron/profile#portfolio-personal).


Este es un proyecto de ejemplo de inyecciones de dependencias en.NET Core.


## ¿Qué es la inyección de dependencias?

La idea principal de la inyección de dependencias es mantener lo más bajo posible los niveles de acomplamiento de nuestras clases. El bajo acomplamiento es uno de los principios del diseño de software. 

Es una técnica o una metodologia que permite a los objetos recibir otros objetos de los que dependen (dependencias), sin la necesidad de instanciarlos, sino que se los suministra otra clase que inyectará la implementación deseada.

# .NET Core

Este es un ejemplo de un controlador sin inyeccion de dependencias.

El controlador utiliza un servicio para obtener objetos de personas instanciandolo.

    public class PersonController : ControllerBase
    {
        IPersonRepository _personRepository = new PersonRepository(); //Sí se instancia
        

        [HttpGet]
        public JsonResult Get([FromQuery]int id) {
            var result = _personRepository.getById(id);

            return new JsonResult(new {
                status=200,
                result=result
            });
        }
    }

El siguiente controlador es en escencia el mismo pero no se instancia la dependencia. El servicio es obtenido mediante un contenedor de instancias que provee el objeto cada vez que es requerido.

Se debe agregar el método constructor que asigna el objeto.

<pre>
public class PersonController : ControllerBase {
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
</pre>

## ¿Cómo se obtiene la instancia del servicio?
El objeto se debe configurar a nivel de aplicación.

En .NET 6 se configura en el archivo Program.cs, que es el punto de entrada de la aplicación. En esta versión del framework se ha reducido notoriamente el texto y es el compilador el encargado de completar el método main.

<pre>
using DI.Interfaces;
using DI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton < IPersonRepository, PersonRepository >();
builder.Services.AddControllers();
</pre>

El objeto builder sirve para construir la aplicación y éste objeto, nos ayuda a configurar variables de entorno, servicios de logs, el pipeline para las Requests HTTP y nuestros servicios definidos a medida que usaremos en la aplicación (entre otras cosas).

En nuestro caso, en la siguiente línea estamos agregando un Singleton, un objeto global que será inyectado desde el contenedor a los objetos que dependan de él.

<pre>
builder.Services.AddSingleton < IPersonRepository, PersonRepository >();
</pre>
[Documentación](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.servicecollectionserviceextensions.addsingleton?view=dotnet-plat-ext-6.0#microsoft-extensions-dependencyinjection-servicecollectionserviceextensions-addsingleton(microsoft-extensions-dependencyinjection-iservicecollection-system-type))

La ventaja que nos provee este patrón es que cuando querramos usar otra implementación de IPersonRepository, solo habrá que reemplazar el segundo parámetro por la implementación deseada, y no en cada punto donde se usaba la implementación anterior. Esto claramente reduce el impacto al cambio de implementaciones.

Un detalle que vale la pena aclarar, es que hay 3 maneras similares pero con una diferencia sutil a la hora de agregar servicios, éstas son:

- .AddSingleton< IPersonRepository, PersonRepository >():
El objeto singleton es el mismo para toda la aplicación.

- .AddTransient< IPersonRepository, PersonRepository >():
Los objetos transient son siempre distintos, se provee de una nueva instancia para cada controlador y cada servicio.

- .AddScoped< IPersonRepository, PersonRepository >(): Un objeto Scoped se mantiene vivo dentro de una request, pero se instancia uno distinto en cada request

### Ejemplo 1
Para demostrar las diferencias entre los objetos Singleton, Scopped y Transient se agrega lo siguiente:

- Interfaces ISingleton, IScopped e ITransient que no contienen nada especial, son para darle semántica al ejemplo.

<pre>
public interface ISingleton {
}

public interface IScopped {
}

public interface ITransient {
}
</pre>

- Una clase llamada InjectionObject que implementa las 3 interfaces y con un atributo value.

<pre>
public class InjectionObject : ISingleton, ITransient, IScopped {
    public readonly int value { get; set; }
    
    //Constructor
    public InjectionObject() {
            this.value = new Random().Next(1, 1000);
    }
}
</pre>

- Un nuevo controlador con 6 dependencias, 2 Singleton, 2 Scopped y 2 Transient; junto a un método GET expuesto con la siguiente url:
http://localhost:5014/api/DI

<pre>
public class DIController : ControllerBase {
    private readonly ISingleton _singleton;
    private readonly IScopped   _scopped;
    private readonly ITransient _transient;

    private readonly ISingleton _singleton2;
    private readonly IScopped _scopped2;
    private readonly ITransient _transient2;
...
}
</pre>

- Se configuran los 3 servicios Singleton, Scopped y Transient, con la implementacion InjectionObject.

<pre>
builder.Services.AddSingleton< ISingleton, InjectionObject >();
builder.Services.AddTransient< ITransient, InjectionObject >();
builder.Services.AddScoped< IScopped, InjectionObject >();
</pre>

Finalmente, el método retorna el atributo value de cada dependencia y con este resultado se puede comprobar el comportamiento de cada configuración.

Ejecucion 1:

<pre>
[
  {
    "singletonValue": 336,
    "scoppedValue": 40,
    "transientValue": 711
  },
  {
    "singletonValue_2": 336,
    "scoppedValue_2": 40,
    "transientValue_2": 142
  }
]
</pre>

Ejecucion 2 (Sin reiniciar la instancia de la aplicación):

<pre>
[
  {
    "singletonValue": 336,
    "scoppedValue": 353,
    "transientValue": 380
  },
  {
    "singletonValue_2": 336,
    "scoppedValue_2": 353,
    "transientValue_2": 459
  }
]
</pre>

En base a las respuestas se puede observar que:
- El objeto Singleton es el mismo en todo el ciclo de vida de la aplicacion.
- El ciclo de vida del objeto Scopped es el de una Request.
- Los objetos Transient son distintos en cada Request y cada referencia.

