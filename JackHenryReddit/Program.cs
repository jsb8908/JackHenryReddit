using JackHenryReddit.Common;
using JackHenryReddit.Data;
using JackHenryReddit.Data.Repositories;
using JackHenryReddit.HostedServices;
using JackHenryReddit.Middleware;
using JackHenryReddit.Options;
using JackHenryReddit.Services.HttpClients;
using JackHenryReddit.Services.Services;
using Microsoft.Extensions.Options;

const string UserAgent = "JHReddit-client/0.1 (by /u/internal_release_983)";

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<GeneralOptions>(builder.Configuration.GetSection(GeneralOptions.General));
builder.Services.Configure<RedditAPIOptions>(builder.Configuration.GetSection(RedditAPIOptions.RedditAPI));

//Singletons
builder.Services.AddSingleton<IDataContext, InMemoryDataContext>();
builder.Services.AddSingleton<IRedditService, RedditService>();

//Transients
builder.Services.AddTransient<IRedditModelRepository, RedditModelRepository>();

//HostedServices
builder.Services.AddHostedService<SubRedditRetrieval>();
builder.Services.AddHostedService<SubRedditStats>();

//HttpClients
builder.Services.AddHttpClient();
builder.Services.AddHttpClient<IRedditAuthHttpClient, RedditAuthHttpClient>()
                .ConfigureHttpClient((serviceProvider, client) =>
                {
                    var redditAPIOptions = serviceProvider.GetRequiredService<IOptions<RedditAPIOptions>>().Value;
                    client.BaseAddress = new Uri(redditAPIOptions.AuthUrl);
                    client.DefaultRequestHeaders.Authorization = new BasicAuthenticationHeaderValue(redditAPIOptions.ClientId, redditAPIOptions.ClientSecret);
                    client.DefaultRequestHeaders.Add("User-Agent", UserAgent);
                });

builder.Services.AddHttpClient<IRedditAPIHttpClient, RedditAPIHttpClient>()
                .ConfigureHttpClient((serviceProvider, client) =>
                {
                    var redditAPIOptions = serviceProvider.GetRequiredService<IOptions<RedditAPIOptions>>().Value;
                    client.BaseAddress = new Uri(redditAPIOptions.BaseUrl);
                    if (!redditAPIOptions.BearerToken.TrimAndCompare(""))
                    {
                        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", redditAPIOptions.BearerToken);
                    }
                    client.DefaultRequestHeaders.Add("User-Agent", UserAgent);
                });

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseWhen(httpContext => httpContext.Request.Path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase),
                           // add Global Exception Handler only for our API routes
                           appBuilder => app.UseMiddleware<JackHenryExceptionHandler>()
           );

app.MapControllers();

app.Run();
