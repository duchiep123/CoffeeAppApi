using Coffee_App.GenericRepository;
using Coffee_App.IRepositories;
using Coffee_App.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Coffee_App.Repositories
{
    public class JwtTokenRepository : BaseRepository<JwtToken>, IJwtTokenRepository
    {
        public JwtTokenRepository(DbContext dbContext) : base(dbContext)
        {

        }
    }
}
