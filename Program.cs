using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();
app.Use(async (context, next) =>
{
    Debug.Write("Before Invoke from 1st app.Use()\n");
    await next();
    Debug.Write("After Invoke from 1st app.Use()\n");
});

app.Use(async (context, next) =>
{
    Debug.Write("Before Invoke from 2nd app.Use()\n");
    await next();
    Debug.Write("After Invoke from 2nd app.Use()\n");
});
//SHortCircuit Here
app.Use(async (context, next) =>
{
    string agent = context.Request.Headers["user-agent"].ToString();
    if (!string.IsNullOrEmpty(context.Request.QueryString.Value))
    {
        context.Response.Headers.Add("X-Frame-Options", "localhost");
        await context.Response.WriteAsync("Query-String is not allowed in MiddleWare Request");
        return;
    }
    await next(context);
});

app.MapControllers();
app.Run(async (context) =>
{
    await context.Response.WriteAsync("Hello from 1st app.Run()\n");
});
//Will Never Be called
app.Run();

//app.Run((context) =>
//{
//    Debug.Write("Hello from 1st app Run()\n");
//    return global::System.Threading.Tasks.Task.CompletedTask;
//});
