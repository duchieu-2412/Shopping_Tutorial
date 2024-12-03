using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Shopping_Tutorial.Models;
using Shopping_Tutorial.Repository;

namespace Shopping_Tutorial.Areas.Admin.Controllers
{
	[Area("Admin")]
	
	public class ProductController : Controller
	{
		private readonly DataContext _dataContext;
		private readonly IWebHostEnvironment _webHostEnvironment;
		public ProductController(DataContext context, IWebHostEnvironment webHostEnvironment)
		{
			_dataContext = context;
			_webHostEnvironment = webHostEnvironment; 
		}
		public async Task<IActionResult> Index()
		{
			return View(await _dataContext.Products.OrderByDescending(p=>p.Id).Include(p=>p.Category).Include(p => p.Brand).ToListAsync());
		}
		[HttpGet]
        public IActionResult Create()
        {
			ViewBag.Categories = new SelectList(_dataContext.Categories,"Id","Name");
			ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name");

			return View();
        }
		[HttpPost]
[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(ProductModel product)
		{
			ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name",product.CategoryId);
			ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name",product.BrandId);
			if (ModelState.IsValid)
			{
				product.Slug = product.Name.Replace(" ", "-");
				var slug = await _dataContext.Products.FirstOrDefaultAsync(p => p.Slug == product.Slug);
				if (slug != null)
				{
					ModelState.AddModelError("", "Sản phẩm đã có trong database");
					return View(product);
				}

				
					if (product.ImageUpload != null)
					{
						string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath,"media/products");
						string imageName = Guid.NewGuid().ToString() + "_" + product.ImageUpload.FileName;
						string filePath = Path.Combine(uploadsDir, imageName);

						FileStream fs = new FileStream(filePath, FileMode.Create);
						await product.ImageUpload.CopyToAsync(fs);
						fs.Close();
						product.Image = imageName;
					}
				
				_dataContext.Add(product);
				await _dataContext.SaveChangesAsync();
				TempData["success"] = "Thêm sản phẩm thành công";
				return RedirectToAction("Index");

			}
			else
			{
				TempData["error"] = "Model có một vài thứ dang bị lỗi";
				List<string> errors = new List<string>();
				
					foreach (var value in ModelState.Values)
					{
						foreach (var error in value.Errors)
						{
							errors.Add(error.ErrorMessage);
						}
					}
					string errorMessage = string.Join("\n", errors);
					return BadRequest(errorMessage);
				
			}
			return View(product);

		}


		[Route("Edit")]
		public async Task<IActionResult> Edit(int id)
		{
			ProductModel product = await _dataContext.Products.FindAsync(id);
			ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryId);
			ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", product.BrandId);

			return View(product);
		}
		[Route("Edit")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(ProductModel product)
		{
			ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryId);
			ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", product.BrandId);


			var existed_product= _dataContext.Products.Find(product.Id);

			if (ModelState.IsValid)
			{
				product.Slug = product.Name.Replace(" ", "-");
				


				if (product.ImageUpload != null)
				{

					string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
					string imageName = Guid.NewGuid().ToString() + "_" + product.ImageUpload.FileName;
					string filePath = Path.Combine(uploadsDir, imageName);
					//delete anh cu
					string oldfilePath = Path.Combine(uploadsDir, existed_product.Image);
					try
					{

						if (System.IO.File.Exists(oldfilePath))
						{
							System.IO.File.Delete(oldfilePath);
						}
					}
					catch (Exception ex)
					{
						ModelState.AddModelError("", "An errer occurred while delething the product iname.");
					}

					FileStream fs = new FileStream(filePath, FileMode.Create);
					await product.ImageUpload.CopyToAsync(fs);
					fs.Close();
					existed_product.Image = imageName;

					
				}

				
				existed_product.Description = product.Description;
				existed_product.Name = product.Name;
				existed_product.Price = product.Price;
				existed_product.Slug = product.Slug;
				existed_product.CategoryId = product.CategoryId;
				existed_product.BrandId = product.BrandId;



				_dataContext.Update(existed_product);

				await _dataContext.SaveChangesAsync();
				TempData["success"] = "Cập nhật sản phẩm thành công";
				return RedirectToAction("Index");

			}
			else
			{
				TempData["error"] = "Model có một vài thứ dang bị lỗi";
				List<string> errors = new List<string>();

				foreach (var value in ModelState.Values)
				{
					foreach (var error in value.Errors)
					{
						errors.Add(error.ErrorMessage);
					}
				}
				string errorMessage = string.Join("\n", errors);
				return BadRequest(errorMessage);

			}
			

		}
		public async Task<IActionResult> Delete(int id)
		{
			ProductModel product = await _dataContext.Products.FindAsync(id);
			if (product == null)
			{
				return NotFound();
			}
			string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
			//delete anh cu
				string oldfileImage = Path.Combine(uploadsDir, product.Image);
			try
			{

				if (System.IO.File.Exists(oldfileImage))
				{
					System.IO.File.Delete(oldfileImage);
				}
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", "An errer occurred while delething the product iname.");
			}
			//Cap nhat anh moi

			
			_dataContext.Products.Remove(product);
			await _dataContext.SaveChangesAsync();
			TempData["serror"] = "Sản phẩm đã xóa";
			return RedirectToAction("Index");

		}


	}
}
