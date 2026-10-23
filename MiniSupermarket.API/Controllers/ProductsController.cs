using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniSupermarket.API.Data;
using MiniSupermarket.API.Filters;
using MiniSupermarket.API.Models;

namespace MiniSupermarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(LogActionFilter))]       // Ghi log Request/Response (Buổi 2 - Action Filter)
    [ServiceFilter(typeof(ExecutionTimeFilter))]   // Đo thời gian thực thi (Buổi 2 - Action Filter)
    public class ProductsController : ControllerBase
    {
        private readonly SupermarketDbContext _context;

        public ProductsController(SupermarketDbContext context)
        {
            _context = context;
        }

        // =========================================================================
        // BUỔI 6 - TIẾT 1 & 2: PHÂN TRANG LINQ SERVER-SIDE & EAGER LOADING (.Include)
        // BÀI TẬP MỞ RỘNG 6.2: HỖ TRỢ LỌC THEO NHÓM HÀNG (categoryId)
        // GET: api/products/paged?keyword=sua&categoryId=1&pageNumber=1&pageSize=5
        // =========================================================================
        [HttpGet("paged")]
        [AllowAnonymous] // Cho phép kiểm thử linh hoạt trên Swagger UI và Client
        public async Task<IActionResult> GetPagedProducts(
            [FromQuery] string? keyword,
            [FromQuery] int? categoryId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 5)
        {
            // 1. Kiểm tra tham số trang đầu vào hợp lệ
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1 || pageSize > 50) pageSize = 5; // Mặc định 5 dòng, tối đa 50 dòng

            // 2. Khởi tạo truy vấn LINQ IQueryable kết hợp Eager Loading (.Include)
            // AsNoTracking giúp tối ưu hóa bộ nhớ RAM cho các truy vấn chỉ đọc dữ liệu
            var query = _context.Products
                .Include(p => p.Category)
                .AsNoTracking()
                .AsQueryable();

            // 3. Lọc theo từ khóa tìm kiếm (nếu người dùng có nhập)
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                string search = keyword.Trim().ToLower();
                query = query.Where(p => p.ProductName.ToLower().Contains(search) 
                                      || p.Barcode.Contains(search));
            }

            // 3b. Lọc theo Nhóm hàng CategoryId (Bài tập mở rộng 6.2)
            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            // 4. Đếm tổng số lượng bản ghi thỏa mãn điều kiện
            int totalItems = await query.CountAsync();

            // 5. Tính toán tổng số trang
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            // 6. Áp dụng toán tử Skip và Take để phân trang trực tiếp dưới SQL Server
            var items = await query
                .OrderBy(p => p.ProductId) // Bắt buộc phải OrderBy trước khi Skip/Take
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProductItemDto
                {
                    ProductId = p.ProductId,
                    Barcode = p.Barcode,
                    ProductName = p.ProductName,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category != null ? p.Category.CategoryName : string.Empty
                })
                .ToListAsync();

            // 7. Đóng gói kết quả trả về cho Client
            var response = new PagedResult<ProductItemDto>
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                Items = items
            };

            return Ok(response);
        }

        // 1. GET: Lấy danh sách sản phẩm (/api/products)
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .AsNoTracking()
                .ToListAsync();
            return Ok(products);
        }

        // 2. GET: Lấy chi tiết sản phẩm theo ID (/api/products/{id})
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound(new { message = "Không tìm thấy sản phẩm!" });
            }
            return Ok(product);
        }

        // 3. GET: Tìm kiếm sản phẩm theo tên hoặc mã vạch (/api/products/search?keyword=...)
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return BadRequest(new { message = "Vui lòng nhập từ khóa tìm kiếm!" });
            }

            var result = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.ProductName.Contains(keyword) || p.Barcode.Contains(keyword))
                .ToListAsync();

            return Ok(result);
        }

        // 3b. GET: Tra cứu sản phẩm theo mã vạch cho FormPOS (/api/products/barcode/{barcode})
        [HttpGet("barcode/{barcode}")]
        public async Task<IActionResult> GetByBarcode(string barcode)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Barcode == barcode);

            if (product == null)
                return NotFound(new { message = $"Không tìm thấy sản phẩm có mã vạch: {barcode}" });

            return Ok(product);
        }

        // 4. POST: Thêm mới sản phẩm (Admin + Warehouse)
        [HttpPost]
        [Authorize(Roles = "Admin,Warehouse")]
        public async Task<IActionResult> Create([FromBody] Product newProduct)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Kiểm tra mã vạch trùng lặp
            bool barcodeExists = await _context.Products.AnyAsync(p => p.Barcode == newProduct.Barcode);
            if (barcodeExists)
            {
                return BadRequest(new { message = "Mã vạch này đã tồn tại trong hệ thống!" });
            }

            _context.Products.Add(newProduct);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = newProduct.ProductId }, newProduct);
        }

        // 5. PUT: Cập nhật sản phẩm (Admin + Warehouse)
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Warehouse")]
        public async Task<IActionResult> Update(int id, [FromBody] Product updateProduct)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound(new { message = "Không tìm thấy sản phẩm cần sửa!" });
            }

            product.ProductName = updateProduct.ProductName;
            product.Barcode = updateProduct.Barcode;
            product.Price = updateProduct.Price;
            product.StockQuantity = updateProduct.StockQuantity;
            product.CategoryId = updateProduct.CategoryId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // 6. DELETE: Xóa sản phẩm (Chỉ Admin)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]  // Chỉ Admin mới được xóa
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound(new { message = "Không tìm thấy sản phẩm cần xóa!" });
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
