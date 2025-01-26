namespace Komit.Base.Values;
public static class DateValues
{
    public static DateTime Now => DateTime.UtcNow;
    public static DateTime At(int year, int month, int day) => DateTime.SpecifyKind(new DateTime(year, month, day), DateTimeKind.Utc);
    public static DateTime AtLastDayOfMonth(int year, int month) => At(year, month, At(year, month < 12 ? month + 1 : 1, 1).AddTicks(-1).Day);
    public static DateTime AtFirstDayOfPeriod(int year, YearPeriods yearPeriod, int period) => At(year, yearPeriod switch
    {
        YearPeriods.Year => 1,
        YearPeriods.Quarter => period == 1 ? 1 : 1 + (period - 1) * 3,
        YearPeriods.Month => period,
        _ => throw new NotImplementedException()
    }, 1);
    public static DateTime AtLastDayOfPeriod(int year, YearPeriods yearPeriod, int period) => AtLastDayOfMonth(year, yearPeriod switch
    {
        YearPeriods.Year => 12,
        YearPeriods.Quarter => period * 3,
        YearPeriods.Month => period,
        _ => throw new NotImplementedException()
    });
    public static DateTime At(string dateString)
    {
        var dateParts = dateString.Split("-").Select(x => int.Parse(x)).ToArray();
        DateTime? result = default;
        if (dateParts.Length == 3)
            result = At(dateParts[2], dateParts[1], dateParts[0]);
        if (!result.HasValue)
            throw new DomainException("Ugyldig dato værdi", $"Dato værdien {dateString} kunne ikke fortolkes, formatet skal være DD-MM-YYYY");
        return DateTime.SpecifyKind(result.Value, DateTimeKind.Utc);
    }
}
public static class DateOnlyValues
{
    public static DateOnly Now => DateOnly.FromDateTime(DateTime.UtcNow);
    public static DateOnly At(int year, int month, int day) => DateOnly.FromDateTime(DateValues.At(year, month, day));
    public static DateOnly At(string dateString)
    {
        var dateParts = dateString.Split("-").Select(x => int.Parse(x)).ToArray();
        DateOnly? result = default;
        if (dateParts.Length == 3)
            result = At(dateParts[2], dateParts[1], dateParts[0]);
        if (!result.HasValue)
            throw new DomainException("Ugyldig dato værdi", $"Dato værdien {dateString} kunne ikke fortolkes, formatet skal være DD-MM-YYYY");
        return result.Value;
    }
    public static DateOnly AtFirstDayOfPeriod(int year, YearPeriods yearPeriod, int period) => At(year, yearPeriod switch
    {
        YearPeriods.Year => 1,
        YearPeriods.Quarter => period == 1 ? 1 : 1 + (period - 1) * 3,
        YearPeriods.Month => period,
        _ => throw new NotImplementedException()
    }, 1);
    public static DateOnly AtLastDayOfPeriod(int year, YearPeriods yearPeriod, int period) => AtLastDayOfMonth(year, yearPeriod switch
    {
        YearPeriods.Year => 12,
        YearPeriods.Quarter => period * 3,
        YearPeriods.Month => period,
        _ => throw new NotImplementedException()
    });
    public static DateOnly AtLastDayOfMonth(int year, int month) =>  At(year, month, DateTime.DaysInMonth(year, month));
}
public enum YearPeriods
{
    Year = 2,
    Quarter = 1,
    Month = 0,
}