using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Infrastructure.Configurations
{
    public class ApplicationUserConfigurations : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.HasIndex(x => x.NationalId)
                .IsUnique();
        }
    }
}
