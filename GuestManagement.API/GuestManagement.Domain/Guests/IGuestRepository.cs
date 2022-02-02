using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GuestManagement.Domain.Guests
{
    public interface IGuestRepository
    {
        public Task<Guest> GetGuestByIdAsync(string guestId);

        public Task<Guest> GetGuestByEmailAsync(string email);

        public void Update(Guest user);

        public Task AddAsync(Guest guest);  
    }
}
