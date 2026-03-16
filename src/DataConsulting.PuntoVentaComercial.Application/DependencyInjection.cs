using DataConsulting.PuntoVentaComercial.Application.Abstractions.Behaviors;
using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Application.Services.ClasesSunat;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace DataConsulting.PuntoVentaComercial.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.Scan(scan => scan.FromAssembliesOf(typeof(DependencyInjection))
            .AddClasses(classes => classes.AssignableToAny(
                typeof(IQueryHandler<,>),
                typeof(ICommandHandler<>),
                typeof(ICommandHandler<,>)
                ), publicOnly: false)
            .AsImplementedInterfaces()
            .WithScopedLifetime()
            .AddClasses(classes => classes.Where(c => c.Name.EndsWith("Service")), publicOnly: false)
            .AsImplementedInterfaces()
            .WithScopedLifetime());

            // Decoradores de validacion y de trazabilidad
            services.Decorate(typeof(ICommandHandler<,>), typeof(ValidationDecorator.CommandHandler<,>));
            //services.Decorate(typeof(ICommandHandler<>), typeof(ValidationDecorator.CommandBaseHandler<>));
            services.Decorate(typeof(IQueryHandler<,>), typeof(LoggingDecorator.QueryHandler<,>));
            services.Decorate(typeof(ICommandHandler<,>), typeof(LoggingDecorator.CommandHandler<,>));
            //services.Decorate(typeof(ICommandHandler<>), typeof(LoggingDecorator.CommandBaseHandler<>));

            services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly, includeInternalTypes: true);

            return services;
        }
    }
}
