namespace Application.Utils
{
    public static class GuidValidator
    {
        public static bool BeAValidGuid(Guid guid)
        {
            return Guid.TryParse(guid.ToString(), out _);
        }
    }
}
