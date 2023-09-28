namespace API.Extentions;

public static class DateTimeExtensions
{
    public static int CalculateAge(this DateOnly dob) {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var age = today.Year - dob.Year;

        // have not birthday (chua toi sinh nhat)
        if(dob > today.AddYears(-age)) age--;

        return age;  
    }
}