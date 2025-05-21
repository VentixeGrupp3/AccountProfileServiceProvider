using AccountProfileServiceProvider.Dto;
using AccountProfileServiceProvider.Entities;
using AccountProfileServiceProvider.Repos;
using Microsoft.AspNetCore.Mvc;

namespace AccountProfileServiceProvider.Services
{
    public class UserProfileServices(UserProfileRepo repo)
    {
        private readonly UserProfileRepo _repo = repo;

        public async Task<UserProfileEntity?> AddUserProfileAsync(AddAcountProfileForm? formData,string appUserId)
        {
            var userProfile = new UserProfileEntity()
            {
                Id = Guid.NewGuid().ToString(),
                AppUserId = appUserId,
                FirstName = formData.FirstName,
                LastName = formData.LastName,
                PhoneNumber = formData.PhoneNumber,
                StreetName = formData.StreetName,
                PostalCode = formData.PostalCode,
                City = formData.City,
                
            };

            var result = await _repo.CreateProfileAsync(userProfile);

            if (result == null)
                return null;

            return result;

        }


        public async Task<UserProfileEntity?> UpdateUserProfile(UpdateAcountProfileForm? formData)
        {
            var entity = await _repo.GetProfileByUserId(formData.Id);
            if (entity == null) 
                return null;
                entity.FirstName = formData.FirstName;
                entity.LastName = formData.LastName;
                entity.PhoneNumber = formData.PhoneNumber;
                entity.StreetName = formData.StreetName;
                entity.PostalCode = formData.PostalCode;
                entity.City = formData.City;
            var result = await _repo.UpdateProfileAsync(entity);
            if (result == null) 
                return null;
            return result;
        }

        public async Task<UserProfileEntity?> GetProfileByAppUserId(string appUserId)
        {
            var entity =  await _repo.GetProfileByAppUserId(appUserId);
            if (entity == null)
                return null;
            return entity;
        }
        public async Task<UserProfileEntity?> GetProfileByIdAsync(string id)
        {
            var entity = await _repo.GetProfileById(id);
            if (entity == null)
                return null;
            return entity;
        }


        public async Task<UserProfileEntity?> DeleteProfileAsync(string id)
        {
            var entity = await _repo.GetProfileById(id);
            if (entity == null)
                return null;
            var result = await _repo.DeleteProfileAsync(entity);
            if (result == null)
                return null;
            return result;
        }





































    }

}

