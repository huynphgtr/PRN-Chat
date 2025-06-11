// PRNChat.Server/Program.cs
using PRNChat.Server.Config;
using PRNChat.Server.Services;
using PRNChat.Shared.Interfaces;
using Supabase;
using SupabaseOptions = Supabase.SupabaseOptions;

var builder = WebApplication.CreateBuilder(args);

// Đọc cấu hình
var supabaseConfig = builder.Configuration.GetSection("Supabase").Get<SupabaseConfig>();
if (supabaseConfig == null)
    throw new InvalidOperationException("Supabase configuration is missing");

// Thêm dịch vụ
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Đăng ký Supabase Client
builder.Services.AddSingleton(provider =>
{
    // ...
    return new Client(supabaseConfig.Url, supabaseConfig.Key, new SupabaseOptions { /* ... */ });
});

// Đăng ký các service
builder.Services.AddScoped<IAuthService, SupabaseAuthService>();
builder.Services.AddScoped<IChatService, SupabaseChatService>();
builder.Services.AddScoped<IUserService, SupabaseUserService>();

var app = builder.Build();

// ...
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Khởi tạo Supabase client
await app.Services.GetRequiredService<Client>().InitializeAsync();

app.Run();