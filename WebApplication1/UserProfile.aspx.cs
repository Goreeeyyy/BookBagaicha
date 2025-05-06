using System;
using System.Web.UI;

namespace WebApplication1
{
    public partial class UserProfile : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Initialization code
            }
        }

        // Corrected method name using PascalCase
        protected void BtnEdit_Click(object sender, EventArgs e)
        {
            // Edit button click handler
        }
    }
}