using System.Linq;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using CQRS.LightInject;
using CQRS.Query.Abstractions;
using FluentAssertions;
using LightInject;
using Xunit;

namespace CQRS.Microsoft.Extensions.DependencyInjection.Tests
{
    public class CommandExecutorTests
    {
        [Fact]
        public async Task ShouldExecuteCommandHandler()
        {
            var container = new ServiceContainer();
            container.RegisterCommandHandlers();
            using (var scope = container.BeginScope())
            {
                var commandExecutor = scope.GetInstance<ICommandExecutor>();
                var command = new SampleCommand();
                await commandExecutor.ExecuteAsync(command);
                command.WasHandled.Should().BeTrue();
            }
        }

        [Fact]
        public async Task ShouldExecuteQueryHandler()
        {
            var container = new ServiceContainer();
            container.RegisterQueryHandlers();
            using (var scope = container.BeginScope())
            {
                var queryExecutor = scope.GetInstance<IQueryExecutor>();
                var query = new SampleQuery();
                var result = await queryExecutor.ExecuteAsync(query);

                query.WasHandled.Should().BeTrue();
            }
        }

        [Fact]
        public void ShouldNotAddQueryExecutorTwice()
        {
            var container = new ServiceContainer();
            container.RegisterQueryHandlers();
            container.RegisterQueryHandlers();

            container.AvailableServices.Count(sr => sr.ServiceType == typeof(IQueryExecutor)).Should().Be(1);
        }

        [Fact]
        public void ShouldNotAddCommandExecutorTwice()
        {
            var container = new ServiceContainer();
            container.RegisterCommandHandlers();
            container.RegisterCommandHandlers();

            container.AvailableServices.Count(sr => sr.ServiceType == typeof(ICommandExecutor)).Should().Be(1);
        }
    }
}