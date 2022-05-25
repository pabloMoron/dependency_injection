using System;
using Microsoft.Extensions.DependencyInjection;
namespace Coupling
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var services = new ServiceCollection();
            services.AddSingleton<IDataAccess, DataAcess>();
            services.AddSingleton<IBusiness, Business>();
            services.AddSingleton<IUserInterface, UserInterface>();
            var serviceProvider = services.BuildServiceProvider();

            var ui = serviceProvider.GetService<IUserInterface>();
            ui.GetData();
        }
    }

    public interface IUserInterface
    {
        public void GetData();
    }

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

    public interface IBusiness {
        public void SignUp(string userName, string Password);    
    }

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

    public interface IDataAccess { 
        public void Store(string userName, string Password);
    }

    public class DataAcess : IDataAccess
    {
        public void Store(string userName, string Password)
        {
            Console.WriteLine("Data access layer...");
            Console.WriteLine("New user stored to db");
        }
    }

    public class DataAcessV2 : IDataAccess
    {
        public void Store(string userName, string Password)
        {
            throw new NotImplementedException();
        }
    }
}
