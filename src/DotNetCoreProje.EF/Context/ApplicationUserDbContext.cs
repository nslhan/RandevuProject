using DotNetCoreProje.Domain.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetCoreProje.EF.Context
{
    public class ApplicationUserDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationUserDbContext(DbContextOptions<ApplicationUserDbContext> options) : base(options)
        {

        }

    }
}
