using System;
using MyApp.Application.Interfaces;
using MyApp.Domain.Entities;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Repositories
{
    public class OtpCodesRepository : GenericRepository<OtpCode>, IOtpCodesRepository
    {
        public OtpCodesRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}

