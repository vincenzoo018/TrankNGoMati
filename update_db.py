with open(r'Models\TrackNgoDbContext.cs', 'r', encoding='utf-8') as f:
    content = f.read()

if 'public virtual DbSet<DocumentTemplate> DocumentTemplates { get; set; }' not in content:
    content = content.replace('public virtual DbSet<WorkflowTransition> WorkflowTransitions { get; set; }', 'public virtual DbSet<WorkflowTransition> WorkflowTransitions { get; set; }\n\n    public virtual DbSet<DocumentTemplate> DocumentTemplates { get; set; }')

if 'entity.Property(e => e.IsUrgent)' not in content:
    doc_config = '''            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.IsUrgent).HasDefaultValue(false);
            entity.Property(e => e.UrgencyJustification).HasColumnType("nvarchar(max)");
            entity.Property(e => e.ParentDocumentId);
            entity.Property(e => e.VersionNumber).HasDefaultValue(1);'''
    content = content.replace('            entity.Property(e => e.Title).HasMaxLength(255);', doc_config)

if 'entity.Property(e => e.IsOutOfOffice)' not in content:
    user_config = '''            entity.Property(e => e.Username).HasMaxLength(100);
            entity.Property(e => e.IsOutOfOffice).HasDefaultValue(false);
            entity.Property(e => e.DelegatedUserId);'''
    content = content.replace('            entity.Property(e => e.Username).HasMaxLength(100);', user_config)

if 'entity.Property(e => e.ParentCommentId)' not in content:
    comment_config = '''            entity.Property(e => e.RemarkType).HasMaxLength(50);
            entity.Property(e => e.ParentCommentId);'''
    content = content.replace('            entity.Property(e => e.RemarkType).HasMaxLength(50);', comment_config)

if 'modelBuilder.Entity<DocumentTemplate>' not in content:
    template_config = '''
        modelBuilder.Entity<DocumentTemplate>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.Category).HasMaxLength(100);
            entity.HasOne(d => d.CreatedByUser)
                  .WithMany()
                  .HasForeignKey(d => d.CreatedByUserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        OnModelCreatingPartial(modelBuilder);'''
    content = content.replace('        OnModelCreatingPartial(modelBuilder);', template_config)

with open(r'Models\TrackNgoDbContext.cs', 'w', encoding='utf-8') as f:
    f.write(content)
