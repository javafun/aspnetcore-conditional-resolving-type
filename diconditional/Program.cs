
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;



using var host = Host.CreateDefaultBuilder(args)
.ConfigureServices((_, services) =>
    services
    .AddTransient<Test>()
    .AddTransient<PaymentGatewayBoo>()
    .AddTransient<PaymentGatewayFoo>()
    .AddTransient<PaymentGatewayResolver>(serviceProvider => key =>
    {
        switch (key)
        {
            case E_PaymentGatewayType.Foo:
                return serviceProvider.GetService<PaymentGatewayFoo>();
            case E_PaymentGatewayType.Boo:
                return serviceProvider.GetService<PaymentGatewayBoo>();
            case E_PaymentGatewayType.Undefined:
            default:
                throw new NotSupportedException($"PaymentGatewayRepositoryResolver, key: {key}");
        }
    })
)
.Build();

var s = host.Services.GetService<Test>();
s.DoWork();
Console.WriteLine("Hello, World!");
await host.RunAsync();

interface IPaymentGateway
{

    void Pay();
}

class PaymentGatewayFoo : IPaymentGateway
{
    public void Pay()
    {
        Console.WriteLine($"Payment class - {nameof(PaymentGatewayFoo)}");
    }
}
class PaymentGatewayBoo : IPaymentGateway
{
    public void Pay()
    {
        Console.WriteLine($"Payment class - {nameof(PaymentGatewayBoo)}");
    }
}


enum E_PaymentGatewayType
{
    Foo,
    Boo,
    Undefined
}


delegate IPaymentGateway PaymentGatewayResolver(E_PaymentGatewayType paymentGatewayType);

class Test
{
    private readonly PaymentGatewayResolver resolver;

    public Test(PaymentGatewayResolver resolver)
    {
        this.resolver = resolver;
    }

    public void DoWork()
    {
        IPaymentGateway paymentGateway = resolver(E_PaymentGatewayType.Foo);
        paymentGateway.Pay();
    }
}