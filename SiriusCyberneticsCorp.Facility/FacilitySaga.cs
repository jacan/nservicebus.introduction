namespace SiriusCyberneticsCorp.Facility
{
    using System;

    using NServiceBus;
    using NServiceBus.Saga;

    using SiriusCyberneticsCorp.Contract.Complaint;
    using SiriusCyberneticsCorp.Contract.Facility;
    using SiriusCyberneticsCorp.Contract.Sales;

    public class FacilitySaga : Saga<FacilitySagaData>, 
                                IAmStartedByMessages<Ordered>,
                                IHandleMessages<ComplainedAbout>,
                                IHandleTimeouts<ReadyToInstall>
    {
        public override void ConfigureHowToFindSaga()
        {
            this.ConfigureMapping<ComplainedAbout>(s => s.FacilityId, m => m.FacilityId);
            this.ConfigureMapping<Installed>(s => s.FacilityId, m => m.FacilityId);
        }

        public void Handle(Ordered message)
        {
            this.Data.FacilityId = message.FacilityId;
            this.Data.OrderId = message.OrderId;
            string facilityId = this.Data.FacilityId.ToString("N");
            this.Data.Name = string.Format("VHPT-{0}", facilityId.Substring(0, 6)).ToUpper();

            Console.WriteLine("Installing facility {0} with name {1} for order {2}.", this.Data.FacilityId, this.Data.Name, this.Data.OrderId);

            // This is only for simulation
            this.RequestUtcTimeout<ReadyToInstall>(TimeSpan.FromSeconds(5));
        }

        public void Timeout(ReadyToInstall state)
        {
            this.Data.Motivation = 100;

            Console.WriteLine("Facility {0} with name {1} for order {2} installed. Running fully motivated!", this.Data.FacilityId, this.Data.Name, this.Data.OrderId);

            var installed = this.Bus.CreateInstance<Installed>(
                m =>
                    {
                        m.FacilityId = this.Data.FacilityId;
                        m.At = DateTime.UtcNow;
                        m.InstalledIn = "Building 42";
                        m.Name = this.Data.Name;
                    });

            this.Bus.Publish(installed);
        }

        public void Handle(ComplainedAbout message)
        {
            this.Data.Motivation -= 20;

            if (this.Data.Motivation > 0)
            {
                Console.WriteLine("Received complaint about facility {0}! Current motivation is at {1}.", this.Data.FacilityId, this.Data.Motivation);

                this.Bus.Publish<MotivationDecreased>(
                    m =>
                        {
                            m.FacilityId = this.Data.FacilityId;
                            m.Amount = 20;
                        });
            }
            else
            {
                Console.WriteLine("Received complaint about facility {0}! Not motivated to do any further work!", this.Data.FacilityId);

                this.Bus.Publish<BecameDemotivated>(
                    m =>
                        {
                            m.FacilityId = this.Data.FacilityId;
                        });
            }
        }
    }

    public class ReadyToInstall
    {
    }
}