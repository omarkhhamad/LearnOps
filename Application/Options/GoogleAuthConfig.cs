namespace Application.Options
{
    public class GoogleAuthConfig
    {
        // اسم السكشن في appsettings.json
        public const string SectionName = "Authentication:Google";

        // معرف العميل الخاص بجوجل
        public string ClientId { get; set; } = string.Empty;

        // السر الخاص بجوجل
        public string ClientSecret { get; set; } = string.Empty;
    }
}
