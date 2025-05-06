using System;
using System.Web.UI;

namespace WebApplication1
{
    public partial class Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Page initialization code
            if (!IsPostBack)
            {
                // Initialization logic for first page load
                // You can load data from database here
            }
        }

        // Example method for subscription button
        protected void BtnSubscribe_Click(object sender, EventArgs e)
        {
            // your subscribe logic here...
        }

        // Example method for adding to booklist
        protected void AddToBooklist_Click(object sender, EventArgs e)
        {
            // Add book to user's list logic
            // You can get book ID from CommandArgument
        }
    }
}