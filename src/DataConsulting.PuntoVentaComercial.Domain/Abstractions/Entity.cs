namespace DataConsulting.PuntoVentaComercial.Domain.Abstractions
{
    public abstract class Entity
    {
        public int Id { get; init; }

        protected Entity(int id)
        {
            Id = id;
        }

        protected Entity()
        {
        }

    }
}
