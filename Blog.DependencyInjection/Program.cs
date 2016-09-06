using Microsoft.Extensions.DependencyInjection;
using System;
using Moq;

namespace Blog.DependencyInjection
{
    class Program
    {
        void Main()
        {
            var collection = new ServiceCollection();
            var repo = Mock.Of<IRepository<MyInterestingObject>>();
            var rsd = new ServiceDescriptor(typeof(IRepository<MyInterestingObject>), repo);
            //collection.Add(rsd);
            // or you can use the helper methods
            collection.AddSingleton(repo);

            var sd = ServiceDescriptor.Describe(typeof(IMyService), typeof(MyService), ServiceLifetime.Transient);
            //collection.Add(sd);
            collection.AddTransient<IMyService, MyService>();

            var container = collection.BuildServiceProvider();
            //------------------------------------------------------------

            var r = container.GetService<IMyService>();
            r.DoSomethingFun(new MyInterestingObject());
        }

        // Define other methods and classes here
        public class MyInterestingObject
        {
            public int Id { get; set; }
            public DateTime CreatedDate { get; set; }
            public string CreatedBy { get; set; }
            public DateTime UpdatedDate { get; set; }
            public string UpdatedBy { get; set; }
            // ...
        }

        public interface IMyService
        {
            void DoSomethingFun(MyInterestingObject obj);
        }

        public interface IRepository<T>
        {
            void Insert(T obj);

            void Update(T obj);

            void Delete(T obj);
        }

        public class MyService : IMyService
        {
            public MyService(IRepository<MyInterestingObject> repo)
            {
                repo.GetType().Dump();
            }

            public void DoSomethingFun(MyInterestingObject obj)
            {
                "doing something Fun!".Dump();
            }
        }
    }
}
