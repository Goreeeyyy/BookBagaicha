<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Books.aspx.cs" Inherits="WebApplication1.Default" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Book Bagsicha</title>
    <link href="Content/style.css" rel="stylesheet" />
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />
</head>
<body>
    <form id="form1" runat="server">
        <!-- Navigation Bar -->
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
            <!-- Rest of the navigation bar remains the same -->
            <div class="d-flex me-3">
                <input class="form-control form-control-sm"
                       type="search"
                       placeholder="Search for products..."
                       aria-label="Search" />
            </div>
            <div class="d-flex gap-2">
                <a href="#" class="btn btn-outline-light btn-sm">
                    <i class="fas fa-user"></i>
                </a>
                <a href="#" class="btn btn-outline-light btn-sm">
                    <i class="fas fa-shopping-basket"></i>
                </a>
            </div>
        </div>
    </div>
</nav>

        <!-- Filters -->
        <div class="container mt-4">
            <div class="filter-bar mb-4 d-flex gap-2">
                <div class="dropdown">
                    <button class="btn btn-outline-dark btn-sm dropdown-toggle"
                            type="button" data-bs-toggle="dropdown">
                        All
                    </button>
                    <ul class="dropdown-menu">
                        <li><a class="dropdown-item small" href="#">All</a></li>
                        <li><a class="dropdown-item small" href="#">Award Winners</a></li>
                        <li><a class="dropdown-item small" href="#">Best Sellers</a></li>
                        <li><a class="dropdown-item small" href="#">New Releases</a></li>
                        <li><a class="dropdown-item small" href="#">Coming Soon</a></li>
                        <li><a class="dropdown-item small" href="#">On Sale</a></li>
                    </ul>
                </div>

                <div class="dropdown">
                    <button class="btn btn-outline-dark btn-sm dropdown-toggle"
                            type="button" data-bs-toggle="dropdown">
                        Sort
                    </button>
                    <ul class="dropdown-menu">
                        <li><a class="dropdown-item small" href="#">Price high to low</a></li>
                        <li><a class="dropdown-item small" href="#">Price low to high</a></li>
                    </ul>
                </div>

                <div class="dropdown">
                    <button class="btn btn-outline-dark btn-sm dropdown-toggle"
                            type="button" data-bs-toggle="dropdown">
                        Genre
                    </button>
                    <ul class="dropdown-menu">
                        <li><a class="dropdown-item small" href="#">Adventure</a></li>
                        <li><a class="dropdown-item small" href="#">Romance</a></li>
                        <li><a class="dropdown-item small" href="#">Mystery</a></li>
                        <li><a class="dropdown-item small" href="#">Science Fiction</a></li>
                    </ul>
                </div>
            </div>

            <!-- Book Cards -->
            <h3 class="mb-4">Explore All Books Here</h3>
            <div class="row row-cols-1 row-cols-sm-2 row-cols-md-3 row-cols-lg-4 g-4">
                <% for (int i = 1; i <= 8; i++) { %>
                <div class="col">
                    <div class="card h-100 text-center p-2">
                        <a href="BookDetails.aspx" class="stretched-link"></a>
                        <img src="Images/default.jpg"
                            class="card-img-top book-img"
                            alt="Book Cover" />

                        <div class="card-body">
                            <h6 class="card-title mb-1">Thunmanhandiya</h6>
                            <p class="card-subtitle text-muted small mb-2">Mahagamasakara</p>
                            <p class="mb-2 text-dark fw-bold">Rs. 790/-</p>
                            <div class="d-flex justify-content-center gap-2">
                                <button class="btn btn-sm btn-outline-primary">Add to Cart</button>
                                <button class="btn btn-sm btn-outline-secondary">
                                    <i class="far fa-heart"></i>
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
                <% } %>
            </div>
        </div>

        <!-- Scripts -->
        <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
        <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/js/bootstrap.bundle.min.js"></script>
    </form>
</body>
</html>
