using FCG_Libraries.Domain.Libraries.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FCG_Libraries.Infrastructure.Libraries.Mappings
{
    public class LibraryMap : IEntityTypeConfiguration<Library>
    {
        public void Configure(EntityTypeBuilder<Library> builder)
        {
            builder.ToTable("Libraries");

            builder.HasKey(x => x.Id)
                .HasName("PK_Library");

            builder.Property(x => x.UserId)
                .HasColumnName("UserId")
                .IsRequired(true);

            builder.Property(x => x.GameId)
                .HasColumnName("GameId")
                .IsRequired(true);

            builder.Property(x => x.Status)
                .HasColumnName("Status")
                .IsRequired(true);

            builder.Property(x=> x.PricePaid)
                .HasColumnName("PricePaid")
                .HasPrecision(12, 2)
                .IsRequired(true);

            builder.Property(x => x.PaymentId)
                .HasColumnName("PaymentId")
                .IsRequired(false);

        }
    }
}
