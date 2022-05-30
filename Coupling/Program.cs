using System;
using Microsoft.Extensions.DependencyInjection;
namespace Coupling
{
    /// <summary>
    /// Caso de ejemplo en una aplicacion de consola.
    /// 
    /// El caso esta planteado por una hipotetica UI, una capa de negocio y una capa de acceso a datos.
    /// La idea del ejemplo a entender conceptualmente lo que se necesita para configurar una inyeccion de dependencias
    /// y entender donde refactorizar cuando hay un cambio de implementacion.
    /// 
    /// 
    /// Para este tipo de aplicaciones hay que instalar el paquete de Nugget Microsft.Extensions.DependencyInjection
    /// </summary>
    internal class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var services = new ServiceCollection();

            /// Si cambia la base de datos, que deberia modificar?
            /// Si cambia la logica del negocio, o parte de ella, que deberia modificar?

            services.AddSingleton<IDataAccess, DataAcess>();
            services.AddSingleton<IBusiness, Business>();
            services.AddSingleton<IUserInterface, UserInterface>();

            var serviceProvider = services.BuildServiceProvider();

            var ui = serviceProvider.GetService<IUserInterface>();
            /// Que tipo de objeto es ui?
            
            /// Como nuestra implementacion de interfaz de usuario se declara como un servicio,
            /// no es necesario pasar el parametro al constructor, se resuelve internamente la inyeccion de dependencias entre servicios.
            ui.GetData();
        }
    }

    /// <summary>
    /// Conceptualmente es una interfaz de usuario, la vista de una pagina web, por ejemplo.
    /// Se definen los metodos.
    /// </summary>
    public interface IUserInterface
    {
        public void GetData();
    }

    /// <summary>
    /// Primera implementacion de una interfaz de usuario
    /// </summary>
    public class UserInterface : IUserInterface
    {
        public IBusiness _business;
        public UserInterface(IBusiness business) {
            this._business = business;
        }
        public void GetData()
        {
            Console.Write("Insert you username: ");
            var username = Console.ReadLine();
            Console.Write("Insert you password: ");
            var password = Console.ReadLine();

            Console.WriteLine("SigingUp");
            _business.SignUp(username, password);
        }
    }

    /// <summary>
    /// Segunda implementacion de una interfaz de usuario
    /// </summary>
    public class UserInterfaceV2 : IUserInterface
    {
        public IBusiness _business;
        public UserInterfaceV2(IBusiness business)
        {
            this._business = business;
        }
        public void GetData()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Interfaz de lo que debe ofrecer una implementacion de la capa de negocios
    /// </summary>
    public interface IBusiness {
        public void SignUp(string userName, string Password);    
    }

    /// <summary>
    /// Primera implementacion de la capa de negocios
    /// </summary>
    public class Business : IBusiness
    {
        public IDataAccess _dataAccess;
        public Business(IDataAccess dataAccess)
        {
            this._dataAccess = dataAccess;
        }
        public void SignUp(string userName, string Password)
        {
            Console.WriteLine("Busines layer...");
            Console.WriteLine("Logic of signup");
            _dataAccess.Store(userName,Password);
        }
    }

    /// <summary>
    /// Segunda implementacion de la capa de negocios
    /// </summary>
    public class BusinessV2 : IBusiness
    {
        public IDataAccess _dataAccess;
        public BusinessV2(IDataAccess dataAccess)
        {
            this._dataAccess = dataAccess;
        }
        public void SignUp(string userName, string Password)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Interfaz de lo que debe ofrecer una capa de acceso a datos
    /// </summary>
    public interface IDataAccess { 
        public void Store(string userName, string Password);
    }

    /// <summary>
    /// Primera implementacion de una capa de acceso a datos.
    /// Es la encargada de transaccionar con la base de datos.
    /// </summary>
    public class DataAcess : IDataAccess
    {
        public void Store(string userName, string Password)
        {
            Console.WriteLine("Data access layer...");
            Console.WriteLine("New user stored to db");
        }
    }

    /// <summary>
    /// Segunda implementacion de una capa de acceso a datos.
    /// Puede transaccionar con otra base de datos
    /// </summary>
    public class DataAcessV2 : IDataAccess
    {
        public void Store(string userName, string Password)
        {
            throw new NotImplementedException();
        }
    }
}
