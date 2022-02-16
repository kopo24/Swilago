using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.EntityFrameworkCore;
using Swilago.Data;
using Swilago.Interfaces;
using Swilago.Services;
using Microsoft.AspNetCore.SpaServices.AngularCli;

/// <summary>
/// 우리 DEV팀이 점심마다 가는 밥집의 결정을 룰렛 프로그램을 만들어 결정하도록 하자.
/// 1. 기존에 우리가 가던 밥집은 기본등록 되어 있다.(SQL DB 등록)
/// 2. 앞으로 밥집이 추가 될 수 있다.(CRUD)
/// 3. 결과가 마땅치 않으면 다시 돌릴 수 있다.
/// 4. 최종 결과는 해당 일자와 함께 저장되어 추후 통계표에 사용할 수 있도록 한다.
/// 5. 이 후에 어느정도 데이터가 쌓였으면 오늘의 추천 밥집을 화면 상단에 뜨도록 한다.(처음엔 단순통계, 이후에 머신러닝)
/// </summary>
const string clientId = "233e7892-1986-41e9-a8f6-7b524d103403";
const string clientSecret = "m5Y7Q~YWVsX_O0fRGOmAbrREJU5UM5VNEi-v0";
const string tenantId = "785087ba-1e72-4e7d-b1d1-4a9639137a66";
//const string secretName = "NoboribetsuSqlServerConnectionString";
//const string secretName = "BreakSqlServerConnectionString";
const string secretName = "PamukkaleSqlServerConnectionString";

/// <summary>
/// 확실하지 않지만 추측하는 내용을 써 봅니다. 미래의 나... 확인 해보자.
/// (
/// 참고 : https://docs.microsoft.com/ko-kr/azure/key-vault/general/developers-guide#authenticate-to-key-vault-in-code
/// KeyVault는 Azure AD 인증을 사용하며, Azure AD 보안 주체가 액세스 권한을 부여해야 합니다./// 
/// )
/// TokenCredential : OAuth 토큰을 제공할 수 있는 자격 증명을 나타냅니다.
/// ChaninedTokenCredential : TokenCredential의 GetToken메서드 중에서 하나가 기본이 아닌 AccessToken들을 받환받을 때 필요한 여러 TokenCredental 구현을 연결해 줍니다.(사용자가 여러 자격 증명을 구성하는 사용자 지정 인증 흐름을 정의할 수 있습니다.)
/// ManagedIdentityCredential : 관리되는 ID로 리소스를 인증할 수 있는 ManagedIdentityCredential의 인스턴스를 만듭니다.(TokenCredential을 inherit 함)
/// EnvironmentCredential : EnvironmentCredential 클래스의 인스턴스를 만들고 환경 변수에서 클라이언트 비밀 세부 정보를 읽습니다. 현재 예상되는 환경 변수를 찾을 수 없는 경우 GetToken 메서드는 호출될 때 기본 Azure.Core.AccessToken을 반환합니다.(TokenCredential을 inherit 함)
/// ClientSecretCredential : 클라이언트 암호를 사용하여 Azure Active Directory에 대해 인증하는 데 필요한 세부 정보를 사용하여 ClientSecretCredential의 인스턴스를 만듭니다.(TokenCredential을 inherit 함)
/// 
/// DefaultAzureCredential : 대부분의 Azure SDK 인증 시나리오를 처리할 수 있는 기본 자격 증명입니다. 이걸 사용해도 되지만 필요한 Data인 세 개만 뽑아 사용하였습니다.(현재 내 추측으론... 미래의 나야... 파이팅)
/// </summary>
TokenCredential tokenCredential = new ChainedTokenCredential(// new DefaultAzureCredential(),
                                        new ClientSecretCredential(tenantId, clientId, clientSecret),
                                        new EnvironmentCredential(),
                                        new ManagedIdentityCredential());

var keyVaultUri = new Uri("https://yang.vault.azure.net/");
var client = new SecretClient(keyVaultUri, tokenCredential);
var secret = client.GetSecret(secretName);

Dictionary<string, string> sqlServerConnectionString = new()
{
    { "SqlServerConnection", secret.Value.Value }
};

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Configuration.AddInMemoryCollection(sqlServerConnectionString);

builder.Services.AddDbContext<PamukkaleContext>(options =>
    options.UseSqlServer(builder.Configuration["SqlServerConnection"]).EnableSensitiveDataLogging());

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        build => build.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader());
});

builder.Services.AddTransient<IRestaurantService, RestaurantService>();

// Add services to the container.

builder.Services.AddControllersWithViews();

builder.Services.AddSpaStaticFiles(configuration =>
{
    configuration.RootPath = "ClientApp/dist";
});

var app = builder.Build();

app.UseCors("CorsPolicy");
//app.UseMvc();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();

if (!app.Environment.IsDevelopment())
    app.UseSpaStaticFiles();

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
              name: "default",
              pattern: "{controller}/{action=Index}/{id?}");
});

app.UseSpa(spa =>
{
    spa.Options.SourcePath = "ClientApp";
    if (app.Environment.IsDevelopment())
        spa.UseAngularCliServer(npmScript: "start");
});

//app.MapFallbackToFile("index.html");

app.Run();
