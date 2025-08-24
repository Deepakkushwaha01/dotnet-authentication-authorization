namespace Authentication.API.Infrastructure.Configurations.Entity
{
    public abstract class Entity
    {
        public int Id { get; set; }

        public Guid Uid { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public DateTime? DeletedOn { get; set; }

    }
}