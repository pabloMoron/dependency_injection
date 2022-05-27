using DI.Interfaces;

namespace DI.Services
{
    public class InjectionObject : ISingleton, ITransient, IScopped
    {
        public readonly int value;
        
        //Constructor
        public InjectionObject()
        {
            this.value = new Random().Next(1, 1000);
        }
    }
}
