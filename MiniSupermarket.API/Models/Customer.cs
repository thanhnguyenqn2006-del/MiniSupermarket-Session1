using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniSupermarket.API.Models
{
    [Table("Customers")] // Bảng quản lý khách hàng thân thiết trong SQL Server
    public class Customer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Tên khách hàng không được để trống!")]
        [StringLength(100, ErrorMessage = "Tên khách hàng tối đa 100 ký tự")]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại không được để trống!")]
        [StringLength(15, ErrorMessage = "Số điện thoại tối đa 15 ký tự")]
        public string PhoneNumber { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Địa chỉ tối đa 200 ký tự")]
        public string? Address { get; set; }

        public int RewardPoints { get; set; } = 0; // Điểm thưởng tích lũy

        [StringLength(50)]
        public string MembershipRank { get; set; } = "Chuẩn"; // Hạng thành viên (Chuẩn, Bạc, Vàng, Kim Cương)
    }
}
