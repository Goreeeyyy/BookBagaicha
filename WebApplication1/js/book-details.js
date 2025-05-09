// book-details.js
document.addEventListener('DOMContentLoaded', () => {
    // Initialize cart and wishlist in localStorage if not exists
    if (!localStorage.getItem('cart')) localStorage.setItem('cart', JSON.stringify([]));
    if (!localStorage.getItem('wishlist')) localStorage.setItem('wishlist', JSON.stringify([]));

    // Book data (could be loaded from an API or JSON file)
    const currentBook = {
        id: 1,
        title: "THUNMANHANDIYA",
        price: 700,
        image: "Images/default.jpg",
        genre: "Fiction",
        author: "Mahagamasakara",
        releaseDate: "2025/10/12",
        publisher: "Example Publications",
        editor: "John Doe"
    };

    // DOM Elements
    const bookImage = document.querySelector('.book-cover-container img');
    const bookTitle = document.querySelector('h1');
    const bookPrice = document.querySelector('.text-primary');
    const detailsList = document.querySelector('.card-body ul');
    const btnCart = document.getElementById('btnCart');
    const btnWishlist = document.getElementById('btnWishlist');

    // Load book data into the page
    function loadBookDetails() {
        bookImage.src = currentBook.image;
        bookTitle.textContent = currentBook.title;
        bookPrice.textContent = `Rs. ${currentBook.price}/-`;

        detailsList.innerHTML = `
            <li class="mb-2"><strong>Genre:</strong> ${currentBook.genre}</li>
            <li class="mb-2"><strong>Author:</strong> ${currentBook.author}</li>
            <li class="mb-2"><strong>Release Date:</strong> ${currentBook.releaseDate}</li>
            <li class="mb-2"><strong>Publisher:</strong> ${currentBook.publisher}</li>
            <li><strong>Editor:</strong> ${currentBook.editor}</li>
        `;
    }

    // Cart functionality
    function addToCart() {
        const cart = JSON.parse(localStorage.getItem('cart'));
        const existingItem = cart.find(item => item.id === currentBook.id);

        if (existingItem) {
            existingItem.quantity += 1;
        } else {
            cart.push({ ...currentBook, quantity: 1 });
        }

        localStorage.setItem('cart', JSON.stringify(cart));
        showAlert('Added to cart!', 'success');
    }

    // Wishlist functionality
    function addToWishlist() {
        const wishlist = JSON.parse(localStorage.getItem('wishlist'));
        const exists = wishlist.some(item => item.id === currentBook.id);

        if (!exists) {
            wishlist.push(currentBook);
            localStorage.setItem('wishlist', JSON.stringify(wishlist));
            showAlert('Added to wishlist!', 'success');
        } else {
            showAlert('Already in wishlist!', 'warning');
        }
    }

    // Review system
    function loadReviews() {
        const reviewsContainer = document.querySelector('.ratings-section');
        const reviews = [
            {
                user: "User Name",
                date: "2024-03-15 14:30",
                rating: 4,
                text: "Lorem ipsum dolor sit amet, consectetur adipiscing elit..."
            },
            {
                user: "Another User",
                date: "2024-03-14 10:15",
                rating: 3,
                text: "Sed ut perspiciatis unde omnis iste natus error sit..."
            }
        ];

        reviewsContainer.innerHTML = `
            <h4 class="mb-4 pb-2 border-bottom">RATINGS & REVIEWS (${reviews.length})</h4>
            ${reviews.map((review, index) => `
                <div class="card mb-4 border-1 shadow-sm highlight-review bg-light">
                    <div class="card-body">
                        <div class="d-flex justify-content-between mb-3">
                            <div>
                                <h6 class="mb-0">${review.user}</h6>
                                <small class="text-muted">${review.date}</small>
                            </div>
                            <div class="star-rating text-warning">
                                ${'<i class="fas fa-star"></i>'.repeat(review.rating)}
                                ${'<i class="far fa-star"></i>'.repeat(5 - review.rating)}
                            </div>
                        </div>
                        <p class="mb-0 review-text">${review.text}</p>
                    </div>
                </div>
            `).join('')}
        `;
    }

    // Alert notification system
    function showAlert(message, type = 'info') {
        const alert = document.createElement('div');
        alert.className = `alert alert-${type} alert-dismissible fade show position-fixed`;
        alert.style.top = '20px';
        alert.style.right = '20px';
        alert.style.zIndex = '1000';
        alert.innerHTML = `
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        `;

        document.body.appendChild(alert);

        // Auto-remove after 3 seconds
        setTimeout(() => {
            alert.remove();
        }, 3000);
    }

    // Event listeners
    btnCart.addEventListener('click', addToCart);
    btnWishlist.addEventListener('click', addToWishlist);

    // Initialize page
    loadBookDetails();
    loadReviews();
});