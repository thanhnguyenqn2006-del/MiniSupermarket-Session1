using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// =========================================================
// 1. CẤU HÌNH JWT AUTHENTICATION (BƯỚC 2)
// =========================================================
var jwtSecret = builder.Configuration["JwtSettings:Secret"]
    ?? "SupermarketSecretKeyDoAnMonHoc2026SecureString!!";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSecret)),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(5)
    };

    // Tự động nhận diện token dù người dùng nhập tiền tố "Bearer " hay chỉ dán mỗi chuỗi token
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrEmpty(authHeader))
            {
                if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    context.Token = authHeader.Substring("Bearer ".Length).Trim();
                }
                else
                {
                    context.Token = authHeader.Trim();
                }
            }
            return Task.CompletedTask;
        },
        OnChallenge = async context =>
        {
            context.HandleResponse();
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";
            var payload = JsonSerializer.Serialize(new
            {
                success = false,
                message = "Chưa xác thực hoặc Token không hợp lệ / đã hết hạn (401 Unauthorized)!"
            });
            await context.Response.WriteAsync(payload);
        },
        OnForbidden = async context =>
        {
            context.Response.StatusCode = 403;
            context.Response.ContentType = "application/json";
            var payload = JsonSerializer.Serialize(new
            {
                success = false,
                message = "Bạn không có quyền truy cập tài nguyên này (403 Forbidden)!"
            });
            await context.Response.WriteAsync(payload);
        }
    };
});

// =========================================================
// 2. CONTROLLERS & DATA ANNOTATIONS MODELSTATE
// =========================================================
builder.Services.AddControllers();

// Tùy biến phản hồi khi ModelState.IsValid == false theo chuẩn Data Annotations (Mục 2.3)
builder.Services.Configure<Microsoft.AspNetCore.Mvc.ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(kvp => kvp.Value != null && kvp.Value.Errors.Count > 0)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
            );

        var problem = new
        {
            success = false,
            message = "Dữ liệu đầu vào không thỏa mãn ràng buộc Data Annotations.",
            errors = errors
        };

        return new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(problem);
    };
});

builder.Services.AddEndpointsApiExplorer();

// =========================================================
// 3. CẤU HÌNH SWAGGER UI HỖ TRỢ XÁC THỰC BEARER TOKEN (PHẦN 3)
// =========================================================
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MiniSupermarket API",
        Version = "v1",
        Description = "Hệ thống MiniSupermarket Web API - Buổi 2: Bảo mật & Phân quyền JWT"
    });

    // Cấu hình Bearer Authentication cho Swagger UI
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Nhập Token theo cú pháp: Bearer {chuỗi_jwt_token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// =========================================================
// 4. PIPELINE XỬ LÝ REQUEST HTTP (MIDDLEWARE)
// =========================================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MiniSupermarket API v1");
    });
}

app.UseHttpsRedirection();

// BẮT BUỘC: UseAuthentication PHẢI được gọi TRƯỚC UseAuthorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();