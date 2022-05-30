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

## Resolucion de inyeccion de dependencias condicional
En muchas situaciones se puede dar el caso de que tengamos varias implementaciones de una misma interfaz y debemos proporcional una u otra dependiendo de una condición, o del resultado de alguna ejecución.

Los métodos AddSingleton, AddScopped y AddTransient tienen una sobrecarga particular que nos permite resolver este escenario.

<pre>
public static IServiceCollection AddSingleton< TService >(
            this IServiceCollection services,
            Func< IServiceProvider, TService > implementationFactory)
            where TService : class
</pre>

La firma del método nos indica que recibe un delegate (una función), que es el que finalmente nos retornará un tipo concreto del servicio.

Este caso se verá en el ejemplo 2.

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

### Ejemplo 2

Para la situación de lógica condicional en las dependencias, se vió que existe un método que nos da soporte a esta situación. Para llevar a cabo el ejemplo se creó una nueva aplicación con los siguientes elementos:

- Una interfaz ITaxCalculator, con un método que simplemente devuelve un entero
<pre>
internal interface ITaxCalculator {
  public int CalculateTax();
}
</pre>

- 2 Implementaciones para la misma interfaz

<pre>

public class AustraliaTaxCalculator : ITaxCalculator {
  public int CalculateTax() {
    return 10;
  }
}
------------------------------------

internal class EuropeTaxCalculator : ITaxCalculator {
  public int CalculateTax() {
    return 20;
  }
}
</pre>

- Un enum de 2 elementos
<pre>
public enum Locations {
  Europe,
  Australia
}
</pre>

- Una clase ficticia que reprensente una compra
<pre>
internal class Purchase {
  private readonly Func< Locations, ITaxCalculator > _accessor;
  public Purchase(Func< Locations, ITaxCalculator > accessor) {
    this._accessor = accessor;
  }

  public int CheckOut(Locations location) {
    var tax = _accessor(location).CalculateTax();
    return tax + 100;
  }
}
</pre>
Es importante destacar que lo que se inyecta en las clases dependientes es el delegado, comunmente llamado accesor.


La idea es que dependiendo de la ubicación se ejecute una implementación o la otra.
Para esto configuramos nuestro singleton de la siguiente manera:

<pre>

services.AddSingleton< Func< Locations, ITaxCalculator > >(
  ServiceProvider => key =>
    {
      switch (key) {
        case Locations.Australia:
          return ServiceProvider.GetService< AustraliaTaxCalculator >();

        case Locations.Europe:
          return ServiceProvider.GetService< EuropeTaxCalculator >();
        
        default:
          return null;
      }
});
</pre>

Con estas instrucciones estamos pasando una función con un parametro del tipo Locations (nuestro enum), que retorne un objeto que implemente ITaxCalculator en base al parámetro que dentro de la función se referencia como 'key'.


Luego configuramos un Singleton de tipo Purchase para que resuelva las dependencias internamente y lo instanciamos
<pre>
services.AddSingleton< Purchase >();
var serviceProvider = services.BuildServiceProvider();
var purchase = serviceProvider.GetService< Purchase >();
</pre>

La ejecución de la aplicación termina con la llamada a Purchase
<pre>
Console.WriteLine($"Australia purchase: {purchase.CheckOut(Locations.Australia)}");
Console.WriteLine($"Europe purchase: {purchase.CheckOut(Locations.Europe)}");
</pre>

Lo que nos da como salida de pantalla
<pre>
Australia purchase: 110
Europe purchase: 120
</pre>
