using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityProvider.Extensions
{
    public static class EntityTypeBuilderExtension
    {
        public static EntityTypeBuilder<TEntity> UseTimestampedProperty<TEntity>(this EntityTypeBuilder<TEntity> entity) where TEntity : class, ITimestampedEntity
        {
            entity.Property(d => d.CreatedTime).ValueGeneratedOnAdd();
            entity.Property(d => d.UpdatedTime).ValueGeneratedOnAddOrUpdate();

            entity.Property(d => d.CreatedTime).Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);
            entity.Property(d => d.CreatedTime).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            entity.Property(d => d.UpdatedTime).Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);
            entity.Property(d => d.UpdatedTime).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

            return entity;
        }
    }
    
    public interface ITimestampedEntity
    {
        DateTime CreatedTime { get; set; }
        DateTime? UpdatedTime { get; set; }
    }
}