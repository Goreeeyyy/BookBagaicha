<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserProfile.aspx.cs" Inherits="WebApplication1.UserProfile" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>User Profile - Book Bagaicha</title>
    <link href="Content/style.css" rel="stylesheet" />
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/css/bootstrap.min.css" 
          rel="stylesheet" />
    <link rel="stylesheet" 
          href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />
</head>
<body>
    <form id="form1" runat="server">
        <!-- Corrected Navigation Bar -->
        <nav class="navbar navbar-expand-lg navbar-dark bg-dark sticky-top">
            <div class="container-fluid">
                <a class="navbar-brand fw-bold" href="HomePage.aspx">Book Bagsicha</a>
                <div class="d-flex align-items-center ms-auto">
                    <ul class="navbar-nav me-4">
                        <li class="nav-item">
                            <a class="nav-link active" href="HomePage.aspx">HOME</a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" href="Books.aspx">BOOKS</a>
                        </li>
                    </ul>
                    <div class="d-flex me-3">
                        <input class="form-control form-control-sm"
                               type="search"
                               placeholder="Search for products..."
                               aria-label="Search" />
                    </div>
                    <div class="d-flex gap-2">
                        <a href="UserProfile.aspx" class="btn btn-outline-light btn-sm">
                            <i class="fas fa-user"></i>
                        </a>
                        <a href="#" class="btn btn-outline-light btn-sm">
                            <i class="fas fa-shopping-basket"></i>
                        </a>
                    </div>
                </div>
            </div>
        </nav>

        <!-- Profile Content -->
        <div class="container my-5">
            <div class="row">
                <!-- Profile Sidebar -->
                <div class="col-md-3 mb-4">
                    <div class="profile-sidebar card p-3">
                        <div class="list-group">
                            <a href="#" class="list-group-item list-group-item-action active">User Profile</a>
                            <a href="#" class="list-group-item list-group-item-action">Order History</a>
                            <a href="#" class="list-group-item list-group-item-action">Logout</a>
                        </div>
                    </div>
                </div>

                <!-- Profile Details -->
                <div class="col-md-9">
                    <div class="profile-details card p-4">
                        <h2 class="mb-4">Alexa Rawles</h2>
                        <p class="text-muted mb-4">olesorawles@gmail.com</p>

                        <div class="row g-4">
                            <!-- Corrected input fields -->
                            <div class="col-md-6">
                                <div class="form-group">
                                    <label>Full Name</label>
                                    <input type="text" class="form-control" 
                                           value="Your First Name" 
                                           readonly="readonly" />
                                </div>
                            </div>
                            <div class="col-md-6">
                                <div class="form-group">
                                    <label>Gender</label>
                                    <input type="text" class="form-control" 
                                           value="Gender" 
                                           readonly="readonly" />
                                </div>
                            </div>
                            <div class="col-md-6">
                                <div class="form-group">
                                    <label>Language</label>
                                    <input type="text" class="form-control" 
                                           value="Language" 
                                           readonly="readonly" />
                                </div>
                            </div>
                            <div class="col-md-6">
                                <div class="form-group">
                                    <label>Nick Name</label>
                                    <input type="text" class="form-control" 
                                           value="Nickname" 
                                           readonly="readonly" />
                                </div>
                            </div>
                            <div class="col-md-6">
                                <div class="form-group">
                                    <label>Country</label>
                                    <input type="text" class="form-control" 
                                           value="Country" 
                                           readonly="readonly" />
                                </div>
                            </div>
                            <div class="col-md-6">
                                <div class="form-group">
                                    <label>Time Zone</label>
                                    <input type="text" class="form-control" 
                                           value="Time Zone" 
                                           readonly="readonly" />
                                </div>
                            </div>
                        </div>

                        <!-- Corrected button with proper event handler -->
                        <asp:Button ID="BtnEdit" runat="server" 
                                  CssClass="btn btn-primary mt-4 px-4" 
                                  Text="Edit" 
                                  OnClick="BtnEdit_Click" />
                    </div>
                </div>
            </div>
        </div>

        <!-- Promotional Banner -->
        <div class="bg-primary text-white py-4">
            <div class="container text-center">
                <h5 class="mb-3">BUY 5 BOOKS TO GET 5% DISCOUNT AND GET 10% DISCOUNT AFTER YOUR 10TH PURCHASE</h5>
            </div>
        </div>

        <!-- Corrected Footer Section -->
        <footer class="bg-dark text-white py-5">
            <div class="container">
                <div class="row g-4">
                    <div class="col-md-6">
                        <div class="row g-4">
                            <div class="col-4">
                                <h6>About</h6>
                                <ul class="list-unstyled">
                                    <li><a href="#" class="text-white text-decoration-none">Features</a></li>
                                    <li><a href="#" class="text-white text-decoration-none">Pricing</a></li>
                                    <li><a href="#" class="text-white text-decoration-none">Gallery</a></li>
                                    <li><a href="#" class="text-white text-decoration-none">Team</a></li>
                                </ul>
                            </div>
                        </div>
                    </div>
                    <div class="col-md-6">
                        <div class="subscribe-section">
                            <h5>Subscribe to stay tuned for new product and latest updates</h5>
                            <div class="input-group mt-3">
                                <input type="email" 
                                       class="form-control" 
                                       placeholder="Enter your email address" />
                                <button class="btn btn-outline-light" type="button">Subscribe</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </footer>

        <!-- Properly closed script tags -->
        <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
        <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/js/bootstrap.bundle.min.js"></script>
    </form>
</body>
</html>