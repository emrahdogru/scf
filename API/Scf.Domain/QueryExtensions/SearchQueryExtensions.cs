namespace Scf.Domain
{
    public static class SearchQueryExtensions
    {
        public static IQueryable<TSource> Search<TSource>(this IQueryable<TSource> query, string keywords) where TSource : ISearchableEntity
        {
            var words = KeywordGenerator.GenerateKeywords(keywords);

            //var words = keywords.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            foreach (var word in words)
            {
                query = query.Where(entity => entity.Keywords.Any(kw => kw.Contains(word)));
            }

            return query;
        }
    }
}
