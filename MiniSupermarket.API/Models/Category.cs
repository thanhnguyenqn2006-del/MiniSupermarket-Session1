using System.ComponentModel.DataAnnotations;

namespace MiniSupermarket.API.Models
{
    // Lớp biểu diễn thực thể Nhóm hàng hóa trong siêu thị mini
    public class Category
    {
        // Mã định danh nhóm hàng (Khóa chính)
        public int CategoryId { get; set; }

        // Tên nhóm hàng (Bắt buộc, không được để trống)
        [Required(ErrorMessage = "Tên nhóm hàng là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Tên nhóm hàng tối đa 100 ký tự.")]
        public string CategoryName { get; set; } = string.Empty;

        // Mô tả chi tiết về nhóm hàng (Có thể để trống, tối đa 500 ký tự)
        [StringLength(500, ErrorMessage = "Mô tả tối đa 500 ký tự.")]
        public string Description { get; set; } = string.Empty;
    }
}
