using System;
using System.Web.UI;

namespace WebApplication1
{
    public partial class BookDetails : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Your book details page initialization code here
            // You can fetch book ID from query string and load data
            // string bookId = Request.QueryString["id"];
        }
    }
}