namespace SOTMDecks.View
{
    /// <summary>
    /// Production access point for the view layer: holds the current <see cref="IOutput"/>
    /// and forwards to the stateless renderers. Call sites use this instead of reaching into
    /// model classes for rendering. (Renderers can be tested directly with any IOutput.)
    /// </summary>
    internal static class Display
    {
        public static IOutput Out { get; set; } = new ConsoleOutput();

        public static void Card(Card card, bool brief = false) => CardRenderer.Render(Out, card, brief);

        public static void CardText(Card card) => CardRenderer.RenderText(Out, card);

        public static void Modifier(Modifier modifier) => Out.WriteLine(modifier.ToString(), modifier.Color);

        public static void List<T>(CardCollection<T> col, bool verbose = false) where T : Card
            => CardCollectionRenderer.RenderList(Out, col, verbose);

        public static void Collection<T>(CardCollection<T> col, CardFilter filter = CardFilter.NONE, bool brief = false) where T : Card
            => CardCollectionRenderer.Render(Out, col, filter, brief);
    }
}
