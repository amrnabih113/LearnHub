using LearnHub.Domain.Courses.Sections.Lessons;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearnHub.Infrastructure.Data.Configuration;

public sealed class SubtitleTrackConfiguration
    : IEntityTypeConfiguration<SubtitleTrack>
{
    public void Configure(EntityTypeBuilder<SubtitleTrack> builder)
    {
        builder.ToTable("SubtitleTracks");

        builder.HasKey(x => x.Id);


        builder.Property(x => x.Url)
            .IsRequired()
            .HasMaxLength(500);


        builder.Property(x => x.IsDefault)
            .IsRequired();


        builder.OwnsOne(
            x => x.Language,
            language =>
            {
                language.Property(x => x.Code)
                    .HasColumnName("LanguageCode")
                    .HasMaxLength(5)
                    .IsRequired();


                language.Property(x => x.Name)
                    .HasColumnName("LanguageName")
                    .HasMaxLength(100)
                    .IsRequired();
            });


        builder.Ignore(x => x.DomainEvents);
    }
}