using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniSupermarket.API.Data;
using MiniSupermarket.API.Models;

namespace MiniSupermarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Bắt buộc đăng nhập để tương tác với dữ liệu khách hàng
    public class CustomersController : ControllerBase
    {
        private readonly SupermarketDbContext _context;

        public CustomersController(SupermarketDbContext context)
        {
            _context = context;
        }

        // =========================================================================
        // BUỔI 6 - BÀI TẬP MỞ RỘNG 6.1: PHÂN TRANG DANH SÁCH KHÁCH HÀNG THÂN THIẾT
        // Sắp xếp giảm dần theo điểm tích lũy (RewardPoints)
        // GET: api/customers/paged?keyword=lan&pageNumber=1&pageSize=5
        // =========================================================================
        [HttpGet("paged")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPagedCustomers(
            [FromQuery] string? keyword,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 5)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1 || pageSize > 50) pageSize = 5;

            var query = _context.Customers
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                string search = keyword.Trim().ToLower();
                query = query.Where(c => c.CustomerName.ToLower().Contains(search)
                                      || c.PhoneNumber.Contains(search)
                                      || (c.Address != null && c.Address.ToLower().Contains(search)));
            }

            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var items = await query
                .OrderByDescending(c => c.RewardPoints) // Gợi ý: Sắp xếp theo thứ tự giảm dần của điểm thưởng
                .ThenBy(c => c.CustomerId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CustomerItemDto
                {
                    CustomerId = c.CustomerId,
                    CustomerName = c.CustomerName,
                    PhoneNumber = c.PhoneNumber,
                    Address = c.Address ?? string.Empty,
                    RewardPoints = c.RewardPoints,
                    MembershipRank = c.MembershipRank ?? "Chuẩn"
                })
                .ToListAsync();

            var response = new PagedResult<CustomerItemDto>
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                Items = items
            };

            return Ok(response);
        }

        // 1. GET: Lấy toàn bộ danh sách khách hàng (/api/customers)
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var customers = await _context.Customers.AsNoTracking().ToListAsync();
            return Ok(customers);
        }

        // 2. GET: Lấy chi tiết khách hàng theo ID (/api/customers/{id})
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound(new { message = "Không tìm thấy khách hàng!" });
            }
            return Ok(customer);
        }

        // 3. GET: Tìm kiếm khách hàng theo tên hoặc số điện thoại (/api/customers/search?keyword=...)
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return BadRequest(new { message = "Vui lòng nhập từ khóa tìm kiếm!" });
            }

            var result = await _context.Customers
                .Where(c => c.CustomerName.Contains(keyword) || c.PhoneNumber.Contains(keyword))
                .ToListAsync();

            return Ok(result);
        }

        // 4. POST: Thêm mới khách hàng thành viên (/api/customers)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Customer newCustomer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Kiểm tra số điện thoại trùng lặp
            bool phoneExists = await _context.Customers.AnyAsync(c => c.PhoneNumber == newCustomer.PhoneNumber);
            if (phoneExists)
            {
                return BadRequest(new { message = "Số điện thoại này đã được đăng ký thành viên!" });
            }

            _context.Customers.Add(newCustomer);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = newCustomer.CustomerId }, newCustomer);
        }

        // 5. PUT: Cập nhật thông tin khách hàng (/api/customers/{id})
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Customer updateCustomer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound(new { message = "Không tìm thấy khách hàng cần sửa!" });
            }

            // Kiểm tra số điện thoại có bị trùng với người khác không
            bool phoneConflict = await _context.Customers.AnyAsync(c => c.PhoneNumber == updateCustomer.PhoneNumber && c.CustomerId != id);
            if (phoneConflict)
            {
                return BadRequest(new { message = "Số điện thoại đã thuộc về khách hàng khác!" });
            }

            customer.CustomerName = updateCustomer.CustomerName;
            customer.PhoneNumber = updateCustomer.PhoneNumber;
            customer.Address = updateCustomer.Address;
            customer.RewardPoints = updateCustomer.RewardPoints;
            customer.MembershipRank = updateCustomer.MembershipRank;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // 6. DELETE: Xóa khách hàng (Chỉ Admin mới có quyền xóa khách hàng) (/api/customers/{id})
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound(new { message = "Không tìm thấy khách hàng cần xóa!" });
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
