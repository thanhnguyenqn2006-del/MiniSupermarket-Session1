using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace MiniSupermarket.API.Filters
{
    /// <summary>
    /// Action Filter: Ghi log thông tin Request/Response vào Console (minh họa vòng đời Request)
    /// Áp dụng: [ServiceFilter(typeof(LogActionFilter))] hoặc đăng ký Global trong Program.cs
    /// </summary>
    public class LogActionFilter : IActionFilter
    {
        private readonly ILogger<LogActionFilter> _logger;

        public LogActionFilter(ILogger<LogActionFilter> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Chạy TRƯỚC khi Action Method thực thi
        /// </summary>
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var actionName = context.ActionDescriptor.DisplayName;
            var method = context.HttpContext.Request.Method;
            var path = context.HttpContext.Request.Path;
            var user = context.HttpContext.User.Identity?.Name ?? "Anonymous";

            _logger.LogInformation(
                "[LOG-BEFORE] ▶ Action: {Action} | {Method} {Path} | User: {User} | Time: {Time}",
                actionName, method, path, user, DateTime.Now.ToString("HH:mm:ss.fff")
            );
        }

        /// <summary>
        /// Chạy SAU khi Action Method thực thi (có kết quả)
        /// </summary>
        public void OnActionExecuted(ActionExecutedContext context)
        {
            var actionName = context.ActionDescriptor.DisplayName;
            var statusCode = context.HttpContext.Response.StatusCode;

            _logger.LogInformation(
                "[LOG-AFTER] ◀ Action: {Action} | StatusCode: {StatusCode} | Time: {Time}",
                actionName, statusCode, DateTime.Now.ToString("HH:mm:ss.fff")
            );
        }
    }
}
