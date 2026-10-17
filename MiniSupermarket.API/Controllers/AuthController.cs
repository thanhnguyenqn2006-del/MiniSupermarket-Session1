using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MiniSupermarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Endpoint Đăng nhập: POST /api/auth/login
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequestDto request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Dữ liệu gửi lên không hợp lệ!" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Danh sách tài khoản mẫu - Buổi 4 (15 tài khoản, 3 vai trò)
            // Thực tế sẽ truy vấn Users table từ SQL Server
            var accounts = new Dictionary<string, (string Password, string Role, string FullName)>
            {
                // ── Admin (Quản trị viên) ──────────────────────────────────
                ["admin01"]       = ("123456", "Admin",     "Nguyễn Quản Trị"),
                ["admin02"]       = ("123456", "Admin",     "Trần Giám Đốc"),
                ["admin_backup"]  = ("123456", "Admin",     "Đặng Hỗ Trợ"),
                ["supervisor"]    = ("123456", "Admin",     "Mai Giám Sát"),
                // Tài khoản cũ tương thích ngược
                ["admin"]         = ("123456", "Admin",     "Administrator"),

                // ── Cashier (Thu ngân) ─────────────────────────────────────
                ["cashier01"]     = ("123456", "Cashier",   "Lê Thu Ngân"),
                ["cashier02"]     = ("123456", "Cashier",   "Phạm Bán Hàng"),
                ["cashier03"]     = ("123456", "Cashier",   "Hoàng Thu Ngân"),
                ["cashier04"]     = ("123456", "Cashier",   "Vũ Thị Quầy"),
                ["cashier05"]     = ("123456", "Cashier",   "Đỗ Bán Lẻ"),
                ["cashier06"]     = ("123456", "Cashier",   "Hồ Ca Chiều"),
                // Tài khoản cũ tương thích ngược
                ["cashier"]       = ("123456", "Cashier",   "Cashier"),

                // ── Warehouse (Thủ kho) ────────────────────────────────────
                ["ware01"]        = ("123456", "Warehouse", "Ngô Quản Kho"),
                ["ware02"]        = ("123456", "Warehouse", "Bùi Kiểm Kê"),
                ["ware03"]        = ("123456", "Warehouse", "Dương Thủ Kho"),
                ["ware04"]        = ("123456", "Warehouse", "Lý Nhập Hàng"),
                ["ware05"]        = ("123456", "Warehouse", "Trương Vận Chuyển"),
            };

            if (accounts.TryGetValue(request.Username, out var account)
                && account.Password == request.Password)
            {
                var token = GenerateJwtToken(request.Username, account.Role, account.FullName);
                return Ok(new
                {
                    success  = true,
                    token    = token,
                    role     = account.Role,
                    fullName = account.FullName,
                    username = request.Username
                });
            }

            return Unauthorized(new { success = false, message = "Sai tài khoản hoặc mật khẩu!" });

        }

        private string GenerateJwtToken(string username, string role, string fullName = "")
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            // Lấy khóa bí mật từ appsettings.json
            var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:Secret"] ?? "SupermarketSecretKeyDoAnMonHoc2026SecureString!!");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name,    username),
                    new Claim(ClaimTypes.Role,    role),
                    new Claim("FullName",         fullName)
                }),
                Expires = DateTime.UtcNow.AddHours(8), // Ca làm việc 8 tiếng
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

    }

    public class LoginRequestDto
    {
        [Required(ErrorMessage = "Tên đăng nhập không được để trống!")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu không được để trống!")]
        public string Password { get; set; } = string.Empty;
    }
}
