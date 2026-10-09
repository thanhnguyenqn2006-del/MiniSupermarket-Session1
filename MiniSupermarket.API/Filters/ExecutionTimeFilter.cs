using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace MiniSupermarket.API.Filters
{
    /// <summary>
    /// Action Filter: Đo thời gian thực thi của Action Method và thêm vào Response Header
    /// Mục đích: Giám sát hiệu năng từng API endpoint (Performance Monitoring)
    /// Cách dùng: [ServiceFilter(typeof(ExecutionTimeFilter))] hoặc Global
    /// </summary>
    public class ExecutionTimeFilter : IActionFilter
    {
        private readonly ILogger<ExecutionTimeFilter> _logger;
        private Stopwatch? _stopwatch;

        public ExecutionTimeFilter(ILogger<ExecutionTimeFilter> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Bắt đầu bấm giờ trước khi Action thực thi
        /// </summary>
        public void OnActionExecuting(ActionExecutingContext context)
        {
            _stopwatch = Stopwatch.StartNew();
        }

        /// <summary>
        /// Dừng bấm giờ, ghi log và thêm thời gian thực thi vào Response Header
        /// </summary>
        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (_stopwatch == null) return;

            _stopwatch.Stop();
            var elapsedMs = _stopwatch.ElapsedMilliseconds;
            var actionName = context.ActionDescriptor.DisplayName;

            // Thêm header tùy chỉnh để client có thể đọc thời gian phản hồi
            context.HttpContext.Response.Headers["X-Execution-Time-Ms"] = elapsedMs.ToString();

            // Cảnh báo nếu Action thực thi quá 500ms
            if (elapsedMs > 500)
            {
                _logger.LogWarning(
                    "[PERF-WARNING] ⚠️ Action chậm: {Action} | Thời gian: {Elapsed}ms (> 500ms ngưỡng cảnh báo)",
                    actionName, elapsedMs
                );
            }
            else
            {
                _logger.LogInformation(
                    "[PERF] ⏱️ Action: {Action} | Thời gian thực thi: {Elapsed}ms",
                    actionName, elapsedMs
                );
            }
        }
    }
}
