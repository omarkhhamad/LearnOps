using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Options
{
    public class JwtSettings
    {
        // ════════════════════════════════════════════════════════════
        // Section Name - اسم القسم في appsettings.json
        // ════════════════════════════════════════════════════════════

        public const string SectionName = "JwtSettings"; 
        // ← const = ثابت لا يتغير
        // نستخدمه في Program.cs:
        // builder.Configuration.GetSection(JwtSettings.SectionName)

        // ════════════════════════════════════════════════════════════
        // Properties - الخصائص (تُملأ من appsettings.json)
        // ════════════════════════════════════════════════════════════

        public string Secret { get; set; } = string.Empty;           
        // ← المفتاح السري
        // = string.Empty: قيمة افتراضية فارغة
        // لتجنب null reference

        public string Issuer { get; set; } = string.Empty;           // ← مُصدر الـ Token

        public string Audience { get; set; } = string.Empty;         // ← المستقبل المقصود

        public int AccessTokenExpirationMinutes { get; set; }        // ← مدة صلاحية Access Token بالدقائق

        public int RefreshTokenExpirationDays { get; set; }          // ← مدة صلاحية Refresh Token بالأيام
    }
}
