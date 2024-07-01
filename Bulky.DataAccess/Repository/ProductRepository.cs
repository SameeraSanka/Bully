using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bully.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _db;

        public ProductRepository(ApplicationDbContext db) :base(db)
        {
            _db = db;
        }

        public void Update(Product obj)
        {
            //mehema thma manual update eka map krnne
           var objFromDb = _db.Products.FirstOrDefault(product => product.Id == obj.Id);
            if (objFromDb != null)
            {
                objFromDb.Title = obj.Title;
                objFromDb.Description = obj.Description;
                objFromDb.ISBN = obj.ISBN;
                objFromDb.Author = obj.Author;
                objFromDb.Price = obj.Price;
                objFromDb.ListPrice = obj.ListPrice;
                objFromDb.Price50 = obj.Price50;
                objFromDb.Price100 = obj.Price100;
                objFromDb.CategoryId = obj.CategoryId;
                if(obj.ImageUrl != null)
                {
                    objFromDb.ImageUrl = obj.ImageUrl;
                }

            }
            //automatic widihata update eka map krnne mehema
            //_db.Products.Update(obj);
        }
    }
}
