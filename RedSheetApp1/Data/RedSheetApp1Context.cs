using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RedSheetApp1.Models;

namespace RedSheetApp1.Data
{
    public class RedSheetApp1Context : DbContext
    {
        public RedSheetApp1Context (DbContextOptions<RedSheetApp1Context> options)
            : base(options)
        {
        }

        public DbSet<Question> Question { get; set; }
    }
}
