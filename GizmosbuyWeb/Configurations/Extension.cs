namespace GizmosbuyWeb.Configurations
{
    public static class Extension
    {
        public static bool IsAjaxRequest(this HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.Headers != null)
                return request.Headers["X-Requested-With"] == "XMLHttpRequest";
            return false;
        }

        public static IEnumerable<T> OrderByDynamic<T>(this IEnumerable<T> source, string orderByMember, string sortDirection)
        {
            var property = typeof(T).GetProperty(orderByMember);
            if (property == null) return source;

            return sortDirection == "asc"
                ? source.OrderBy(x => property.GetValue(x, null))
                : source.OrderByDescending(x => property.GetValue(x, null));

        }
    }
}
