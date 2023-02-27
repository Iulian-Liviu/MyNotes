using Microsoft.AspNetCore.Identity;

namespace MyNotes.API.Helpers;

public static class HelpersMethods {
    public static string HasHPassword(string password) {
        var hashedPassword = new PasswordHasher<object>().HashPassword(null!, password);

        return hashedPassword;
    }

    public static bool IsPasswordMatch(string hashPassword, string inputPassword) {
        var hashedPassword = new PasswordHasher<object>().VerifyHashedPassword(null!, hashPassword, inputPassword);
        if (hashedPassword == PasswordVerificationResult.Success)
            return true;
        if (hashedPassword == PasswordVerificationResult.Failed)
            return false;
        return false;
    }
}