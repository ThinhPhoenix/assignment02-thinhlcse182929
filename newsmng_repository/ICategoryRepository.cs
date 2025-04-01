using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using newsmng_bussinessobject;
using newsmng_dao;

namespace newsmng_repository
{
    public interface ICategoryRepository
    {
        public List<object> GetSelectList();
        public List<Category> GetAll();
    }
}
