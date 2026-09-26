> **Môn h?c:** L?p trình ?ng d?ng .NET Core (Mã môn: 229162)  
> **Bu?i th?c hành:** Bu?i 1 - Xây d?ng Web API qu?n lý danh m?c và k?t n?i WinForms Client (CRUD)

---

## ??? 1. Mô hình Ki?n trúc H? th?ng (Client - Server)
D? án ???c xây d?ng theo mô hình phân t?ng hi?n ??i, tách bi?t hoàn toàn gi?a Backend và Frontend:
* **`MiniSupermarket.API` (Backend):** D? án ASP.NET Core Web API ch?u trách nhi?m x? lý logic nghi?p v?, qu?n lý d? li?u và cung c?p các RESTful API chu?n hóa.
* **`MiniSupermarket.WinForms` (Frontend Client):** ?ng d?ng Windows Forms ?óng vai trò là máy tr?m POS t?i qu?y, s? d?ng `HttpClient` ?? g?i d? li?u t? API qua m?ng và hi?n th? tr?c quan lên `DataGridView`.

---

## ??? 2. Công ngh? S? d?ng
* **Ngôn ng?:** C# (.NET 8.0)
* **Backend:** ASP.NET Core Web API, Controllers, In-Memory Data, LINQ
* **Frontend:** Windows Forms (.NET 8.0), `System.Net.Http.Json`
* **Công c? ki?m th?:** Swagger UI

---

## ?? 3. C?u trúc Solution
```text
MiniSupermarketSystem/
?
??? MiniSupermarket.API/          # D? án Web API (Backend)
?   ??? Controllers/              # Ch?a CategoriesController (CRUD & Search)
?   ??? Models/                   # Ch?a l?p th?c th? Category.cs
?   ??? Program.cs                # C?u hình d?ch v? và Middleware
?
??? MiniSupermarket.WinForms/     # D? án Windows Forms (Frontend Client)
    ??? FormCategoryManagement.cs # Giao di?n qu?n lý danh m?c CRUD


    