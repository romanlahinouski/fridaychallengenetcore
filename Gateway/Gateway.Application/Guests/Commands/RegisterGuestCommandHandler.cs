using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Gateway.Application.Guests.Commands
{
    public class RegisterGuestCommandHandler : AsyncRequestHandler<RegisterGuestCommand>
    {
        private readonly IGuestService guestGrpcService;

        public RegisterGuestCommandHandler(IGuestService guestGrpcService)
        {
            this.guestGrpcService = guestGrpcService;
        }
        
        protected override async Task Handle(RegisterGuestCommand request, CancellationToken cancellationToken)
        {
            await guestGrpcService.RegisterForAVisitAsync(request);
        }
    }
}
