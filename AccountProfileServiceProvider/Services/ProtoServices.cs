using AccountProfileServiceProvider.Dto;
using AccountProfileServiceProvider.Protos;
using Grpc.Core;
using System.Runtime.CompilerServices;

namespace AccountProfileServiceProvider.Services
{
    public class ProtoServices(UserProfileServices userProfileServices) : UserProfileProtoService.UserProfileProtoServiceBase
    {
        UserProfileServices _userProfileServices = userProfileServices;
        public async override Task<getUserProfileByIdResponse?> getUserProfileByAppUserId(getUserProfileByAppUserIdRequest request, ServerCallContext context)
        {
           var entity = await _userProfileServices.GetProfileByAppUserId(request.AppUserId);
            if (entity == null)
                return null;

            return new getUserProfileByIdResponse()
            {
                AppUserId = entity.AppUserId,
                Id = entity.Id,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                PhoneNumber = entity.PhoneNumber,
                StreetName = entity.StreetName,
                PostalCode = entity.PostalCode,
                City = entity.City,
            };
        }

        public async override Task<getUserProfileByIdResponse?> getUserProfileById(getUserProfileByIdRequest request, ServerCallContext context)
        {
            var entity = await _userProfileServices.GetProfileByIdAsync(request.Id);
            if (entity == null)
                return null;

            return new getUserProfileByIdResponse()
            {
                AppUserId = entity.AppUserId,
                Id = entity.Id,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                PhoneNumber = entity.PhoneNumber,
                StreetName = entity.StreetName,
                PostalCode = entity.PostalCode,
                City = entity.City,
            };
           
        }

        public async override Task<deleteUserProfileResponse?> deleteUserProfile(deleteUserProfileRequest request, ServerCallContext context)
        {
            var entity = _userProfileServices.GetProfileByIdAsync(request.Id);
            if (entity == null)
                return new deleteUserProfileResponse() { Succeeded = false, Message = "Profile not deleted, entity with given id was not found" };
            var result = await _userProfileServices.DeleteProfileAsync(request.Id);
            if (result == null)
                return new deleteUserProfileResponse() { Succeeded = false , Message = "Profile not deleted, result was null"};
            return new deleteUserProfileResponse() { Succeeded = true };

        }

        public async override Task<createUserProfileResponse?> createUserProfile(createUserProfileRequest request, ServerCallContext context)
        {
            var entity = new AddAcountProfileForm()
            {
                AppUserId = request.AppUserId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                StreetName = request.StreetName,
                PostalCode = request.PostalCode,
                City = request.City,
            };
            
            var result = await _userProfileServices.AddUserProfileAsync(entity, request.AppUserId);
            if (result == null)
                return null;
            return new createUserProfileResponse() {
                AppUserId = entity.AppUserId,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                PhoneNumber = entity.PhoneNumber,
                StreetName = entity.StreetName,
                PostalCode = entity.PostalCode,
                City = entity.City
            };
             
        }



    }
}
