namespace FastPCB.Services.Infrastructure.Cache
{
    public static class CacheKeys
    {
        public const string ProjectPrefix = "projects";
        public const string AdminDashboardPrefix = "admin:dashboard";

        public static string ProjectList(string fingerprint) => $"{ProjectPrefix}:list:{fingerprint}";
        public static string ProjectDetail(int projectId) => $"{ProjectPrefix}:detail:{projectId}";
        public static string AdminDashboard() => $"{AdminDashboardPrefix}:stats";
    }
}
