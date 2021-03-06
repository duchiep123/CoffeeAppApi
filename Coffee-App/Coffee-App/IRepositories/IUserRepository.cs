﻿using Coffee_App.GenericRepository;
using Coffee_App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Coffee_App.IRepositories
{
    public interface IUserRepository : IBaseRepository<User>
    {
        object RefreshToken(string token, string refreshToken); 
    }
}
