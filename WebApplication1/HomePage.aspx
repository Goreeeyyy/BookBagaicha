<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HomePage.aspx.cs" Inherits="WebApplication1.Default" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml" xml:lang="en" lang="en">
<head runat="server">
    <!-- XHTML5-compliant charset declaration -->
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Book Bagaicha</title>
    <meta name="viewport" content="width=device-width, initial-scale=1" />

    <!-- General site CSS -->
    <link href="Content/style.css" rel="stylesheet" type="text/css" />

    <!-- Bootstrap & FontAwesome -->
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/css/bootstrap.min.css"
          rel="stylesheet"
          type="text/css" />
    <link rel="stylesheet"
          href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css"
          integrity="sha512-KfkfwYDsLkIlwQp6LFnl8zNdLGxu9YAA1QvwINks4PhcElQSvqcyVLLD9aMhXd13uQjoXtEKNosOWaZqXgel0g=="
          crossorigin="anonymous"
          referrerpolicy="no-referrer"
          type="text/css" />

    <!-- Carousel-specific CSS -->
    <link href="Content/style.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
        <!-- Navigation Bar -->
        <nav class="navbar navbar-expand-lg navbar-dark bg-dark sticky-top">
            <div class="container-fluid">
                <a class="navbar-brand fw-bold" href="HomePage.aspx">Book Bagaicha</a>
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

        <!-- Hero Section -->
        <div class="hero-section bg-light py-5">
            <div class="container">
                <div class="row align-items-center">
                    <div class="col-md-6 order-md-1 mb-4 mb-md-0">
                        <h2>Welcome to Book Bagaicha</h2>
                        <h1>Where Stories Blossom!</h1>
                        <p class="lead text-muted mb-4">
                            Explore a vibrant collection of books that spark joy, imagine wonders,
                            and find imagination from countless to one finds. We work hard to make
                            every story bloom in your heart. As the Bagaicha, we cultivate connections
                            through literature building a community where each page opens a new adventure.
                        </p>
                        <div class="subscribe-section">
                            <h5 class="mb-3">
                                Join us today and begin your literary journey with us!
                            </h5>
                            <div class="input-group w-75">
                                <asp:TextBox ID="TxtEmail" runat="server"
                                             CssClass="form-control"
                                             Placeholder="Enter your email"
                                             TextMode="Email" />
                                <asp:Button ID="BtnSubscribe"
                                            runat="server"
                                            CssClass="btn btn-primary"
                                            Text="Subscribe"
                                            OnClick="BtnSubscribe_Click" />
                            </div>
                        </div>
                    </div>
                    <div class="col-md-6 order-md-2">
                        <div class="hero-image-container">
                            <img src="Images/collection.jpg"
                                 alt="Book Collection"
                                 class="fixed-size-img rounded-3 shadow-lg" />
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Top Sellers Section -->
        <div class="container py-5">
            <h2 class="mb-4 text-center">Top Sellers</h2>
            <div class="row row-cols-1 row-cols-md-2 g-4">
                <!-- Book 1 -->
                <div class="col">
                    <div class="card h-100 shadow-sm">
                        <div class="card-body">
                            <div class="text-center mb-3">
                                <img src="Images/catastrophe.jpg"
                                     alt="I WANT A BETTER CATASTROPHE"
                                     class="fixed-size-img rounded-3 shadow-lg book-img" />
                            </div>
                            <h3 class="card-title">I WANT A BETTER CATASTROPHE</h3>
                            <p class="text-muted mb-2">Andrew Boyd</p>
                            <p class="card-text small">
                                With global warming projected to rocket past the 1.5°C benchmark,
                                this book is an existential toolkit for the climate crisis.
                            </p>
                            <div class="d-flex justify-content-between align-items-center mt-4">
                                <div>
                                    <s class="text-muted me-2">$29.99</s>
                                    <span class="h5 text-primary">$26.89</span>
                                </div>
                                <button class="btn btn-sm btn-outline-primary" type="button">
                                    <i class="fas fa-shopping-basket me-2"></i>Add to basket
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
                <!-- Book 2 -->
                <div class="col">
                    <div class="card h-100 shadow-sm">
                        <div class="card-body">
                            <div class="text-center mb-3">
                                <img src="Images/time.jpg"
                                     alt="THE TIME HAS COME"
                                     class="fixed-size-img rounded-3 shadow-lg book-img" />
                            </div>
                            <h3 class="card-title">THE TIME HAS COME</h3>
                            <p class="text-muted mb-2">Will Leitch</p>
                            <p class="card-text small">
                                Lindbergh's Pharmacy is an Athens, Georgia, institution—the kind of
                                small-town business that...
                            </p>
                            <div class="d-flex justify-content-between align-items-center mt-4">
                                <div>
                                    <s class="text-muted me-2">$30.99</s>
                                    <span class="h5 text-primary">$27.89</span>
                                </div>
                                <button class="btn btn-sm btn-outline-primary" type="button">
                                    <i class="fas fa-shopping-basket me-2"></i>Add to basket
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- About Us Section -->
        <section id="about-us" class="py-5 text-center">
            <div class="container">
                <h2 class="mb-5">About Us</h2>
                <div class="about-carousel d-flex align-items-center justify-content-center">
                    <button type="button" class="carousel-btn" id="prevBtn" aria-label="Previous">
                        <i class="fas fa-chevron-left"></i>
                    </button>
                    <div class="carousel-track">
                        <!-- Five upload slots -->
                        <div class="carousel-item">
                            <label class="placeholder-circle">
                                <input type="file"
                                       class="image-upload"
                                       accept="image/*"
                                       hidden="hidden" />
                                <span class="upload-text">Upload</span>
                            </label>
                        </div>
                        <div class="carousel-item">
                            <label class="placeholder-circle">
                                <input type="file"
                                       class="image-upload"
                                       accept="image/*"
                                       hidden="hidden" />
                                <span class="upload-text">Upload</span>
                            </label>
                        </div>
                        <div class="carousel-item">
                            <label class="placeholder-circle">
                                <input type="file"
                                       class="image-upload"
                                       accept="image/*"
                                       hidden="hidden" />
                                <span class="upload-text">Upload</span>
                            </label>
                        </div>
                        <div class="carousel-item">
                            <label class="placeholder-circle">
                                <input type="file"
                                       class="image-upload"
                                       accept="image/*"
                                       hidden="hidden" />
                                <span class="upload-text">Upload</span>
                            </label>
                        </div>
                        <div class="carousel-item">
                            <label class="placeholder-circle">
                                <input type="file"
                                       class="image-upload"
                                       accept="image/*"
                                       hidden="hidden" />
                                <span class="upload-text">Upload</span>
                            </label>
                        </div>
                    </div>
                    <button type="button" class="carousel-btn" id="nextBtn" aria-label="Next">
                        <i class="fas fa-chevron-right"></i>
                    </button>
                </div>
            </div>
        </section>

        <!-- Bootstrap JS & Carousel Script -->
        <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/js/bootstrap.bundle.min.js"
                type="text/javascript"></script>
        <script type="text/javascript">
            (function () {
                const track = document.querySelector('.carousel-track');
                const itemWidth = 140;

                document.getElementById('prevBtn').addEventListener('click', function () {
                    track.scrollBy({ left: -itemWidth, behavior: 'smooth' });
                });
                document.getElementById('nextBtn').addEventListener('click', function () {
                    track.scrollBy({ left: itemWidth, behavior: 'smooth' });
                });

                document.querySelectorAll('.image-upload').forEach(function (input) {
                    input.addEventListener('change', function (event) {
                        const file = event.target.files[0];
                        if (!file) return;
                        const reader = new FileReader();
                        reader.onload = function (e) {
                            const img = document.createElement('img');
                            img.src = e.target.result;
                            img.alt = "Uploaded image";
                            const label = input.parentElement;
                            label.innerHTML = '';
                            label.appendChild(img);
                        };
                        reader.readAsDataURL(file);
                    });
                });
            })();
        </script>
    </form>
</body>
</html>
