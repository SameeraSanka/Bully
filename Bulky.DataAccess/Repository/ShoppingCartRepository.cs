﻿using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bully.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository
{
	public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
	{
        private readonly ApplicationDbContext _db;
        public ShoppingCartRepository(ApplicationDbContext db) :base(db) 
        {
            _db = db;            
        }

		public void Update(ShoppingCart obj)
		{
			_db.shoppingCarts.Update(obj);	
		}
	}
}
