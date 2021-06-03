using Memento.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memento.Tests
{
    public class DbFixture
    {
        public DbFixture()
        {

            var serviceCollection = new ServiceCollection();
            serviceCollection
                .AddDbContext<MementoDbContext>(options => options.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=Memento;MultipleActiveResultSets=true"),
                    ServiceLifetime.Transient);

            ServiceProvider = serviceCollection.BuildServiceProvider();
        }

        public ServiceProvider ServiceProvider { get; private set; }
    }
}
