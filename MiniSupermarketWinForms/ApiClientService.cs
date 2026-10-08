using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace MiniSupermarketWinForms
{
    public static class ApiClientService
    {
        // Địa chỉ URL gốc của Web API Backend (cấu hình theo cổng chạy thực tế của dự án Web API)
        public static string BaseApiUrl { get; set; } = "https://localhost:7006/api/";

        public static HttpClient CreateHttpClient()
        {
            var handler = new HttpClientHandler
            {
                // Bỏ qua lỗi SSL không tin cậy của chứng chỉ dev localhost
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };

            var client = new HttpClient(handler)
            {
                BaseAddress = new Uri(BaseApiUrl)
            };

            // Đính kèm Bearer Token nếu đã có phiên đăng nhập
            if (!string.IsNullOrEmpty(SessionManager.JwtToken))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", SessionManager.JwtToken);
            }

            return client;
        }

        // Hàm gọi API đăng nhập lấy Token
        public static async Task<bool> LoginAsync(string username, string password)
        {
            using (var client = CreateHttpClient())
            {
                var loginObj = new { Username = username, Password = password };
                var response = await client.PostAsJsonAsync("auth/login", loginObj);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    using (var doc = JsonDocument.Parse(jsonString))
                    {
                        SessionManager.JwtToken = doc.RootElement.GetProperty("token").GetString() ?? string.Empty;
                        SessionManager.CurrentRole = doc.RootElement.GetProperty("role").GetString() ?? string.Empty;
                        SessionManager.CurrentUsername = username;

                        if (doc.RootElement.TryGetProperty("refreshToken", out var rt))
                        {
                            SessionManager.RefreshToken = rt.GetString() ?? string.Empty;
                        }

                        return true;
                    }
                }
                return false;
            }
        }

        // Hàm gọi API lấy dữ liệu có gắn kèm Bearer Token bảo mật
        public static async Task<string> GetDataWithTokenAsync(string endpoint)
        {
            using (var client = CreateHttpClient())
            {
                var response = await client.GetAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    throw new Exception("Phiên làm việc hết hạn hoặc chưa đăng nhập!");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    throw new Exception("Bạn không có quyền truy cập tài nguyên này (403 Forbidden)!");
                }

                throw new Exception($"Lỗi khi gọi dữ liệu từ Server: {response.StatusCode}");
            }
        }
    }
}
