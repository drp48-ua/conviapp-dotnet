namespace ConviAppWeb.Services
{
    /// <summary>
    /// PlanService — Controls feature access based on user subscription plan.
    /// </summary>
    public static class PlanService
    {
        // Plan tiers
        public const string Basic = "Basico";
        public const string Pro = "Profesional";
        public const string Enterprise = "Enterprise";

        /// <summary>Whether the user can access reservations (Pro+)</summary>
        public static bool CanAccessReservas(string role) => role == Pro || role == Enterprise;

        /// <summary>Whether the user can access incidents (Pro+)</summary>
        public static bool CanAccessIncidencias(string role) => role == Pro || role == Enterprise;

        /// <summary>Whether the user can access contracts & payments (Enterprise only)</summary>
        public static bool CanAccessContratos(string role) => role == Enterprise;

        /// <summary>Whether the user can manage multiple properties (Pro+)</summary>
        public static bool CanManageProperties(string role) => role == Pro || role == Enterprise;

        /// <summary>Whether the user can upload documents (Pro+)</summary>
        public static bool CanUploadDocuments(string role) => role == Pro || role == Enterprise;

        /// <summary>Whether ads should be shown (Basic only)</summary>
        public static bool ShowAds(string role) => role == Basic || string.IsNullOrEmpty(role);

        /// <summary>Maximum properties allowed</summary>
        public static int MaxProperties(string role)
        {
            return role switch
            {
                Enterprise => 999,
                Pro => 3,
                _ => 1
            };
        }

        /// <summary>Maximum users per property</summary>
        public static int MaxUsersPerProperty(string role)
        {
            return role switch
            {
                Enterprise => 999,
                Pro => 8,
                _ => 4
            };
        }

        /// <summary>Get plan display name</summary>
        public static string DisplayName(string role)
        {
            return role switch
            {
                Enterprise => "Enterprise 🏢",
                Pro => "Pro ⭐",
                _ => "Basic 📦"
            };
        }

        /// <summary>Get plan color class</summary>
        public static string PlanColor(string role)
        {
            return role switch
            {
                Enterprise => "color: var(--warning);",
                Pro => "color: var(--primary-light);",
                _ => "color: var(--text-muted);"
            };
        }
    }
}
