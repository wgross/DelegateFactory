using Microsoft.Extensions.DependencyInjection;
using System.Linq.Expressions;
using System.Reflection;

namespace DelegateFactory;

using static System.Linq.Expressions.Expression;

/// <summary>
/// Implements partial application for delegates by inspection of the delegates parameters and
/// the available services in the given <see cref="IServiceProvider"/>.
/// </summary>
public class DelegateInjection(IServiceProvider serviceProvider)
{
    private readonly IServiceProvider serviceProvider = serviceProvider;
    private readonly IServiceProviderIsService serviceProviderIsService = serviceProvider.GetRequiredService<IServiceProviderIsService>();

    class KnownMethods
    {
        public static readonly MethodInfo GetRequiredServiceMethod = typeof(ServiceProviderServiceExtensions).GetMethod(
            name: nameof(ServiceProviderServiceExtensions.GetRequiredService),
            bindingAttr: BindingFlags.Public | BindingFlags.Static,
            types: [typeof(IServiceProvider)])!;

        public static MethodInfo GetRequiredServiceByTypeParamMethod(Type type) => GetRequiredServiceMethod.MakeGenericMethod(type);
    }

    class KnownExpressions
    {
        // this pulls dependency at every invocation: KnownExpressions.GetRequiredServiceMethodCallExpression(sp, p.ParameterType)
        // this might still be useful. It could be a property of the created delegate to update
        // its dependencies on every call? : A call is a like a scope, dependencies are resolved on every call not on creation
        public static MethodCallExpression GetRequiredServiceMethodCallExpression(IServiceProvider serviceProvider, Type serviceType) => Call(
            method: KnownMethods.GetRequiredServiceByTypeParamMethod(serviceType),
            arguments: [Constant(serviceProvider)]);

        // makes dependency a constant expression. It won't change on every call
        public static ConstantExpression ResolvedGetRequiredServiceExpression(IServiceProvider serviceProvider, Type serviceType)
            => Constant(serviceProvider.GetService(serviceType));
    }

    /// <summary>
    /// Apply dependency from <paramref name="appliedFromServices"/> to the delegate <paramref name="appliedToDelegate"/>  and returns
    /// a newly compiled delegate of type <typeparamref name="T"/>.
    /// </summary>
    public static T Apply<T>(Delegate appliedToDelegate, IServiceProvider appliedFromServices) where T : Delegate => (T)CreateAppliedLambda(
        appliedToDelegate,
        appliedFromServices,
        appliedFromServices.GetRequiredService<IServiceProviderIsService>(),
        KnownExpressions.ResolvedGetRequiredServiceExpression).Compile();

    /// <summary>
    /// Apply dependency from the <see cref="IServiceProvider"/> given in the constructor to the delegate <paramref name="appliedToDelegate"/>  and returns
    /// a newly compiled delegate of type <typeparamref name="T"/>.
    /// </summary>
    public T Apply<T>(Delegate appliedToDelegate) where T : Delegate => (T)CreateAppliedLambda(
        appliedToDelegate,
        this.serviceProvider,
        this.serviceProviderIsService,
        KnownExpressions.ResolvedGetRequiredServiceExpression).Compile();

    private static LambdaExpression CreateAppliedLambda(
        Delegate d,
        IServiceProvider sp,
        IServiceProviderIsService isService,
        Func<IServiceProvider, Type, Expression> createResolveServiceExpression)
    {
        var argumentExpressions = d.Method
            .GetParameters()
            .Select(p => isService.IsService(p.ParameterType) ? createResolveServiceExpression(sp, p.ParameterType) : Parameter(p.ParameterType))
            .ToArray();

        MethodCallExpression callExpression = (d.HasSingleTarget && d.Target is { } targetNotNull)
            ? Call(Constant(targetNotNull), method: d.Method, arguments: argumentExpressions)
            : Call(method: d.Method, arguments: argumentExpressions);

        return Lambda(callExpression, argumentExpressions.OfType<ParameterExpression>().ToArray());
    }
}