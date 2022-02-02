using GuestManagement.Domain.Guests.Visits;
using System;
using System.Threading.Tasks;


namespace GuestManagement.Domain.Guests
{
    public class VisitRegistrationService : IVisitRegistrationService
    {    
     
        private readonly IGuestService guestRegistrationService;

        public VisitRegistrationService(       
            IGuestService guestRegistrationService
            )           
        {              
            this.guestRegistrationService = guestRegistrationService;
        }

 
        public async Task RegisterVisit(Guest guest, int restaurantId)
        {
                                                         
         // await guestRegistrationService.RegisterGuest(guest.GuestId, restaurantId);
           
           var visit = Visit.CreateVisit(restaurantId, DateTime.Now, 0);

           guest.AddVisit(visit);
                       
        }

    }
}
