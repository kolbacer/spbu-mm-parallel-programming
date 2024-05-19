using Task4.Implementation.ExamSystem;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

SetType setType = Environment.GetEnvironmentVariable("SET_TYPE") switch
{
    "LazySet" => SetType.LazySet,
    "StripedHashSet" => SetType.StripedHashSet,
    _ => SetType.LazySet
};
Console.WriteLine($"Set type: {setType}");

// inject ExamSystem
builder.Services.AddSingleton<IExamSystem, ExamSystem>(_ => new ExamSystem(setType));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();