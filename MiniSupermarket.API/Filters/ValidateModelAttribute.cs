using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MiniSupermarket.API.Filters
{
    /// <summary>
    /// Action Filter: Tùy biến vòng đời Request - Tự động kiểm tra tính hợp lệ của Model trước khi vào Action
    /// </summary>
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                context.Result = new BadRequestObjectResult(new
                {
                    success = false,
                    message = "Dữ liệu gửi lên không thỏa mãn ràng buộc Data Annotations!",
                    errors = context.ModelState
                });
            }
        }
    }
}
