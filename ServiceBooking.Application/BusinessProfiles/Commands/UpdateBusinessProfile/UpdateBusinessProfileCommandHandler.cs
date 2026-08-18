using MediatR;
using ServiceBooking.Application.Bookings.Commands.CreateBooking;
using ServiceBooking.Application.Common.Interfaces;
using ServiceBooking.Domain.Entities;
using ServiceBooking.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBooking.Application.BusinessProfiles.Commands.UpdateBusinessProfile
{
    public class UpdateBusinessProfileCommandHandler : IRequestHandler<UpdateBusinessProfileCommand, string>
    {
        private IBusinessProfileRepository _businessProfileRepository;
        private IUnitOfWork _unitOfWork;

        public UpdateBusinessProfileCommandHandler(IBusinessProfileRepository _businessProfileRepository, IUnitOfWork unitOfWork)
        {
            this._businessProfileRepository = _businessProfileRepository;
            this._unitOfWork = unitOfWork;
        }

        public async Task<string> Handle(UpdateBusinessProfileCommand request, CancellationToken cancellationToken)
        {
            var businessProfile = await _businessProfileRepository.GetByIdAsync(request.Id);
            if (businessProfile == null)
            {
                throw new Exception($"Business profile with ID {request.Id} not found.");
            }

            // Update the business profile properties
            var domainEmployees = request.Employees.Select(dto => new Employee(
                   dto.Name,
                   Email.Create(dto.Email),
                   dto.Position
               )).ToList();
            var domainCategories = request.Categories.Select(dto => new ServiceCategory(
                  dto.Name,
                  dto.ImageUrl
              )).ToList();

            businessProfile.UpdateDetails(
                 request.LogoUrl,
                 request.Name, // Note: You mapped Name to Email in your snippet, check if that's intended
                 domainEmployees,
                 domainCategories
             );

            await _businessProfileRepository.UpdateAsync(businessProfile);
            await _unitOfWork.SaveChangesAsync();
            return businessProfile.Id;

        }
    }
}
