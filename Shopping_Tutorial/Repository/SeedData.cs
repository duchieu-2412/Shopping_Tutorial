using Microsoft.EntityFrameworkCore;
using Shopping_Tutorial.Models;

namespace Shopping_Tutorial.Repository
{
	public class SeedData
	{
		public static void SeedingData(DataContext _context)
		{
			_context.Database.Migrate();
			if(!_context.Products.Any())
			{
				CategoryModel Skincare = new CategoryModel { Name="Skincare",Slug="skincare",Description="Sản phẩm đành cho da nhạy cảm",Status=1 };
				CategoryModel Vitamin = new CategoryModel { Name = "Vitamin", Slug = "vitamin", Description = "Sản phẩm bổ sung dưỡng chất cơ thể", Status = 1 };

				BrandModel bio = new BrandModel { Name = "Biodema", Slug = "biodema", Description = "Tẩy trang dành cho da nhạy cảm", Status = 1 };
				BrandModel vitamin = new BrandModel { Name = "Vitamin C", Slug = "vitamin C", Description = "Cung cấp dưỡng chất cho cơ thể", Status = 1 };

				_context.Products.AddRange(
					
					new ProductModel { Name = "Biodema", Slug = "biodema", Description = "Sản phẩm dành cho da nhạy cảm", Image = "biodema.png", Category = Skincare, Brand=bio,Price = 399000 },

					new ProductModel { Name = "Vitamin C", Slug = "vitamin C", Description = "Cung cấp dưỡng chất cho cơ thể", Image = "vitamin.png", Category = Vitamin,Brand=vitamin, Price = 79000 }
				    
				 );
				_context.SaveChanges();

			}
		}
	}
}
