namespace MyPortfolio.Models
{
    public class ErrorViewModel
    {
        public int? StatusCode { get; set; }

        public string Headline => StatusCode switch
        {
            404 => "This page doesn't exist",
            >= 500 => "Something went wrong",
            _ => "This request couldn't be completed"
        };

        public string Blurb => StatusCode switch
        {
            404 => "The link is out of date or the record was archived. Everything else on the site is still where you left it.",
            >= 500 => "The page could not be built. Nothing you did destroyed anything — reload it, or head back to the portfolio.",
            _ => "The request was rejected before it could be served. Try again, or head back to the portfolio."
        };
    }
}
