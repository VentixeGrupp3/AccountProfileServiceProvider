using AccountProfileServiceProvider.Contexts;
using AccountProfileServiceProvider.Entities;
using Microsoft.EntityFrameworkCore;

namespace AccountProfileServiceProvider.Repos;

public class UserProfileRepo(UserProfileContext context)
{
    private readonly UserProfileContext _context = context;


    public async Task<UserProfileEntity?> GetProfileByUserId(string userId)
    {
        var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(x => x.AppUserId == userId);
        if (userProfile == null)
            return null;
        return userProfile;
    }

    public async Task<UserProfileEntity?> GetProfileById(string id)
    {
        return await _context.UserProfiles.FirstOrDefaultAsync(x => x.Id == id);
    }
    public async Task<UserProfileEntity?> GetProfileByAppUserId(string appUserId)
    {
        return await _context.UserProfiles.FirstOrDefaultAsync(x => x.AppUserId == appUserId);
    }


    public async Task<UserProfileEntity> CreateProfileAsync(UserProfileEntity userProfile)
    {
        _context.UserProfiles.Add(userProfile);
        await _context.SaveChangesAsync();
        return userProfile;
    }

    public async Task<UserProfileEntity> UpdateProfileAsync(UserProfileEntity userProfile)
    {
        _context.UserProfiles.Update(userProfile);
        await _context.SaveChangesAsync();
        return userProfile;
    }

    public async Task<UserProfileEntity?> DeleteProfileAsync(UserProfileEntity userProfile)
    {
        _context.UserProfiles.Remove(userProfile);
        await _context.SaveChangesAsync();
        return userProfile;
    }

}
