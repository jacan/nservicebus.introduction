namespace SiriusCyberneticsCorp.Sales.Backend
{
    using System;

    using NServiceBus.Saga;

    public class OrderFacilitySagaData : ISagaEntity
    {
        public Guid Id { get; set; }

        public string Originator { get; set; }

        public string OriginalMessageId { get; set; }

        [Unique]
        public Guid OrderId { get; set; }

        public Guid FacilityId { get; set; }

        public Guid CategoryId { get; set; }

        public bool IsDone
        {
            get
            {
                return this.InstalledAt.HasValue;
            }
        }

        public DateTime? InstalledAt { get; private set; }

        public string InstalledIn { get; private set; }

        public void Installed(DateTime at, string installedIn)
        {
            this.InstalledAt = at;
            this.InstalledIn = installedIn;
        }
    }
}