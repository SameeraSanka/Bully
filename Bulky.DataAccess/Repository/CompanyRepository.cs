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
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private readonly ApplicationDbContext _db;
        public CompanyRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void update(Company obj)
        {
           _db.Companies.Update(obj);
        }
    }
}
